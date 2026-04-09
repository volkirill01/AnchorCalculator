using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.AnchorCalculator.Migrations
{
	public partial class floatthreadStep : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<float>(
				name: "ThreadStep",
				table: "Anchors",
				type: "float",
				nullable: false,
				oldClrType: typeof(double),
				oldType: "double"
			);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<double>(
				name: "ThreadStep",
				table: "Anchors",
				type: "double",
				nullable: false,
				oldClrType: typeof(float),
				oldType: "float"
			);
		}
	}
}
