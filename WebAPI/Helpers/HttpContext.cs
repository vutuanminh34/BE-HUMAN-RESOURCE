using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace WebAPI.Helpers
{
    public static class HttpContext
    {
        private static IHttpContextAccessor m_httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            m_httpContextAccessor = httpContextAccessor;
        }

        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                return m_httpContextAccessor.HttpContext;
            }
        }

        public static string CurrentUser
        {
            get
            {
                return m_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }
        }

    }
}
