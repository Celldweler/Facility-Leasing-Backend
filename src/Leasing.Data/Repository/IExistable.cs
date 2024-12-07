namespace Leasing.Data.Repository;

public interface IExistable<in T>
{
    /// <summary>
    /// Checks entity elements for uniqueness.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <returns>Bool.</returns>
    Task<bool> SameExists(T entity);
}