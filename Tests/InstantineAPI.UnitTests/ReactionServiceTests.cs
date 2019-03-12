using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using InstantineAPI.Core.Photos;
using InstantineAPI.Data;
using InstantineAPI.UnitTests.Builders;
using InstantineAPI.UnitTests.Mock;
using Xunit;

namespace InstantineAPI.UnitTests
{
    public class ReactionServiceTests
    {
        private User GetUser() => new User { Email = "mail@mail.com", FirstName = "John", LastName = "Doe" };
        private Photo GetPhoto() => new Photo
        {
            PhotoId = "1234",
            PhotoComments = new List<PhotoComment>(),
            PhotoLikes = new List<PhotoLike>()
        };

        [Fact]
        public async Task User_Should_Comment()
        {
            var photoService = A.Fake<IPhotoService>();
            var photo = GetPhoto();
            var user = GetUser();
            var guid = Guid.NewGuid();
            A.CallTo(() => photoService.GetPhoto(photo.PhotoId)).Returns(photo);
            var reactionService = new ReactionServiceBuilder()
                                        .WithGuid(guid)
                                        .WithPhotoService(photoService)
                                        .WithPermissionsService(new AllPermissionsGrantedService())
                                        .Build();
            var comment = await reactionService.AddComment("1234", "albumId", "This photo rocks !", user);
            comment.Should().NotBeNull();
            comment.ReactionId.Should().Be(guid.ToString());
        }

        [Fact]
        public async Task User_Should_Like()
        {
            var photoService = A.Fake<IPhotoService>();
            var photo = GetPhoto();
            var user = GetUser();
            var guid = Guid.NewGuid();
            A.CallTo(() => photoService.GetPhoto(photo.PhotoId)).Returns(photo);
            var reactionService = new ReactionServiceBuilder()
                                        .WithGuid(guid)
                                        .WithPhotoService(photoService)
                                        .WithPermissionsService(new AllPermissionsGrantedService())
                                        .Build();
            var like = await reactionService.LikePhoto("1234", "albumId", user);
            like.Should().NotBeNull();
            like.ReactionId.Should().Be(guid.ToString());
        }

        [Fact]
        public async Task Comment_Should_Be_Stored()
        {
            var photoService = A.Fake<IPhotoService>();
            var photo = GetPhoto();
            var user = GetUser();
            var guid = Guid.NewGuid();
            var unitOfWork = new UnitOfWorkBuilder().Build();
            A.CallTo(() => photoService.GetPhoto(photo.PhotoId)).Returns(photo);
            var reactionService = new ReactionServiceBuilder()
                                        .WithGuid(guid)
                                        .WithPhotoService(photoService)
                                        .WithUnitOfWork(unitOfWork)
                                        .WithPermissionsService(new AllPermissionsGrantedService())
                                        .Build();
            var comment = await reactionService.AddComment("1234", "albumId", "This photo rocks !", user);
            var storedPhoto = await unitOfWork.Photos.GetFirst(x => x.PhotoId == "1234");
            storedPhoto.Comments.Should().NotBeNull();
            storedPhoto.Comments.Should().NotBeEmpty();
            storedPhoto.Comments.Should().ContainSingle();
            storedPhoto.Comments.Should().Contain(comment);
        }
    }
}
