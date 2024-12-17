using Core.DTOs.OnlineTrainingSubscriptionDTO;
using Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnlineTrainingSubscriptionController : ControllerBase
    {
        private readonly IOnlineTrainingSubscriptionRepository _subscriptionRepository;
        public OnlineTrainingSubscriptionController(IOnlineTrainingSubscriptionRepository subscriptionRepository)
        {
            this._subscriptionRepository = subscriptionRepository;
        }
        [HttpGet("{id:int}")]
        public ActionResult GetById(int id)
        {
            var result = _subscriptionRepository.Get(id);
            if (result == null)
            {
                return BadRequest("No Online Training Subscription has this Id");
            }
            return Ok(result);
        }
        [HttpPost]
        public ActionResult Add(AddSubscriptionDTO subscription)
        {
            if (ModelState.IsValid)
            {
                var result = _subscriptionRepository.Add(subscription);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                string url = Url.Action(nameof(GetById), new { id = result.Id });
                return Created(url, _subscriptionRepository.Get(result.Id));
            }
            return BadRequest(ModelState);
        }
        [HttpGet("ActiveForTraine/{traineId}/{page:int}/{pageSize:int}")]
        public ActionResult ShowForTraineActivePagination(string traineId, int page, int pageSize)
        {
            return Ok(_subscriptionRepository.ShowForTraineActivePagination(traineId, page, pageSize));
        }
        [HttpGet("CompletedForTraine/{traineId}/{page:int}/{pageSize:int}")]
        public ActionResult ShowForTraineCompletedPagination(string traineId, int page, int pageSize)
        {
            return Ok(_subscriptionRepository.ShowForTraineCompletedPagination(traineId, page, pageSize));
        }
        [HttpGet("AllForTraine/{traineId}/{page:int}/{pageSize:int}")]
        public ActionResult ShowForTraineAllPagination(string traineId, int page, int pageSize)
        {
            return Ok(_subscriptionRepository.ShowForTraineAllPagination(traineId, page, pageSize));
        }
        [HttpGet("CompletedForOnlineTraining/{onlineTrainingId:int}/{page:int}/{pageSize:int}")]
        public ActionResult ShowForOnlineTrainingCompletedPagination(int onlineTrainingId, int page, int pageSize)
        {
            return Ok(_subscriptionRepository.ShowForOnlineTrainingCompletedPagination(onlineTrainingId,page,pageSize));
        }
        [HttpGet("ActiveForOnlineTraining/{onlineTrainingId:int}/{page:int}/{pageSize:int}")]
        public ActionResult ShowForOnlineTrainingActivePagination(int onlineTrainingId, int page, int pageSize)
        {
            return Ok(_subscriptionRepository.ShowForOnlineTrainingActivePagination(onlineTrainingId, page, pageSize));
        }
        [HttpGet("AllForOnlineTraining/{onlineTrainingId:int}/{page:int}/{pageSize:int}")]
        public ActionResult ShowForOnlineTrainingAllPagination(int onlineTrainingId, int page, int pageSize)
        {
            return Ok(_subscriptionRepository.ShowForOnlineTrainingAllPagination(onlineTrainingId, page, pageSize));
        }
    }
}
