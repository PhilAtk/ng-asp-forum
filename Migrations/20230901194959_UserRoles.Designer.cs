﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ng_asp_forum.Migrations
{
    [DbContext(typeof(ForumContext))]
    [Migration("20230901194959_UserRoles")]
    partial class UserRoles
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

                    b.Property<int>("authoruserID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("date")
                        .HasColumnType("TEXT");

                    b.Property<string>("text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("threadID")
                        .HasColumnType("INTEGER");

                    b.HasKey("postID");

                    b.HasIndex("authoruserID");

                    b.HasIndex("threadID");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("ForumThread", b =>
                {
                    b.Property<int>("threadID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("authoruserID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("date")
                        .HasColumnType("TEXT");

                    b.Property<string>("topic")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("threadID");

                    b.HasIndex("authoruserID");

                    b.ToTable("Threads");
                });

            modelBuilder.Entity("ForumUser", b =>
                {
                    b.Property<int>("userID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("role")
                        .HasColumnType("INTEGER");

                    b.Property<string>("userName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("userState")
                        .HasColumnType("INTEGER");

                    b.HasKey("userID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ForumPost", b =>
                {
                    b.HasOne("ForumUser", "author")
                        .WithMany()
                        .HasForeignKey("authoruserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ForumThread", "thread")
                        .WithMany("posts")
                        .HasForeignKey("threadID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("author");

                    b.Navigation("thread");
                });

            modelBuilder.Entity("ForumThread", b =>
                {
                    b.HasOne("ForumUser", "author")
                        .WithMany()
                        .HasForeignKey("authoruserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("author");
                });

            modelBuilder.Entity("ForumThread", b =>
                {
                    b.Navigation("posts");
                });
#pragma warning restore 612, 618
        }
    }
}
