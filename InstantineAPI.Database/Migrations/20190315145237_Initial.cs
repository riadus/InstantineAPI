using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InstantineAPI.Database.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    InvitationSent = table.Column<bool>(nullable: false),
                    InvitationAccepted = table.Column<bool>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    SendingDate = table.Column<DateTime>(nullable: false),
                    AcceptingDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Role = table.Column<int>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    PasswordSalt = table.Column<string>(nullable: true),
                    RefreshToken = table.Column<string>(nullable: true),
                    RefreshTokenSalt = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReactionId = table.Column<string>(nullable: true),
                    ReactorId = table.Column<int>(nullable: true),
                    ReactionDate = table.Column<DateTime>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Users_ReactorId",
                        column: x => x.ReactorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReactionId = table.Column<string>(nullable: true),
                    ReactorId = table.Column<int>(nullable: true),
                    ReactionDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Likes_Users_ReactorId",
                        column: x => x.ReactorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlbumAdmin",
                columns: table => new
                {
                    AlbumId = table.Column<int>(nullable: false),
                    AdminId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumAdmin", x => new { x.AlbumId, x.AdminId });
                    table.ForeignKey(
                        name: "FK_AlbumAdmin_Users_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlbumFollower",
                columns: table => new
                {
                    AlbumId = table.Column<int>(nullable: false),
                    FollowerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumFollower", x => new { x.AlbumId, x.FollowerId });
                    table.ForeignKey(
                        name: "FK_AlbumFollower_Users_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Link = table.Column<string>(nullable: true),
                    TakeDate = table.Column<DateTime>(nullable: false),
                    PhotoId = table.Column<string>(nullable: true),
                    AuthorId = table.Column<int>(nullable: true),
                    AlbumId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Albums",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    CreatorId = table.Column<int>(nullable: true),
                    AlbumId = table.Column<string>(nullable: true),
                    CoverId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Albums_Photos_CoverId",
                        column: x => x.CoverId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Albums_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhotoComment",
                columns: table => new
                {
                    PhotoId = table.Column<int>(nullable: false),
                    CommentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoComment", x => new { x.PhotoId, x.CommentId });
                    table.ForeignKey(
                        name: "FK_PhotoComment_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhotoComment_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhotoLike",
                columns: table => new
                {
                    PhotoId = table.Column<int>(nullable: false),
                    LikeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoLike", x => new { x.PhotoId, x.LikeId });
                    table.ForeignKey(
                        name: "FK_PhotoLike_Likes_LikeId",
                        column: x => x.LikeId,
                        principalTable: "Likes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhotoLike_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumAdmin_AdminId",
                table: "AlbumAdmin",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumFollower_FollowerId",
                table: "AlbumFollower",
                column: "FollowerId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_CoverId",
                table: "Albums",
                column: "CoverId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_CreatorId",
                table: "Albums",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ReactorId",
                table: "Comments",
                column: "ReactorId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_ReactorId",
                table: "Likes",
                column: "ReactorId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoComment_CommentId",
                table: "PhotoComment",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoLike_LikeId",
                table: "PhotoLike",
                column: "LikeId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_AlbumId",
                table: "Photos",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_AuthorId",
                table: "Photos",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumAdmin_Albums_AlbumId",
                table: "AlbumAdmin",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumFollower_Albums_AlbumId",
                table: "AlbumFollower",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Albums_AlbumId",
                table: "Photos",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Albums_Users_CreatorId",
                table: "Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Users_AuthorId",
                table: "Photos");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Albums_AlbumId",
                table: "Photos");

            migrationBuilder.DropTable(
                name: "AlbumAdmin");

            migrationBuilder.DropTable(
                name: "AlbumFollower");

            migrationBuilder.DropTable(
                name: "PhotoComment");

            migrationBuilder.DropTable(
                name: "PhotoLike");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Albums");

            migrationBuilder.DropTable(
                name: "Photos");
        }
    }
}
