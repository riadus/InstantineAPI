using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using InstantineAPI.Data;
using InstantineAPI.UnitTests.Builders;
using Xunit;

namespace InstantineAPI.UnitTests
{
    public class AlbumServiceTests
    {
        private User GetUser() => new User { UserId = "1", Email = "mail@mail.com", FirstName = "John", LastName = "Doe" };
        private User GetFollower() => new User { UserId = "2", Email = "second.mail@mail.com", FirstName = "Jane", LastName = "Doe" };
        private User GetAdmin() => new User { UserId="3", Email = "third.mail@mail.com", FirstName = "Liz", LastName = "Doe" };
        private Album GetAlbum(User user) => new Album
        {
            Name = "AlbumName",
            Creator = user
        };

        [Fact]
        public async Task Should_Create_Album()
        {
            var albumService = new AlbumServiceBuild().Build();
            var user = GetUser();
            var album = await albumService.CreateAlbum(user, "albumName");
            album.Should().NotBeNull();
            album.Creator.Should().Equals(user);
        }

        [Fact]
        public async Task Create_Album_Should_Save()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var albumService = new AlbumServiceBuild()
                                    .WithUnitOfWork(unitOfWork)
                                    .Build();
            var user = GetUser();
            var album = await albumService.CreateAlbum(user, "albumName");
            var savedAlbum = await unitOfWork.Albums.GetFirst(x => x.Name == "albumName");
            album.Should().NotBeNull();
            album.Should().Equals(savedAlbum);
        }

        [Fact]
        public async Task Creator_Should_Get_Album()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var albumService = new AlbumServiceBuild()
                                    .WithUnitOfWork(unitOfWork)
                                    .Build();
            var user = GetUser();
            var album = GetAlbum(user);
            await unitOfWork.Albums.Add(album);
            var albums = await albumService.GetAlbums(user);
            albums.Should().Contain(album);
        }

        [Fact]
        public async Task Should_Get_Album_By_Id()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var albumService = new AlbumServiceBuild()
                                    .WithUnitOfWork(unitOfWork)
                                    .Build();
            var user = GetUser();
            var album = GetAlbum(user);
            album.AlbumId = "1234";
            await unitOfWork.Albums.Add(album);

            var savedAlbum = await albumService.GetAlbum("1234");
            savedAlbum.Should().NotBeNull();
            savedAlbum.Should().Be(album);
        }

        [Fact]
        public async Task Creator_Should_Add_Follower()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var albumService = new AlbumServiceBuild()
                                    .WithUnitOfWork(unitOfWork)
                                    .Build();
            var user = GetUser();
            var album = GetAlbum(user);
            album.AlbumId = "1234";
            await unitOfWork.Albums.Add(album);
            var follower = GetFollower();
            var successfullyAdded = await albumService.AddFollower("1234", follower, user);
            var savedAlbum = await unitOfWork.Albums.GetFirst(x => x.AlbumId == "1234");
            successfullyAdded.Should().BeTrue();
            savedAlbum.Followers.Should().ContainSingle();
            savedAlbum.Followers.Should().Contain(follower);
        }

        [Fact]
        public async Task Creator_Should_Add_Admin()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var albumService = new AlbumServiceBuild()
                                    .WithUnitOfWork(unitOfWork)
                                    .Build();
            var user = GetUser();
            var album = GetAlbum(user);
            album.AlbumId = "1234";
            await unitOfWork.Albums.Add(album);
            var admin = GetAdmin();
            var successfullyAdded = await albumService.AddAdmin("1234", admin, user);
            var savedAlbum = await unitOfWork.Albums.GetFirst(x => x.AlbumId == "1234");
            successfullyAdded.Should().BeTrue();
            savedAlbum.Admins.Should().ContainSingle();
            savedAlbum.Admins.Should().Contain(admin);
        }

        [Fact]
        public async Task Follower_Should_Not_Add_Follower()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var albumService = new AlbumServiceBuild()
                                    .WithUnitOfWork(unitOfWork)
                                    .Build();
            var user = GetUser();
            var album = GetAlbum(user);
            album.AlbumId = "1234";
            await unitOfWork.Albums.Add(album);
            var follower = GetFollower();
            await albumService.AddFollower("1234", follower, user);
            var successfullyAdded = await albumService.AddFollower("1234", user, follower);
            var savedAlbum = await unitOfWork.Albums.GetFirst(x => x.AlbumId == "1234");
            successfullyAdded.Should().BeFalse();
            savedAlbum.Followers.Should().ContainSingle();
            savedAlbum.Followers.Should().NotContain(user);
        }

        [Fact]
        public async Task Follower_Should_Not_Add_Admin()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var albumService = new AlbumServiceBuild()
                                    .WithUnitOfWork(unitOfWork)
                                    .Build();
            var user = GetUser();
            var album = GetAlbum(user);
            album.AlbumId = "1234";
            await unitOfWork.Albums.Add(album);

            var follower = GetFollower();
            var admin = GetAdmin();
            await albumService.AddFollower("1234", follower, user);

            var successfullyAdded = await albumService.AddAdmin("1234", admin, follower);
            var savedAlbum = await unitOfWork.Albums.GetFirst(x => x.AlbumId == "1234");
            successfullyAdded.Should().BeFalse();
            savedAlbum.Admins.Should().NotContain(admin);
            savedAlbum.Admins.Should().BeEmpty();
        }

        [Fact]
        public async Task Admin_Should_Not_Add_Follower()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var albumService = new AlbumServiceBuild()
                                    .WithUnitOfWork(unitOfWork)
                                    .Build();
            var user = GetUser();
            var album = GetAlbum(user);
            album.AlbumId = "1234";
            await unitOfWork.Albums.Add(album);
            var admin = GetAdmin();
            var follower = GetFollower();
            await albumService.AddAdmin("1234", admin, user);
            var successfullyAdded = await albumService.AddFollower("1234", follower, admin);
            var savedAlbum = await unitOfWork.Albums.GetFirst(x => x.AlbumId == "1234");
            successfullyAdded.Should().BeFalse();
            savedAlbum.Followers.Should().NotContain(follower);
            savedAlbum.Followers.Should().BeEmpty();
        }

        [Fact]
        public async Task Admin_Should_Not_Add_Admin()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var albumService = new AlbumServiceBuild()
                                    .WithUnitOfWork(unitOfWork)
                                    .Build();
            var user = GetUser();
            var album = GetAlbum(user);
            album.AlbumId = "1234";
            await unitOfWork.Albums.Add(album);
            var admin = GetAdmin();
            await albumService.AddAdmin("1234", admin, user);
            var successfullyAdded = await albumService.AddAdmin("1234", user, admin);
            var savedAlbum = await unitOfWork.Albums.GetFirst(x => x.AlbumId == "1234");
            successfullyAdded.Should().BeFalse();
            savedAlbum.Admins.Should().ContainSingle();
            savedAlbum.Admins.Should().NotContain(user);
        }

        [Fact]
        public async Task Creator_Should_Remove_Follower()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var albumService = new AlbumServiceBuild()
                                    .WithUnitOfWork(unitOfWork)
                                    .Build();
            var user = GetUser();
            var album = GetAlbum(user);
            album.AlbumId = "1234";
            var follower = GetFollower();

            album.AlbumFollowers = new List<AlbumFollower> { new AlbumFollower { Album = album, Follower = follower } };
            await unitOfWork.Albums.Add(album);

            var successfullyRemoved = await albumService.RemoveFollower("1234", follower, user);
            var savedAlbum = await unitOfWork.Albums.GetFirst(x => x.AlbumId == "1234");
            successfullyRemoved.Should().BeTrue();
            savedAlbum.Followers.Should().NotContain(follower);
            savedAlbum.Followers.Should().BeEmpty();
        }

        [Fact]
        public async Task Creator_Should_Remove_Admin()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var albumService = new AlbumServiceBuild()
                                    .WithUnitOfWork(unitOfWork)
                                    .Build();
            var user = GetUser();
            var album = GetAlbum(user);
            album.AlbumId = "1234";
            var admin = GetAdmin();

            album.AlbumAdmins = new List<AlbumAdmin> { new AlbumAdmin { Album = album, Admin = admin } };
            await unitOfWork.Albums.Add(album);

            var successfullyRemoved = await albumService.RemoveAdmin("1234", admin, user);
            var savedAlbum = await unitOfWork.Albums.GetFirst(x => x.AlbumId == "1234");
            successfullyRemoved.Should().BeTrue();
            savedAlbum.Admins.Should().NotContain(admin);
            savedAlbum.Admins.Should().BeEmpty();
        }

        [Fact]
        public async Task Follower_Should_Remove_Self()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var albumService = new AlbumServiceBuild()
                                    .WithUnitOfWork(unitOfWork)
                                    .Build();
            var user = GetUser();
            var album = GetAlbum(user);
            album.AlbumId = "1234";
            var follower = GetFollower();

            album.AlbumFollowers = new List<AlbumFollower> { new AlbumFollower { Album = album, Follower = follower } };
            await unitOfWork.Albums.Add(album);

            var successfullyRemoved = await albumService.RemoveFollower("1234", follower, follower);
            var savedAlbum = await unitOfWork.Albums.GetFirst(x => x.AlbumId == "1234");
            successfullyRemoved.Should().BeTrue();
            savedAlbum.Followers.Should().NotContain(follower);
            savedAlbum.Followers.Should().BeEmpty();
        }

        [Fact]
        public async Task Admin_Should_Remove_Self()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var albumService = new AlbumServiceBuild()
                                    .WithUnitOfWork(unitOfWork)
                                    .Build();
            var user = GetUser();
            var album = GetAlbum(user);
            album.AlbumId = "1234";
            var admin = GetAdmin();

            album.AlbumAdmins = new List<AlbumAdmin> { new AlbumAdmin { Album = album, Admin = admin } };
            await unitOfWork.Albums.Add(album);

            var successfullyRemoved = await albumService.RemoveAdmin("1234", admin, admin);
            var savedAlbum = await unitOfWork.Albums.GetFirst(x => x.AlbumId == "1234");
            successfullyRemoved.Should().BeTrue();
            savedAlbum.Admins.Should().NotContain(admin);
            savedAlbum.Admins.Should().BeEmpty();
        }

        [Fact]
        public async Task Admin_Should_Not_Remove_Admin()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var albumService = new AlbumServiceBuild()
                                    .WithUnitOfWork(unitOfWork)
                                    .Build();
            var user = GetUser();
            var album = GetAlbum(user);
            album.AlbumId = "1234";
            var admin = GetAdmin();

            album.AlbumAdmins = new List<AlbumAdmin> { new AlbumAdmin { Album = album, Admin = admin } };
            await unitOfWork.Albums.Add(album);

            var successfullyRemoved = await albumService.RemoveAdmin("1234", user, admin);
            var savedAlbum = await unitOfWork.Albums.GetFirst(x => x.AlbumId == "1234");
            successfullyRemoved.Should().BeFalse();
            savedAlbum.Admins.Should().ContainSingle();
            savedAlbum.Admins.Should().Contain(admin);
        }

        [Fact]
        public async Task Follower_Should_Not_Remove_Follower()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var albumService = new AlbumServiceBuild()
                                    .WithUnitOfWork(unitOfWork)
                                    .Build();
            var user = GetUser();
            var album = GetAlbum(user);
            album.AlbumId = "1234";
            var follower = GetFollower();

            album.AlbumFollowers = new List<AlbumFollower> { new AlbumFollower { Album = album, Follower = follower } };
            await unitOfWork.Albums.Add(album);

            var successfullyRemoved = await albumService.RemoveFollower("1234", user, follower);
            var savedAlbum = await unitOfWork.Albums.GetFirst(x => x.AlbumId == "1234");
            successfullyRemoved.Should().BeFalse();
            savedAlbum.Followers.Should().ContainSingle();
            savedAlbum.Followers.Should().Contain(follower);
        }

        [Fact]
        public async Task Should_Get_Album_Followers()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var albumService = new AlbumServiceBuild()
                                    .WithUnitOfWork(unitOfWork)
                                    .Build();
            var user = GetUser();
            var album = GetAlbum(user);
            album.AlbumId = "1234";
            var follower = GetFollower();
            album.AlbumFollowers = new List<AlbumFollower> { new AlbumFollower { Album = album, Follower = follower } };

            await unitOfWork.Albums.Add(album);

            var followers = await albumService.GetFollowers("1234");
            followers.Should().ContainSingle();
            followers.Should().Contain(follower);
        }

        [Fact]
        public async Task Should_Get_Album_Admins()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var albumService = new AlbumServiceBuild()
                                    .WithUnitOfWork(unitOfWork)
                                    .Build();
            var user = GetUser();
            var album = GetAlbum(user);
            album.AlbumId = "1234";
            var admin = GetAdmin();
            album.AlbumAdmins = new List<AlbumAdmin> { new AlbumAdmin { Album = album, Admin = admin } };

            await unitOfWork.Albums.Add(album);

            var admins = await albumService.GetAdmins("1234");
            admins.Should().ContainSingle();
            admins.Should().Contain(admin);
        }
    }
}