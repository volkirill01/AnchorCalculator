using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.AnchorCalculator.Migrations
{
	public partial class addedfieldstomaterial : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<double>(
				name: "BandSawBladeLengthMeters",
				table: "Materials",
				type: "double",
				nullable: false,
				defaultValue: 0.0
			);

			migrationBuilder.AddColumn<double>(
				name: "BandSawHours",
				table: "Materials",
				type: "double",
				nullable: false,
				defaultValue: 0.0
			);

			migrationBuilder.AddColumn<double>(
				name: "TheradRollingHours",
				table: "Materials",
				type: "double",
				nullable: false,
				defaultValue: 0.0
			);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "BandSawBladeLengthMeters",
				table: "Materials"
			);

			migrationBuilder.DropColumn(
				name: "BandSawHours",
				table: "Materials"
			);

			migrationBuilder.DropColumn(
				name: "TheradRollingHours",
				table: "Materials"
			);
		}
	}
}
