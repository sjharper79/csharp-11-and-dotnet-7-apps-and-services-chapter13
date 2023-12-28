namespace Northwind.Chat.Models;

public class UserModel
{
    public string Name { get; set; }
    public string ConnectionId { get; set; }
    public string? Groups { get; set; }
}
