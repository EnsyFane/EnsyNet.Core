namespace EnsyNet.DataAccess.Abstractions.Models;

public class EntityUpdates<T> where T : DbEntity
{
#pragma warning disable IDE0060 // Remove unused parameter
    public EntityUpdates<T> AddUpdate<TProp>(Func<T, TProp> propertyExpression, Func<T, TProp> valueExpression)
    {
        return this;
    }
#pragma warning restore IDE0060 // Remove unused parameter

}

