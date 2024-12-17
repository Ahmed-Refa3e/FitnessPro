using Core.DTOs.GeneralDTO;
using Core.DTOs.OnlineTrainingSubscriptionDTO;
using Core.DTOs.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories
{
    public interface IOnlineTrainingSubscriptionRepository
    {
        ShowSubscriptionDTO Get(int id);
        IntResult Add(AddSubscriptionDTO subscription);
        List<ShowSubscriptionForTraineDTO> ShowForTraineActivePagination(string traineId,int page,int pageSize);
        List<ShowSubscriptionForTraineDTO> ShowForTraineAllPagination(string traineId, int page, int pageSize);
        List<ShowSubscriptionForTraineDTO> ShowForTraineCompletedPagination(string traineId, int page, int pageSize);
        List<ShowSubscriptionOfOnlineTrainingDTO> ShowForOnlineTrainingCompletedPagination(int onlineTrainingId, int page, int pageSize);
        List<ShowSubscriptionOfOnlineTrainingDTO> ShowForOnlineTrainingActivePagination(int onlineTrainingId, int page, int pageSize);
        List<ShowSubscriptionOfOnlineTrainingDTO> ShowForOnlineTrainingAllPagination(int onlineTrainingId, int page, int pageSize);
    }
}
