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
    }
}
