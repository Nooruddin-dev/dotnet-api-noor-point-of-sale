
using Entities.ModuleSpecificModels.Users;
using Helpers.CommonHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.AuthorizationHelpers.JwtTokenHelper
{
    public static class JwtManager
    {
        private static string SecretKey = "VGhpcyBpcyBJbnN0YWtpbiBQcml2YXRlIEtleSAsIFdlIHNlcnZlcyB0aGUgcGVvcGxlIGFyb3VuZCB0aGUgd29ybGQ=";

        public static string GenerateJwtToken(string UserID)
        {
            byte[] key = Convert.FromBase64String(SecretKey);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                      new Claim(ClaimTypes.Name, UserID)}),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = StaticMethodsDependencyInjctHelper.config?.GetSection("AppSetting")?.GetSection("ApiValidationIssuer")?.Value ?? "",
                SigningCredentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }
        public static ClaimsPrincipal? GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                byte[] key = Convert.FromBase64String(SecretKey);

                string ValidIssuerOne = StaticMethodsDependencyInjctHelper.config?.GetSection("AppSetting")?.GetSection("ApiValidationIssuer")?.Value ?? "";

                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuers = new[] { ValidIssuerOne }
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                      parameters, out securityToken);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static TokenValidation? ValidateToken(string token)
        {
            ClaimsPrincipal? principal = GetPrincipal(token);
            if (principal == null)
                return null;
            ClaimsIdentity? identity = null;
            try
            {
                identity = principal != null && principal.Identity != null ? (ClaimsIdentity)principal.Identity : null;
            }
            catch (NullReferenceException)
            {
                return null;
            }

            if (identity != null)
            {
                Claim? userIDClaim = identity.FindFirst(ClaimTypes.Name);
                string? expirationClaim = identity.FindFirst("exp")?.Value;
                TokenValidation tokenValidation = new TokenValidation();
                tokenValidation.UserID = userIDClaim?.Value;


                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(Convert.ToDouble(expirationClaim)).ToLocalTime();
                tokenValidation.Expiration = dtDateTime;
                return tokenValidation;
            }
            else
            {
                return null;
            }


        }

        #region JWTToken
        public static string GetJwtToken(string response)
        {
            response = String.IsNullOrWhiteSpace(response) ? "[]" : response;

            string token = string.Empty;
            if (response.Contains("BusnPartnerId"))
            {
                UserLoginResponse? usr = new UserLoginResponse();
                usr = JsonConvert.DeserializeObject<UserLoginResponse?>(response);
                if (usr != null && usr.BusnPartnerId > 0)
                {
                    token = JwtManager.GenerateJwtToken(usr.BusnPartnerId.ToString());
                }

                //var DeserialzedJUserData = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                //if (DeserialzedJUserData != null && DeserialzedJUserData.ContainsKey("UserID"))
                //{
                //    token = JwtManager.GenerateJwtToken(DeserialzedJUserData["UserID"]?.ToString() ?? "");
                //}

            }
            return token;
        }

        #endregion JWTToken

    }

    public class TokenValidation
    {
        public string? UserID { get; set; }
        public DateTime Expiration { get; set; }
    }
}
