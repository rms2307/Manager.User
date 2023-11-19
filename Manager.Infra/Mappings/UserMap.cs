using Manager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manager.Infra.Mappings
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER")
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(80)
                .HasColumnName("name")
                .HasColumnType("TEXT");

            builder.Property(x => x.Password)
                .IsRequired()
                .HasMaxLength(1000)
                .HasColumnName("password")
                .HasColumnType("TEXT");

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(180)
                .HasColumnName("email")
                .HasColumnType("TEXT");

        }
    }
}
