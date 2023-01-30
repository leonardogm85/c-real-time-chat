using Microsoft.EntityFrameworkCore;
using RealTimeChat.Api.Models;

namespace RealTimeChat.Api.Data
{
    public class ChatContext : DbContext
    {
        public ChatContext(DbContextOptions<ChatContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<Message> Messages => Set<Message>();
    }
}
