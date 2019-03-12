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
    public class PhotoServiceTests
    {
        private User GetUser() => new User { UserId = "1", Email = "mail@mail.com", FirstName = "John", LastName = "Doe" };
        private Album GetAlbum(User user) => new Album
        {
            Name = "AlbumName",
            Creator = user,
            AlbumId = "1234",
            Photos = new List<Photo>()
        };

        private Photo GetPhoto() => new Photo { PhotoId = "1234" };

        [Fact]
        public async Task User_Should_Not_Store_If_Not_Allowed()
        {
            var photoService = new PhotoServiceBuilder().WithPermissionsService(new AllPermissionsDeniedService()).Build();
            var photo = await photoService.StorePhoto(new byte[] { 0x10, 0x11 }, GetUser(), "1234");
            photo.Should().BeNull();
        }

        [Fact]
        public async Task Should_Store_Photo_On_Ftp()
        {
            var ftpService = A.Fake<IFtpService>();
            var albumService = A.Fake<IAlbumService>();
            var user = GetUser();
            var album = GetAlbum(user);
            A.CallTo(() => albumService.GetAlbum(album.AlbumId)).Returns(Task.FromResult(album));

            var guid = Guid.NewGuid();
            var photoService = new PhotoServiceBuilder()
                                    .WithGuid(guid)
                                    .WithFtpService(ftpService)
                                    .WithAlbumService(albumService)
                                    .WithPermissionsService(new AllPermissionsGrantedService()).Build();
            var image = new byte[] { 0x10, 0x11 };
            var photo = await photoService.StorePhoto(image, user, album.AlbumId);
            photo.Should().NotBeNull();
            A.CallTo(() => ftpService.StorePhotoOnFtp(image, new Photo { PhotoId = guid.ToString() })).MustHaveHappened();
        }

        [Fact]
        public async Task Should_Save_Photo_In_Db()
        {
            var ftpService = A.Fake<IFtpService>();
            var albumService = A.Fake<IAlbumService>();
            var user = GetUser();
            var album = GetAlbum(user);
            A.CallTo(() => albumService.GetAlbum(album.AlbumId)).Returns(Task.FromResult(album));

            var unitOfWork = new UnitOfWorkBuilder().Build();

            var photoService = new PhotoServiceBuilder()
                                    .WithUnitOfWork(unitOfWork)
                                    .WithAlbumService(albumService)
                                    .WithPermissionsService(new AllPermissionsGrantedService()).Build();
            var image = new byte[] { 0x10, 0x11 };
            var photo = await photoService.StorePhoto(image, user, album.AlbumId);
            photo.Should().NotBeNull();
            var albums = await unitOfWork.Albums.GetFirst(x => x.AlbumId == album.AlbumId);
            albums.Photos.Should().NotBeNull();
            albums.Photos.Should().ContainSingle();
            albums.Photos.Should().Contain(photo);
        }

        [Fact]
        public async Task Should_Get_Photo()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var photo = GetPhoto();
            await unitOfWork.Photos.Add(photo);

            var photoService = new PhotoServiceBuilder()
                                    .WithUnitOfWork(unitOfWork).Build();
            var savedPhoto = await photoService.GetPhoto("1234");
            savedPhoto.Should().NotBeNull();
            savedPhoto.Should().Be(photo);
        }

        [Fact]
        public async Task User_Should_Get_Photos_If_Allowed()
        {
            var albumService = A.Fake<IAlbumService>();
            var user = GetUser();
            var album = GetAlbum(user);
            var photo = GetPhoto();
            album.Photos = new List<Photo>
            {
                photo
            };
            A.CallTo(() => albumService.GetAlbum(album.AlbumId)).Returns(Task.FromResult(album));

            var photoService = new PhotoServiceBuilder()
                                .WithAlbumService(albumService)
                                .WithPermissionsService(new AllPermissionsGrantedService()).Build();
            var photos = await photoService.GetPhotos(album.AlbumId, user);
            photos.Should().NotBeNull();
            photos.Should().NotBeEmpty();
            photos.Should().Contain(photo);
        }

        [Fact]
        public async Task User_Should_Not_Get_Photos_If_Not_Allowed()
        {
            var albumService = A.Fake<IAlbumService>();
            var user = GetUser();
            var album = GetAlbum(user);
            var photo = GetPhoto();
            album.Photos = new List<Photo>
            {
                photo
            };
            A.CallTo(() => albumService.GetAlbum(album.AlbumId)).Returns(Task.FromResult(album));

            var photoService = new PhotoServiceBuilder()
                                .WithAlbumService(albumService)
                                .WithPermissionsService(new AllPermissionsDeniedService()).Build();
            var photos = await photoService.GetPhotos(album.AlbumId, user);
            photos.Should().BeNull();
        }

        [Fact]
        public async Task Should_Delete_From_Ftp()
        {
            var ftpService = A.Fake<IFtpService>();
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var user = GetUser();
            var photo = GetPhoto();
            await unitOfWork.Photos.Add(photo);

            var photoService = new PhotoServiceBuilder()
                                    .WithFtpService(ftpService)
                                    .WithUnitOfWork(unitOfWork)
                                    .WithPermissionsService(new AllPermissionsGrantedService()).Build();
           var successfullyDeleted = await photoService.DeletePhoto(photo.PhotoId, "1234", user);
            successfullyDeleted.Should().BeTrue();
            A.CallTo(() => ftpService.DeletePhoto(photo)).MustHaveHappened();
        }

        [Fact]
        public async Task Should_Delete_From_Db()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var user = GetUser();
            var photo = GetPhoto();
            await unitOfWork.Photos.Add(photo);

            var photoService = new PhotoServiceBuilder()
                                    .WithUnitOfWork(unitOfWork)
                                    .WithPermissionsService(new AllPermissionsGrantedService()).Build();

            var successfullyDeleted = await photoService.DeletePhoto(photo.PhotoId, "1234", user);
            successfullyDeleted.Should().BeTrue();
            var photos = await unitOfWork.Photos.GetAll();
            photos.Should().BeEmpty();
        }

        [Fact]
        public async Task Get_Picture_Should_Get_From_Ftp()
        {
            var ftpService = A.Fake<IFtpService>();
            var albumService = A.Fake<IAlbumService>();
            var user = GetUser();
            var photo = GetPhoto();
            var album = GetAlbum(user);
            album.Photos = new List<Photo> { photo };
            var image = new byte[] { 0x12, 0x13 };
            A.CallTo(() => albumService.GetAlbum("1234")).Returns(album);

            var photoService = new PhotoServiceBuilder()
                                    .WithFtpService(ftpService)
                                    .WithAlbumService(albumService)
                                    .WithPermissionsService(new AllPermissionsGrantedService()).Build();
            var picture = await photoService.GetPicture(photo.PhotoId, "1234", user);
            A.CallTo(() => ftpService.GetPhoto(photo)).MustHaveHappened();
        }

        [Fact]
        public async Task Should_Get_Picture()
        {
            var ftpService = A.Fake<IFtpService>();
            var albumService = A.Fake<IAlbumService>();
            var user = GetUser();
            var photo = GetPhoto();
            var album = GetAlbum(user);
            album.Photos = new List<Photo> { photo };
            var image = new byte[] { 0x12, 0x13 };
            A.CallTo(() => ftpService.GetPhoto(photo)).Returns(Task.FromResult(image));
            A.CallTo(() => albumService.GetAlbum("1234")).Returns(album);

            var photoService = new PhotoServiceBuilder()
                                    .WithAlbumService(albumService)
                                    .WithFtpService(ftpService)
                                    .WithPermissionsService(new AllPermissionsGrantedService()).Build();
            var picture = await photoService.GetPicture(photo.PhotoId, "1234", user);
            picture.Should().BeEquivalentTo(image);
        }
    }
}
