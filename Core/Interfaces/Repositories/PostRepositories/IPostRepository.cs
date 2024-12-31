using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories.PostRepositories
{
    public interface IPostRepository
    {
        public IntResult Add(AddPostDTO post);
        public IntResult Delete(int id);
        //public ShowPostDTO Get(int id);
    }
}