using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using WebApplication_InformationSecurityRiskAssessmentSystem.Models;
using WebApplication_InformationSecurityRiskAssessmentSystem.ViewModels;

[Authorize(Roles = "Admin")] // доступ тільки для адмінів
public class RoleController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    //UserManager є сервісом для управління користувачами, який містить методи для додавання, видалення ролей і отримання інформації про користувачів.

    //Конструктор, який ініціалізує приватне поле
    public RoleController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    //Асинхронний метод, який повертає результат дії
    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users;
        //Отримання всіх користувачів.

        var userRolesViewModel = new List<UserRolesViewModel>();
        //Цикл по всіх користувачах для створення моделі з їхніми ролями
        foreach (var user in users)
        {
            var thisViewModel = new UserRolesViewModel();
            thisViewModel.UserId = user.Id;
            thisViewModel.UserName = user.UserName;
            thisViewModel.FirstName = user.FirstName;
            thisViewModel.LastName = user.LastName;
            thisViewModel.Roles = await _userManager.GetRolesAsync(user);
            //Асинхронний виклик для отримання списку ролей кожного користувача.
            userRolesViewModel.Add(thisViewModel);
        }


        return View(userRolesViewModel); // Передаємо модель у представлення
    }
    [HttpPost]
    public async Task<IActionResult> MakeAdmin(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        //Шукає користувача за його ID
        if (user != null)
        {
            
            if (await _userManager.IsInRoleAsync(user, "Expert"))
            //Перевіряє, чи користувач має роль "Expert".
            {
                await _userManager.RemoveFromRoleAsync(user, "Expert");
               //Видаляє роль "Expert", якщо така є.
            }

            // Додаємо роль "Admin"
            var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
        }

        return BadRequest("Не вдалося призначити роль.");
        // Якщо не вдалося, повертає помилку.
    }
    [HttpPost]
    public async Task<IActionResult> RemoveAdmin(string userId)
    {
        var currentUserId = _userManager.GetUserId(User); // ID поточного користувача

        // Перевірка: якщо адміністратор намагається зняти з себе роль "Admin"
        if (userId == currentUserId)
        {
            return BadRequest("Ви не можете зняти роль адміністратора з самого себе.");
        }
        //Шукає користувача за його ID
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {

            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Expert");
                return RedirectToAction("Index");
            }
        }

        return BadRequest("Не вдалося зняти роль.");
    }
    // Метод для повного видалення користувача
    [HttpPost]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var currentUserId = _userManager.GetUserId(User); // ID поточного користувача

        // Перевірка: якщо адміністратор намагається видалити себе
        if (userId == currentUserId)
        {
            return BadRequest("Ви не можете видалити себе.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return BadRequest("Не вдалося видалити користувача.");
            }
        }

        return NotFound("Користувача не знайдено.");
    }
}
public class UserRolesViewModel
{
    //Клас для представлення інформації про користувачів та їх ролі.

    public string UserId { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public IList<string> Roles { get; set; }
    //Список ролей, які має користувач.


}

