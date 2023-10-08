namespace MiniApi.Application.Common;

public class AuthRole
{
    public static readonly string Admin = "Admin";
    public static readonly string User = "User";

    public static string[] GetAuthRoles()
    {
        return new []
        {
            Admin,
            User
        };
    }
}