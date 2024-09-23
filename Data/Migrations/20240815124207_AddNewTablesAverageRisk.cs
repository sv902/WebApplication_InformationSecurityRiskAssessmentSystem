using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication_InformationSecurityRiskAssessmentSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTablesAverageRisk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AverageRiskPerAssets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    AverageRisk = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AverageRiskPerAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AverageRiskPerAssets_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AverageRiskPerAssetThreats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    ThreatId = table.Column<int>(type: "int", nullable: false),
                    AverageRisk = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AverageRiskPerAssetThreats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AverageRiskPerAssetThreats_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AverageRiskPerAssetThreats_Threats_ThreatId",
                        column: x => x.ThreatId,
                        principalTable: "Threats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AverageRiskPerAssets_AssetId",
                table: "AverageRiskPerAssets",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AverageRiskPerAssetThreats_AssetId",
                table: "AverageRiskPerAssetThreats",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AverageRiskPerAssetThreats_ThreatId",
                table: "AverageRiskPerAssetThreats",
                column: "ThreatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AverageRiskPerAssets");

            migrationBuilder.DropTable(
                name: "AverageRiskPerAssetThreats");
        }
    }
}
