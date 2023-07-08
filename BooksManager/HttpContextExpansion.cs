using System.Security.Claims;

namespace BooksManager
{
    public static class HttpContextExpansion
    {
        public static int GetUserID(this HttpContext context) {
            return int.Parse(context.User.FindFirst("ID").Value);
        }
    }
}
