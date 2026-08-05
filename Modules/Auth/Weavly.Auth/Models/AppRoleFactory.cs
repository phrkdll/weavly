namespace Weavly.Auth.Models;

public static class AppRoleFactory
{
    extension(AppRole)
    {
        public static AppRole Create(string name)
        {
            return new AppRole(name);
        }
    }
}
