using LmsModels.Administrator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.Areas.Administrator.Models;

namespace MVC.Data.Configurations
{
	public class FirmModelConfiguration : IEntityTypeConfiguration<Firm>
	{
		public void Configure(EntityTypeBuilder<Firm> builder)
		{
			builder.HasKey(f => f.FirmId); // Primary Key

			builder.HasIndex(f => f.FirmShortName).IsUnique(); // Unique constraint

			builder.Property(f => f.FirmShortName)
				.IsRequired()
				.HasMaxLength(100); // Required with Max Length

			builder.Property(f => f.FirmName)
				.IsRequired(); // Required

			builder.Property(f => f.FirmEmail)
				.IsRequired(); // Required

			builder.Property(f => f.FirmContact)
				.IsRequired(); // Required

			builder.Property(f => f.Status)
				.HasDefaultValue("active"); // Default value for Status

			builder.Property(f => f.DeletedAt)
				.HasColumnType("datetime2")
				.HasDefaultValue(null); // Default value for DeletedAt
		}
	}
}
