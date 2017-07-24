using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GoogleToken.AuthPolicies
{
    public class ValidGoogleToken : AuthorizationHandler<ValidGoogleRequirement>, IAuthorizationRequirement
    {
        private IMemoryCache _cache;
        private HttpClient _client;

        public ValidGoogleToken(IMemoryCache cache, HttpClient client)
        {
            _cache = cache;
            _client = client;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ValidGoogleRequirement requirement)
        {
            var mvcContext = context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext;
            var headers = mvcContext.HttpContext.Request.Headers;
            var authHeader = headers.FirstOrDefault(h => h.Key == "Authorization").Value.ToString();
            if (String.IsNullOrWhiteSpace(authHeader))
            {
                context.Fail();
                return;
            }
            var token = authHeader.Split(' ')[1];
            string cacheToken;
            _cache.TryGetValue(token, out cacheToken);
            if (cacheToken != null)
            {
                EnrichClams(context, cacheToken);
                context.Succeed(requirement);
                return;
            }

            var client = new HttpClient();
            var url = $"https://www.googleapis.com/oauth2/v1/userinfo?access_token={token}";
            var result = await client.GetAsync(url);
            if (!result.IsSuccessStatusCode)
            {
                context.Fail();
                return;
            }

            var json = await result.Content.ReadAsStringAsync();
            EnrichClams(context, json);


            _cache.Set(token, json);
            context.Succeed(requirement);
        }

        private void EnrichClams(AuthorizationHandlerContext context, string jsonResponse)
        {
            var userInfo = JsonConvert.DeserializeObject<UserInfo>(jsonResponse);

            context.User.AddIdentity(new ClaimsIdentity(new List<Claim> {
                new Claim("id", userInfo.id),
                new Claim("email", userInfo.email),
                new Claim("family_name", userInfo.family_name),
                new Claim("given_name", userInfo.given_name),
                new Claim("link", userInfo.link),
                new Claim("locale", userInfo.locale),
                new Claim("name", userInfo.name),
                new Claim("picture", userInfo.picture),
                new Claim("verified_email", userInfo.verified_email.ToString())
            }));
        }

    }
}
