using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Variants_VariantId",
                schema: "Catalog",
                table: "ProductImages");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Variants_VariantId",
                schema: "Catalog",
                table: "ProductImages",
                column: "VariantId",
                principalSchema: "Catalog",
                principalTable: "Variants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Variants_VariantId",
                schema: "Catalog",
                table: "ProductImages");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Variants_VariantId",
                schema: "Catalog",
                table: "ProductImages",
                column: "VariantId",
                principalSchema: "Catalog",
                principalTable: "Variants",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
