using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using STB_backend.Models;
using Supabase.Gotrue;

namespace STB_backend.Security
{
    public class UserRoleRequirement : IAuthorizationRequirement
    {
        public string RequiredRole { get; }
        public UserRoleRequirement(string requiredRole)
        {
            RequiredRole = requiredRole;
        }
    }
    public class UserRoleHandler : AuthorizationHandler<UserRoleRequirement>
    {
        private readonly Supabase.Client _supabaseClient;
        public UserRoleHandler(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRoleRequirement requirement)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? context.User.FindFirst("sub")?.Value;

            var userEmail = context.User.FindFirst(ClaimTypes.Email)?.Value
                            ?? context.User.FindFirst("email")?.Value;

            if (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(userEmail))
            {
                return;
            }

            try
            {
                AppUser? user = null;
                if (!string.IsNullOrEmpty(userId))
                {
                    var res = await _supabaseClient.From<AppUser>().Where(u => u.Id == userId).Get();
                    user = res.Models.FirstOrDefault();
                }
                if (user == null && !string.IsNullOrEmpty(userEmail))
                {
                    var res = await _supabaseClient.From<AppUser>().Where(u => u.Email == userEmail).Get();
                    user = res.Models.FirstOrDefault();
                }

                if (user != null && string.Equals(user.UserType.ToString(), requirement.RequiredRole, StringComparison.OrdinalIgnoreCase))
                {
                    context.Succeed(requirement);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba a szerep lekérdezése közben: {ex.Message}");
            }
        }
    }
}
