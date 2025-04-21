using Core.Interfaces.Repositories.PostRepositories;

namespace Core.Interfaces.Factories
{
    public interface IPostRepositoryFactory
    {
        IGeneralPostRepository CreateRepository(string repositoryType);
    }
}
