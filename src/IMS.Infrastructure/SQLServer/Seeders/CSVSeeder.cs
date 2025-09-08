using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using IMS.Config;
using IMS.Domain.Entities;
using IMS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace IMS.Infrastructure.SQLServer.Seeders;

public sealed class CSVSeeder(IMSDBContext context, ILoggerFactory loggerFactory) : IDataSeeder
{
    private readonly ILogger<CSVSeeder> _logger = loggerFactory.CreateLogger<CSVSeeder>();

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var unitOfWork = new UnitOfWork(context, loggerFactory);

        var csvPath = CONFIG.CSVSeedPath;

        if (!File.Exists(csvPath))
        {
            _logger.LogWarning("CSV seed file not found at {Path}. Skipping seeding.", csvPath);
            return;
        }

        List<CsvProductRow> rows;
        try
        {
            rows = ReadCsv(csvPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read CSV file at {Path}", csvPath);
            throw;
        }

        if (rows.Count == 0)
        {
            _logger.LogInformation("CSV is empty. Nothing to seed.");
            return;
        }

        // 1) Create categories
        var distinctCategoryNames = rows
            .Select(r => r.CategoryName?.Trim())
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        // Keep a local map so we can assign CategoryId (For products)
        // while staying within one SaveChanges call
        var categoriesByName = new Dictionary<string, Category>(StringComparer.OrdinalIgnoreCase);

        foreach (var name in distinctCategoryNames)
        {
            // Create domain entity (assign Guid now so we can reference it for products)
            var category = Category.Create(Guid.NewGuid(), name!).Value;

            var createCatResult = await unitOfWork.Categories.CreateAsync(category, cancellationToken);
            if (createCatResult.IsFailure)
            {
                _logger.LogError("Unable to create category '{Name}': {Error}", name, createCatResult.Error);
                throw new InvalidOperationException($"Seeding failed: {createCatResult.Error}");
            }

            categoriesByName[name!] = category;
        }

        // 2) Create products (skip duplicate barcodes within the CSV)
        var seenBarcodes = new HashSet<string>(StringComparer.Ordinal);
        var createdCount = 0;

        foreach (var r in rows)
        {
            var catName = r.CategoryName?.Trim();
            if (string.IsNullOrWhiteSpace(catName) || !categoriesByName.TryGetValue(catName, out var category))
            {
                _logger.LogWarning("Skipping product '{Name}' due to unknown category '{Category}'.", r.Name, r.CategoryName);
                continue;
            }

            var barcode = r.Barcode?.Trim();
            if (string.IsNullOrWhiteSpace(barcode))
            {
                _logger.LogWarning("Skipping product '{Name}' due to missing barcode.", r.Name);
                continue;
            }

            if (!seenBarcodes.Add(barcode))
            {
                _logger.LogWarning("Skipping duplicate barcode in CSV: {Barcode}", barcode);
                continue;
            }

            var statusValue = r.Status;
            ProductStatus statusEnum;
            try
            {
                statusEnum = (ProductStatus)statusValue;
            }
            catch
            {
                _logger.LogWarning("Invalid status '{Status}' for product '{Name}'. Defaulting to 0.", statusValue, r.Name);
                statusEnum = 0;
            }

            var product = Product.Create(Guid.NewGuid(), r.Name?.Trim() ?? string.Empty, barcode,
                string.IsNullOrWhiteSpace(r.Description) ? null : r.Description!.Trim(),
                (double)r.Weight, statusEnum, category).Value;

            var createProdResult = await unitOfWork.Products.CreateAsync(product, cancellationToken);
            if (createProdResult.IsFailure)
            {
                _logger.LogError("Unable to create product '{Name}': {Error}", product.Name, createProdResult.Error);
                throw new InvalidOperationException($"Seeding failed: {createProdResult.Error}");
            }

            createdCount++;
        }

        // 3) Persist once (your UnitOfWork disposes the DbContext here)
        var saveResult = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            _logger.LogError("Seeding save failed: {Error}", saveResult.Error);
            throw new InvalidOperationException($"Seeding save failed: {saveResult.Error}");
        }

        _logger.LogInformation("Seeding completed. Categories: {CatCount}, Products: {ProdCount}.",
            categoriesByName.Count, createdCount);
    }

    private static List<CsvProductRow> ReadCsv(string path)
    {
        var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ",",
            TrimOptions = TrimOptions.Trim,
            BadDataFound = null,
            DetectColumnCountChanges = true
        };

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, cfg);
        csv.Context.RegisterClassMap<CsvProductRowMap>();
        return csv.GetRecords<CsvProductRow>().ToList();
    }

    // matches: Name,Barcode,Description,Weight,Status,CategoryName
    private sealed class CsvProductRow
    {
        public string? Name { get; set; }
        public string? Barcode { get; set; }
        public string? Description { get; set; }
        public decimal Weight { get; set; }
        public int Status { get; set; }
        public string? CategoryName { get; set; }
    }

    private sealed class CsvProductRowMap : ClassMap<CsvProductRow>
    {
        public CsvProductRowMap()
        {
            Map(m => m.Name).Name("Name");
            Map(m => m.Barcode).Name("Barcode");
            Map(m => m.Description).Name("Description");
            Map(m => m.Weight).Name("Weight").TypeConverterOption.CultureInfo(CultureInfo.InvariantCulture);
            Map(m => m.Status).Name("Status");
            Map(m => m.CategoryName).Name("CategoryName");
        }
    }
}
