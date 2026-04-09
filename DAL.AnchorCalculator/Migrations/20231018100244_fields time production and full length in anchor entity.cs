using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.AnchorCalculator.Migrations
{
	public partial class fieldstimeproductionandfulllengthinanchorentity : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "TimeProductionUnity",
				table: "Anchors",
				newName: "TimeProductionThread"
			);

			migrationBuilder.RenameColumn(
				name: "TimeProductionBatch",
				table: "Anchors",
				newName: "TimeProductionBend"
			);

			migrationBuilder.AddColumn<double>(
				name: "LengthFull",
				table: "Anchors",
				type: "double",
				nullable: false,
				defaultValue: 0.0
			);

			migrationBuilder.AddColumn<double>(
				name: "TimeProductionBandSaw",
				table: "Anchors",
				type: "double",
				nullable: false,
				defaultValue: 0.0
			);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "LengthFull",
				table: "Anchors"
			);

			migrationBuilder.DropColumn(
				name: "TimeProductionBandSaw",
				table: "Anchors"
			);

			migrationBuilder.RenameColumn(
				name: "TimeProductionThread",
				table: "Anchors",
				newName: "TimeProductionUnity"
			);

			migrationBuilder.RenameColumn(
				name: "TimeProductionBend",
				table: "Anchors",
				newName: "TimeProductionBatch"
			);
		}
	}
}
