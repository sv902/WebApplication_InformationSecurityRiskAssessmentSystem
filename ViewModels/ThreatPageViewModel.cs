using WebApplication_InformationSecurityRiskAssessmentSystem.Models;

namespace WebApplication_InformationSecurityRiskAssessmentSystem.ViewModels
{
    public class ThreatPageViewModel
    {
        public List<Threat>? Threats {  get; set; }

        public PageViewModel? Paginator { get; set; }
    }
}
