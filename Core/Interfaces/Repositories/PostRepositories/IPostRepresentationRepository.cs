using Core.DTOs.PostDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories.PostRepositories
{
    public  interface IPostRepresentationRepository
    {
        ShowPostDTO Get(int id);
        List<ShowShopPostDTO> GetShopPosts(int id);
        List<ShowGymPostDTO> GetGymPosts(int id);
        List<ShowCoachPostDTO> GetCoachPosts(int id);
    }
}
