using EnsyNet.DataAccess.Abstractions.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnsyNet.DataAccess.EntityFramework.Configuration;

public static class DbEntityConfigurationExtensions
{
    public static void Configure<T>(this EntityTypeBuilder<T> builder) where T : DbEntity
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();
        builder.Property(e => e.Id)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.Id)
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(e => e.CreatedAt)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("getdate()");
        builder.Property(e => e.CreatedAt)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.CreatedAt)
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(e => e.UpdatedAt)
            .ValueGeneratedOnUpdate()
            .HasDefaultValueSql("getdate()");
        builder.Property(e => e.UpdatedAt)
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(e => e.DeletedAt)
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
    }
}
