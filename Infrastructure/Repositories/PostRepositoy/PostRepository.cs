using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using Core.Entities.PostEntities;
using Core.Interfaces.Repositories.PostRepositories;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.PostRepositoy
{
    public class PostRepository : IPostRepository
    {
        private readonly FitnessContext _context;

        public PostRepository(FitnessContext context)
        {
            _context = context;
        }

        public IntResult Add(AddPostDTO postDto)
        {
            Post newPost = postDto switch
            {
                AddGymPostDTO gymPostDto => new GymPost(gymPostDto),
                AddCoachPostDTO coachPostDto => new CoachPost(coachPostDto),
                AddShopPostDTO shopPostDto => new ShopPost(shopPostDto),
                _ => null
            };
            if (newPost == null)
            {
                return new IntResult { Massage = "Unvalid post type" };
            }
            _context.Posts.Add(newPost);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }

            return new IntResult { Id = newPost.Id };
        }

        public IntResult Delete(int id)
        {
            var post = GetPost(id);
            if (post is null)
            {
                return new IntResult { Massage = "No Post has this Id." };
            }

            _context.Posts.Remove(post);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }

            return new IntResult { Id = 1 };
        }
        /*
        public ShowPostDTO Get(int id)
        {
            var post = GetPost(id);
            if (post is null)
            {
                return null;
            }

            return post switch
            {
                CoachPost coachPost => new ShowCoachPostDTO(coachPost),
                GymPost gymPost => new ShowGymPostDTO(gymPost),
                ShopPost shopPost => new ShowShopPostDTO(shopPost),
                _ => null
            };
        }
        /*
        public List<ShowPostDTO> ShowPagination(string traineId, int page, int pageSize)
        {
            /*
            return posts.Select(post => post switch
            {
                CoachPost coachPost => new ShowCoachPostDTO(coachPost),
                GymPost gymPost => new ShowGymPostDTO(gymPost),
                ShopPost shopPost => new ShowShopPostDTO(shopPost),
                _ => throw new InvalidOperationException("Unknown post type")
            }).ToList();
            return null;
        }
    */
        private Post GetPost(int id) => _context.Posts.Find(id);
    }

}
