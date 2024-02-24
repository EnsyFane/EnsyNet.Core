using EnsyNet.DataAccess.Abstractions.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnsyNet.DataAccess.EntityFramework.Configuration;

public static class DbEntityConfigurationExtensions
{
    public static void ConfigureBaseProperties<T>(this EntityTypeBuilder<T> builder) where T : DbEntity
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWSEQUENTIALID()");
        builder.Property(e => e.Id)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.Id)
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();
        builder.Property(e => e.CreatedAt)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.CreatedAt)
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnUpdate();
        builder.Property(e => e.UpdatedAt)
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(e => e.DeletedAt)
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
    }
}
