using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication_InformationSecurityRiskAssessmentSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRiskRatingQualitativeInTablesAverageRisk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssessmentDate",
                table: "RiskAssessments");

            migrationBuilder.DropColumn(
                name: "RiskRatingQualitative",
                table: "RiskAssessments");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssessmentDate",
                table: "AverageRiskPerAssetThreats",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "RiskRatingQualitative",
                table: "AverageRiskPerAssetThreats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RiskRatingQualitative",
                table: "AverageRiskPerAssets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssessmentDate",
                table: "AverageRiskPerAssetThreats");

            migrationBuilder.DropColumn(
                name: "RiskRatingQualitative",
                table: "AverageRiskPerAssetThreats");

            migrationBuilder.DropColumn(
                name: "RiskRatingQualitative",
                table: "AverageRiskPerAssets");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssessmentDate",
                table: "RiskAssessments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "RiskRatingQualitative",
                table: "RiskAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
