using WebApplication_InformationSecurityRiskAssessmentSystem.Models;

namespace WebApplication_InformationSecurityRiskAssessmentSystem.ViewModels
{
    public class AssetThreatPageViewModel
    {
        public List<AssetThreat>? AssetThreats { get; set; }
        public PageViewModel? Paginator { get; set; }
    }
}
