namespace EcoFleetLogistics.Application.Common.Models.Authentication
{
        public record TokenResult(
            string AccessToken,
            string RefreshToken,
            DateTime RefreshTokenExpiresAt
        );      
}