using Authentication.Core.Application.Auth.Interfaces;
using Authentication.Core.Domain.Auth.DTO;
using Authentication.Core.Domain.Common.DTO;
using Authentication.Entities;
using Authentication.Views;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Core.Application.Auth.Implementation
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;

        public UserService(IConfiguration configuration, IEmailService emailService, UserManager<AppUser> userManager, TokenService tokenService)
        {
            _configuration = configuration;
            _emailService = emailService;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new UserManagerResponse
                {
                    Message = "Usuário não encontrado",
                    IsSuccess = false,
                };

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    Message = "Email confirmado com sucesso!",
                    IsSuccess = true,
                };
            return new UserManagerResponse
            {
                Message = "Email não pode ser confirmado",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    Message = "Usuário deletado com sucesso!",
                    IsSuccess = true,
                };
            return new UserManagerResponse
            {
                Message = "Erro ao deletar usuário",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return new LoginResponseDTO
                {
                    Token = "Não existe usuário com esse email",
                    IsSuccess = false,
                };

            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
                return new LoginResponseDTO
                {
                    Message = "Senha inválida",
                    IsSuccess = false,
                };

            var token = _tokenService.CreateToken(user);
            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponseDTO
            {
                Message = "Login efetuado com sucesso!",
                Token = tokenAsString,
                IsSuccess = true,
                ExpireDate = token.ValidTo,
            };

        }

        public async Task<UserManagerResponse> RegisterAsync(RegisterViewModel model)
        {
            if (model == null) throw new NullReferenceException("Formulário de registro não pode ser vazio");

            if (model.Password != model.ConfirmPassword)
                return new UserManagerResponse
                {
                    Message = "Falha na confirmação da senha.",
                    IsSuccess = false,
                };

            var identityUser = new AppUser
            {
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            //Implementar o email com token
            if (result.Succeeded)
            {
                #region Confirmação por link email
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);

                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppUrl"]}/api/Auth/ConfirmEmail?userId={identityUser.Id}&token={validEmailToken}";

                #endregion

                #region Confirmação código 6 dígitos por e-mail
                var code = _userManager.GenerateChangePhoneNumberTokenAsync(identityUser, model.PhoneNumber);
                #endregion

                await _emailService.SendEmailAsync(
                    identityUser.Email,
                    "Confirme seu email",
                    $"<h1>Bem vindo a Uipe!</h1>" +
                    //$"<p>Por favor confirme seu email clicando no link</p><a href='{url}'>Clique aqui</a>",
                    $"<p>Por favor confirme seu telefone copiando o código <strong> {code.Result} </strong></p>",
                    true);

                return new UserManagerResponse
                {
                    Message = "Usuário criado com sucesso!",
                    IsSuccess = true,
                };
            }

            return new UserManagerResponse
            {
                Message = "Usuário não foi criado",
                IsSuccess = true,
                Errors = result.Errors.Select(e => e.Description)
            };
        }
        public async Task<UserManagerResponse> ForgetPasswordAsync(string email)
        {
            //validar se usuario existe
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new UserManagerResponse
                {
                    Message = "Não existe usuário com esse email",
                    IsSuccess = false,
                };
            //Gerar token de reset de password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            string? validToken = WebEncoders.Base64UrlEncode(encodedToken);

            //criar url com token para abrir a pagina de nova senha
            string url = $"{_configuration["AppUrl"]}/ResetPassword?email={email}&token={validToken}";

            //enviar o link por email
            await _emailService.SendEmailAsync(
                    email,
                    "Solicitação de alteração de senha",
                    "<h1>Siga as instruções para criar uma nova senha</h1>" +
                    $"<p>Por favor click no link para criar uma nova senha</p><a href='{url}'>Criar nova senha</a>",
                    true);

            return new UserManagerResponse
            {
                IsSuccess = true,
                Message = "Solicitação enviado por email com sucesso!"
            };
        }

        public async Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            //validar usuário
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Não existe usuário com esse email."
                };
            //validar se a nova senha confere
            if (model.NewPassword != model.ConfirmPassword)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "As senhas não conferem."
                };
            //decodificar o token
            var decodeToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodeToken);


            //enviar para função de resetPassword
            var result = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    IsSuccess = true,
                    Message = "Nova senha criada com sucesso!"
                };
            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Erro ao criar nova senha.",
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> RequestPhoneConfirmationAsync(string userId, string phone)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Usuário não encontrado"
                };

            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phone);

            user.CodePhoneNumber = code;

            await _userManager.UpdateAsync(user);

            await _emailService.SendEmailAsync(
                    user.Email,
                    "Confirme seu email",
                    $"<h1>Bem vindo a Uipe!</h1>" +
                    $"<p>Por favor confirme seu telefone copiando o código <strong> {code} </strong></p>",
                    true);

            return new UserManagerResponse
            {
                Message = "Usuário criado com sucesso!",
                IsSuccess = true,
            };
        }

        public async Task<UserManagerResponse> ConfirmPhoneNumberAsync(string code)
        {
            var user = await _userManager.Users
                .Where(u => u.CodePhoneNumber == code)
                .FirstAsync();

            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Não existe usuário com esse telefone."
                };

            user.PhoneNumberConfirmed = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    IsSuccess = true,
                    Message = "Telefone validado com sucesso!"
                };
            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Não foi possível confirmar o telefone do usuário.",
                Errors = result.Errors.Select(e => e.Description)
            };

        }
    }
}
