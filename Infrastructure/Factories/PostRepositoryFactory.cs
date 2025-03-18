using Core.Interfaces.Factories;
using Core.Interfaces.Repositories.PostRepositories;
using Infrastructure.Repositories.PostRepositoy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Factories
{
    public class PostRepositoryFactory:IPostRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PostRepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IGeneralPostRepository CreateRepository(string repositoryType)
        {
            return repositoryType switch
            {
                "SHOP" => _serviceProvider.GetRequiredService<ShopPostRepository>(),
                "GYM" => _serviceProvider.GetRequiredService<GymPostRepository>(),
                "COACH" => _serviceProvider.GetRequiredService<CoachPostRepository>(),
                _ => null
            };
        }
    }
}
