namespace IMS.SharedKernel.ResultPattern
{
    public static class CommonErrors
    {
        public static Error InvalidFieldError(string entityName, string fieldName)
        {
            return new Error(
                $"{entityName}.{fieldName}.Invalid",
                $"{entityName} {fieldName} is invalid.",
                ErrorType.Validation
            );
        }
    }
}
