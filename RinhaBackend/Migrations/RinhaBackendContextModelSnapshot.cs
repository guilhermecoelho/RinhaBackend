﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RinhaBackend.Data;

#nullable disable

namespace RinhaBackend.Migrations
{
    [DbContext(typeof(RinhaBackendContext))]
    partial class RinhaBackendContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.21")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("RinhaBackend.Models.PessoasModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Apelido")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<DateTime>("Nascimento")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.ToTable("Pessoas", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("b9558519-bd25-4c55-af23-cae6acac491a"),
                            Apelido = "Apelido teste",
                            Nascimento = new DateTime(2023, 8, 10, 20, 55, 7, 371, DateTimeKind.Utc).AddTicks(9357),
                            Nome = "Nome test"
                        },
                        new
                        {
                            Id = new Guid("ca24d819-c098-4579-8da3-0ad38af51160"),
                            Apelido = "Apelido teste 2",
                            Nascimento = new DateTime(2023, 8, 10, 20, 55, 7, 371, DateTimeKind.Utc).AddTicks(9360),
                            Nome = "Nome test 2"
                        });
                });

            modelBuilder.Entity("RinhaBackend.Models.StackModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("PessoaId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("PessoaId");

                    b.ToTable("Stacks", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("e9eb04d8-b3e7-4c7c-ac0f-7ce1ddcbf6a5"),
                            Nome = "c#",
                            PessoaId = new Guid("b9558519-bd25-4c55-af23-cae6acac491a")
                        },
                        new
                        {
                            Id = new Guid("32ed1aed-a94a-4315-8d20-0a40a7c80678"),
                            Nome = "go",
                            PessoaId = new Guid("b9558519-bd25-4c55-af23-cae6acac491a")
                        },
                        new
                        {
                            Id = new Guid("f2d10190-dc6a-4d86-81f1-16f694bdd852"),
                            Nome = "python",
                            PessoaId = new Guid("ca24d819-c098-4579-8da3-0ad38af51160")
                        },
                        new
                        {
                            Id = new Guid("4226bbcd-a9ea-429a-be44-0c3c44c00667"),
                            Nome = "c++",
                            PessoaId = new Guid("ca24d819-c098-4579-8da3-0ad38af51160")
                        });
                });

            modelBuilder.Entity("RinhaBackend.Models.StackModel", b =>
                {
                    b.HasOne("RinhaBackend.Models.PessoasModel", "Pessoa")
                        .WithMany("Stacks")
                        .HasForeignKey("PessoaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pessoa");
                });

            modelBuilder.Entity("RinhaBackend.Models.PessoasModel", b =>
                {
                    b.Navigation("Stacks");
                });
#pragma warning restore 612, 618
        }
    }
}
