using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleToken.AuthPolicies
{
    public class ValidGoogleRequirement : IAuthorizationRequirement
    {
        public string token { get; set; }
    }

}
