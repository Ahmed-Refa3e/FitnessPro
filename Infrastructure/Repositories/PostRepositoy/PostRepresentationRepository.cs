using Core.DTOs.PostDTO;
using Core.Entities.PostEntities;
using Core.Interfaces.Repositories.PostRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.PostRepositoy
{
    public class PostRepresentationRepository: IPostRepresentationRepository
    {
        private readonly FitnessContext _context;

        public PostRepresentationRepository(FitnessContext context)
        {
            _context = context;
        }

        public ShowPostDTO Get(int id)
        {
            var post = GetPost(id);
            if (post is null)
            {
                return null;
            }

            return post switch
            {
                CoachPost coachPost => new ShowCoachPostDTO(_context.CoachPosts.Where(x=>x.Id==id).Include(x => x.PictureUrls).Include(x=>x.Coach).FirstOrDefault()),
                GymPost gymPost => new ShowGymPostDTO(_context.GymPosts.Where(x => x.Id == id).Include(x => x.PictureUrls).Include(x => x.Gym).FirstOrDefault()),
                ShopPost shopPost => new ShowShopPostDTO(_context.ShopPosts.Where(x => x.Id == id).Include(x => x.Shop).Include(x=>x.PictureUrls).FirstOrDefault()),
                _ => null
            };
        }

        public List<ShowCoachPostDTO> GetCoachPosts(int id)
        {
            throw new NotImplementedException();
        }

        public List<ShowGymPostDTO> GetGymPosts(int id)
        {
            throw new NotImplementedException();
        }

        public List<ShowShopPostDTO> GetShopPosts(int id)
        {
            throw new NotImplementedException();
        }

        private Post GetPost(int id) => _context.Posts.Find(id);
    }
}
