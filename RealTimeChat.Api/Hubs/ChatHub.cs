﻿using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RealTimeChat.Api.Data;
using RealTimeChat.Api.ViewModels;
using System.Text.Json;

namespace RealTimeChat.Api.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatContext _context;

        public ChatHub(ChatContext context)
        {
            _context = context;
        }

        public async Task Register(RegisterViewModel viewModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == viewModel.Email);

            if (user is null)
            {
                user = new(viewModel.Name, viewModel.Email, viewModel.Password);

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                await Clients.Caller.SendAsync("ReceiveRegisteredUser", true, (UserViewModel)user, "Registered successfully!");
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveRegisteredUser", false, null, "Email already registered!");
            }
        }

        public async Task LogIn(LogInViewModel viewModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == viewModel.Email && u.Password == viewModel.Password);

            if (user is null)
            {
                await Clients.Caller.SendAsync("ReceiveLoggedInUser", false, null, "Email or password invalid!");
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveLoggedInUser", true, (UserViewModel)user, "Logged In successfully!");
            }
        }

        public async Task AddConnection(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user is not null)
            {
                var connectionsId = new List<string>();

                if (string.IsNullOrEmpty(user!.ConnectionsId))
                {
                    connectionsId.Add(Context.ConnectionId);
                }
                else
                {
                    connectionsId = JsonSerializer.Deserialize<List<string>>(user!.ConnectionsId);

                    if (!connectionsId!.Any(c => c == Context.ConnectionId))
                    {
                        connectionsId!.Add(Context.ConnectionId);
                    }
                }

                user!.SetConnectionsId(JsonSerializer.Serialize(connectionsId));
                user!.SetIsOnline(true);
                await _context.SaveChangesAsync();

                var users = await _context.Users
                    .AsNoTracking()
                    .ToListAsync();
                await Clients.All.SendAsync("ReceiveUsers", users);
            }
        }

        public async Task RemoveConnection(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user is not null && !string.IsNullOrEmpty(user!.ConnectionsId))
            {
                var connectionsId = JsonSerializer.Deserialize<List<string>>(user!.ConnectionsId);

                if (connectionsId!.Any(c => c == Context.ConnectionId))
                {
                    connectionsId!.Remove(Context.ConnectionId);
                }

                user!.SetConnectionsId(JsonSerializer.Serialize(connectionsId));
                user!.SetIsOnline(false);
                await _context.SaveChangesAsync();

                var users = await _context.Users
                    .AsNoTracking()
                    .ToListAsync();
                await Clients.All.SendAsync("ReceiveUsers", users);
            }
        }

        public async Task GetUsers()
        {
            var users = await _context.Users
                .AsNoTracking()
                .ToListAsync();
            await Clients.Caller.SendAsync("ReceiveUsers", users);
        }
    }
}
