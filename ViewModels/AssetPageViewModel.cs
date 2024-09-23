using WebApplication_InformationSecurityRiskAssessmentSystem.Models;

namespace WebApplication_InformationSecurityRiskAssessmentSystem.ViewModels
{
    public class AssetPageViewModel
    {
        public List<Asset>? Assets { get; set; }
        public PageViewModel? Paginator { get; set; }
        
    }
}
