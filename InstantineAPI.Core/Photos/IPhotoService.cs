using System.Collections.Generic;
using System.Threading.Tasks;
using InstantineAPI.Data;

namespace InstantineAPI.Core.Photos
{
    public interface IPhotoService
    {
        Task<Photo> StorePhoto(byte[] image, User author, string almbumId);
        Task<byte[]> GetPicture(string photoId, string albumId, User requestor);
        Task<Photo> GetPhoto(string photoId);
        Task<IEnumerable<Photo>> GetPhotos(string almbumId, User requestor);
        Task<bool> DeletePhoto(string photoId, string albumId, User requestor);
    }
}
