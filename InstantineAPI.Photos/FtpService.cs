using System.Collections.Generic;
using System.Threading.Tasks;
using InstantineAPI.Core.Photos;
using InstantineAPI.Data;

namespace InstantineAPI.Photos
{
    public class FtpService : IFtpService
    {
        private readonly Dictionary<string, byte[]> _photoCache;

        public FtpService()
        {
            _photoCache = new Dictionary<string, byte[]>();
        }

        public FtpService(Dictionary<string, byte[]> photoCache)
        {
            _photoCache = photoCache;
        }

        public Task DeletePhoto(Photo photo)
        {
            if(_photoCache.ContainsKey(photo.PhotoId))
            {
                _photoCache.Remove(photo.PhotoId);
            }
            return Task.FromResult(0);
        }

        public Task<byte[]> GetPhoto(Photo photo)
        {
            if (_photoCache.ContainsKey(photo.PhotoId))
            {
                return Task.FromResult(_photoCache[photo.PhotoId]);
            }
            return null;
        }

        public Task StorePhotoOnFtp(byte[] image, Photo photo)
        {
            _photoCache.TryAdd(photo.PhotoId, image);
            return Task.FromResult(0);
        }
    }
}
