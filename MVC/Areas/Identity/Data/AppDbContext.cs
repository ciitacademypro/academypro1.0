using LmsModels.Admin;
using LmsModels.Administrator;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.Areas.Identity.Data;
using MVC.Areas.Admin;
using MVC.Areas.Administrator;
using Newtonsoft.Json;
using System.Reflection.Emit;
using MVC.Areas.Admin.Models;
using MVC.Areas.Administrator.Models;

namespace MVC.Areas.Identity.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
		// Customize the ASP.NET Identity model and override the defaults if needed.
		// For example, you can rename the ASP.NET Identity table names and more.
		// Add your customizations after calling base.OnModelCreating(builder);

		builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());

	
		// for ignoring migration building

		//builder.Ignore<Branch>();

	}

	public DbSet<Firm> Firms { get; set; }
	public DbSet<Branch> Branches { get; set; }
}

public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<AppUser>
{
	public void Configure(EntityTypeBuilder<AppUser> builder)
	{
		builder.Property(x => x.FirmId).IsRequired(false).HasDefaultValue(null);
		builder.Property(x => x.BranchId).IsRequired(false).HasDefaultValue(null);
		builder.Property(x => x.Name).IsRequired().HasMaxLength(60);
		builder.Property(x => x.Uid1).HasMaxLength(30).IsRequired(false).HasDefaultValue(null);
		builder.Property(x => x.Uid2).HasMaxLength(30).IsRequired(false).HasDefaultValue(null);
		builder.Property(x => x.Status).IsRequired(false).HasDefaultValue(false);
		builder.Property(x => x.DeletedAt).IsRequired(false).HasDefaultValue(null);

		// Add unique constraint on Uid1
		builder.HasIndex(x => x.Uid1).IsUnique();
		builder.Property(e => e.BranchId).HasColumnName("BranchId");

		// Define the relationship between AppUser and Firm
		builder.HasOne(u => u.Firm)
			.WithMany(f => f.Users)
			.HasForeignKey(u => u.FirmId)
			.OnDelete(DeleteBehavior.Restrict); // Optional: Prevent cascade delete


		// Define the relationship between AppUser and Branch
		builder.HasOne(u => u.Branch)
			.WithMany(b => b.Users)
			.HasForeignKey(u => u.BranchId)
			.OnDelete(DeleteBehavior.Restrict); // Optional: Prevent cascade delete


	}
}