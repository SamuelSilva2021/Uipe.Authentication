using Authentication.Core.Application.Auth.Interfaces;
using Authentication.Entities;
using Authentication.Infrastructure;
using Authentication.Views;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(IUserService userService, IEmailService emailService, IConfiguration configuration, AppDbContext context)
        {
            _userService = userService;
            _emailService = emailService;
            _configuration = configuration;
            _context = context;
        }
        [Authorize]
        [HttpGet]
        [Route("Users")]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();

            var result = await _userService.ConfirmEmailAsync(userId, token);

            if (result.IsSuccess)
                return Redirect($"{_configuration["AppUrl"]}/ConfirmEmail.html");
            return BadRequest(result);
        }

        [HttpPost]
        [Route("Register")]// api/register
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterAsync(model);

                if (result.IsSuccess)
                    return Ok(result);
                return BadRequest(result);// 401
            }
            return BadRequest("Alguns campos estão invalidos");
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginAsync(model);

                if (result.IsSuccess)
                {
                    //await _emailService.SendEmailAsync(
                    //    model.Email,
                    //    "Novo login",
                    //    "<h1>Olá! Novo login na sua conta</h1><p>Horário de login " + DateTime.Now +"</p>",
                    //    true
                    //    );
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Alguns campos estão inválidos");
        }
        [HttpPost]
        [Route("ForgetPassword")]
        public async Task<IActionResult> ForgetPasswordAsync(ForgetPasswordViewModel model)
        {
            var result = await _userService.ForgetPasswordAsync(model.Email);

            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);

        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync([FromForm] ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ResetPasswordAsync(model);

                if (result.IsSuccess)
                    return Ok(result);
                return BadRequest(result);
            }
            return BadRequest("Alguns campos estão inválidos");
        }

        [HttpPost]
        [Route("RequestPhoneConfirmation")]
        public async Task<IActionResult> RequestPhoneConfirmationAsync([FromBody] ConfirmPhoneNumberViewModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return BadRequest("ID do usuário não encontrado no token.");

            var result = await _userService.RequestPhoneConfirmationAsync(userId, model.PhoneNumber);

            if(result.IsSuccess)
                return Ok(result);
            return BadRequest(result);

        }

        [HttpPost]
        [Route("ConfirmPhoneNumber")]
        public async Task<IActionResult> ConfirmPhoneNumberAsync([FromForm] CodeConfirmationNumberPhone model)
        {
            var result = await _userService.ConfirmPhoneNumberAsync(model.Code);

            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);

        }

        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUserAsync(string userId)
        {
            var result = await _userService.DeleteUserAsync(userId);
            if(result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }


    }
}
