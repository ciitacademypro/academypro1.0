using LmsModels.Administrator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MVC.Data.Configurations
{
	public class QualificationModelConfiguration : IEntityTypeConfiguration<QualificationModel>
	{
		public void Configure(EntityTypeBuilder<QualificationModel> builder)
		{
			builder.HasKey(f => f.QualificationId); // Primary Key

			builder.HasIndex(f => f.QualificationName).IsUnique(); // Unique constraint


			builder.Property(f => f.Status)
				.IsRequired() // Required
				.HasDefaultValue(1); // Default value for DeletedAt


			builder.Property(f => f.DeletedAt)
				.HasColumnType("datetime2")
				.HasDefaultValue(null); // Default value for DeletedAt
		}
	}

}