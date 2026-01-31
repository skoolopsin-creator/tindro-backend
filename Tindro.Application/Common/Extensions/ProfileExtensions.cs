using System.Linq;

namespace Tindro.Application.Common.Extensions
{
    public static class ProfileExtensions
    {
        public static string? GetMainPhotoUrl(this Tindro.Domain.Users.Profile? profile)
        {
            if (profile == null) return null;
            var photo = profile.Photos?.FirstOrDefault();
            return photo?.Url;
        }
    }
}
