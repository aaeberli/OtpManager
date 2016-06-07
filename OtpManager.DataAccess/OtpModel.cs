namespace OtpManager.DataAccess
{
    using Domain.Model;
    using System.Data.Entity;
    public partial class OtpModel : DbContext
    {
        public OtpModel()
            : base("name=OtpModel")
        {
        }

        public virtual DbSet<Otp> Otps { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Otp>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.UserId)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasOptional(e => e.Otp)
                .WithRequired(e => e.User);
        }

    }
}
