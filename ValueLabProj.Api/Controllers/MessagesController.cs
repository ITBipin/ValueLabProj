using CleanArchitecture.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Results = CleanArchitecture.Application.Common.Results;
using Services = CleanArchitecture.Application.Services;

namespace ValueLabProj.Api.Controllers
{
    [ApiController]
    [Route("api/v1/organizations/{organizationId}/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly Services.IMessageService _service;

        public MessagesController(Services.IMessageService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Message>> GetAll(Guid organizationId)
        {
            var result = _service.GetAll(organizationId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<Message> Get(Guid organizationId, Guid id)
        {
            var res = _service.GetById(organizationId, id);
            if (res == null) return NotFound();
            return Ok(res);
        }

        [HttpPost]
        public ActionResult Create(Guid organizationId, [FromBody] Message msg)
        {
            msg.OrganizationId = organizationId;
            var res = _service.Create(msg);
            return res switch
            {
                Results.CreatedResult<Message> c => CreatedAtAction(nameof(Get), new { organizationId = organizationId, id = c.Value.Id }, c.Value),
                Results.ValidationErrorResult ve => BadRequest(ve.Errors),
                Results.ConflictResult cr => Conflict(cr.Text),
                _ => StatusCode(500)
            };
        }

        [HttpPut("{id}")]
        public ActionResult Update(Guid organizationId, Guid id, [FromBody] Message msg)
        {
            msg.Id = id;
            msg.OrganizationId = organizationId;
            var res = _service.Update(msg);
            return res switch
            {
                Results.SuccessResult _ => NoContent(),
                Results.NotFoundResult _ => NotFound(),
                Results.ValidationErrorResult ve => BadRequest(ve.Errors),
                Results.ConflictResult cr => Conflict(cr.Text),
                _ => StatusCode(500)
            };
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid organizationId, Guid id)
        {
            var res = _service.Delete(organizationId, id);
            return res switch
            {
                Results.SuccessResult _ => NoContent(),
                Results.NotFoundResult _ => NotFound(),
                Results.ValidationErrorResult ve => BadRequest(ve.Errors),
                _ => StatusCode(500)
            };
        }
    }
}
