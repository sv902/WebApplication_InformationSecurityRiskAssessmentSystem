using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
using OfficeOpenXml;
using WebApplication_InformationSecurityRiskAssessmentSystem.Data;
using WebApplication_InformationSecurityRiskAssessmentSystem.Models;
using WebApplication_InformationSecurityRiskAssessmentSystem.Services;
using WebApplication_InformationSecurityRiskAssessmentSystem.ViewModels;
using static RiskAssessmentService;
using Asset = WebApplication_InformationSecurityRiskAssessmentSystem.Models.Asset;

namespace WebApplication_InformationSecurityRiskAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RiskAssessmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RiskAssessmentService _riskAssessmentService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RiskAssessmentsController(ApplicationDbContext context, RiskAssessmentService riskAssessmentService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _riskAssessmentService = riskAssessmentService;
            _httpContextAccessor = httpContextAccessor;
        }
        // GET: RiskAssessments *************************
        [Authorize]
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            const int pageSize = 5;

            // Отримання середніх оцінок ризиків для активів та загроз з пагінацією
            var averageRisks = await _context.AverageRiskPerAssetThreats
                .Include(ar => ar.Asset)    
                .Include(ar => ar.Threat)  
                .OrderBy(ar => ar.AssessmentDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Отримання загальної кількості записів для пагінації
            int count = await _context.AverageRiskPerAssetThreats.CountAsync();

            // Створення моделі представлення для передачі в представлення
            var viewModel = new RiskAssessmentViewModel
            {
                Assets = await _context.Assets.ToListAsync(),
                Threats = await _context.Threats.ToListAsync(),
                AverageRiskPerAssetThreats = averageRisks,
                Paginator = new PageViewModel(count, pageNumber, pageSize)
            };

            return View(viewModel);
        }


        // GET: RiskAssessments/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var riskAssessment = await _context.AverageRiskPerAssetThreats
                .Include(r => r.Asset)
                .Include(r => r.Threat)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (riskAssessment == null)
            {
                return NotFound();
            }

            var threatAssessment = await _context.ThreatAssessments
                .Include(t => t.User)
                .Include(t => t.Asset)
                .Include(t => t.Threat)
                .FirstOrDefaultAsync(t => t.AssetId == riskAssessment.AssetId && t.ThreatId == riskAssessment.ThreatId);

            if (threatAssessment == null)
            {
                return NotFound();
            }

            var viewModel = new RiskAssessmentViewModel
            {
                AverageRiskPerAssetThreats = new List<AverageRiskPerAssetThreat> { riskAssessment },
                ThreatAssessment = threatAssessment,
                Assets = new List<Asset> { riskAssessment.Asset! },
                Threats = new List<Threat> { riskAssessment.Threat! }
            };

            return View(viewModel);
        }

        // POST: RiskAssessments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,AssetId,ThreatId,Risk,AssessmentDate")] RiskAssessment riskAssessment)
        {
            if (ModelState.IsValid)
            {
                var existingRiskAssessment = await _context.RiskAssessments
                    .FirstOrDefaultAsync(r => r.AssetId == riskAssessment.AssetId
                                           && r.ThreatId == riskAssessment.ThreatId
                                           && r.UserId == riskAssessment.UserId);

                if (existingRiskAssessment != null)
                {
                    ModelState.AddModelError("", "Оцінка ризику для даної пари 'актив-загроза' вже існує.");
                }
                else
                {
                    _context.Add(riskAssessment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["AssetId"] = new SelectList(_context.Assets, "Id", "Name", riskAssessment.AssetId);
            ViewData["ThreatId"] = new SelectList(_context.Threats, "Id", "Name", riskAssessment.ThreatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", "LastName", riskAssessment.UserId);
            return View(riskAssessment);
        }

        // GET: RiskAssessments/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var riskAssessment = await _context.RiskAssessments.FindAsync(id);
            if (riskAssessment == null)
            {
                return NotFound();
            }
            ViewData["AssetId"] = new SelectList(_context.Assets, "Id", "Name", riskAssessment.AssetId);
            ViewData["ThreatId"] = new SelectList(_context.Threats, "Id", "Name", riskAssessment.ThreatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", "LastName", riskAssessment.UserId);
            return View(riskAssessment);
        }

        // POST: RiskAssessments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,AssetId,ThreatId,Risk,AssessmentDate")] RiskAssessment riskAssessment)
        {
            if (id != riskAssessment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(riskAssessment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RiskAssessmentExists(riskAssessment.Id))
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
            ViewData["AssetId"] = new SelectList(_context.Assets, "Id", "Name", riskAssessment.AssetId);
            ViewData["ThreatId"] = new SelectList(_context.Threats, "Id", "Name", riskAssessment.ThreatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", "LastName", riskAssessment.UserId);
            return View(riskAssessment);
        }

        // GET: RiskAssessments/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var riskAssessment = await _context.RiskAssessments
                .Include(r => r.Asset)
                .Include(r => r.Threat)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (riskAssessment == null)
            {
                return NotFound();
            }

            return View(riskAssessment);
        }

        // POST: RiskAssessments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var riskAssessment = await _context.RiskAssessments.FindAsync(id);
            if (riskAssessment != null)
            {
                _context.RiskAssessments.Remove(riskAssessment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RiskAssessmentExists(int id)
        {
            return _context.RiskAssessments.Any(e => e.Id == id);
        }

        //********************************************************//

        [HttpPost]
        public async Task<IActionResult> CalculateRisks()
        {
            try
            {
                var assets = await _context.Assets.ToListAsync();
                var threats = await _context.Threats.ToListAsync();
                var allThreatAssessments = await _context.ThreatAssessments.ToListAsync();

                if (assets == null || threats == null || allThreatAssessments == null)
                {
                    throw new InvalidOperationException("Не вдалося отримати необхідні дані з бази даних.");
                }

                var existingRiskAssessments = await _context.RiskAssessments
                    .Select(r => new { r.AssetId, r.ThreatId })
                    .ToListAsync();

                var existingRiskAssessmentPairs = existingRiskAssessments
                    .Select(r => (r.AssetId, r.ThreatId))
                    .ToHashSet();

                var threatAssessmentsGrouped = allThreatAssessments
                    .GroupBy(ta => ta.ThreatId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(ta => ta.Likelihood).ToArray()
                    );

                var consequencesGrouped = allThreatAssessments
                    .GroupBy(ta => ta.ThreatId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(ta => ta.Consequences).ToArray()
                    );

                var newRiskAssessments = new List<RiskAssessment>();

                foreach (var asset in assets)
                {
                    foreach (var threat in threats)
                    {
                        var currentPair = (asset.Id, threat.Id);

                        if (!existingRiskAssessmentPairs.Contains(currentPair))
                        {
                            if (!threatAssessmentsGrouped.TryGetValue(threat.Id, out var likelihoods))
                            {
                                likelihoods = new decimal[0];
                            }

                            if (!consequencesGrouped.TryGetValue(threat.Id, out var consequences))
                            {
                                consequences = new decimal[0];
                            }

                            if (likelihoods.Any() && consequences.Any())
                            {
                                var calcRiskAssessments = await _riskAssessmentService.CalculateRisksAsync(
                                    new List<Asset> { asset },
                                    new List<Threat> { threat },
                                    new List<decimal[]> { likelihoods },
                                    new List<decimal[]> { consequences }
                                );

                                newRiskAssessments.AddRange(calcRiskAssessments);
                            }
                            else
                            {
                                Console.WriteLine($"Немає даних для обчислення ризиків для пари: {asset.Id}-{threat.Id}");
                            }
                        }
                    }
                }

                if (newRiskAssessments.Any())
                {
                    _context.RiskAssessments.AddRange(newRiskAssessments);
                    await _context.SaveChangesAsync();
                }

                var riskTerms = _riskAssessmentService.GetRiskTerms();
                await _riskAssessmentService.CalculateAverageRiskPerAssetThreatAsync(riskTerms);
                await _riskAssessmentService.CalculateAverageRiskPerAssetAsync(riskTerms);
                // Отримання оновлених даних для часткового вигляду
                var averageRisks = await _context.AverageRiskPerAssetThreats
                    .Include(ar => ar.Asset)
                    .Include(ar => ar.Threat)
                    .OrderBy(ar => ar.AssessmentDate)
                    .ToListAsync();

                var viewModel = new RiskAssessmentViewModel
                {
                    AverageRiskPerAssetThreats = averageRisks,
                    Paginator = new PageViewModel(averageRisks.Count, 1, averageRisks.Count)
                };

                // Повертаємо частковий вигляд
                return PartialView("_RiskAssessmentResultsPartial", viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка під час розрахунку ризиків: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }
        public async Task<IActionResult> Report(int? assetId, int pageNumber = 1)
        {
            const int pageSize = 5;

            var query = _context.AverageRiskPerAssetThreats
                .Include(ar => ar.Asset)
                .Include(ar => ar.Threat)
                .AsQueryable();

            if (assetId.HasValue)
            {
                query = query.Where(ar => ar.AssetId == assetId.Value);
            }

            int count = await query.CountAsync();

            var averageRisks = await query
                .OrderBy(ar => ar.AssessmentDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new RiskAssessmentViewModel
            {
                Assets = await _context.Assets.ToListAsync(),
                Threats = await _context.Threats.ToListAsync(),
                AverageRiskPerAssetThreats = averageRisks,
                Paginator = new PageViewModel(count, pageNumber, pageSize)
            };

            // Генерація даних для графіка
            var averageRiskData = await _context.AverageRiskPerAssets
                .Include(a => a.Asset)
                .Select(a => new
                {
                    AssetName = a.Asset!.Name,
                    AverageRisk = a.AverageRisk
                })
                .ToListAsync();

            ViewBag.RiskData = averageRiskData;
            var assets = await _context.Assets.ToListAsync();
            ViewBag.Assets = new SelectList(assets, "Id", "Name");
            ViewBag.AssetId = assetId;

            return View(viewModel);
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var averageRisksPerAssetThreat = await _context.AverageRiskPerAssetThreats
                .Include(ar => ar.Asset)
                .Include(ar => ar.Threat)
                .OrderBy(ar => ar.AssessmentDate)
                .ToListAsync();

            var averageRisksPerAsset = await _context.AverageRiskPerAssets
                .Include(ar => ar.Asset)
                .ToListAsync();

            var memoryStream = new MemoryStream();

            using (var package = new ExcelPackage(memoryStream))
            {
                // Аркуш для AverageRiskPerAssetThreats
                var worksheet1 = package.Workbook.Worksheets.Add("Оцінки ризику");

                // Header row
                worksheet1.Cells[1, 1].Value = "Назва активу";
                worksheet1.Cells[1, 2].Value = "Назва загрози";
                worksheet1.Cells[1, 3].Value = "Ризик";
                worksheet1.Cells[1, 4].Value = "Дата оцінювання";

                // Data rows
                for (var i = 0; i < averageRisksPerAssetThreat.Count; i++)
                {
                    var riskAssessment = averageRisksPerAssetThreat[i];
                    worksheet1.Cells[i + 2, 1].Value = riskAssessment.Asset!.Name;
                    worksheet1.Cells[i + 2, 2].Value = riskAssessment.Threat!.Name;
                    worksheet1.Cells[i + 2, 3].Value = riskAssessment.AverageRisk;
                    worksheet1.Cells[i + 2, 4].Value = riskAssessment.AssessmentDate.ToString("dd/MM/yyyy"); 
                    worksheet1.Cells[i + 2, 4].Style.Numberformat.Format = "dd/MM/yyyy";
                }

                // Аркуш для AverageRiskPerAssets
                var worksheet2 = package.Workbook.Worksheets.Add("Середні оцінки ризику для активів");

                // Header row
                worksheet2.Cells[1, 1].Value = "Назва активу";
                worksheet2.Cells[1, 2].Value = "Середній ризик";

                // Data rows
                for (var i = 0; i < averageRisksPerAsset.Count; i++)
                {
                    var assetRisk = averageRisksPerAsset[i];
                    worksheet2.Cells[i + 2, 1].Value = assetRisk.Asset!.Name;
                    worksheet2.Cells[i + 2, 2].Value = assetRisk.AverageRisk;
                }

                // Додавання діаграми
                var chart = worksheet2.Drawings.AddChart("AverageRiskChart", OfficeOpenXml.Drawing.Chart.eChartType.ColumnClustered);
                chart.Title.Text = "Середній ризик для активів";
                chart.SetPosition(1, 0, 3, 0); 
                chart.SetSize(600, 400); 

                var series = chart.Series.Add(worksheet2.Cells[2, 2, averageRisksPerAsset.Count + 1, 2], worksheet2.Cells[2, 1, averageRisksPerAsset.Count + 1, 1]);
                series.Header = "Середній ризик";

                package.Save();
            }

            memoryStream.Position = 0;
            string excelName = $"RiskAssessmentsReport_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

    }
}
