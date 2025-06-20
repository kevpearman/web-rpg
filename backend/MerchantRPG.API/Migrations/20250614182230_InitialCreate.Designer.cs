﻿// <auto-generated />
using System;
using MerchantRPG.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MerchantRPG.API.Migrations
{
    [DbContext(typeof(GameDbContext))]
    [Migration("20250614182230_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MerchantRPG.API.Models.Adventurer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Agility")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Experience")
                        .HasColumnType("integer");

                    b.Property<int>("Intelligence")
                        .HasColumnType("integer");

                    b.Property<bool>("IsOnMission")
                        .HasColumnType("boolean");

                    b.Property<int>("Level")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("PlayerId")
                        .HasColumnType("integer");

                    b.Property<int>("Strength")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId", "Name");

                    b.ToTable("Adventurers");
                });

            modelBuilder.Entity("MerchantRPG.API.Models.GameMap", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Difficulty")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<int>("Duration")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("RequiredLevel")
                        .HasColumnType("integer");

                    b.Property<string>("RewardRanges")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UnlocksMapId")
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("UnlocksMapId");

                    b.ToTable("GameMaps");

                    b.HasData(
                        new
                        {
                            Id = "forest_clearing",
                            Difficulty = "easy",
                            Duration = 300,
                            Name = "Forest Clearing",
                            RequiredLevel = 1,
                            RewardRanges = "{\"gold\": {\"min\": 10, \"max\": 25}, \"materials\": [{\"type\": \"wood\", \"min\": 1, \"max\": 3}]}",
                            UnlocksMapId = "goblin_camp"
                        },
                        new
                        {
                            Id = "goblin_camp",
                            Difficulty = "easy",
                            Duration = 600,
                            Name = "Goblin Camp",
                            RequiredLevel = 2,
                            RewardRanges = "{\"gold\": {\"min\": 20, \"max\": 40}, \"materials\": [{\"type\": \"iron_ore\", \"min\": 1, \"max\": 2}, {\"type\": \"leather\", \"min\": 1, \"max\": 2}]}",
                            UnlocksMapId = "dark_cave"
                        },
                        new
                        {
                            Id = "dark_cave",
                            Difficulty = "medium",
                            Duration = 900,
                            Name = "Dark Cave",
                            RequiredLevel = 5,
                            RewardRanges = "{\"gold\": {\"min\": 50, \"max\": 80}, \"materials\": [{\"type\": \"gems\", \"min\": 1, \"max\": 2}, {\"type\": \"rare_metals\", \"min\": 1, \"max\": 1}]}"
                        });
                });

            modelBuilder.Entity("MerchantRPG.API.Models.Mission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AdventurerId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Duration")
                        .HasColumnType("integer");

                    b.Property<string>("MapId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Rewards")
                        .HasColumnType("text");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.HasKey("Id");

                    b.HasIndex("StartTime");

                    b.HasIndex("AdventurerId", "Status");

                    b.ToTable("Missions");
                });

            modelBuilder.Entity("MerchantRPG.API.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Gold")
                        .HasColumnType("integer");

                    b.Property<DateTime>("LastActive")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Players");
                });

            modelBuilder.Entity("MerchantRPG.API.Models.PlayerResource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("PlayerId")
                        .HasColumnType("integer");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<string>("ResourceType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId", "ResourceType")
                        .IsUnique();

                    b.ToTable("PlayerResources");
                });

            modelBuilder.Entity("MerchantRPG.API.Models.Adventurer", b =>
                {
                    b.HasOne("MerchantRPG.API.Models.Player", "Player")
                        .WithMany("Adventurers")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("MerchantRPG.API.Models.GameMap", b =>
                {
                    b.HasOne("MerchantRPG.API.Models.GameMap", "UnlocksMap")
                        .WithMany()
                        .HasForeignKey("UnlocksMapId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("UnlocksMap");
                });

            modelBuilder.Entity("MerchantRPG.API.Models.Mission", b =>
                {
                    b.HasOne("MerchantRPG.API.Models.Adventurer", "Adventurer")
                        .WithMany("Missions")
                        .HasForeignKey("AdventurerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Adventurer");
                });

            modelBuilder.Entity("MerchantRPG.API.Models.PlayerResource", b =>
                {
                    b.HasOne("MerchantRPG.API.Models.Player", "Player")
                        .WithMany("Resources")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("MerchantRPG.API.Models.Adventurer", b =>
                {
                    b.Navigation("Missions");
                });

            modelBuilder.Entity("MerchantRPG.API.Models.Player", b =>
                {
                    b.Navigation("Adventurers");

                    b.Navigation("Resources");
                });
#pragma warning restore 612, 618
        }
    }
}
