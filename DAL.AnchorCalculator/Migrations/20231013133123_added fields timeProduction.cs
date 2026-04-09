using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.AnchorCalculator.Migrations
{
	public partial class addedfieldstimeProduction : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<double>(
				name: "TimeProductionBatch",
				table: "Anchors",
				type: "double",
				nullable: false,
				defaultValue: 0.0
			);

			migrationBuilder.AddColumn<double>(
				name: "TimeProductionUnity",
				table: "Anchors",
				type: "double",
				nullable: false,
				defaultValue: 0.0
			);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "TimeProductionBatch",
				table: "Anchors"
			);

			migrationBuilder.DropColumn(
				name: "TimeProductionUnity",
				table: "Anchors"
			);
		}
	}
}
