using Authentication.Core.Domain.Auth.DTO;
using Authentication.Core.Domain.Common.DTO;
using Authentication.Views;

namespace Authentication.Core.Application.Auth.Interfaces
{
    public interface IUserService
    {
        Task<UserManagerResponse> RegisterAsync(RegisterViewModel model);
        Task<LoginResponseDTO> LoginAsync(LoginViewModel model);
        Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token);
        Task<UserManagerResponse> ForgetPasswordAsync(string email);
        Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model);
        Task<UserManagerResponse> DeleteUserAsync(string userId);
        Task<UserManagerResponse> RequestPhoneConfirmationAsync(string userId, string phone);
        Task<UserManagerResponse> ConfirmPhoneNumberAsync(string code);
    }
}
