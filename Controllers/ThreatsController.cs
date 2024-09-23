using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication_InformationSecurityRiskAssessmentSystem.Data;
using WebApplication_InformationSecurityRiskAssessmentSystem.Models;
using WebApplication_InformationSecurityRiskAssessmentSystem.ViewModels;

namespace WebApplication_InformationSecurityRiskAssessmentSystem.Controllers
{
    public class ThreatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThreatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Threats
        [Authorize]
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            //*******************
            // Розмір сторінки
            const int pageSize = 5;

            // Запит до бази даних для отримання активів
            var threatsQuery = _context.Threats.AsQueryable();

            // Загальна кількість записів
            int count = await threatsQuery.CountAsync();

            // Отримуємо активи для поточної сторінки
            var threats = await threatsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Створюємо модель для представлення
            var pageViewModel = new ThreatPageViewModel
            {
                Threats = threats,
                Paginator = new PageViewModel(count, pageNumber, pageSize)
            };

            return View(pageViewModel);
            /*return View(await _context.Threats.ToListAsync());*/
        }

        // GET: Threats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var threat = await _context.Threats
                .FirstOrDefaultAsync(m => m.Id == id);
            if (threat == null)
            {
                return NotFound();
            }

            return View(threat);
        }

        // GET: Threats/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Threats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Threat threat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(threat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(threat);
        }

        // GET: Threats/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var threat = await _context.Threats.FindAsync(id);
            if (threat == null)
            {
                return NotFound();
            }
            return View(threat);
        }

        // POST: Threats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Threat threat)
        {
            if (id != threat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(threat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThreatExists(threat.Id))
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
            return View(threat);
        }

        // GET: Threats/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var threat = await _context.Threats
                .FirstOrDefaultAsync(m => m.Id == id);
            if (threat == null)
            {
                return NotFound();
            }

            return View(threat);
        }

        // POST: Threats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var threat = await _context.Threats.FindAsync(id);
            if (threat != null)
            {
                _context.Threats.Remove(threat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThreatExists(int id)
        {
            return _context.Threats.Any(e => e.Id == id);
        }
    }
}
