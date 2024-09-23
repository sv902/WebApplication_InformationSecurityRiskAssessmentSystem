using WebApplication_InformationSecurityRiskAssessmentSystem.Models;

namespace WebApplication_InformationSecurityRiskAssessmentSystem.ViewModels
{
    public class ThreatAssessmentPageViewModel
    {
        public IEnumerable<ThreatAssessment>? ThreatAssessments { get; set; }
        public PageViewModel? Paginator { get; set; }

    }
}