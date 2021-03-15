using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace WerewolfCircle.Auth
{
    public class Policies
    {
        public const string AdminPolicyName = "admin_policy";
        public const string AdminRole = "Admin";


        public static AuthorizationPolicy BuildAdminPolicy() => new AuthorizationPolicyBuilder()
                                                                    .RequireAuthenticatedUser()
                                                                    .RequireRole(AdminRole)
                                                                    .Build();
    }
}
