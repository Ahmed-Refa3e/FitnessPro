using Core.Interfaces.Repositories.PostRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Factories
{
    public interface IPostRepositoryFactory
    {
        IPostRepository CreateRepository(string repositoryType);
    }
}
