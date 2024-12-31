using Core.DTOs.GeneralDTO;
using Core.DTOs.OnlineTrainingDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories.OnlineTrainingRepositories
{
    public interface IOnlineTrainingPrivateRepository
    {
        IntResult Add(AddOnlineTrainingPrivateDTO privt);
        IntResult Update(UpdateOnlineTrainingPrivateDTO privt, int id);
        IntResult Delete(int id);
        GetOnlineTrainingPrivateWithDetailsDTO Get(int id);
        List<GetOnlineTrainingPrivateDTO> ShowTrainingPrivateForCouchPagination(string coachID, int page, int pageSize);
        int NumOfPagesForTrainingPrivateOfCoach(string coachID, int pageSize);
    }
}
