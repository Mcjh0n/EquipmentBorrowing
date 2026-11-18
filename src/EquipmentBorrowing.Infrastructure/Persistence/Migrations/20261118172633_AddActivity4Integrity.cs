using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquipmentBorrowing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddActivity4Integrity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Students_Contact_NotBlank",
                table: "Students",
                sql: "\"ContactInformation\" IS NULL OR length(btrim(\"ContactInformation\")) > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Students_Name_NotBlank",
                table: "Students",
                sql: "length(btrim(\"Name\")) > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Students_StudentNumber_Normalized",
                table: "Students",
                sql: "\"StudentNumberNormalized\" = upper(btrim(\"StudentNumber\"))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Equipment_Description_NotBlank",
                table: "Equipment",
                sql: "\"Description\" IS NULL OR length(btrim(\"Description\")) > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Equipment_Identifier_Normalized",
                table: "Equipment",
                sql: "\"EquipmentIdentifierNormalized\" = upper(btrim(\"EquipmentIdentifier\"))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Equipment_Name_NotBlank",
                table: "Equipment",
                sql: "length(btrim(\"Name\")) > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Borrowings_Due_After_Checkout",
                table: "Borrowings",
                sql: "\"ExpectedReturnDate\" > \"DateBorrowed\"");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Borrowings_Return_After_Checkout",
                table: "Borrowings",
                sql: "\"ReturnedDate\" IS NULL OR \"ReturnedDate\" >= \"DateBorrowed\"");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Borrowings_Return_Status",
                table: "Borrowings",
                sql: "(\"Status\" = 0 AND \"ReturnedDate\" IS NULL) OR (\"Status\" = 1 AND \"ReturnedDate\" IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Students_Contact_NotBlank",
                table: "Students");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Students_Name_NotBlank",
                table: "Students");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Students_StudentNumber_Normalized",
                table: "Students");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Equipment_Description_NotBlank",
                table: "Equipment");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Equipment_Identifier_Normalized",
                table: "Equipment");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Equipment_Name_NotBlank",
                table: "Equipment");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Borrowings_Due_After_Checkout",
                table: "Borrowings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Borrowings_Return_After_Checkout",
                table: "Borrowings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Borrowings_Return_Status",
                table: "Borrowings");
        }
    }
}
