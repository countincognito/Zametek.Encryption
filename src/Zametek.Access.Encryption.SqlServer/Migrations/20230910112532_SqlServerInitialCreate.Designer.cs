﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zametek.Access.Encryption;

#nullable disable

namespace Zametek.Access.Encryption.Migrations
{
    [DbContext(typeof(EncryptionDbContext))]
    [Migration("20230910112532_SqlServerInitialCreate")]
    partial class SqlServerInitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Zametek.Access.Encryption.SymmetricKey", b =>
                {
                    b.Property<Guid>("SymmetricKeyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AsymmetricKeyId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AsymmetricKeyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AsymmetricKeyVersion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("InitializationVector")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("ModifiedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("SymmetricKeyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WrappedSymmetricKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SymmetricKeyId", "AsymmetricKeyId");

                    b.ToTable("SymmetricKeys");
                });
#pragma warning restore 612, 618
        }
    }
}