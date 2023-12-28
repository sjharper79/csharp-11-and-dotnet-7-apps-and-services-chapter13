using Microsoft.AspNetCore.SignalR;
using Northwind.Chat.Models; // UserModel class

namespace Northwind.SignalR.Service.Hubs;

public class ChatHub : Hub
{
    private static Dictionary<string, UserModel> Users = new();

    public async Task Register(UserModel newUser)
    {
        UserModel user;
        string action = "registered as a new user";
        // If the user is already in the dictionary, set the local user object to the user in the dictionary
        if (Users.ContainsKey(newUser.Name))
        {
            user = Users[newUser.Name];
            if (user.Groups is not null)
            {
                foreach (string group in user.Groups.Split(','))
                {
                    await Groups.RemoveFromGroupAsync(user.ConnectionId, group);
                }
            }
            user.Groups = newUser.Groups;
            user.ConnectionId = Context.ConnectionId;

            action = "updated your registered user";
        }

        // But if it isn't in the dictionary, make a new user with a name that is a Guid
        else
        {
            if (string.IsNullOrEmpty(newUser.Name))
            {
                newUser.Name = Guid.NewGuid().ToString();
            }
            newUser.ConnectionId = Context.ConnectionId;
            Users.Add(key: newUser.Name, value: newUser);
            user = newUser;
        }

        if (user.Groups is not null)
        {
            foreach (string group in user.Groups.Split(','))
            {
                await Groups.AddToGroupAsync(user.ConnectionId, group);
            }
        }

        MessageModel message = new()
        {
            From = "SignalR Hub",
            To = user.Name,
            Body = string.Format(
                "You have successfully {0} with connection ID {1}.",
                action, user.ConnectionId)
        };

        IClientProxy proxy = Clients.Client(user.ConnectionId);
        await proxy.SendAsync("ReceiveMessage", message);
    }

    public async Task SendMessage(MessageModel message)
    {
        IClientProxy proxy;
        if (string.IsNullOrEmpty(message.To))
        {
            message.To = "Everyone";
            proxy = Clients.All;
            await proxy.SendAsync("ReceiveMessage", message);
        }

        string[] userAndGroupList = message.To.Split(",");

        foreach (string userOrGroup in userAndGroupList)
        {
            if (Users.ContainsKey(userOrGroup))
            {
                message.To = $"User: {Users[userOrGroup].Name}";
                proxy = Clients.Client(Users[userOrGroup].ConnectionId);
            }
            else
            {
                message.To = $"Group: {userOrGroup}";
                proxy = Clients.Group(userOrGroup);
            }
            await proxy.SendAsync("ReceiveMessage", message);
        }
    }
}