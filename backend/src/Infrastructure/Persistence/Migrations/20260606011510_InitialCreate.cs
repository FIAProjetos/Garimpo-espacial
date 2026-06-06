using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garimpo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clusters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<int>(type: "integer", nullable: false),
                    CentroidAltitudeKm = table.Column<double>(type: "double precision", nullable: false),
                    CentroidInclinationDegrees = table.Column<double>(type: "double precision", nullable: false),
                    MemberCount = table.Column<int>(type: "integer", nullable: false),
                    Density = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clusters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "debris",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NoradId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Line1 = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Line2 = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    InclinationDegrees = table.Column<double>(type: "double precision", nullable: false),
                    Eccentricity = table.Column<double>(type: "double precision", nullable: false),
                    MeanMotionRevsPerDay = table.Column<double>(type: "double precision", nullable: false),
                    AltitudeKm = table.Column<double>(type: "double precision", nullable: false),
                    Classification = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CapturedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClusterId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_debris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_debris_clusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "clusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_debris_AltitudeKm",
                table: "debris",
                column: "AltitudeKm");

            migrationBuilder.CreateIndex(
                name: "IX_debris_ClusterId",
                table: "debris",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_debris_NoradId",
                table: "debris",
                column: "NoradId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "debris");

            migrationBuilder.DropTable(
                name: "clusters");
        }
    }
}
