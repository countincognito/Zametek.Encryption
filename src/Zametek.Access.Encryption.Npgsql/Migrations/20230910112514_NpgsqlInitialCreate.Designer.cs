﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Zametek.Access.Encryption;

#nullable disable

namespace Zametek.Access.Encryption.Migrations
{
    [DbContext(typeof(EncryptionDbContext))]
    [Migration("20230910112514_NpgsqlInitialCreate")]
    partial class NpgsqlInitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Zametek.Access.Encryption.SymmetricKey", b =>
                {
                    b.Property<Guid>("SymmetricKeyId")
                        .HasColumnType("uuid");

                    b.Property<string>("AsymmetricKeyId")
                        .HasColumnType("text");

                    b.Property<string>("AsymmetricKeyName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("AsymmetricKeyVersion")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("InitializationVector")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SymmetricKeyName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("WrappedSymmetricKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("SymmetricKeyId", "AsymmetricKeyId");

                    b.ToTable("SymmetricKeys");
                });
#pragma warning restore 612, 618
        }
    }
}
