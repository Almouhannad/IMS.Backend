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

        public static Error EntityNotFoundError(string entityName)
        {
            return new Error(
                $"{entityName}.NotFound",
                $"{entityName} was not found.",
                ErrorType.Validation
            );
        }

        public static Error OperationFailureError(string operationName, string entityName)
        {
            return new Error(
                $"{entityName}.{operationName}.Failure",
                $"Unable to {operationName} {entityName}",
                ErrorType.Failure
            );
        }
    }
}
