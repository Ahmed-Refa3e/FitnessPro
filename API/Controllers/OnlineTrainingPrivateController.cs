using Core.DTOs.OnlineTrainingDTO;
using Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnlineTrainingPrivateController : ControllerBase
    {
        private readonly IOnlineTrainingPrivateRepository _privateRepository;
        public OnlineTrainingPrivateController(IOnlineTrainingPrivateRepository privateRepository)
        {
            _privateRepository = privateRepository;
        }
        [HttpGet("{id:int}")]
        public ActionResult GetById(int id)
        {
            var result = _privateRepository.Get(id);
            if (result == null)
            {
                return BadRequest("No Private Online Training has this Id");
            }
            return Ok(result);
        }
        [HttpPost]
        public ActionResult Add(AddOnlineTrainingPrivateDTO privt)
        {
            if (ModelState.IsValid)
            {
                var result = _privateRepository.Add(privt);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                string url = Url.Action(nameof(GetById), new { id = result.Id });
                return Created(url, _privateRepository.Get(result.Id));
            }
            return BadRequest(ModelState);
        }
        [HttpPatch("{id:int}")]
        public ActionResult Update(int id, UpdateOnlineTrainingPrivateDTO privt)
        {
            if (ModelState.IsValid)
            {
                var result = _privateRepository.Update(privt, id);
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
            var result = _privateRepository.Delete(id);
            if (result.Id == 0)
            {
                return BadRequest(result.Massage);
            }
            return StatusCode(StatusCodes.Status204NoContent);
        }
        [HttpGet("{couchId}/{page:int}/{pageSize:int}")]
        public ActionResult PaginationForCouch(string couchId, int page, int pageSize)
        {
            var result = _privateRepository.ShowTrainingPrivateForCouchPagination(couchId, page, pageSize);
            return Ok(result);
        }
    }
}
