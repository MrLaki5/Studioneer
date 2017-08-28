namespace WebRole1.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Channel> Channels { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Parameter> Parameters { get; set; }
        public virtual DbSet<Published> Publisheds { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Response> Responses { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answer>()
                .Property(e => e.Tag)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Answer>()
                .Property(e => e.Text)
                .IsUnicode(false);

            modelBuilder.Entity<Answer>()
                .HasMany(e => e.Responses)
                .WithRequired(e => e.Answer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Channel>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Channel>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<Channel>()
                .HasMany(e => e.Responses)
                .WithRequired(e => e.Channel)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Channel>()
                .HasMany(e => e.Publisheds)
                .WithRequired(e => e.Channel)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Channel>()
                .HasMany(e => e.Subscriptions)
                .WithRequired(e => e.Channel)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.State)
                .IsUnicode(false);

            modelBuilder.Entity<Question>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<Question>()
                .Property(e => e.Text)
                .IsUnicode(false);

            modelBuilder.Entity<Question>()
                .Property(e => e.Image)
                .IsUnicode(false);

            modelBuilder.Entity<Question>()
                .Property(e => e.ImageName)
                .IsUnicode(false);

            modelBuilder.Entity<Question>()
                .HasMany(e => e.Answers)
                .WithRequired(e => e.Question)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Question>()
                .HasMany(e => e.Publisheds)
                .WithRequired(e => e.Question)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Question>()
                .HasMany(e => e.Responses)
                .WithRequired(e => e.Question)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Mail)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Lastname)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Channels)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Questions)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Responses)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Subscriptions)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);
        }
    }
}
