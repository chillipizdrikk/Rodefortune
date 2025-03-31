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

namespace RodeFortune.PresentationLayer.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, IEmailService emailService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _emailService = emailService;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _logger.LogInformation("Запит на відновлення пароля для: {Email}", model.Email);

            try
            {
                var user = await _userService.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    _logger.LogWarning("Запит на відновлення пароля для неіснуючого користувача: {Email}", model.Email);
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // Generate password reset token
                var token = GenerateResetToken();
                
                // Store the token in database with expiration date
                await _userService.SavePasswordResetTokenAsync(user.Id.ToString(), token, DateTime.UtcNow.AddHours(24));

                // Generate callback URL
                var callbackUrl = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { userId = user.Id, token = token },
                    protocol: HttpContext.Request.Scheme);

                // Send email
                await _emailService.SendPasswordResetEmailAsync(model.Email, callbackUrl);
                
                _logger.LogInformation("Посилання для відновлення пароля надіслано: {Email}", model.Email);
                
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при обробці запиту на відновлення пароля: {Email}", model.Email);
                ModelState.AddModelError("", "Виникла помилка при обробці запиту. Будь ласка, спробуйте пізніше.");
                return View(model);
            }
        }

        private string GenerateResetToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            try
            {
                var isValid = await _userService.ValidatePasswordResetTokenAsync(userId, token);
                if (!isValid)
                {
                    _logger.LogWarning("Недійсний токен для відновлення пароля: {UserId}", userId);
                    return RedirectToAction("Login");
                }

                var model = new ResetPasswordViewModel
                {
                    UserId = userId,
                    Token = token
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при перевірці токена відновлення пароля: {UserId}", userId);
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var isValid = await _userService.ValidatePasswordResetTokenAsync(model.UserId, model.Token);
                if (!isValid)
                {
                    _logger.LogWarning("Спроба використання недійсного токена відновлення пароля: {UserId}", model.UserId);
                    ModelState.AddModelError("", "Недійсний або застарілий токен для відновлення пароля.");
                    return View(model);
                }

                string hashedPassword = HashPassword(model.Password);
                await _userService.UpdateUserPasswordAsync(model.UserId, hashedPassword);
                await _userService.InvalidatePasswordResetTokenAsync(model.UserId);
                
                _logger.LogInformation("Пароль успішно скинуто: {UserId}", model.UserId);
                
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при скиданні пароля: {UserId}", model.UserId);
                ModelState.AddModelError("", "Виникла помилка при скиданні пароля. Будь ласка, спробуйте пізніше.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}