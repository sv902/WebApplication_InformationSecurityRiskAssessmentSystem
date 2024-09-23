using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication_InformationSecurityRiskAssessmentSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateBaseTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Owner = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Threats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Threats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetThreats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    ThreatId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetThreats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetThreats_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetThreats_Threats_ThreatId",
                        column: x => x.ThreatId,
                        principalTable: "Threats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RiskAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    ThreatId = table.Column<int>(type: "int", nullable: false),
                    Risk = table.Column<float>(type: "real", nullable: false),
                    AssessmentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskAssessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RiskAssessments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RiskAssessments_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RiskAssessments_Threats_ThreatId",
                        column: x => x.ThreatId,
                        principalTable: "Threats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThreatAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    ThreatId = table.Column<int>(type: "int", nullable: false),
                    Likelihood = table.Column<float>(type: "real", nullable: false),
                    Consequences = table.Column<float>(type: "real", nullable: false),
                    AssessmentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreatAssessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThreatAssessments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThreatAssessments_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThreatAssessments_Threats_ThreatId",
                        column: x => x.ThreatId,
                        principalTable: "Threats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetThreats_AssetId",
                table: "AssetThreats",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetThreats_ThreatId",
                table: "AssetThreats",
                column: "ThreatId");

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessments_AssetId",
                table: "RiskAssessments",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessments_ThreatId",
                table: "RiskAssessments",
                column: "ThreatId");

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessments_UserId",
                table: "RiskAssessments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreatAssessments_AssetId",
                table: "ThreatAssessments",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreatAssessments_ThreatId",
                table: "ThreatAssessments",
                column: "ThreatId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreatAssessments_UserId",
                table: "ThreatAssessments",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetThreats");

            migrationBuilder.DropTable(
                name: "RiskAssessments");

            migrationBuilder.DropTable(
                name: "ThreatAssessments");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "Threats");
        }
    }
}
