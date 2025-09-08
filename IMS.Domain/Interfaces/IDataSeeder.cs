namespace IMS.Domain.Interfaces;

public interface IDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}