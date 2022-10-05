﻿// <auto-generated />
using System;
using CommercialClothes.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CommercialClothes.Migrations
{
    [DbContext(typeof(ECommerceSellingClothesContext))]
    partial class ECommerceSellingClothesContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("CommercialClothes.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<Guid>("ActivationCode")
                        .HasColumnType("uuid");

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActivated")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<Guid>("ResetPasswordCode")
                        .HasColumnType("uuid");

                    b.Property<int?>("ShopId")
                        .HasColumnType("integer");

                    b.Property<int?>("UserGroupId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ShopId");

                    b.HasIndex("UserGroupId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("CommercialClothes.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("Gender")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.Property<int?>("ShopId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ShopId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("CommercialClothes.Models.Credential", b =>
                {
                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.Property<int>("UserGroupId")
                        .HasColumnType("integer");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserGroupId");

                    b.ToTable("Credentials");
                });

            modelBuilder.Entity("CommercialClothes.Models.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ItemId")
                        .HasColumnType("integer");

                    b.Property<string>("Path")
                        .HasColumnType("text");

                    b.Property<int?>("ShopId")

                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("ShopId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("CommercialClothes.Models.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("Price")
                        .HasColumnType("integer");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<int>("ShopId")
                        .HasColumnType("integer");

                    b.Property<string>("Size")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ShopId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("CommercialClothes.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AccountId")
                        .HasColumnType("integer");

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("PaymentId")
                        .HasColumnType("integer");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<int>("StatusId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("PaymentId");

                    b.HasIndex("StatusId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("CommercialClothes.Models.OrderDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ItemId")
                        .HasColumnType("integer");

                    b.Property<int>("OrderId")
                        .HasColumnType("integer");

                    b.Property<int?>("Quantity")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("CommercialClothes.Models.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Type")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("CommercialClothes.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("CommercialClothes.Models.Shop", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("PhomeNumber")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Shops");
                });

            modelBuilder.Entity("CommercialClothes.Models.Status", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Statuses");
                });

            modelBuilder.Entity("CommercialClothes.Models.UserGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("UserGroups");
                });

            modelBuilder.Entity("CommercialClothes.Models.Account", b =>
                {
                    b.HasOne("CommercialClothes.Models.Shop", "Shop")
                        .WithMany("Accounts")
                        .HasForeignKey("ShopId");

                    b.HasOne("CommercialClothes.Models.UserGroup", "UserGroup")
                        .WithMany("Accounts")
                        .HasForeignKey("UserGroupId");

                    b.Navigation("Shop");

                    b.Navigation("UserGroup");
                });

            modelBuilder.Entity("CommercialClothes.Models.Category", b =>
                {
                    b.HasOne("CommercialClothes.Models.Shop", "Shop")
                        .WithMany("Categories")
                        .HasForeignKey("ShopId");

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("CommercialClothes.Models.Credential", b =>
                {
                    b.HasOne("CommercialClothes.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CommercialClothes.Models.UserGroup", "UserGroup")
                        .WithMany()
                        .HasForeignKey("UserGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("UserGroup");
                });

            modelBuilder.Entity("CommercialClothes.Models.Image", b =>
                {
                    b.HasOne("CommercialClothes.Models.Item", "Item")
                        .WithMany("Images")
                        .HasForeignKey("ItemId");

                    b.HasOne("CommercialClothes.Models.Shop", "Shop")
                        .WithMany("Images")
                        .HasForeignKey("ShopId");

                    b.Navigation("Item");

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("CommercialClothes.Models.Item", b =>
                {
                    b.HasOne("CommercialClothes.Models.Category", "Category")
                        .WithMany("Items")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.HasOne("CommercialClothes.Models.Shop", "Shop")

                        .WithMany("Items")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("CommercialClothes.Models.Order", b =>
                {
                    b.HasOne("CommercialClothes.Models.Account", "Account")
                        .WithMany("Ordereds")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CommercialClothes.Models.Payment", "Payment")
                        .WithMany("Ordereds")
                        .HasForeignKey("PaymentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CommercialClothes.Models.Status", "Status")
                        .WithMany("Orders")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Payment");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("CommercialClothes.Models.OrderDetail", b =>
                {
                    b.HasOne("CommercialClothes.Models.Item", "Item")
                        .WithMany("OrderDetails")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CommercialClothes.Models.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("CommercialClothes.Models.Payment", b =>
                {
                    b.HasOne("CommercialClothes.Models.Shop", "Shop")
                        .WithOne("Payment")
                        .HasForeignKey("CommercialClothes.Models.Payment", "Id")
                        .HasConstraintName("FK_Payment_Shop")
                        .IsRequired();

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("CommercialClothes.Models.Account", b =>
                {
                    b.Navigation("Ordereds");
                });

            modelBuilder.Entity("CommercialClothes.Models.Category", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("CommercialClothes.Models.Item", b =>
                {
                    b.Navigation("Images");

                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("CommercialClothes.Models.Order", b =>
                {
                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("CommercialClothes.Models.Payment", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("CommercialClothes.Models.Shop", b =>
                {
                    b.Navigation("Accounts");

                    b.Navigation("Categories");

                    b.Navigation("Images");

                    b.Navigation("Items");

                    b.Navigation("Payment");

                });

            modelBuilder.Entity("CommercialClothes.Models.Status", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("CommercialClothes.Models.UserGroup", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}
