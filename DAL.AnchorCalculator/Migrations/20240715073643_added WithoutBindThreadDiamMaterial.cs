using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.AnchorCalculator.Migrations
{
	public partial class addedWithoutBindThreadDiamMaterial : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "WithoutBindThreadDiamMaterial",
				table: "Anchors",
				type: "tinyint(1)",
				nullable: false,
				defaultValue: false
			);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "WithoutBindThreadDiamMaterial",
				table: "Anchors"
			);
		}
	}
}
