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
            var storedComment = await unitOfWork.Comments.GetAll(x => x.ReactionId == guid.ToString());
            storedComment.Should().NotBeNull();
            storedComment.Should().NotBeEmpty();
            storedComment.Should().ContainSingle();
            storedComment.Should().Contain(comment);
        }

        [Fact]
        public async Task Like_Should_Be_Stored()
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
            var like = await reactionService.LikePhoto("1234", "albumId", user);
            var storedLike = await unitOfWork.Likes.GetAll(x => x.ReactionId == guid.ToString());
            storedLike.Should().NotBeNull();
            storedLike.Should().NotBeEmpty();
            storedLike.Should().ContainSingle();
            storedLike.Should().Contain(like);
        }

        [Fact]
        public async Task User_Should_Remove_Comment()
        {
            var photoService = A.Fake<IPhotoService>();
            var photo = GetPhoto();
            var user = GetUser();
            var guid = Guid.NewGuid();
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var comment = new Comment { ReactionId = "1234", Reactor = user, Text = "Héhé" };
            photo.PhotoComments = new List<PhotoComment>
            {
                new PhotoComment{ Photo = photo, Comment = comment}
            };
            await unitOfWork.Photos.Add(photo);
            A.CallTo(() => photoService.GetPhoto("1234")).Returns(Task.FromResult(photo));
            var reactionService = new ReactionServiceBuilder()
                                        .WithGuid(guid)
                                        .WithPhotoService(photoService)
                                        .WithUnitOfWork(unitOfWork)
                                        .WithPermissionsService(new AllPermissionsGrantedService())
                                        .Build();
            var sucessfullyDeleted = await reactionService.DeleteComment("1234", "AlbumId", "1234", user);
            var comments = await unitOfWork.Comments.GetAll();
            sucessfullyDeleted.Should().BeTrue();
            comments.Should().BeEmpty();
        }

        [Fact]
        public async Task User_Should_Remove_Like()
        {
            var photoService = A.Fake<IPhotoService>();
            var photo = GetPhoto();
            var user = GetUser();
            var guid = Guid.NewGuid();
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var like = new Like { ReactionId = "1234", Reactor = user };
            photo.PhotoLikes = new List<PhotoLike>
            {
                new PhotoLike{ Photo = photo, Like = like}
            };
            await unitOfWork.Photos.Add(photo);
            A.CallTo(() => photoService.GetPhoto("1234")).Returns(Task.FromResult(photo));
            var reactionService = new ReactionServiceBuilder()
                                        .WithGuid(guid)
                                        .WithPhotoService(photoService)
                                        .WithUnitOfWork(unitOfWork)
                                        .WithPermissionsService(new AllPermissionsGrantedService())
                                        .Build();
            var sucessfullyDeleted = await reactionService.UnlikePhoto("1234", "AlbumId", user);
            var likes = await unitOfWork.Likes.GetAll();
            sucessfullyDeleted.Should().BeTrue();
            likes.Should().BeEmpty();
        }

        [Fact]
        public async Task User_Should_Not_Comment_If_Not_Allowed()
        {
            var photoService = A.Fake<IPhotoService>();
            var photo = GetPhoto();
            var user = GetUser();
            var guid = Guid.NewGuid();
            A.CallTo(() => photoService.GetPhoto(photo.PhotoId)).Returns(photo);
            var reactionService = new ReactionServiceBuilder()
                                        .WithGuid(guid)
                                        .WithPhotoService(photoService)
                                        .WithPermissionsService(new AllPermissionsDeniedService())
                                        .Build();
            var comment = await reactionService.AddComment("1234", "albumId", "This photo rocks !", user);
            comment.Should().BeNull();
        }

        [Fact]
        public async Task User_Should_Not_Like_If_Not_Allowed()
        {
            var photoService = A.Fake<IPhotoService>();
            var photo = GetPhoto();
            var user = GetUser();
            var guid = Guid.NewGuid();
            A.CallTo(() => photoService.GetPhoto(photo.PhotoId)).Returns(photo);
            var reactionService = new ReactionServiceBuilder()
                                        .WithGuid(guid)
                                        .WithPhotoService(photoService)
                                        .WithPermissionsService(new AllPermissionsDeniedService())
                                        .Build();
            var like = await reactionService.LikePhoto("1234", "albumId", user);
            like.Should().BeNull();
        }

        [Fact]
        public async Task User_Should_Not_Remove_Comment_If_Not_Allowed()
        {
            var photoService = A.Fake<IPhotoService>();
            var photo = GetPhoto();
            var user = GetUser();
            var guid = Guid.NewGuid();
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var comment = new Comment { ReactionId = "1234", Reactor = user, Text = "Héhé" };
            photo.PhotoComments = new List<PhotoComment>
            {
                new PhotoComment{ Photo = photo, Comment = comment}
            };
            await unitOfWork.Photos.Add(photo);
            A.CallTo(() => photoService.GetPhoto("1234")).Returns(Task.FromResult(photo));
            var reactionService = new ReactionServiceBuilder()
                                        .WithGuid(guid)
                                        .WithPhotoService(photoService)
                                        .WithUnitOfWork(unitOfWork)
                                        .WithPermissionsService(new AllPermissionsDeniedService())
                                        .Build();
            var sucessfullyDeleted = await reactionService.DeleteComment("1234", "AlbumId", "1234", user);
            sucessfullyDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task User_Should_Remove_Like_If_Not_Allowed()
        {
            var photoService = A.Fake<IPhotoService>();
            var photo = GetPhoto();
            var user = GetUser();
            var guid = Guid.NewGuid();
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var like = new Like { ReactionId = "1234", Reactor = user };
            photo.PhotoLikes = new List<PhotoLike>
            {
                new PhotoLike{ Photo = photo, Like = like}
            };
            await unitOfWork.Photos.Add(photo);
            A.CallTo(() => photoService.GetPhoto("1234")).Returns(Task.FromResult(photo));
            var reactionService = new ReactionServiceBuilder()
                                        .WithGuid(guid)
                                        .WithPhotoService(photoService)
                                        .WithUnitOfWork(unitOfWork)
                                        .WithPermissionsService(new AllPermissionsDeniedService())
                                        .Build();
            var sucessfullyDeleted = await reactionService.UnlikePhoto("1234", "AlbumId", user);
            sucessfullyDeleted.Should().BeFalse();
        }
    }
}
