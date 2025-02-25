namespace EnsyNet.DataAccess.Abstractions.Attributes;

/// <summary>
/// Attribute to define the partition key of an entity.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class DbEntityPartitionAttribute : Attribute
{
    internal string PartitionKey { get; init; }
    internal int Priority { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbEntityPartitionAttribute"/> class.
    /// </summary>
    /// <param name="partitionKey">The name of the field to be used as a partition key.</param>
    /// <param name="priority">The priority of the partition key. Lower means higher priority.</param>
    public DbEntityPartitionAttribute(string partitionKey, int priority)
    {
        PartitionKey = partitionKey;
        Priority = priority;
    }
}
