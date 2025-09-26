using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicaDeNegocio.Migrations
{
    /// <inheritdoc />
    public partial class FixDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Clientes_HuespedId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Empleados_EmpleadoId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Habitaciones_HabitacionId",
                table: "Reservas");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Clientes_HuespedId",
                table: "Reservas",
                column: "HuespedId",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Empleados_EmpleadoId",
                table: "Reservas",
                column: "EmpleadoId",
                principalTable: "Empleados",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Habitaciones_HabitacionId",
                table: "Reservas",
                column: "HabitacionId",
                principalTable: "Habitaciones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Clientes_HuespedId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Empleados_EmpleadoId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Habitaciones_HabitacionId",
                table: "Reservas");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Clientes_HuespedId",
                table: "Reservas",
                column: "HuespedId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Empleados_EmpleadoId",
                table: "Reservas",
                column: "EmpleadoId",
                principalTable: "Empleados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Habitaciones_HabitacionId",
                table: "Reservas",
                column: "HabitacionId",
                principalTable: "Habitaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
