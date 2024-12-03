namespace Core.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetQueryable();
        Task<IReadOnlyList<T>> ExecuteQueryAsync(IQueryable<T> query);
        Task<T?> GetByIdAsync(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<bool> ExistsAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}
