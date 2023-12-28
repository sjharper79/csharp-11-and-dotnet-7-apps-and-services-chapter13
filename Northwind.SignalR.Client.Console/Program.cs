﻿using Microsoft.AspNetCore.SignalR.Client;
using Northwind.Chat.Models;

Write("Enter a username (required): ");
string? username = ReadLine();
if (string.IsNullOrEmpty(username))
{
    WriteLine("You must enter a username to register with chat!");
    return;
}

Write("Enter your groups (optional): ");
string? groups = ReadLine();

HubConnection hubConnection = new HubConnectionBuilder().WithUrl("https://localhost:5131/chat").Build();

hubConnection.On<MessageModel>("ReceiveMessage", message =>
{
    WriteLine($"To {message.To}, From {message.From}: {message.Body}");
});

await hubConnection.StartAsync();

WriteLine("Successfully started.");

UserModel registration = new()
{
    Name = username,
    Groups = groups,
};

await hubConnection.InvokeAsync("Register", registration);

WriteLine("Successfully Registered");
WriteLine("Listening... (press Enter to stop.)");
ReadLine();