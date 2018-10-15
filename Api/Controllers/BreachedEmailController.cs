using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using GrainInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Api.Controllers
{
    [Route("")]
    [ApiController]
    public class BreachedEmailController : ControllerBase
    {
        private readonly IClusterClient client;

        public BreachedEmailController(IClusterClient client)
        {
            this.client = client;
        }

        // GET: api/BreachedEmail
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var breached = client.GetGrain<IEmailDomain>("test.si");
            var response = await breached.GetBreachedEmails();

            return Ok(response);
        }

        // GET: api/BreachedEmail/5
        [HttpGet("{email}")]
        public async Task<IActionResult> Get(string email)
        {
            var mail = new MailAddress(email);
            var breached = client.GetGrain<IEmailDomain>(mail.Host);

            var isBreached = await breached.CheckEmailBreached(email);

            if (isBreached)
            {
                return Ok();
            }

            return NotFound();
        }

        // POST: api/BreachedEmail
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            var mail = new MailAddress(value);
            var breached = client.GetGrain<IEmailDomain>(mail.Host);

            try
            {
                await breached.AddBreachedEmail(value);
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex.Message);
            }

            return CreatedAtAction(nameof(this.Get), new { email = value }, value);
        }
    }
}
