using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.AnchorCalculator.Migrations
{
	public partial class addednewfieldssecondThread : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<double>(
				name: "LengthBeforeEndPathRoller",
				table: "Anchors",
				type: "double",
				nullable: false,
				defaultValue: 0.0
			);

			migrationBuilder.AddColumn<int>(
				name: "ThreadSecondLengthMillimeters",
				table: "Anchors",
				type: "int",
				nullable: false,
				defaultValue: 0
			);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "LengthBeforeEndPathRoller",
				table: "Anchors"
			);

			migrationBuilder.DropColumn(
				name: "ThreadSecondLengthMillimeters",
				table: "Anchors"
			);
		}
	}
}
