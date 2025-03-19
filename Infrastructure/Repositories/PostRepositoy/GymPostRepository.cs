using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using Core.Entities.PostEntities;
using Core.Interfaces.Repositories.PostRepositories;
using Infrastructure.Data;
using Microsoft.IdentityModel.Tokens;
using Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.PostRepositoy
{
    public class GymPostRepository : GeneralPostRepository
    {
        public GymPostRepository(FitnessContext context):base(context)
        {
        }
        public override async Task<IntResult> Add(AddPostDTO post)
        {
            var newPost = post as AddGymPostDTO;
            if(post is null)
            {
                return new IntResult { Massage = "The post is not valid" };
            }
            var gymPost = new GymPost { Content = newPost.Content,GymId= newPost.GymId};
            _context.GymPosts.Add(gymPost);
            return await AddPicturesToPost(post, gymPost);
        }
    }
}
