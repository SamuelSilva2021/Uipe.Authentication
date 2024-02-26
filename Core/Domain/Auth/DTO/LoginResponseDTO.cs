namespace Authentication.Core.Domain.Auth.DTO
{
    public class LoginResponseDTO
    {
        public string Message { get; set; }
        public string Token { get; set; }
        public bool ResetPassword { get; set; } = false;
        public string AccessControlId { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public DateTime? ExpireDate { get; set; }

    }
}
