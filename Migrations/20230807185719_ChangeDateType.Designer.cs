﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ng_asp.Migrations
{
    [DbContext(typeof(ForumContext))]
    [Migration("20230807185719_ChangeDateType")]
    partial class ChangeDateType
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.9");

            modelBuilder.Entity("ForumPost", b =>
                {
                    b.Property<int>("postID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("date")
                        .HasColumnType("TEXT");

                    b.Property<string>("text")
                        .HasColumnType("TEXT");

                    b.Property<int>("threadID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("userID")
                        .HasColumnType("INTEGER");

                    b.HasKey("postID");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("ForumThread", b =>
                {
                    b.Property<int>("threadID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("date")
                        .HasColumnType("TEXT");

                    b.Property<string>("topic")
                        .HasColumnType("TEXT");

                    b.Property<int>("userID")
                        .HasColumnType("INTEGER");

                    b.HasKey("threadID");

                    b.ToTable("Threads");
                });

            modelBuilder.Entity("ForumUser", b =>
                {
                    b.Property<int>("userID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("userName")
                        .HasColumnType("TEXT");

                    b.HasKey("userID");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
