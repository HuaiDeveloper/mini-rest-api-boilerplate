namespace MiniApi.Common;

public static class AuthRole
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