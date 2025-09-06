using IMS.SharedKernel.ResultPattern;

namespace IMS.Domain.Errors;

public static class CategoryErrors
{
    public static readonly Error InvalidName = CommonErrors.InvalidFieldError("Category", "Name");
}