using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<ActionResult<IEnumerable<Message>>> GetAll(Guid organizationId)
        {
            var result = await _service.GetAllAsync(organizationId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> Get(Guid organizationId, Guid id)
        {
            var res = await _service.GetByIdAsync(organizationId, id);
            if (res == null)
                return NotFound();

            return Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult> Create(
            Guid organizationId,
            [FromBody] Message msg)
        {
            msg.OrganizationId = organizationId;

            var res = await _service.CreateAsync(msg);

            return res switch
            {
                Results.CreatedResult<Message> c =>
                    CreatedAtAction(
                        nameof(Get),
                        new { organizationId, id = c.Value.Id },
                        c.Value),

                Results.ValidationErrorResult ve =>
                    BadRequest(ve.Errors),

                Results.ConflictResult cr =>
                    Conflict(cr.Text),

                _ => StatusCode(500)
            };
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(
            Guid organizationId,
            Guid id,
            [FromBody] Message msg)
        {
            msg.Id = id;
            msg.OrganizationId = organizationId;

            var res = await _service.UpdateAsync(msg);

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
        public async Task<ActionResult> Delete(Guid organizationId, Guid id)
        {
            var res = await _service.DeleteAsync(organizationId, id);

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
