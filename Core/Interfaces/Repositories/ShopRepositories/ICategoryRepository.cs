using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories.ShopRepositories
{
    public interface ICategoryRepository
    {
        List<ShowCategoryDTO> GetAll();
        ShowCategoryDTO GetById(int id);
        IntResult Add(ShowCategoryDTO categoryDTO);
        IntResult Update(ShowCategoryDTO categoryDTO, int id);
        IntResult Delete(int id);
    }
}
