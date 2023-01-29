using Microsoft.EntityFrameworkCore;

namespace RealTimeChat.Data
{
    public sealed class ChatContext : DbContext
    {
        public ChatContext(DbContextOptions<ChatContext> options) : base(options)
        {
        }
    }
}
