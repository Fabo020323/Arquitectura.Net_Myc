using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectTemplate.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHotel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Hoteles_Ciudad_Pais",
                table: "Hoteles");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Hoteles");

            migrationBuilder.DropColumn(
                name: "Ciudad",
                table: "Hoteles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Hoteles");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Hoteles");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Hoteles");

            migrationBuilder.DropColumn(
                name: "Pais",
                table: "Hoteles");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Hoteles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Hoteles");

            migrationBuilder.RenameColumn(
                name: "Estrellas",
                table: "Hoteles",
                newName: "CantidadHabitaciones");

            migrationBuilder.RenameColumn(
                name: "Direccion",
                table: "Hoteles",
                newName: "Ubicacion");

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioPorNoche",
                table: "Hoteles",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecioPorNoche",
                table: "Hoteles");

            migrationBuilder.RenameColumn(
                name: "Ubicacion",
                table: "Hoteles",
                newName: "Direccion");

            migrationBuilder.RenameColumn(
                name: "CantidadHabitaciones",
                table: "Hoteles",
                newName: "Estrellas");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Hoteles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Ciudad",
                table: "Hoteles",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Hoteles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Hoteles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Hoteles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pais",
                table: "Hoteles",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Hoteles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Hoteles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hoteles_Ciudad_Pais",
                table: "Hoteles",
                columns: new[] { "Ciudad", "Pais" });
        }
    }
}
