using Core.AnchorCalculator.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.AnchorCalculator
{
	public class ApplicationDbContext : IdentityDbContext<User>, IDataProtectionKeyContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
		public DbSet<Anchor> Anchors { get; set; }
		public DbSet<Material> Materials { get; set; }
		public override DbSet<User> Users { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			SetNullBehaviour(builder);
		}

		private static void SetNullBehaviour(ModelBuilder builder)
		{
			builder.Entity<Anchor>()
				.HasOne(b => b.Material)
				.WithMany(a => a.Anchors)
				.OnDelete(DeleteBehavior.SetNull
			);

			builder.Entity<Anchor>()
				.HasOne(b => b.User)
				.WithMany(a => a.Anchors)
				.OnDelete(DeleteBehavior.SetNull
			);
		}
	}
}
