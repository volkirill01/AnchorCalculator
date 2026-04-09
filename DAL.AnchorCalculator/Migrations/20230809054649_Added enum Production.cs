using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.AnchorCalculator.Migrations
{
	public partial class AddedenumProduction : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "Production",
				table: "Anchors",
				type: "int",
				nullable: false,
				defaultValue: 0
			);

			migrationBuilder.AddColumn<int>(
				name: "ProductionId",
				table: "Anchors",
				type: "int",
				nullable: false,
				defaultValue: 0
			);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Production",
				table: "Anchors"
			);

			migrationBuilder.DropColumn(
				name: "ProductionId",
				table: "Anchors"
			);
		}
	}
}
