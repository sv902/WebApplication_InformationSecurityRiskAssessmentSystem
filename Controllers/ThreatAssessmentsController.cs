using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication_InformationSecurityRiskAssessmentSystem.Data;
using WebApplication_InformationSecurityRiskAssessmentSystem.Models;
using WebApplication_InformationSecurityRiskAssessmentSystem.ViewModels;

namespace WebApplication_InformationSecurityRiskAssessmentSystem.Controllers
{
    public class ThreatAssessmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ThreatAssessmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ThreatAssessments
        [Authorize]
        public async Task<IActionResult> Index(int? assetId, int pageNumber = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); // Якщо користувач не авторизований
            }

            // Базовий запит для всіх оцінок
            var query = _context.ThreatAssessments
                .Include(t => t.Asset)
                .Include(t => t.Threat)
                .Include(t => t.User)
                .AsQueryable();

            // Якщо користувач не адміністратор, показуємо тільки його оцінки
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                query = query.Where(t => t.UserId == user.Id);
            }

            // Фільтрація по активам, якщо це необхідно
            if (assetId.HasValue && assetId.Value > 0)
            {
                query = query.Where(t => t.AssetId == assetId.Value);
            }

            int count = await query.CountAsync();

            const int pageSize = 5;
            var threatAssessments = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pageViewModel = new ThreatAssessmentPageViewModel
            {
                ThreatAssessments = threatAssessments,
                Paginator = new PageViewModel(count, pageNumber, pageSize)
            };

            var assets = await _context.Assets.ToListAsync();
            ViewBag.Assets = new SelectList(assets, "Id", "Name");
            ViewBag.AssetId = assetId;

            return View(pageViewModel);
        }


        // GET: ThreatAssessments/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            // Перевіряємо, чи є користувач адміністратором
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            var threatAssessment = await _context.ThreatAssessments
                .Include(t => t.Asset)
                .Include(t => t.Threat)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id && (isAdmin || m.UserId == user.Id));

            if (threatAssessment == null)
            {
                return NotFound();
            }

            return View(threatAssessment);
        }

        // GET: ThreatAssessments/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); // Redirect to login if user is not authenticated
            }

            var model = new ThreatAssessment
            {
                UserId = user.Id,
                AssessmentDate = DateTime.Now
            };

            ViewData["AssetId"] = new SelectList(_context.Assets, "Id", "Name");
            ViewData["ThreatId"] = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");

            // Перевіряємо, чи є активи для підвантаження загроз
            if (_context.Assets.Any())
            {
                var firstAssetId = _context.Assets.First().Id;
                ViewData["ThreatId"] = new SelectList(await GetAvailableThreats(firstAssetId), "Value", "Text");
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,AssetId,ThreatId,Likelihood,Consequences,AssessmentDate")] ThreatAssessment threatAssessment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (threatAssessment.AssessmentDate == default)
                    {
                        threatAssessment.AssessmentDate = DateTime.Now;
                    }

                    if (User.Identity!.IsAuthenticated)
                    {
                        var userName = User.Identity.Name;
                        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userName);
                        if (user != null)
                        {
                            threatAssessment.UserId = user.Id;
                        }
                        else
                        {
                            ModelState.AddModelError("UserId", "Не вдалося знайти користувача.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("UserId", "Користувач не авторизований.");
                    }

                    var existingAssessment = await _context.ThreatAssessments
                     .FirstOrDefaultAsync(ta => ta.AssetId == threatAssessment.AssetId &&
                                        ta.ThreatId == threatAssessment.ThreatId &&
                                        ta.UserId == threatAssessment.UserId);

                    if (existingAssessment != null)
                    {
                        ModelState.AddModelError("", "Ця загроза вже була оцінена для вибраного активу.");
                    }
                    else
                    {
                        _context.Add(threatAssessment);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred: {ex.Message}");
                    ModelState.AddModelError("", "Помилка збереження даних.");
                }
            }

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            ViewData["AssetId"] = new SelectList(_context.Assets, "Id", "Name", threatAssessment.AssetId);
            ViewData["ThreatId"] = new SelectList(_context.Threats, "Id", "Name", threatAssessment.ThreatId);
            return View(threatAssessment);
        }



        // GET: ThreatAssessments/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            // Перевіряємо, чи є користувач адміністратором
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var threatAssessment = await _context.ThreatAssessments
               .FirstOrDefaultAsync(m => m.Id == id && (isAdmin || m.UserId == user.Id));
            
            if (threatAssessment == null)
            {
                return NotFound();
            }

            ViewData["AssetId"] = new SelectList(_context.Assets, "Id", "Name", threatAssessment.AssetId);
            ViewData["ThreatId"] = new SelectList(await GetAvailableThreats(threatAssessment.AssetId, threatAssessment.ThreatId), "Value", "Text", threatAssessment.ThreatId);
            return View(threatAssessment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,AssetId,ThreatId,Likelihood,Consequences,AssessmentDate")] ThreatAssessment threatAssessment)
        {
            if (id != threatAssessment.Id)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || threatAssessment.UserId != user.Id) // Перевірка доступу
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                var alreadyAssessed = await _context.ThreatAssessments
                    .AnyAsync(ta => ta.AssetId == threatAssessment.AssetId && ta.ThreatId == threatAssessment.ThreatId && ta.Id != threatAssessment.Id);

                if (alreadyAssessed)
                {
                    ModelState.AddModelError("ThreatId", "Ця загроза вже була оцінена для цього активу.");
                }

                if (threatAssessment.Likelihood < 0 || threatAssessment.Likelihood > 10)
                {
                    ModelState.AddModelError("Likelihood", "Ймовірність повинна бути в межах від 0 до 10.");
                }

                if (threatAssessment.Consequences < 0 || threatAssessment.Consequences > 15)
                {
                    ModelState.AddModelError("Consequences", "Наслідки повинні бути в межах від 0 до 15.");
                }

                if (!ModelState.Values.Any(v => v.Errors.Count > 0))
                {
                    try
                    {
                        _context.Update(threatAssessment);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ThreatAssessmentExists(threatAssessment.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["AssetId"] = new SelectList(_context.Assets, "Id", "Name", threatAssessment.AssetId);
            ViewData["ThreatId"] = new SelectList(await GetAvailableThreats(threatAssessment.AssetId, threatAssessment.ThreatId), "Value", "Text", threatAssessment.ThreatId);
            return View(threatAssessment);
        }

        // GET: ThreatAssessments/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            // Перевіряємо, чи є користувач адміністратором
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var threatAssessment = await _context.ThreatAssessments
                .Include(t => t.Asset)
                .Include(t => t.Threat)
                .Include(t => t.User)
                    .FirstOrDefaultAsync(m => m.Id == id && (isAdmin || m.UserId == user.Id));

            if (threatAssessment == null)
            {
                return NotFound();
            }

            return View(threatAssessment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var threatAssessment = await _context.ThreatAssessments.FirstOrDefaultAsync(ta => ta.Id == id && ta.UserId == user.Id); // Перевірка доступу


            if (threatAssessment != null)
            {
                _context.ThreatAssessments.Remove(threatAssessment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThreatAssessmentExists(int id)
        {
            return _context.ThreatAssessments.Any(e => e.Id == id);
        }

        public async Task<IActionResult> GetAvailableThreatsJson(int assetId)
        {
            var userId = _userManager.GetUserId(User);

            // Перевіряємо, чи користувач є адміністратором
            var isAdmin = await _userManager.IsInRoleAsync(await _userManager.GetUserAsync(User), "Admin");

            List<int> assessedThreatIds;

            // Якщо користувач не адміністратор, фільтруємо загрози за оцінками користувача
            if (!isAdmin)
            {
                // Отримуємо всі оцінені загрози для конкретного активу і користувача (експерта)
                assessedThreatIds = await _context.ThreatAssessments
                    .Where(ta => ta.AssetId == assetId && ta.UserId == userId)
                    .Select(ta => ta.ThreatId)
                    .ToListAsync();
            }
            else
            {
                // Якщо користувач адміністратор, не фільтруємо оцінки користувача
                assessedThreatIds = new List<int>(); // Адміністратор бачить всі загрози
            }

            // Отримуємо всі загрози, які ще не оцінені конкретним користувачем для вибраного активу
            var availableThreats = await _context.AssetThreats
                .Where(at => at.AssetId == assetId && !assessedThreatIds.Contains(at.ThreatId))
                .Include(at => at.Threat)
                .Select(at => new
                {
                    Value = at.Threat!.Id,
                    Text = at.Threat.Name
                })
                .ToListAsync();

            return Json(availableThreats);
        }



        private async Task<IEnumerable<SelectListItem>> GetAvailableThreats(int assetId, int? selectedThreatId = null)
        {
            var assessedThreatIds = await _context.ThreatAssessments
                .Where(ta => ta.AssetId == assetId)
                .Select(ta => ta.ThreatId)
                .ToListAsync();

            var availableThreats = await _context.AssetThreats
                .Where(at => at.AssetId == assetId && (selectedThreatId == null || at.ThreatId == selectedThreatId || !assessedThreatIds.Contains(at.ThreatId)))
                .Include(at => at.Threat)
                .Select(at => new SelectListItem
                {
                    Value = at.Threat!.Id.ToString(),
                    Text = at.Threat.Name
                })
                .ToListAsync();

            return availableThreats;
        }
    }
}
