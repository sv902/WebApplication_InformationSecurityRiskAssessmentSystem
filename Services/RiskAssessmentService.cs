using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication_InformationSecurityRiskAssessmentSystem.Data;
using WebApplication_InformationSecurityRiskAssessmentSystem.Models;
using WebApplication_InformationSecurityRiskAssessmentSystem.Services;

public class RiskAssessmentService
{
    private readonly IntervalFuzzificationService _fuzzificationService;
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    private List<CalcRiskAssessment.FuzzyNumber> _probabilityTerms;
    private List<CalcRiskAssessment.FuzzyNumber> _consequenceTerms;
    private List<CalcRiskAssessment.FuzzyNumber>? _riskTerms;
    private List<double>? _riskTermRightValues;

    public RiskAssessmentService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _fuzzificationService = new IntervalFuzzificationService();
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public void CalculateProbabilityTerms()
    {
        List<double[]> intervals = new List<double[]>
        {
            new double[] { 0, 2.5 },
            new double[] { 2.5, 5 },
            new double[] { 5, 7.5 },
            new double[] { 7.5, 10 }
        };
        _fuzzificationService.FuzzifyIntervals(intervals);
        _probabilityTerms = ConvertToFuzzyNumbers(intervals);
    }

    public void CalculateConsequenceTerms()
    {
        List<double[]> intervals = new List<double[]>
        {
            new double[] { 0, 3 },
            new double[] { 3, 7.5 },
            new double[] { 7.5, 12.5 },
            new double[] { 12.5, 15 }
        };
        _fuzzificationService.FuzzifyIntervals(intervals);
        _consequenceTerms = ConvertToFuzzyNumbers(intervals);
    }

    public void CalculateRiskTerms()
    {
        List<double[]> intervals = new List<double[]>
        {
            new double[] { 0, 30 },
            new double[] { 30, 37 },
            new double[] { 37, 63.5 },
            new double[] { 63.5, 100 }
        };

        _riskTermRightValues = intervals.Select(interval => interval[1]).ToList();

        Console.WriteLine("Risk Term Right Values:");
        foreach (var value in _riskTermRightValues)
        {
            Console.WriteLine(value);
        }

        _fuzzificationService.FuzzifyIntervals(intervals);
        _riskTerms = ConvertToFuzzyNumbers(intervals);
    }

    public List<CalcRiskAssessment.FuzzyNumber> GetRiskTerms()
    {
        return _riskTerms!;
    }

    private List<CalcRiskAssessment.FuzzyNumber> ConvertToFuzzyNumbers(List<double[]> intervals)
    {
        List<CalcRiskAssessment.FuzzyNumber> fuzzyNumbers = new List<CalcRiskAssessment.FuzzyNumber>();
        foreach (var interval in intervals)
        {
            double ai = interval[0];
            double b1i = interval[1];
            double b2i = interval[2];
            double ci = interval[3];

            CalcRiskAssessment.FuzzyNumber fuzzyNumber = new CalcRiskAssessment.FuzzyNumber { a = ai, b1 = b1i, b2 = b2i, c = ci };
            fuzzyNumbers.Add(fuzzyNumber);
        }
        return fuzzyNumbers;
    }

    public async Task<List<RiskAssessment>> CalculateRisksAsync(
      List<Asset> assets,
      List<Threat> threats,
      List<decimal[]> probabilityParameters,
      List<decimal[]> consequenceParameters)
    {
        CalculateProbabilityTerms();
        CalculateConsequenceTerms();
        CalculateRiskTerms();

        var riskAssessments = new List<RiskAssessment>();

        // Конвертуємо параметри в double[]
        var doubleProbabilityParameters = ConvertDecimalListToDoubleList(probabilityParameters);
        var doubleConsequenceParameters = ConvertDecimalListToDoubleList(consequenceParameters);

        foreach (var asset in assets)
        {
            foreach (var threat in threats)
            {
                var threatAssessments = await _context.ThreatAssessments
                    .Where(ta => ta.AssetId == asset.Id && ta.ThreatId == threat.Id)
                    .ToListAsync();

                Console.WriteLine($"ЗАГАЛЬНА КІЛЬКІСТЬ ОЦІНОК ЗАГРОЗ ДЛЯ АКТИВІВ: {asset.Id}, Загроз: {threat.Id} - {threatAssessments.Count}");

                if (threatAssessments.Count > 0)
                {
                    var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

                    foreach (var threatAssessment in threatAssessments)
                    {
                        var calcRiskAssessment = new CalcRiskAssessment();
                        double currentValueProbability = (double)threatAssessment.Likelihood;
                        double currentValueConsequence = (double)threatAssessment.Consequences;

                        Console.WriteLine($"Processing Asset: {asset.Id}, Threat: {threat.Id}, Likelihood: {currentValueProbability}, Consequences: {currentValueConsequence}");

                        if (doubleProbabilityParameters.Count >= 2 && doubleConsequenceParameters.Count >= 2)
                        {
                            // Обчислюємо матрицю лямбда
                            List<List<double>> lambdaMatrix = calcRiskAssessment.CalculateLambdaMatrix(
                                currentValueProbability,
                                currentValueConsequence,
                                doubleProbabilityParameters,
                                doubleConsequenceParameters,
                                _probabilityTerms,
                                _consequenceTerms);

                            // Оцінюємо ризик
                            double riskValue = calcRiskAssessment.EvaluateRL(lambdaMatrix, _riskTerms!, _riskTermRightValues!);
                            string structuredRisk = calcRiskAssessment.FormStructuredRiskParameter(riskValue, _riskTerms!);

                            Console.WriteLine($"Risk calculated: {riskValue} for Asset: {asset.Id}, Threat: {threat.Id}");

                            // Додаємо результат до списку
                            riskAssessments.Add(new RiskAssessment
                            {
                                UserId = userId,
                                AssetId = asset.Id,
                                ThreatId = threat.Id,
                                Risk = (decimal)riskValue
                            });
                        }
                        else
                        {
                            Console.WriteLine("Недостатні параметри ймовірності або наслідків для загрози:: " + threat.Id);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Threat assessments not found for Asset: {asset.Id}, Threat: {threat.Id}");
                }
            }
        }

        Console.WriteLine($"Всього оцінок ризиків для зв'язків актив-загроза: {riskAssessments.Count}");

        return riskAssessments;
    }

    private List<double[]> ConvertDecimalListToDoubleList(List<decimal[]> decimalList)
    {
        return decimalList.Select(arr => arr.Select(d => (double)d).ToArray()).ToList();
    }

    public class CalcRiskAssessment
    {
        public CalcRiskAssessment() { }

        public class FuzzyNumber
        {
            public double a { get; set; }
            public double b1 { get; set; }
            public double b2 { get; set; }
            public double c { get; set; }

            public double MembershipFunction(double value)
            {
                double result;
                if (value >= a && value <= b1)
                {
                    result = (value - a) / (b1 - a);
                }
                else if (value > b1 && value <= b2)
                {
                    result = 1.0;
                }
                else if (value > b2 && value <= c)
                {
                    result = (c - value) / (c - b2);
                }
                else
                {
                    result = 0.0;
                }

                // Проверка на NaN
                if (double.IsNaN(result))
                {
                    throw new InvalidOperationException($"Membership function returned NaN for value {value} with interval [{a}, {b1}, {b2}, {c}]");
                }

                Console.WriteLine($"Membership function for value {value} with interval [{a}, {b1}, {b2}, {c}] returned {result}");
                return result;
            }
        }

        public List<double> EvaluateComponentSignificance(List<double[]> LS)
        {
            int g = 2;
            double significance = 1.0 / g;

            List<double> significances = new List<double>();
            for (int i = 0; i < g; i++)
            {
                significances.Add(significance);
                Console.WriteLine($"LS={significances[i]}");
            }
            return significances;
        }

        // Создание матрицы лямбда для всех параметров
        public List<List<double>> CalculateLambdaMatrix(double currentValueProbability, double currentValueConsequence, List<double[]> probabilityParameters, List<double[]> consequenceParameters, List<FuzzyNumber> probabilityTerms, List<FuzzyNumber> consequenceTerms)
        {
            int g = 2; // количество параметров
            int m = 4; // количество термов

            List<List<double>> lambdaMatrix = new List<List<double>>();

            for (int i = 0; i < g; i++)
            {
                List<double> lambdaRow = new List<double>();

                for (int j = 0; j < m; j++)
                {
                    double membershipValue;

                    if (i == 0)
                    {
                        membershipValue = probabilityTerms[j].MembershipFunction(currentValueProbability);
                    }
                    else
                    {
                        membershipValue = consequenceTerms[j].MembershipFunction(currentValueConsequence);
                    }

                    // Проверка на NaN
                    if (double.IsNaN(membershipValue))
                    {
                        throw new InvalidOperationException($"Membership function returned NaN for value with term index {j} and parameter index {i}");
                    }

                    lambdaRow.Add(membershipValue);
                }

                double rowSum = lambdaRow.Sum();
                Console.WriteLine($"Row {i} Sum: {rowSum}"); // Додати логування суми рядка

                if (rowSum > 0) // Додати перевірку, щоб уникнути ділення на нуль
                {
                    lambdaRow = lambdaRow.Select(value => value / rowSum).ToList();
                }
                else
                {
                    // Додати більше інформації до виключення для діагностики
                    throw new InvalidOperationException($"Row sum is zero. CurrentValueProbability: {currentValueProbability}, CurrentValueConsequence: {currentValueConsequence}, Row: {string.Join(",", lambdaRow)}");
                }

                lambdaMatrix.Add(lambdaRow);
            }
            foreach (var row in lambdaMatrix)
            {
                foreach (var value in row)
                {
                    Console.Write(value + "\t");
                }
                Console.WriteLine();
            }

            return lambdaMatrix;
        }

        // Крок 2: Оцінка ризику
        public double EvaluateRL(List<List<double>> lambdaMatrix, List<FuzzyNumber> fuzzyNumbers, List<double> riskTermRightValues)
        {
            int g = 2;
            List<double> LS = EvaluateComponentSignificance(lambdaMatrix.Select(row => row.ToArray()).ToList());
            double ks = 1.0 / LS.Sum();
            int m = fuzzyNumbers.Count;
            double RiskLevel = 0.0;

            for (int j = 0; j < m; j++)
            {
                // double KofNorm = 100-20*(m - j);
                double KofNorm = riskTermRightValues[j];

                double Sum = 0.0;

                for (int i = 0; i < g; i++)
                {
                    if (i >= 0 && i < lambdaMatrix.Count && j >= 0 && j < lambdaMatrix[i].Count)
                    {
                        Sum += (ks * LS[i]) * lambdaMatrix[i][j];
                    }
                }

                Console.WriteLine($"KofNorm for term {j}: {KofNorm}");
                Console.WriteLine($"Sum for term {j}: {Sum}");

                RiskLevel += KofNorm * Sum;
            }

            Console.WriteLine($"Final RiskLevel: {RiskLevel}");

            return RiskLevel;
        }

        // Крок 4: Формування структурованого параметру ризику
        public string FormStructuredRiskParameter(double riskValue, List<FuzzyNumber> riskTerms)
        {
            double maxMembership = 0.0;
            string primaryRiskLevel = "Невизначений ризик";
            List<string> riskLevels = new List<string>();

            foreach (var term in riskTerms)
            {
                double membership = term.MembershipFunction(riskValue);
                Console.WriteLine($"Risk value: {riskValue}, Membership: {membership}, Term: [{term.a}, {term.b1}, {term.b2}, {term.c}]");
                string riskLevel = "";

                if (term.Equals(riskTerms[0]))
                    riskLevel = "Низький ризик";
                else if (term.Equals(riskTerms[1]))
                    riskLevel = "Середній ризик";
                else if (term.Equals(riskTerms[2]))
                    riskLevel = "Високий ризик";
                else if (term.Equals(riskTerms[3]))
                    riskLevel = "Критичний ризик";

                if (membership > maxMembership)
                {
                    maxMembership = membership;
                    primaryRiskLevel = riskLevel;
                }

                if (membership > 0)
                {
                    riskLevels.Add($"{riskLevel} ({membership:F1})");
                }
            }

            string formattedRiskLevels = string.Join("; ", riskLevels);
            return $"{riskValue:F2}; {formattedRiskLevels}";
        }

    }
    public async Task<List<AverageRiskPerAssetThreat>> CalculateAverageRiskPerAssetThreatAsync(List<CalcRiskAssessment.FuzzyNumber> riskTerms)
    {
        var averageRisks = new List<AverageRiskPerAssetThreat>();
        var assets = await _context.Assets.ToListAsync();
        var threats = await _context.Threats.ToListAsync();

        foreach (var asset in assets)
        {
            foreach (var threat in threats)
            {
                var riskAssessments = await _context.RiskAssessments
                    .Where(ra => ra.AssetId == asset.Id && ra.ThreatId == threat.Id)
                    .ToListAsync();

                if (riskAssessments.Any())
                {
                    var averageRisk = riskAssessments.Average(ra => ra.Risk);

                    var existingRisk = await _context.AverageRiskPerAssetThreats
                        .FirstOrDefaultAsync(ar => ar.AssetId == asset.Id && ar.ThreatId == threat.Id);

                    if (existingRisk == null || existingRisk.AverageRisk != averageRisk)
                    {
                        var calcRiskAssessment = new CalcRiskAssessment();
                        var structuredRisk = calcRiskAssessment.FormStructuredRiskParameter((double)averageRisk, riskTerms);

                        averageRisks.Add(new AverageRiskPerAssetThreat
                        {
                            AssetId = asset.Id,
                            ThreatId = threat.Id,
                            AverageRisk = averageRisk,
                            RiskRatingQualitative = structuredRisk,
                            AssessmentDate = DateTime.Now
                        });
                    }
                }
            }
        }

        if (averageRisks.Any())
        {
            // Видаляємо старі записи перед додаванням нових
            var assetIds = averageRisks.Select(a => a.AssetId).Distinct().ToList();
            var threatIds = averageRisks.Select(a => a.ThreatId).Distinct().ToList();

            var oldRecords = await _context.AverageRiskPerAssetThreats
                .Where(ar => assetIds.Contains(ar.AssetId) && threatIds.Contains(ar.ThreatId))
                .ToListAsync();

            _context.AverageRiskPerAssetThreats.RemoveRange(oldRecords);
            _context.AverageRiskPerAssetThreats.AddRange(averageRisks);
            await _context.SaveChangesAsync();
        }

        return averageRisks;
    }

    public async Task<List<AverageRiskPerAsset>> CalculateAverageRiskPerAssetAsync(List<CalcRiskAssessment.FuzzyNumber> riskTerms)
    {
        var averageRisks = new List<AverageRiskPerAsset>();
        var assets = await _context.Assets.ToListAsync();

        foreach (var asset in assets)
        {
            var riskAssessments = await _context.RiskAssessments
                .Where(ra => ra.AssetId == asset.Id)
                .ToListAsync();

            if (riskAssessments.Any())
            {
                var averageRisk = riskAssessments.Average(ra => ra.Risk);

                var existingRisk = await _context.AverageRiskPerAssets
                    .FirstOrDefaultAsync(ar => ar.AssetId == asset.Id);

                if (existingRisk == null || existingRisk.AverageRisk != averageRisk)
                {
                    var calcRiskAssessment = new CalcRiskAssessment();
                    var structuredRisk = calcRiskAssessment.FormStructuredRiskParameter((double)averageRisk, riskTerms);

                    averageRisks.Add(new AverageRiskPerAsset
                    {
                        AssetId = asset.Id,
                        AverageRisk = averageRisk,
                        RiskRatingQualitative = structuredRisk
                    });
                }
            }
        }

        if (averageRisks.Any())
        {
            var assetIds = averageRisks.Select(a => a.AssetId).Distinct().ToList();

            var oldRecords = await _context.AverageRiskPerAssets
                .Where(ar => assetIds.Contains(ar.AssetId))
                .ToListAsync();

            _context.AverageRiskPerAssets.RemoveRange(oldRecords);
            _context.AverageRiskPerAssets.AddRange(averageRisks);
            await _context.SaveChangesAsync();
        }

        return averageRisks;
    }

}
