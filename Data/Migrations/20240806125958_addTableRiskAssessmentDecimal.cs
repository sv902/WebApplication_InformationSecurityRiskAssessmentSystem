using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication_InformationSecurityRiskAssessmentSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class addTableRiskAssessmentDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Risk",
                table: "RiskAssessments",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Risk",
                table: "RiskAssessments",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
