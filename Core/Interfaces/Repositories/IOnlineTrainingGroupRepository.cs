using Core.DTOs.GeneralDTO;
using Core.DTOs.OnlineTrainingDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories
{
    public interface IOnlineTrainingGroupRepository
    {
        IntResult Add(AddOnlineTrainingGroupDTO group);
        IntResult Update(UpdateOnlineTrainingGroupDTO group,int id);
        IntResult Delete(int id);
        GetOnlineTrainingGroupWithDetailsDTO Get(int id);
        List<GetOnlineTrainingGroupDTO> ShowAvailableTrainingGroupForCouchPagination(string coachID,int page,int pageSize);
        int NumOfPagesForAvailableTrainingGroupsOfCoach(string coachID, int pageSize);
    }
}
