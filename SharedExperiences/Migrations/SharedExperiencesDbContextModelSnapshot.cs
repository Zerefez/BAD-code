﻿// <auto-generated />
using System;
using ExperienceService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace SharedExperiences.Migrations
{
    [DbContext(typeof(SharedExperiencesDbContext))]
    partial class SharedExperiencesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ExperienceService.Models.Billing", b =>
                {
                    b.Property<int>("BillingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BillingId"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("GuestId")
                        .HasColumnType("int");

                    b.Property<int>("ProviderId")
                        .HasColumnType("int");

                    b.HasKey("BillingId");

                    b.HasIndex("GuestId");

                    b.HasIndex("ProviderId");

                    b.ToTable("Billings");
                });

            modelBuilder.Entity("ExperienceService.Models.Discount", b =>
                {
                    b.Property<int>("DiscountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DiscountId"));

                    b.Property<decimal>("DiscountValue")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("GuestCount")
                        .HasColumnType("int");

                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.HasKey("DiscountId");

                    b.HasIndex("ServiceId")
                        .IsUnique();

                    b.ToTable("Discounts");
                });

            modelBuilder.Entity("ExperienceService.Models.Guest", b =>
                {
                    b.Property<int>("GuestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GuestId"));

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("GuestId");

                    b.ToTable("Guests");
                });

            modelBuilder.Entity("ExperienceService.Models.Provider", b =>
                {
                    b.Property<int>("ProviderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProviderId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("TouristicOperatorPermit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProviderId");

                    b.ToTable("Providers");
                });

            modelBuilder.Entity("ExperienceService.Models.Service", b =>
                {
                    b.Property<int>("ServiceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ServiceId"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("ProviderId")
                        .HasColumnType("int");

                    b.HasKey("ServiceId");

                    b.HasIndex("ProviderId");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("ExperienceService.Models.SharedExperience", b =>
                {
                    b.Property<int>("SharedExperienceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SharedExperienceId"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("SharedExperienceId");

                    b.ToTable("SharedExperiences");
                });

            modelBuilder.Entity("GuestService", b =>
                {
                    b.Property<int>("GuestsGuestId")
                        .HasColumnType("int");

                    b.Property<int>("ServicesServiceId")
                        .HasColumnType("int");

                    b.HasKey("GuestsGuestId", "ServicesServiceId");

                    b.HasIndex("ServicesServiceId");

                    b.ToTable("GuestService", (string)null);
                });

            modelBuilder.Entity("GuestSharedExperience", b =>
                {
                    b.Property<int>("GuestsGuestId")
                        .HasColumnType("int");

                    b.Property<int>("SharedExperiencesSharedExperienceId")
                        .HasColumnType("int");

                    b.HasKey("GuestsGuestId", "SharedExperiencesSharedExperienceId");

                    b.HasIndex("SharedExperiencesSharedExperienceId");

                    b.ToTable("GuestSharedExperience", (string)null);
                });

            modelBuilder.Entity("ServiceSharedExperience", b =>
                {
                    b.Property<int>("ServicesServiceId")
                        .HasColumnType("int");

                    b.Property<int>("SharedExperiencesSharedExperienceId")
                        .HasColumnType("int");

                    b.HasKey("ServicesServiceId", "SharedExperiencesSharedExperienceId");

                    b.HasIndex("SharedExperiencesSharedExperienceId");

                    b.ToTable("ServiceSharedExperience", (string)null);
                });

            modelBuilder.Entity("ExperienceService.Models.Billing", b =>
                {
                    b.HasOne("ExperienceService.Models.Guest", "Guest")
                        .WithMany("Billings")
                        .HasForeignKey("GuestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ExperienceService.Models.Provider", "Provider")
                        .WithMany("Billings")
                        .HasForeignKey("ProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guest");

                    b.Navigation("Provider");
                });

            modelBuilder.Entity("ExperienceService.Models.Discount", b =>
                {
                    b.HasOne("ExperienceService.Models.Service", "Service")
                        .WithOne("Discount")
                        .HasForeignKey("ExperienceService.Models.Discount", "ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Service");
                });

            modelBuilder.Entity("ExperienceService.Models.Service", b =>
                {
                    b.HasOne("ExperienceService.Models.Provider", "Provider")
                        .WithMany("Services")
                        .HasForeignKey("ProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Provider");
                });

            modelBuilder.Entity("GuestService", b =>
                {
                    b.HasOne("ExperienceService.Models.Guest", null)
                        .WithMany()
                        .HasForeignKey("GuestsGuestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ExperienceService.Models.Service", null)
                        .WithMany()
                        .HasForeignKey("ServicesServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GuestSharedExperience", b =>
                {
                    b.HasOne("ExperienceService.Models.Guest", null)
                        .WithMany()
                        .HasForeignKey("GuestsGuestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ExperienceService.Models.SharedExperience", null)
                        .WithMany()
                        .HasForeignKey("SharedExperiencesSharedExperienceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ServiceSharedExperience", b =>
                {
                    b.HasOne("ExperienceService.Models.Service", null)
                        .WithMany()
                        .HasForeignKey("ServicesServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ExperienceService.Models.SharedExperience", null)
                        .WithMany()
                        .HasForeignKey("SharedExperiencesSharedExperienceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ExperienceService.Models.Guest", b =>
                {
                    b.Navigation("Billings");
                });

            modelBuilder.Entity("ExperienceService.Models.Provider", b =>
                {
                    b.Navigation("Billings");

                    b.Navigation("Services");
                });

            modelBuilder.Entity("ExperienceService.Models.Service", b =>
                {
                    b.Navigation("Discount")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
