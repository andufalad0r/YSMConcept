﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using YSMConcept.Infrastructure.Data;

#nullable disable

namespace YSMConcept.Infrastructure.Migrations
{
    [DbContext(typeof(YsmDbContext))]
    partial class YsmDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("YSMConcept.Domain.Entities.ImageEntity", b =>
                {
                    b.Property<string>("ImageId")
                        .HasColumnType("text")
                        .HasColumnName("image_id");

                    b.Property<string>("ImageURL")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("image_url");

                    b.Property<bool>("IsMain")
                        .HasColumnType("boolean")
                        .HasColumnName("is_main");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uuid")
                        .HasColumnName("project_id");

                    b.HasKey("ImageId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("YSMConcept.Domain.Entities.Project", b =>
                {
                    b.Property<Guid>("ProjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("project_id");

                    b.Property<int>("Area")
                        .HasColumnType("integer")
                        .HasColumnName("area");

                    b.Property<string>("BuildingType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("building_type");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("ProjectId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("YSMConcept.Domain.Entities.ImageEntity", b =>
                {
                    b.HasOne("YSMConcept.Domain.Entities.Project", "Project")
                        .WithMany("CollectionImages")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("YSMConcept.Domain.Entities.Project", b =>
                {
                    b.OwnsOne("YSMConcept.Domain.ValueObjects.Address", "Address", b1 =>
                        {
                            b1.Property<Guid>("ProjectId")
                                .HasColumnType("uuid");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasMaxLength(18)
                                .HasColumnType("character varying(18)")
                                .HasColumnName("city");

                            b1.Property<string>("Street")
                                .IsRequired()
                                .HasMaxLength(40)
                                .HasColumnType("character varying(40)")
                                .HasColumnName("street");

                            b1.HasKey("ProjectId");

                            b1.ToTable("Projects");

                            b1.WithOwner()
                                .HasForeignKey("ProjectId");
                        });

                    b.OwnsOne("YSMConcept.Domain.ValueObjects.Date", "Date", b1 =>
                        {
                            b1.Property<Guid>("ProjectId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Month")
                                .HasColumnType("integer")
                                .HasColumnName("month");

                            b1.Property<int>("Year")
                                .HasColumnType("integer")
                                .HasColumnName("year");

                            b1.HasKey("ProjectId");

                            b1.ToTable("Projects");

                            b1.WithOwner()
                                .HasForeignKey("ProjectId");
                        });

                    b.Navigation("Address")
                        .IsRequired();

                    b.Navigation("Date")
                        .IsRequired();
                });

            modelBuilder.Entity("YSMConcept.Domain.Entities.Project", b =>
                {
                    b.Navigation("CollectionImages");
                });
#pragma warning restore 612, 618
        }
    }
}
