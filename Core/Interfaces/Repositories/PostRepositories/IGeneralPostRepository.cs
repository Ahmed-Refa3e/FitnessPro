using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories.PostRepositories
{
    public interface IGeneralPostRepository
    {
        Task<IntResult> Add(AddPostDTO post);
    }
}