namespace Tryout_Respond
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class RespondContext : DbContext
    {
        public RespondContext()
            : base("name=RespondContext")
        {
        }

        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Taart> Taart { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                .Property(e => e.userID)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.username)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.passwordHash)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.token)
                .IsUnicode(false);
        }
    }
}
