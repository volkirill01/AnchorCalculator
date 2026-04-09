using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.AnchorCalculator.Migrations
{
	public partial class addpathRollerfield : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<double>(
				name: "LengthPathRoller",
				table: "Anchors",
				type: "double",
				nullable: false,
				defaultValue: 0.0
			);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "LengthPathRoller",
				table: "Anchors"
			);
		}
	}
}
