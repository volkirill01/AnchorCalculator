using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.AnchorCalculator.Migrations
{
	public partial class addednewfieldsproductioncost : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "PriceMaterial",
				table: "Anchors",
				newName: "BatchPriceProductionThread"
			);

			migrationBuilder.AddColumn<double>(
				name: "BatchPriceProductionBandSaw",
				table: "Anchors",
				type: "double",
				nullable: false,
				defaultValue: 0.0
			);

			migrationBuilder.AddColumn<double>(
				name: "BatchPriceProductionBend",
				table: "Anchors",
				type: "double",
				nullable: false,
				defaultValue: 0.0
			);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "BatchPriceProductionBandSaw",
				table: "Anchors"
			);

			migrationBuilder.DropColumn(
				name: "BatchPriceProductionBend",
				table: "Anchors"
			);

			migrationBuilder.RenameColumn(
				name: "BatchPriceProductionThread",
				table: "Anchors",
				newName: "PriceMaterial"
			);
		}
	}
}
