using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductManager.API.Filters.ActionFilters;
using ProductManager.Core.Domain.Entities;
using ProductManager.Core.DTOs.UserDTOs;
using ProductManager.Core.ServiceContracts;

namespace ProductManager.API.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        public AccountController(UserManager<User> userManager, 
            SignInManager<User> signInManager, 
            IMapper mapper,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return Problem("کاربر با این مشخصات وجود دارد", statusCode: StatusCodes.Status400BadRequest);

            var user = _mapper.Map<User>(request);
            user.UserName = request.Email;
            var res = await _userManager.CreateAsync(user, request.Password);

            if(!res.Succeeded)
                return Problem(string.Join(',',
                        res.Errors.Select(e => e.Description)),
                    statusCode: StatusCodes.Status400BadRequest);

            await _signInManager.SignInAsync(user, false);
            var authResponse = _jwtService.CreateToken(user);

            return Ok(authResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Problem("کاربری با این نام کاربری یافت نشد", statusCode: StatusCodes.Status404NotFound);

            var res = await _signInManager.PasswordSignInAsync(request.Email, request.Password,
                request.RememberMe, true);
           
            if (!res.Succeeded)
                return Problem("ایمیل یا رمز عبور معتبر نیست");

            var authResponse = _jwtService.CreateToken(user);

            return Ok(authResponse);

        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteAcount(string email, string password)
        {
            if (User.Identity.Name != email.Trim())
                return Problem("شما فقط میتوانید حساب کاربری خودتان را حذف کنید", 
                    statusCode: StatusCodes.Status401Unauthorized);

            var user = await _userManager.FindByEmailAsync(email);
            if (!await _userManager.CheckPasswordAsync(user, password))
                return Problem("رمز عبور درست نیست",
                    statusCode: StatusCodes.Status400BadRequest);

            await _userManager.DeleteAsync(user);
            return NoContent();
        }
        

        [HttpGet("register-email-check")]
        [TypeFilter(typeof(AjaxOnlyActionFilter))]
        public async Task<IActionResult> IsEmailInUseForRegister(string email)
            => Ok(await _userManager.FindByEmailAsync(email) == null);

        [HttpGet("login-email-check")]
        [TypeFilter(typeof(AjaxOnlyActionFilter))]
        public async Task<IActionResult> IsEmailInUseForLogin(string email)
            => Ok(await _userManager.FindByEmailAsync(email) != null);
    }
}
