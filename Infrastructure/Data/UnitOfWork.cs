using Core.Interfaces.Repositories;
using Core.Interfaces.Repositories.OnlineTrainingRepositories;
using Infrastructure.Repositories;

namespace Infrastructure.Data;
public class UnitOfWork(
    FitnessContext context,
    IGymRepository gymRepository,
    IOnlineTrainingRepository onlineTrainingRepository,
    IOnlineTrainingSubscriptionRepository onlineTrainingSubscriptionRepository) : IUnitOfWork
{
    private readonly FitnessContext context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly Dictionary<Type, object> Repositories = [];

    public IGymRepository GymRepository { get; } = gymRepository ?? throw new ArgumentNullException(nameof(gymRepository));
    public IOnlineTrainingRepository OnlineTrainingRepository { get; } =
        onlineTrainingRepository ?? throw new ArgumentNullException(nameof(onlineTrainingRepository));
    public IOnlineTrainingSubscriptionRepository OnlineTrainingSubscriptionRepository { get; } =
        onlineTrainingSubscriptionRepository ?? throw new ArgumentNullException(nameof(onlineTrainingSubscriptionRepository));

    public async Task<bool> CompleteAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }

    public IGenericRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);

        if (!Repositories.TryGetValue(type, out object? value))
        {
            GenericRepository<T> repository = new(context);
            value = repository;
            Repositories[type] = value;
        }

        return (IGenericRepository<T>)value;
    }

}
