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
    [Migration("20230920152329_UserAudits")]
    partial class UserAudits
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

                    b.Property<int?>("authoruserID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("date")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("dateModified")
                        .HasColumnType("TEXT");

                    b.Property<bool>("edited")
                        .HasColumnType("INTEGER");

                    b.Property<string>("text")
                        .HasColumnType("TEXT");

                    b.Property<int?>("threadID")
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

                    b.Property<DateTime?>("dateModified")
                        .HasColumnType("TEXT");

                    b.Property<bool>("edited")
                        .HasColumnType("INTEGER");

                    b.Property<string>("topic")
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

                    b.Property<string>("bio")
                        .HasColumnType("TEXT");

                    b.Property<string>("code")
                        .HasColumnType("TEXT");

                    b.Property<string>("email")
                        .HasColumnType("TEXT");

                    b.Property<string>("password")
                        .HasColumnType("TEXT");

                    b.Property<string>("userName")
                        .HasColumnType("TEXT");

                    b.Property<int>("userRole")
                        .HasColumnType("INTEGER");

                    b.Property<int>("userState")
                        .HasColumnType("INTEGER");

                    b.HasKey("userID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ForumPost", b =>
                {
                    b.HasOne("ForumUser", "author")
                        .WithMany()
                        .HasForeignKey("authoruserID");

                    b.HasOne("ForumThread", "thread")
                        .WithMany("posts")
                        .HasForeignKey("threadID");

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
