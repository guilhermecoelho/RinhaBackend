using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RinhaBackend.Migrations
{
    public partial class stack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pessoas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Apelido = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Nascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pessoas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    PessoaId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stacks_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Pessoas",
                columns: new[] { "Id", "Apelido", "Nascimento", "Nome" },
                values: new object[,]
                {
                    { new Guid("b9558519-bd25-4c55-af23-cae6acac491a"), "Apelido teste", new DateTime(2023, 8, 10, 20, 55, 7, 371, DateTimeKind.Utc).AddTicks(9357), "Nome test" },
                    { new Guid("ca24d819-c098-4579-8da3-0ad38af51160"), "Apelido teste 2", new DateTime(2023, 8, 10, 20, 55, 7, 371, DateTimeKind.Utc).AddTicks(9360), "Nome test 2" }
                });

            migrationBuilder.InsertData(
                table: "Stacks",
                columns: new[] { "Id", "Nome", "PessoaId" },
                values: new object[,]
                {
                    { new Guid("32ed1aed-a94a-4315-8d20-0a40a7c80678"), "go", new Guid("b9558519-bd25-4c55-af23-cae6acac491a") },
                    { new Guid("4226bbcd-a9ea-429a-be44-0c3c44c00667"), "c++", new Guid("ca24d819-c098-4579-8da3-0ad38af51160") },
                    { new Guid("e9eb04d8-b3e7-4c7c-ac0f-7ce1ddcbf6a5"), "c#", new Guid("b9558519-bd25-4c55-af23-cae6acac491a") },
                    { new Guid("f2d10190-dc6a-4d86-81f1-16f694bdd852"), "python", new Guid("ca24d819-c098-4579-8da3-0ad38af51160") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stacks_PessoaId",
                table: "Stacks",
                column: "PessoaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stacks");

            migrationBuilder.DropTable(
                name: "Pessoas");
        }
    }
}
