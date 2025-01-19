﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shopp.Services.ProductAPI.Data;

#nullable disable

namespace Shopp.Services.ProductAPI.Migrations
{
    [DbContext(typeof(ProductDbContext))]
    partial class ProductDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Shopp.Services.ProductAPI.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageLocalPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.HasKey("ProductId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            ProductId = 1,
                            CategoryName = "Mouse",
                            Description = "An ergonomic mouse for less joint pain",
                            ImageUrl = "https://placehold.co/603x403",
                            Name = "Ergonomic Mouse",
                            Price = 50.990000000000002
                        },
                        new
                        {
                            ProductId = 2,
                            CategoryName = "Keyboard",
                            Description = "Top tier gaming keyboard",
                            ImageUrl = "https://placehold.co/602x402",
                            Name = "Gaming Keyboard",
                            Price = 133.99000000000001
                        },
                        new
                        {
                            ProductId = 3,
                            CategoryName = "Monitor",
                            Description = "Gaming curved monitor X\" and refresh rate of YYY Mhz",
                            ImageUrl = "https://placehold.co/601x401",
                            Name = "Monitor",
                            Price = 299.99000000000001
                        },
                        new
                        {
                            ProductId = 4,
                            CategoryName = "Headphones",
                            Description = "Basic office headphones",
                            ImageUrl = "https://placehold.co/600x400",
                            Name = "Office Headphones",
                            Price = 15.0
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
