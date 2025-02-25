using EnsyNet.DataAccess.Abstractions.Attributes;
using EnsyNet.DataAccess.Abstractions.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace EnsyNet.DataAccess.EntityFramework.Configuration;

/// <summary>
/// Extension methods for configuring <see cref="DbEntity"/> properties.
/// </summary>
public static class DbEntityConfigurationExtensions
{
    /// <summary>
    /// Configures the base properties of a <see cref="DbEntity"/>.
    /// </summary>
    /// <typeparam name="T">The type of the entity to configure.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{T}"/> to use for configuring the entity.</param>
    /// <param name="indexes">A list of indexes to be added to the table</param>
    public static void ConfigureBaseProperties<T>(this EntityTypeBuilder<T> builder, params string[][] indexes) where T : DbEntity
    {
        var partitionKeys = typeof(T).GetCustomAttributes<DbEntityPartitionAttribute>()
            .OrderBy(a => a.Priority)
            .Select(a => a.PartitionKey)
            .ToArray();
        builder.ConfigureEntity();
        builder.AddPartitionedIndexes(partitionKeys, indexes);
    }

    private static void ConfigureEntity<T>(this EntityTypeBuilder<T> builder) where T : DbEntity
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
        builder.HasQueryFilter(e => e.DeletedAt == null);
        builder.Property(e => e.DeletedAt)
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
        builder.HasQueryFilter(e => e.DeletedAt == null);
    }

    private static void AddPartitionedIndexes<T>(this EntityTypeBuilder<T> builder, string[] partitionKeys, string[][] indexes) where T : DbEntity
    {
        builder.HasIndex(e => e.Id)
            .HasFilter("DeletedAt is NULL");

        if (partitionKeys.Length != 0)
        {
            builder.HasIndex(partitionKeys)
                .HasFilter("DeletedAt is NULL");

            foreach (var index in indexes)
            {
                builder.HasIndex([.. partitionKeys, .. index])
                    .HasFilter("DeletedAt is NULL");
            }
        }
        else
        {
            foreach (var index in indexes)
            {
                builder.HasIndex(index)
                    .HasFilter("DeletedAt is NULL");
            }
        }
    }
}
