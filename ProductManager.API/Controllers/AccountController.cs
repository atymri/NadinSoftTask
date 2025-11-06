using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Core.Domain.Entities;
using ProductManager.Core.DTOs.UserDTOs;

namespace ProductManager.API.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
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

            var userResponse = _mapper.Map<UserResponse>(user);
            return Ok(userResponse);
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

            var userResponse = _mapper.Map<UserResponse>(user);

            return Ok(userResponse);

        }
    }
}
