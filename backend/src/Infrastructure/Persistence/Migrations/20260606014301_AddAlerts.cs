using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garimpo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Severity = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    TriggeredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAcknowledged = table.Column<bool>(type: "boolean", nullable: false),
                    AcknowledgedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    discriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    ClusterId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClusterLabel = table.Column<int>(type: "integer", nullable: true),
                    Density = table.Column<double>(type: "double precision", nullable: true),
                    CentroidAltitudeKm = table.Column<double>(type: "double precision", nullable: true),
                    MemberCount = table.Column<int>(type: "integer", nullable: true),
                    SensorId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    RejectedRecords = table.Column<int>(type: "integer", nullable: true),
                    TotalRecords = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alerts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alerts_Severity",
                table: "alerts",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_alerts_TriggeredAt",
                table: "alerts",
                column: "TriggeredAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerts");
        }
    }
}
