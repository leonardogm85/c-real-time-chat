using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RealTimeChat.Api.Data;
using RealTimeChat.Api.Models;

namespace RealTimeChat.Api.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatContext _context;

        public ChatHub(ChatContext context)
        {
            _context = context;
        }

        public async Task Register(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                await Clients.Caller.SendAsync("ReceiveRegisteredUser", false, user, "Email already registered!");
            }
            else
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                await Clients.Caller.SendAsync("ReceiveRegisteredUser", true, user, "Registered successfully!");
            }
        }

        public async Task LogIn(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email && u.Password == user.Password))
            {
                await Clients.Caller.SendAsync("ReceiveLoggedInUser", true, user, "Logged In successfully!");
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveLoggedInUser", false, user, "Email or password invalid!");
            }
        }
    }
}
