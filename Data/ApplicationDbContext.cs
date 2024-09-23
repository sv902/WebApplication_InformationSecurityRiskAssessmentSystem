using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication_InformationSecurityRiskAssessmentSystem.Models;

namespace WebApplication_InformationSecurityRiskAssessmentSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {}

     /*   public ApplicationDbContext()
           : base()
        { }*/

        public virtual DbSet<Asset> Assets { get; set; }
        public virtual DbSet<Threat> Threats { get; set; }
        public virtual DbSet<AssetThreat> AssetThreats { get; set; }
        public virtual DbSet<ThreatAssessment> ThreatAssessments { get; set; }
        public virtual DbSet<RiskAssessment> RiskAssessments { get; set; }         
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public virtual DbSet<AverageRiskPerAssetThreat> AverageRiskPerAssetThreats { get; set; }
        public virtual DbSet<AverageRiskPerAsset> AverageRiskPerAssets { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
    }
}
