using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.AnchorCalculator.Migrations
{
	public partial class deletedfieldprodthread : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "BatchPriceProductionThread",
				table: "Anchors"
			);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<double>(
				name: "BatchPriceProductionThread",
				table: "Anchors",
				type: "double",
				nullable: false,
				defaultValue: 0.0
			);
		}
	}
}
