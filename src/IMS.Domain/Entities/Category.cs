using IMS.SharedKernel.ResultPattern;

namespace IMS.Domain.Entities;

public sealed class Category
{
    private Category(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }

    public static Result<Category> Create(Guid id, string name)
    {
        // Add business rules (Not validation ones) here if needed

        return Result.Success(new Category(id, name));
    }
}