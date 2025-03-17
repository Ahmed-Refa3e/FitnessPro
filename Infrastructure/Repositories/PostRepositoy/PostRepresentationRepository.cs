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
                CoachPost coachPost => _context.CoachPosts.Select(p => new ShowCoachPostDTO
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    PhotoPass = p.Coach.ProfilePictureUrl??"",
                    Name = p.Coach.FirstName + " " + p.Coach.FirstName,
                    CoachId = p.CoachId,
                    PictureUrls = p.PictureUrls.Select(x => x.Url).ToList()
                }).Where(x => x.Id == id)
                .FirstOrDefault(),
                GymPost gymPost => _context.GymPosts.Select(p => new ShowGymPostDTO
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    PhotoPass = p.Gym.PictureUrl ?? "",
                    Name = p.Gym.GymName,
                    GymId = p.GymId,
                    PictureUrls = p.PictureUrls.Select(x => x.Url).ToList()
                }).Where(x => x.Id == id)
                .FirstOrDefault(),
                ShopPost shopPost => _context.ShopPosts.Select(p => new ShowShopPostDTO
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    PhotoPass = p.Shop.PictureUrl ?? "",
                    Name = p.Shop.Name,
                    ShopId = p.ShopId,
                    PictureUrls = p.PictureUrls.Select(x => x.Url).ToList()
                }).Where(x => x.Id == id)
                .FirstOrDefault(),
                _ => null
            };
        }
        private Post GetPost(int id) => _context.Posts.Find(id);
    }
}
