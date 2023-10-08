namespace MiniApi.Model;

public class Staff
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public string? Description { get; private set; }

    public Staff(string name, string email, string password, string? description)
    {
        Name = name;
        Email = email;
        Password = password;
        Description = description;
    }

    public Staff Update(string email, string? description)
    {
        Email = email;
        Description = description;

        return this;
    }
}