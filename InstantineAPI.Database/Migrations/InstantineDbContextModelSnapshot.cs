﻿// <auto-generated />
using System;
using InstantineAPI.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InstantineAPI.Database.Migrations
{
    [DbContext(typeof(InstantineDbContext))]
    partial class InstantineDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity("InstantineAPI.Data.Album", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AlbumId");

                    b.Property<int?>("CoverId");

                    b.Property<DateTime>("CreationDate");

                    b.Property<int?>("CreatorId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("CoverId");

                    b.HasIndex("CreatorId");

                    b.ToTable("Albums");
                });

            modelBuilder.Entity("InstantineAPI.Data.AlbumAdmin", b =>
                {
                    b.Property<int>("AlbumId");

                    b.Property<int>("AdminId");

                    b.HasKey("AlbumId", "AdminId");

                    b.HasIndex("AdminId");

                    b.ToTable("AlbumAdmin");
                });

            modelBuilder.Entity("InstantineAPI.Data.AlbumFollower", b =>
                {
                    b.Property<int>("AlbumId");

                    b.Property<int>("FollowerId");

                    b.HasKey("AlbumId", "FollowerId");

                    b.HasIndex("FollowerId");

                    b.ToTable("AlbumFollower");
                });

            modelBuilder.Entity("InstantineAPI.Data.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ReactionDate");

                    b.Property<string>("ReactionId");

                    b.Property<int?>("ReactorId");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("ReactorId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("InstantineAPI.Data.Like", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ReactionDate");

                    b.Property<string>("ReactionId");

                    b.Property<int?>("ReactorId");

                    b.HasKey("Id");

                    b.HasIndex("ReactorId");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("InstantineAPI.Data.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AlbumId");

                    b.Property<int?>("AuthorId");

                    b.Property<string>("Link");

                    b.Property<string>("PhotoId");

                    b.Property<DateTime>("TakeDate");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.HasIndex("AuthorId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("InstantineAPI.Data.PhotoComment", b =>
                {
                    b.Property<int>("PhotoId");

                    b.Property<int>("CommentId");

                    b.HasKey("PhotoId", "CommentId");

                    b.HasIndex("CommentId");

                    b.ToTable("PhotoComment");
                });

            modelBuilder.Entity("InstantineAPI.Data.PhotoLike", b =>
                {
                    b.Property<int>("PhotoId");

                    b.Property<int>("LikeId");

                    b.HasKey("PhotoId", "LikeId");

                    b.HasIndex("LikeId");

                    b.ToTable("PhotoLike");
                });

            modelBuilder.Entity("InstantineAPI.Data.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AcceptingDate");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<bool>("InvitationAccepted");

                    b.Property<bool>("InvitationSent");

                    b.Property<string>("LastName");

                    b.Property<string>("Password");

                    b.Property<string>("PasswordSalt");

                    b.Property<string>("RefreshToken");

                    b.Property<string>("RefreshTokenSalt");

                    b.Property<int>("Role");

                    b.Property<DateTime>("SendingDate");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("InstantineAPI.Data.Album", b =>
                {
                    b.HasOne("InstantineAPI.Data.Photo", "Cover")
                        .WithMany()
                        .HasForeignKey("CoverId");

                    b.HasOne("InstantineAPI.Data.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId");
                });

            modelBuilder.Entity("InstantineAPI.Data.AlbumAdmin", b =>
                {
                    b.HasOne("InstantineAPI.Data.User", "Admin")
                        .WithMany()
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InstantineAPI.Data.Album", "Album")
                        .WithMany("AlbumAdmins")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InstantineAPI.Data.AlbumFollower", b =>
                {
                    b.HasOne("InstantineAPI.Data.Album", "Album")
                        .WithMany("AlbumFollowers")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InstantineAPI.Data.User", "Follower")
                        .WithMany()
                        .HasForeignKey("FollowerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InstantineAPI.Data.Comment", b =>
                {
                    b.HasOne("InstantineAPI.Data.User", "Reactor")
                        .WithMany()
                        .HasForeignKey("ReactorId");
                });

            modelBuilder.Entity("InstantineAPI.Data.Like", b =>
                {
                    b.HasOne("InstantineAPI.Data.User", "Reactor")
                        .WithMany()
                        .HasForeignKey("ReactorId");
                });

            modelBuilder.Entity("InstantineAPI.Data.Photo", b =>
                {
                    b.HasOne("InstantineAPI.Data.Album")
                        .WithMany("Photos")
                        .HasForeignKey("AlbumId");

                    b.HasOne("InstantineAPI.Data.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");
                });

            modelBuilder.Entity("InstantineAPI.Data.PhotoComment", b =>
                {
                    b.HasOne("InstantineAPI.Data.Comment", "Comment")
                        .WithMany()
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InstantineAPI.Data.Photo", "Photo")
                        .WithMany("PhotoComments")
                        .HasForeignKey("PhotoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InstantineAPI.Data.PhotoLike", b =>
                {
                    b.HasOne("InstantineAPI.Data.Like", "Like")
                        .WithMany()
                        .HasForeignKey("LikeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InstantineAPI.Data.Photo", "Photo")
                        .WithMany("PhotoLikes")
                        .HasForeignKey("PhotoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
