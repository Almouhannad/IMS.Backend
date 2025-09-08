using IMS.Application.Products.Commands.ChangeStatus;
using IMS.Domain.Entities;
using IMS.Domain.Interfaces;
using IMS.SharedKernel.ResultPattern;

namespace IMS.Tests.Integration.Application;

public class ChangeProductStatusCommandHandlerTests
{
    [Fact]
    public async Task Handle_ChangesStatusAndPersists()
    {
        // Arrange
        var category = Category.Create(Guid.NewGuid(), "Toys").Value;
        var product = Product.Create(Guid.NewGuid(), "Car", "123", null, 1.0, ProductStatus.InStock, category).Value;
        var repository = new FakeProductRepository(product);
        var unitOfWork = new FakeUnitOfWork(repository);

        var handler = new ChangeProductStatusCommandHandler(unitOfWork);
        var command = new ChangeProductStatusCommand(product.Id, "Sold");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ProductStatus.Sold, repository.Product!.Status);
        Assert.True(repository.UpdateCalled);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    private sealed class FakeProductRepository(Product product) : IProductRepository
    {
        public Product? Product { get; set; } = product;
        public bool UpdateCalled { get; private set; }

        public Task<Result> CreateAsync(Product product, CancellationToken cancellationToken = default)
            => Task.FromResult(Result.Success());

        public Task<Result<Product?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(Result.Success(Product));

        public Task<Result<IReadOnlyList<Product>>> GetAllAsync(ProductStatus? status, int page, int pageSize, CancellationToken cancellationToken = default)
            => Task.FromResult(Result.Success((IReadOnlyList<Product>)[]));

        public Task<Result<IReadOnlyDictionary<ProductStatus, int>>> CountByStatusAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(Result.Success((IReadOnlyDictionary<ProductStatus, int>)new Dictionary<ProductStatus, int>()));

        public Result Update(Product product)
        {
            UpdateCalled = true;
            Product = product;
            return Result.Success();
        }

        public Result Delete(Product product) => Result.Success();
    }

    private sealed class FakeCategoryRepository : ICategoryRepository
    {
        public Task<Result> CreateAsync(Category category, CancellationToken cancellationToken = default)
            => Task.FromResult(Result.Success());

        public Task<Result<Category?>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
            => Task.FromResult(Result.Success<Category?>(null));
    }

    private sealed class FakeUnitOfWork(FakeProductRepository productRepository) : IUnitOfWork
    {
        public FakeProductRepository ProductRepository { get; } = productRepository;
        public bool SaveChangesCalled { get; private set; }
        public bool DisposeCalled { get; private set; }

        public IProductRepository Products => ProductRepository;
        public ICategoryRepository Categories { get; } = new FakeCategoryRepository();

        public ValueTask DisposeAsync()
        {
            DisposeCalled = true;
            return ValueTask.CompletedTask;
        }

        public Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCalled = true;
            return Task.FromResult(Result.Success());
        }
    }
}