using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication_InformationSecurityRiskAssessmentSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRiskRatingQualitativeToRiskAssessment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RiskRatingQualitative",
                table: "RiskAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RiskRatingQualitative",
                table: "RiskAssessments");
        }
    }
}
