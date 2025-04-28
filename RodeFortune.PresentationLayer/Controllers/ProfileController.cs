using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RodeFortune.BLL.Dto;
using RodeFortune.BLL.Services.Interfaces;
using RodeFortune.PresentationLayer.Models;
using System.Security.Claims;

[Authorize]
public class ProfileController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(IUserService userService, ILogger<ProfileController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        _logger.LogInformation("Завантаження профілю користувача {UserId}", userId);

        var userProfile = await _userService.GetUserByIdAsync(userId);
        if (userProfile == null)
        {
            _logger.LogWarning("Профіль користувача {UserId} не знайдено", userId);
            return NotFound("Профіль користувача не знайдено");
        }

        var viewModel = new UserProfileViewModel
        {
            UserId = userProfile.Id.ToString(),
            Username = userProfile.Username,
            Email = userProfile.Email,
            BirthDate = userProfile.BirthDate,
            ZodiacSign = userProfile.ZodiacSign,
            CreatedAt = userProfile.CreatedAt,
            Base64Avatar = userProfile.Avatar != null && userProfile.Avatar.Length > 0 ?
                $"data:image/png;base64,{Convert.ToBase64String(userProfile.Avatar)}" : string.Empty
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(UserProfileViewModel model, IFormFile? avatarFile = null)
    {

        ModelState.Remove("Base64Avatar");
        ModelState.Remove("UserId");
        ModelState.Remove("CreatedAt");

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Невалідна модель при оновленні профілю:");
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _logger.LogWarning("Помилка валідації: {ErrorMessage}",
                        error.ErrorMessage ?? error.Exception?.Message);
                }
            }


            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(currentUserId))
            {
                var currentProfile = await _userService.GetUserByIdAsync(currentUserId);
                if (currentProfile != null)
                {
                    model.Base64Avatar = currentProfile.Avatar != null ?
                        $"data:image/png;base64,{Convert.ToBase64String(currentProfile.Avatar)}" : string.Empty;
                }
            }

            return View("Index", model);
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("UserId не знайдено в клеймах");
                return RedirectToAction("Login", "Account");
            }

            var userProfile = await _userService.GetUserByIdAsync(userId);
            if (userProfile == null)
            {
                _logger.LogWarning("Профіль користувача {UserId} не знайдено", userId);
                return NotFound("Профіль користувача не знайдено");
            }


            byte[]? newAvatarBytes = null;
            bool hasNewAvatar = false;

            if (avatarFile != null && avatarFile.Length > 0)
            {
                hasNewAvatar = true;
                using (var memoryStream = new MemoryStream())
                {
                    await avatarFile.CopyToAsync(memoryStream);
                    newAvatarBytes = memoryStream.ToArray();
                }
                _logger.LogInformation("Завантажено новий аватар. Розмір: {Size} байт", newAvatarBytes.Length);
            }


            var updateDto = new UserRequestDto
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = "", // ПОКИ Не змінюємо пароль ДЛЯ НАСТУПНИХ ЮЗКЕЙСІВ
                BirthDate = model.BirthDate,
                ZodiacSign = model.ZodiacSign,
                Role = userProfile.Role,
                Avatar = hasNewAvatar ? newAvatarBytes : userProfile.Avatar
            };

            var result = await _userService.UpdateUserAsync(userId, updateDto);

            if (result == null)
            {
                _logger.LogWarning("Не вдалося оновити профіль користувача {UserId}", userId);
                ModelState.AddModelError("", "Не вдалося оновити профіль");

                model.Base64Avatar = userProfile.Avatar != null ?
                    $"data:image/png;base64,{Convert.ToBase64String(userProfile.Avatar)}" : string.Empty;

                return View("Index", model);
            }

            await UpdateUserClaims(result);

            TempData["SuccessMessage"] = hasNewAvatar
                ? "Профіль та аватар успішно оновлено"
                : "Профіль успішно оновлено";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при оновленні профілю: {Message}", ex.Message);
            ModelState.AddModelError("", $"Помилка при оновленні профілю: {ex.Message}");

            return View("Index", model);
        }
    }

    private async Task UpdateUserClaims(UserResponseDto user)
    {
        try
        {
            _logger.LogInformation("Оновлення claims для користувача {Username}", user.Username);

            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var usernameClaim = identity.FindFirst(ClaimTypes.Name);
                if (usernameClaim != null)
                    identity.RemoveClaim(usernameClaim);
                identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));


                var emailClaim = identity.FindFirst(ClaimTypes.Email);
                if (emailClaim != null)
                    identity.RemoveClaim(emailClaim);
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));


                var dobClaim = identity.FindFirst(ClaimTypes.DateOfBirth);
                if (dobClaim != null)
                    identity.RemoveClaim(dobClaim);
                identity.AddClaim(new Claim(ClaimTypes.DateOfBirth, user.BirthDate.ToString("yyyy-MM-dd")));

                await HttpContext.SignInAsync(new ClaimsPrincipal(identity));

                _logger.LogInformation("Claims успішно оновлено");
            }
            else
            {
                _logger.LogWarning("Не вдалося оновити claims: identity is null");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при оновленні claims користувача");
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("UserId не знайдено в клеймах");
            return RedirectToAction("Login", "Account");
        }

        try
        {
            // Виклик методу сервісу для видалення користувача
            var deleteResult = await _userService.DeleteUserAsync(userId);

            if (deleteResult)
            {
                _logger.LogInformation("Акаунт користувача {UserId} успішно видалено", userId);

                // Логаут після видалення акаунту
                await HttpContext.SignOutAsync();
                TempData["SuccessMessage"] = "Ваш акаунт було успішно видалено.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                _logger.LogWarning("Не вдалося видалити акаунт користувача {UserId}", userId);
                TempData["ErrorMessage"] = "Сталася помилка при видаленні акаунту. Спробуйте ще раз.";
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при видаленні акаунту для користувача {UserId}: {Message}", userId, ex.Message);
            TempData["ErrorMessage"] = $"Сталася помилка: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Сталася помилка при зміні паролю. Перевірте введені дані.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Користувача не знайдено.";
                return RedirectToAction("Login", "Account");
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Користувача не знайдено.";
                return RedirectToAction(nameof(Index));
            }

            // Хешуйте введений поточний пароль
            string hashedCurrentPassword = HashPassword(model.CurrentPassword);

            // Перевірка поточного пароля
            if (!await _userService.ValidateUserCredentialsAsync(user.Email, hashedCurrentPassword))
            {
                TempData["ErrorMessage"] = "Поточний пароль введено невірно.";
                return RedirectToAction(nameof(Index));
            }

            // Хешування нового пароля
            string hashedNewPassword = HashPassword(model.NewPassword);

            // Оновлення пароля
            await _userService.UpdateUserPasswordAsync(userId, hashedNewPassword);

            TempData["SuccessMessage"] = "Ваш пароль успішно змінено.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при зміні паролю для користувача: {Message}", ex.Message);
            TempData["ErrorMessage"] = $"Сталася помилка: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    private static string HashPassword(string password)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}