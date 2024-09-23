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
    public class AssetThreatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssetThreatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AssetThreats
        [Authorize]
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
          /*  var applicationDbContext = _context.AssetThreats.Include(a => a.Asset).Include(a => a.Threat);*/
            // Розмір сторінки
            const int pageSize = 5;

            // Запит до бази даних для отримання активів
            var assetThreatsQuery = _context.AssetThreats
             .Include(a => a.Asset)
             .Include(a => a.Threat);

            // Загальна кількість записів
            int count = await assetThreatsQuery.CountAsync();

            // Отримуємо активи для поточної сторінки
            var assetThreats = await assetThreatsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Створюємо модель для представлення
            var pageViewModel = new AssetThreatPageViewModel
            {
                AssetThreats = assetThreats,
                Paginator = new PageViewModel(count, pageNumber, pageSize)
            };

            return View(pageViewModel);
            /* return View(await applicationDbContext.ToListAsync());*/
        }

        // GET: AssetThreats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetThreat = await _context.AssetThreats
                .Include(a => a.Asset)
                .Include(a => a.Threat)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assetThreat == null)
            {
                return NotFound();
            }

            return View(assetThreat);
        }

        // GET: AssetThreats/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["AssetId"] = new SelectList(_context.Assets, "Id", "Name");
            ViewData["ThreatId"] = new SelectList(_context.Threats, "Id", "Name");
            return View();
        }

        // POST: AssetThreats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AssetId,ThreatId")] AssetThreat assetThreat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assetThreat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssetId"] = new SelectList(_context.Assets, "Id", "Name", assetThreat.AssetId);
            ViewData["ThreatId"] = new SelectList(_context.Threats, "Id", "Name", assetThreat.ThreatId);
            return View(assetThreat);
        }

        // GET: AssetThreats/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetThreat = await _context.AssetThreats.FindAsync(id);
            if (assetThreat == null)
            {
                return NotFound();
            }
            ViewData["AssetId"] = new SelectList(_context.Assets, "Id", "Name", assetThreat.AssetId);
            ViewData["ThreatId"] = new SelectList(_context.Threats, "Id", "Name", assetThreat.ThreatId);
            return View(assetThreat);
        }

        // POST: AssetThreats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AssetId,ThreatId")] AssetThreat assetThreat)
        {
            if (id != assetThreat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assetThreat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssetThreatExists(assetThreat.Id))
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
            ViewData["AssetId"] = new SelectList(_context.Assets, "Id", "Name", assetThreat.AssetId);
            ViewData["ThreatId"] = new SelectList(_context.Threats, "Id", "Name", assetThreat.ThreatId);
            return View(assetThreat);
        }

        // GET: AssetThreats/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetThreat = await _context.AssetThreats
                .Include(a => a.Asset)
                .Include(a => a.Threat)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assetThreat == null)
            {
                return NotFound();
            }

            return View(assetThreat);
        }

        // POST: AssetThreats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assetThreat = await _context.AssetThreats.FindAsync(id);
            if (assetThreat != null)
            {
                _context.AssetThreats.Remove(assetThreat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssetThreatExists(int id)
        {
            return _context.AssetThreats.Any(e => e.Id == id);
        }

        //------------------------///
        public async Task<IActionResult> GetAvailableThreats(int assetId)
        {
            // Отримуємо вже прив'язані загрози
            var usedThreatIds = await _context.AssetThreats
                .Where(at => at.AssetId == assetId)
                .Select(at => at.ThreatId)
                .ToListAsync();

            // Отримуємо доступні загрози
            var availableThreats = await _context.Threats
                .Where(t => !usedThreatIds.Contains(t.Id))
                .Select(t => new
                {
                    Value = t.Id,
                    Text = t.Name
                })
                .ToListAsync();

            return Json(availableThreats);
        }

    }
}
