using System.ComponentModel.DataAnnotations;
using WebApplication_InformationSecurityRiskAssessmentSystem.Models;

namespace WebApplication_InformationSecurityRiskAssessmentSystem.ViewModels
{
    public class RiskAssessmentViewModel
    {
        public List<Asset>? Assets { get; set; }
        public List<Threat>? Threats { get; set; }
        public List<AssetThreat>? AssetThreats { get; set; }
        public List<RiskAssessment>? RiskAssessments { get; set; }
        public ThreatAssessment? ThreatAssessment { get; set; }
        public List<AverageRiskPerAssetThreat>? AverageRiskPerAssetThreats { get; set; }
        public List<AverageRiskPerAsset>? AverageRiskPerAssets { get; set; }
        public PageViewModel? Paginator { get; set; }
    }
}
