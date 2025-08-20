using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProyectTemplate.Migrations
{
    /// <inheritdoc />
    public partial class AddReservasTPT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservasAlojamiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    AlojamientoId = table.Column<int>(type: "integer", nullable: false),
                    CantidadPersonas = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservasAlojamiento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservasAlojamiento_Alojamientos_AlojamientoId",
                        column: x => x.AlojamientoId,
                        principalTable: "Alojamientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservasAlojamiento_Reservas_Id",
                        column: x => x.Id,
                        principalTable: "Reservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReservasHotel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    HotelId = table.Column<int>(type: "integer", nullable: false),
                    CantidadHabitaciones = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservasHotel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservasHotel_Hoteles_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hoteles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservasHotel_Reservas_Id",
                        column: x => x.Id,
                        principalTable: "Reservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_Estado",
                table: "Reservas",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_FechaInicio_FechaFin",
                table: "Reservas",
                columns: new[] { "FechaInicio", "FechaFin" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_UsuarioId",
                table: "Reservas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservasAlojamiento_AlojamientoId",
                table: "ReservasAlojamiento",
                column: "AlojamientoId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservasHotel_HotelId",
                table: "ReservasHotel",
                column: "HotelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservasAlojamiento");

            migrationBuilder.DropTable(
                name: "ReservasHotel");

            migrationBuilder.DropTable(
                name: "Reservas");
        }
    }
}
