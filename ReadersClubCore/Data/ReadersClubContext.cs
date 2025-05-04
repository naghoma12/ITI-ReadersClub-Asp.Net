using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReadersClubCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersClubCore.Data
{
    public class ReadersClubContext: IdentityDbContext<ApplicationUser,ApplicationRole,int>
    {
        public ReadersClubContext(DbContextOptions<ReadersClubContext> options):base(options)
        {
            
        }
        public ReadersClubContext()
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ReadingProgress>()
                .HasOne(x => x.Story)
                .WithMany(x => x.ReadingProgresses)
                .HasForeignKey(x => x.StoryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<SavedStories>()
                .HasOne(x => x.Story)
                .WithMany(x => x.SavedStories)
                .HasForeignKey(x => x.StoryId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Review>()
                .HasOne(x => x.Story)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.StoryId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Story>()
                .HasOne(x => x.Channel)
                .WithMany(x => x.Stories)
                .HasForeignKey(x => x.ChannelId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Subscribtion>()
                .HasOne(x => x.Channel)
                .WithMany(x => x.Subscribtions)
                .HasForeignKey(x => x.ChannelId)
                .OnDelete(DeleteBehavior.NoAction);
        }
        public DbSet<Story> Stories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<ReadingProgress> ReadingProgresses { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<SavedStories> SavedStories { get; set; }
        public DbSet<Subscribtion> Subscribtions { get; set; }

    }
}
