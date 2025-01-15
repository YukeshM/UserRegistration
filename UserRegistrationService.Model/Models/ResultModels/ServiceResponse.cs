namespace UserRegistrationService.Core.Models.ResultModels
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ServiceResponse<T> SuccessResponse(T data, string message = null)
        {
            return new ServiceResponse<T>
            {
                Success = true,
                Message = message ?? "Operation successful",
                Data = data
            };
        }

        public static ServiceResponse<T> ErrorResponse(string message, T data = default)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Message = message,
                Data = data
            };
        }
    }

}
