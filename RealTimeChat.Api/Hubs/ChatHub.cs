using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RealTimeChat.Api.Data;
using RealTimeChat.Api.Models;
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

        public async Task AddConnection(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user is not null)
            {
                var connectionsId = new List<string>();

                if (string.IsNullOrEmpty(user.ConnectionsId))
                {
                    connectionsId.Add(Context.ConnectionId);
                }
                else
                {
                    connectionsId = JsonSerializer.Deserialize<List<string>>(user.ConnectionsId);

                    if (!connectionsId!.Any(c => c == Context.ConnectionId))
                    {
                        connectionsId!.Add(Context.ConnectionId);
                    }
                }

                user.SetConnectionsId(JsonSerializer.Serialize(connectionsId));
                user.SetIsOnline(true);
                await _context.SaveChangesAsync();

                var users = await _context.Users
                    .AsNoTracking()
                    .Select(u => (UserViewModel)u)
                    .ToListAsync();
                await Clients.Others.SendAsync("ReceiveUsers", users);

                var groups = await _context.Groups
                    .AsNoTracking()
                    .Where(g => g.Users.Contains(user.Email))
                    .ToListAsync();

                groups.ForEach(g =>
                {
                    connectionsId?.ForEach(async c =>
                    {
                        await Groups.AddToGroupAsync(c, g.Name);
                    });
                });
            }
        }

        public async Task RemoveConnection(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user is not null && !string.IsNullOrEmpty(user.ConnectionsId))
            {
                var connectionsId = JsonSerializer.Deserialize<List<string>>(user.ConnectionsId);

                if (connectionsId!.Any(c => c == Context.ConnectionId))
                {
                    connectionsId!.Remove(Context.ConnectionId);
                }

                user.SetConnectionsId(JsonSerializer.Serialize(connectionsId));
                user.SetIsOnline(false);
                await _context.SaveChangesAsync();

                var users = await _context.Users
                    .AsNoTracking()
                    .Select(u => (UserViewModel)u)
                    .ToListAsync();
                await Clients.Others.SendAsync("ReceiveUsers", users);

                var groups = await _context.Groups
                    .AsNoTracking()
                    .Where(g => g.Users.Contains(user.Email))
                    .ToListAsync();

                groups.ForEach(g =>
                {
                    connectionsId?.ForEach(async c =>
                    {
                        await Groups.RemoveFromGroupAsync(c, g.Name);
                    });
                });
            }
        }

        public async Task GetUsers()
        {
            var users = await _context.Users
                .AsNoTracking()
                .Select(u => (UserViewModel)u)
                .ToListAsync();
            await Clients.Caller.SendAsync("ReceiveUsers", users);
        }

        public async Task CreateGroup(string loggedInUserEmail, string selectedUserEmail)
        {
            var loggedInUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == loggedInUserEmail);

            var selectedUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == selectedUserEmail);

            if (loggedInUser is not null && selectedUser is not null)
            {
                var emails = new List<string>
                {
                    loggedInUserEmail,
                    selectedUserEmail
                };

                var groupName = emails
                    .OrderBy(e => e)
                    .Aggregate((a, e) => $"{a}-{e}");

                var group = await _context.Groups
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.Name == groupName);

                if (group is null)
                {
                    var serializedUsers = JsonSerializer.Serialize(emails.OrderBy(e => e));

                    group = new(groupName, serializedUsers);

                    await _context.Groups.AddAsync(group);
                    await _context.SaveChangesAsync();
                }

                var users = new List<User> {
                    loggedInUser,
                    selectedUser
                };

                users.ForEach(u =>
                {
                    var connectionsId = JsonSerializer.Deserialize<List<string>>(u.ConnectionsId);

                    connectionsId?.ForEach(async c =>
                    {
                        await Groups.AddToGroupAsync(c, groupName);
                    });
                });

                var messages = await _context.Messages
                    .AsNoTracking()
                    .Include(m => m.Group)
                    .Include(m => m.User)
                    .Where(m => m.GroupId == group.Id)
                    .OrderBy(m => m.CreateAt)
                    .Select(m => (MessageViewModel)m)
                    .ToListAsync();

                await Clients.Caller.SendAsync("ReceiveGroup", groupName, messages);
            }
        }

        public async Task SendMessage(Guid userId, string groupName, string textMessage)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == groupName);

            if (user is not null && group is not null && group.Users.Contains(user.Email))
            {
                var message = new Message(group.Id, user.Id, textMessage);

                message.SetGroup(group);
                message.SetUser(user);

                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                await Clients.Group(group.Name).SendAsync("ReceiveMessage", (MessageViewModel)message);
            }
        }
    }
}
