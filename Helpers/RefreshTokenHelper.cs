using BabeNest_Backend.Entities;
using System.Security.Cryptography;

namespace BabeNest_Backend.Helpers
{
    public class RefreshTokenHelper
    {
        public static RefreshToken GenerateRefreshToken(int userId)
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                ExpiryTime = DateTime.UtcNow.AddDays(7), // valid for 7 days
                UserId = userId,
                IsRevoked = false
            };
        }
    }
}
