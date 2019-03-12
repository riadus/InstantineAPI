using System.Threading.Tasks;
using InstantineAPI.Data;

namespace InstantineAPI.Core.Photos
{
    public interface IFtpService
    {
        Task StorePhotoOnFtp(byte[] image, Photo photo);
        Task<byte[]> GetPhoto(Photo photo);
        Task DeletePhoto(Photo photo);
    }
}
