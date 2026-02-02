using Microsoft.EntityFrameworkCore;
using HabitConnectAPI.Entities;
using System.Collections.Generic;


namespace HabitConnectAPI.Data
{
    public class HabitConnectContext : DbContext
    {
        public HabitConnectContext(DbContextOptions<HabitConnectContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Habit> Habits { get; set; }
        public DbSet<Journal> Journals { get; set; }
        public DbSet<Community> Communities { get; set; }
        public DbSet<UserCommunity> UserCommunities { get; set; }
    }
}
