using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using InstantineAPI.Core.Photos;
using InstantineAPI.Data;
using InstantineAPI.UnitTests.Builders;
using Xunit;

namespace InstantineAPI.UnitTests
{
    public class PermissionsServiceTests
    {
        private async Task<IPermissionsService> BuildPermissionsService()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var album = new Album
            {
                AlbumId = "1234",
                Creator = PermissionsData.Creator
            };
            album.AlbumFollowers = new List<AlbumFollower>
                                     {
                                        new AlbumFollower { Album = album, Follower = PermissionsData.Follower }
                                     };
            album.AlbumAdmins = new List<AlbumAdmin>
                                    {
                                        new AlbumAdmin { Album = album, Admin = PermissionsData.Admin}
                                    };
            await unitOfWork.Albums.Add(album);
            var permissionsService = new PermissionsServiceBuilder().WithUnitOfWork(unitOfWork).Build();
            return permissionsService;
        }

        [Theory]
        [MemberData(nameof(PermissionsData.CanSeePicture), MemberType = typeof(PermissionsData))]
        public async Task Should_See_Picture(User user, bool canSee)
        {
            var permissionsService = await BuildPermissionsService();
            var permission = await permissionsService.CanSeePicture(user, "1234");
            permission.Should().Be(canSee);
        }

        [Theory]
        [MemberData(nameof(PermissionsData.CanAddPicture), MemberType = typeof(PermissionsData))]
        public async Task CanAddPicture(User user, bool canAdd)
        {
            var permissionsService = await BuildPermissionsService();
            var permission = await permissionsService.CanAddPicture(user, "1234");
            permission.Should().Be(canAdd);
        }


        [Theory]
        [MemberData(nameof(PermissionsData.CanDeletePicture), MemberType = typeof(PermissionsData))]
        public async Task CanDeletePicture(User user, bool canDelete)
        {
            var permissionsService = await BuildPermissionsService();
            var permission = await permissionsService.CanDeletePicture(user, "1234");
            permission.Should().Be(canDelete);
        }

        [Theory]
        [MemberData(nameof(PermissionsData.CanComment), MemberType = typeof(PermissionsData))]
        public async Task CanComment(User user, bool canComment)
        {
            var permissionsService = await BuildPermissionsService();
            var permission = await permissionsService.CanComment(user, "1234");
            permission.Should().Be(canComment);
        }

        [Theory]
        [MemberData(nameof(PermissionsData.CanDeleteComment), MemberType = typeof(PermissionsData))]
        public async Task CanDeleteComment(User user, User reactor, bool canDeleteComment)
        {
            var permissionsService = await BuildPermissionsService();
            var comment = new Comment
            {
                Reactor = reactor
            };
            var permission = await permissionsService.CanDeleteComment(user, comment, "1234");
            permission.Should().Be(canDeleteComment);
        }

        [Theory]
        [MemberData(nameof(PermissionsData.CanLike), MemberType = typeof(PermissionsData))]
        public async Task CanLike(User user, bool canLike)
        {
            var permissionsService = await BuildPermissionsService();
            var permission = await permissionsService.CanLike(user, "1234");
            permission.Should().Be(canLike);
        }

        [Theory]
        [MemberData(nameof(PermissionsData.CanUnLike), MemberType = typeof(PermissionsData))]
        public async Task CanUnLike(User user, User reactor, bool canUnlike)
        {
            var permissionsService = await BuildPermissionsService();
            var like = new Like
            {
                Reactor = reactor
            };
            var permission = await permissionsService.CanUnLike(user, like, "1234");
            permission.Should().Be(canUnlike);
        }

        static class PermissionsData
        {
            public static readonly User Creator = new User { UserId = "creator" };
            public static readonly User Follower = new User { UserId = "follower" };
            public static readonly User Admin = new User { UserId = "admin" };
            public static readonly User OtherUser = new User { UserId = "other" };


            public static IEnumerable<object[]> CanSeePicture =>
                new List<object[]>
                {
                new object[] { Creator, true },
                new object[] { Follower, true },
                new object[] { Admin, true },
                new object[] { OtherUser, false }
                };

            public static IEnumerable<object[]> CanAddPicture =>
                new List<object[]>
                {
                new object[] { Creator, true },
                new object[] { Follower, false },
                new object[] { Admin, true },
                new object[] { OtherUser, false }
                };

            public static IEnumerable<object[]> CanDeletePicture =>
                new List<object[]>
                {
                new object[] { Creator, true },
                new object[] { Follower, false },
                new object[] { Admin, true },
                new object[] { OtherUser, false }
                };

            public static IEnumerable<object[]> CanComment =>
                new List<object[]>
                {
                new object[] { Creator, true },
                new object[] { Follower, true },
                new object[] { Admin, true },
                new object[] { OtherUser, false }
                };

            public static IEnumerable<object[]> CanDeleteComment =>
                new List<object[]>
                {
                new object[] { Creator, Creator, true },
                new object[] { Creator, Follower, true },
                new object[] { Creator, Admin, true },
                new object[] { Creator, OtherUser, true },
                new object[] { Follower, Creator, false },
                new object[] { Follower, Follower, true },
                new object[] { Admin, Admin, true },
                new object[] { Admin, Follower, false },
                new object[] { OtherUser, Follower, false }
                };

            public static IEnumerable<object[]> CanLike =>
                new List<object[]>
                {
                new object[] { Creator, true },
                new object[] { Follower, true },
                new object[] { Admin, true },
                new object[] { OtherUser, false }
                };

            public static IEnumerable<object[]> CanUnLike =>
                new List<object[]>
                {
                new object[] { Creator, Creator, true },
                new object[] { Creator, Follower, true },
                new object[] { Creator, Admin, true },
                new object[] { Creator, OtherUser, true },
                new object[] { Follower, Creator, false },
                new object[] { Follower, Follower, true },
                new object[] { Admin, Admin, true },
                new object[] { Admin, Follower, false },
                new object[] { OtherUser, Follower, false }
                };
        }
    }
}