using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;

namespace Core.Interfaces.Repositories.PostRepositories
{
    public interface IGeneralPostRepository
    {
        Task<IntResult> Add(AddPostDTO post);
    }
}