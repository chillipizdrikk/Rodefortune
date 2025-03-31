using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RodeFortune.BLL.Dto;
using RodeFortune.BLL.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using System.Security.Claims;
using MongoDB.Bson;

namespace RodeFortune.PresentationLayer.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            _logger.LogInformation("Відображення форми реєстрації");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Помилка валідації при реєстрації користувача");
                return View(registerDto);
            }

            _logger.LogInformation("Спроба реєстрації користувача: {Username}", registerDto.Username);

            try
            {
                var existingUser = await _userService.GetUserByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Спроба реєстрації з існуючою поштою: {Email}", registerDto.Email);
                    ModelState.AddModelError("Email", "Користувач з такою електронною поштою вже існує");
                    return View(registerDto);
                }

                string zodiacSign = DetermineZodiacSign(registerDto.BirthDate);

                var userDto = new UserRequestDto
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    PasswordHash = HashPassword(registerDto.Password),
                    BirthDate = registerDto.BirthDate,
                    ZodiacSign = zodiacSign,
                    Role = "User"
                };

                var user = await _userService.CreateUserAsync(userDto);

                _logger.LogInformation("Користувач успішно зареєстрований: {Username}", registerDto.Username);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role);

                return RedirectToAction("Index", "Blog"); // Will be changed to profile after the addition of profile usecase
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при реєстрації користувача: {Username}", registerDto.Username);
                ModelState.AddModelError("", "Виникла помилка при реєстрації. Будь ласка, спробуйте пізніше.");
                return View(registerDto);
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private string DetermineZodiacSign(DateTime birthDate)
        {
            int day = birthDate.Day;
            int month = birthDate.Month;

            return (month, day) switch
            {
                (1, _) when day <= 20 => "Козеріг",
                (1, _) => "Водолій",
                (2, _) when day <= 19 => "Водолій",
                (2, _) => "Риби",
                (3, _) when day <= 20 => "Риби",
                (3, _) => "Овен",
                (4, _) when day <= 20 => "Овен",
                (4, _) => "Телець",
                (5, _) when day <= 21 => "Телець",
                (5, _) => "Близнюки",
                (6, _) when day <= 21 => "Близнюки",
                (6, _) => "Рак",
                (7, _) when day <= 22 => "Рак",
                (7, _) => "Лев",
                (8, _) when day <= 23 => "Лев",
                (8, _) => "Діва",
                (9, _) when day <= 23 => "Діва",
                (9, _) => "Терези",
                (10, _) when day <= 23 => "Терези",
                (10, _) => "Скорпіон",
                (11, _) when day <= 22 => "Скорпіон",
                (11, _) => "Стрілець",
                (12, _) when day <= 21 => "Стрілець",
                (12, _) => "Козеріг",
                _ => "Невідомо",
            };
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            _logger.LogInformation("Спроба входу користувача: {Email}", loginDto.Email);

            try
            {
                string hashedPassword = HashPassword(loginDto.Password);

                bool isValid = await _userService.ValidateUserCredentialsAsync(loginDto.Email, hashedPassword);

                if (!isValid)
                {
                    _logger.LogWarning("Невдала спроба входу: {Email}", loginDto.Email);
                    ModelState.AddModelError("", "Неправильна електронна пошта або пароль");
                    return View(loginDto);
                }

                var user = await _userService.GetUserByEmailAsync(loginDto.Email);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = loginDto.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role);

                _logger.LogInformation("Користувач успішно увійшов: {Email}", loginDto.Email);

                return RedirectToAction("Index", "Blog"); // Will be changed to profile after the addition of profile usecase
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при вході користувача: {Email}", loginDto.Email);
                ModelState.AddModelError("", "Виникла помилка при вході. Будь ласка, спробуйте пізніше.");
                return View(loginDto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Користувач виходить із системи: {Username}", HttpContext.Session.GetString("Username"));

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult PasswordRecovery()
        {
            _logger.LogInformation("Відображення форми відновлення паролю");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordRecovery(PasswordRecoveryRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Помилка валідації при відновленні паролю");
                return View(model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Паролі не співпадають");
                return View(model);
            }

            _logger.LogInformation("Спроба відновлення паролю для: {Email}", model.Email);

            try
            {
                var user = await _userService.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    _logger.LogWarning("Спроба відновлення паролю для неіснуючого користувача: {Email}", model.Email);
                    ModelState.AddModelError("Email", "Користувач з такою електронною поштою не знайдений");
                    return View(model);
                }

                var userUpdateDto = new UserRequestDto
                {
                    Username = user.Username,
                    Email = user.Email,
                    PasswordHash = HashPassword(model.NewPassword),
                    BirthDate = user.BirthDate,
                    ZodiacSign = user.ZodiacSign,
                    Role = user.Role,
                    Avatar = user.Avatar
                };

                await _userService.UpdateUserAsync(user.Id.ToString(), userUpdateDto);

                _logger.LogInformation("Пароль успішно відновлено для: {Email}", model.Email);
                ViewBag.Message = "Ваш пароль було успішно змінено. Тепер ви можете увійти з новим паролем.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при відновленні паролю для: {Email}", model.Email);
                ModelState.AddModelError("", "Виникла помилка при відновленні паролю. Будь ласка, спробуйте пізніше.");
            }

            return View(model);
        }
    }
}