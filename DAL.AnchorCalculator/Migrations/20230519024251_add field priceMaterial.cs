using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.AnchorCalculator.Migrations
{
	public partial class addfieldpriceMaterial : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<double>(
				name: "BatchPriceMaterial",
				table: "Anchors",
				type: "double",
				nullable: false,
				defaultValue: 0.0
			);

			migrationBuilder.AddColumn<double>(
				name: "PriceMaterial",
				table: "Anchors",
				type: "double",
				nullable: false,
				defaultValue: 0.0
			);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "BatchPriceMaterial",
				table: "Anchors"
			);

			migrationBuilder.DropColumn(
				name: "PriceMaterial",
				table: "Anchors"
			);
		}
	}
}
