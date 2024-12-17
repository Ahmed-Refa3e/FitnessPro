using Core.DTOs.OnlineTrainingDTO;
using Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnlineTrainingGroupController : ControllerBase
    {
        private readonly IOnlineTrainingGroupRepository _groupRepository;
        public OnlineTrainingGroupController(IOnlineTrainingGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }
        [HttpGet("{id:int}")]
        public ActionResult GetById(int id)
        {
            var result=_groupRepository.Get(id);
            if(result == null)
            {
                return BadRequest("No Online Training Group has this Id");
            }
            return Ok(result);
        }
        [HttpPost]
        public ActionResult Add(AddOnlineTrainingGroupDTO group)
        {
            if(ModelState.IsValid)
            {
                var result = _groupRepository.Add(group);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                string url = Url.Action(nameof(GetById), new { id = result.Id });
                return Created(url, _groupRepository.Get(result.Id));
            }
            return BadRequest(ModelState);
        }
        [HttpPatch("{id:int}")]
        public ActionResult Update(int id,UpdateOnlineTrainingGroupDTO group)
        {
            if(ModelState.IsValid)
            {
                var result = _groupRepository.Update(group, id);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                return StatusCode(StatusCodes.Status204NoContent);
            }
            return BadRequest(ModelState);
        }
        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var result= _groupRepository.Delete(id);
            if (result.Id == 0)
            {
                return BadRequest(result.Massage);
            }
            return StatusCode(StatusCodes.Status204NoContent);
        }
        [HttpGet("All/{couchId}/{page:int}/{pageSize:int}")]
        public ActionResult PaginationForCouch(string couchId,int page,int pageSize)
        {
            var result = _groupRepository.ShowAvailableTrainingGroupForCouchPagination(couchId, page, pageSize);
            return Ok(result);
        }
        [HttpGet ("NumPage/{couchId}/{pageSize:int}")]
        public ActionResult num(string couchId, int pageSize)
        {
            var result = _groupRepository.NumOfPagesForAvailableTrainingGroupsOfCoach(couchId, pageSize);
            return Ok(result);
        }
    }
}
