using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.AnchorCalculator.Migrations
{
	public partial class newdb : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterDatabase()
				.Annotation("MySql:CharSet", "utf8mb4");

			migrationBuilder.CreateTable(
				name: "AspNetRoles",
				columns: table => new
				{
					Id = table.Column<string>(type: "varchar(255)", nullable: false)
						.Annotation("MySql:CharSet", "utf8mb4"),
					Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4"),
					NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4"),
					ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4")
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetRoles", x => x.Id);
				}
			).Annotation("MySql:CharSet", "utf8mb4");

			migrationBuilder.CreateTable(
				name: "AspNetUsers",
				columns: table => new
				{
					Id = table.Column<string>(type: "varchar(255)", nullable: false)
						.Annotation("MySql:CharSet", "utf8mb4"),
					UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4"),
					NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4"),
					Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4"),
					NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4"),
					EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
					PasswordHash = table.Column<string>(type: "longtext", nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4"),
					SecurityStamp = table.Column<string>(type: "longtext", nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4"),
					ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4"),
					PhoneNumber = table.Column<string>(type: "longtext", nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4"),
					PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
					TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
					LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
					LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
					AccessFailedCount = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUsers", x => x.Id);
				}
			).Annotation("MySql:CharSet", "utf8mb4");

			migrationBuilder.CreateTable(
				name: "Materials",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					Name = table.Column<string>(type: "longtext", nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4"),
					Size = table.Column<double>(type: "double", nullable: false),
					TypeId = table.Column<int>(type: "int", nullable: false),
					Type = table.Column<int>(type: "int", nullable: false),
					PricePerMeter = table.Column<double>(type: "double", nullable: false),
					DateUpdate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Materials", x => x.Id);
				}
			).Annotation("MySql:CharSet", "utf8mb4");

			migrationBuilder.CreateTable(
				name: "AspNetRoleClaims",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
						.Annotation("MySql:CharSet", "utf8mb4"),
					ClaimType = table.Column<string>(type: "longtext", nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4"),
					ClaimValue = table.Column<string>(type: "longtext", nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4")
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
					table.ForeignKey(
						name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
						column: x => x.RoleId,
						principalTable: "AspNetRoles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				}
			).Annotation("MySql:CharSet", "utf8mb4");

			migrationBuilder.CreateTable(
				name: "AspNetUserClaims",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					UserId = table.Column<string>(type: "varchar(255)", nullable: false)
						.Annotation("MySql:CharSet", "utf8mb4"),
					ClaimType = table.Column<string>(type: "longtext", nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4"),
					ClaimValue = table.Column<string>(type: "longtext", nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4")
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
					table.ForeignKey(
						name: "FK_AspNetUserClaims_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				}
			).Annotation("MySql:CharSet", "utf8mb4");

			migrationBuilder.CreateTable(
				name: "AspNetUserLogins",
				columns: table => new
				{
					LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
						.Annotation("MySql:CharSet", "utf8mb4"),
					ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false)
						.Annotation("MySql:CharSet", "utf8mb4"),
					ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4"),
					UserId = table.Column<string>(type: "varchar(255)", nullable: false)
						.Annotation("MySql:CharSet", "utf8mb4")
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
					table.ForeignKey(
						name: "FK_AspNetUserLogins_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				}
			).Annotation("MySql:CharSet", "utf8mb4");

			migrationBuilder.CreateTable(
				name: "AspNetUserRoles",
				columns: table => new
				{
					UserId = table.Column<string>(type: "varchar(255)", nullable: false)
						.Annotation("MySql:CharSet", "utf8mb4"),
					RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
						.Annotation("MySql:CharSet", "utf8mb4")
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
					table.ForeignKey(
						name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
						column: x => x.RoleId,
						principalTable: "AspNetRoles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_AspNetUserRoles_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				}
			).Annotation("MySql:CharSet", "utf8mb4");

			migrationBuilder.CreateTable(
				name: "AspNetUserTokens",
				columns: table => new
				{
					UserId = table.Column<string>(type: "varchar(255)", nullable: false)
						.Annotation("MySql:CharSet", "utf8mb4"),
					LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
						.Annotation("MySql:CharSet", "utf8mb4"),
					Name = table.Column<string>(type: "varchar(255)", nullable: false)
						.Annotation("MySql:CharSet", "utf8mb4"),
					Value = table.Column<string>(type: "longtext", nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4")
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
					table.ForeignKey(
						name: "FK_AspNetUserTokens_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				}
			).Annotation("MySql:CharSet", "utf8mb4");

			migrationBuilder.CreateTable(
				name: "Anchors",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
					Length = table.Column<int>(type: "int", nullable: false),
					Diameter = table.Column<float>(type: "float", nullable: false),
					Weight = table.Column<double>(type: "double", nullable: false),
					Sebes = table.Column<double>(type: "double", nullable: false),
					BatchSebes = table.Column<double>(type: "double", nullable: false),
					Price = table.Column<double>(type: "double", nullable: false),
					BendLength = table.Column<int>(type: "int", nullable: false),
					BendRadius = table.Column<int>(type: "int", nullable: false),
					ThreadLength = table.Column<int>(type: "int", nullable: false),
					ThreadDiameter = table.Column<int>(type: "int", nullable: false),
					ThreadStep = table.Column<double>(type: "double", nullable: false),
					Amount = table.Column<double>(type: "double", nullable: false),
					Quantity = table.Column<int>(type: "int", nullable: false),
					DateCreate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
					SvgElement = table.Column<string>(type: "longtext", nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4"),
					BatchWeight = table.Column<double>(type: "double", nullable: false),
					BilletLength = table.Column<double>(type: "double", nullable: false),
					MaterialJson = table.Column<string>(type: "longtext", nullable: false)
						.Annotation("MySql:CharSet", "utf8mb4"),
					UserJson = table.Column<string>(type: "longtext", nullable: false)
						.Annotation("MySql:CharSet", "utf8mb4"),
					KindId = table.Column<int>(type: "int", nullable: false),
					Kind = table.Column<int>(type: "int", nullable: false),
					PricePerMeter = table.Column<double>(type: "double", nullable: false),
					MaterialId = table.Column<int>(type: "int", nullable: true),
					UserId = table.Column<string>(type: "varchar(255)", nullable: true)
						.Annotation("MySql:CharSet", "utf8mb4")
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Anchors", x => x.Id);
					table.ForeignKey(
						name: "FK_Anchors_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.SetNull);
					table.ForeignKey(
						name: "FK_Anchors_Materials_MaterialId",
						column: x => x.MaterialId,
						principalTable: "Materials",
						principalColumn: "Id",
						onDelete: ReferentialAction.SetNull);
				}
			).Annotation("MySql:CharSet", "utf8mb4");

			migrationBuilder.CreateIndex(
				name: "IX_Anchors_MaterialId",
				table: "Anchors",
				column: "MaterialId"
			);

			migrationBuilder.CreateIndex(
				name: "IX_Anchors_UserId",
				table: "Anchors",
				column: "UserId"
			);

			migrationBuilder.CreateIndex(
				name: "IX_AspNetRoleClaims_RoleId",
				table: "AspNetRoleClaims",
				column: "RoleId"
			);

			migrationBuilder.CreateIndex(
				name: "RoleNameIndex",
				table: "AspNetRoles",
				column: "NormalizedName",
				unique: true
			);

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserClaims_UserId",
				table: "AspNetUserClaims",
				column: "UserId"
			);

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserLogins_UserId",
				table: "AspNetUserLogins",
				column: "UserId"
			);

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserRoles_RoleId",
				table: "AspNetUserRoles",
				column: "RoleId"
			);

			migrationBuilder.CreateIndex(
				name: "EmailIndex",
				table: "AspNetUsers",
				column: "NormalizedEmail"
			);

			migrationBuilder.CreateIndex(
				name: "UserNameIndex",
				table: "AspNetUsers",
				column: "NormalizedUserName",
				unique: true
			);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Anchors"
			);

			migrationBuilder.DropTable(
				name: "AspNetRoleClaims"
			);

			migrationBuilder.DropTable(
				name: "AspNetUserClaims"
			);

			migrationBuilder.DropTable(
				name: "AspNetUserLogins"
			);

			migrationBuilder.DropTable(
				name: "AspNetUserRoles"
			);

			migrationBuilder.DropTable(
				name: "AspNetUserTokens"
			);

			migrationBuilder.DropTable(
				name: "Materials"
			);

			migrationBuilder.DropTable(
				name: "AspNetRoles"
			);

			migrationBuilder.DropTable(
				name: "AspNetUsers"
			);
		}
	}
}
