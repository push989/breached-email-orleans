using System;
using System.Net.Mail;
using System.Threading.Tasks;
using GrainInterfaces;
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

        [HttpGet("{email}")]
        public async Task<IActionResult> Get(string email)
        {
            var mail = new MailAddress(email);
            var emailDomain = client.GetGrain<IEmailDomain>(mail.Host);

            var isBreached = await emailDomain.CheckEmailBreached(email);

            if (isBreached)
            {
                return Ok();
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            var mail = new MailAddress(value);
            var emailDomain = client.GetGrain<IEmailDomain>(mail.Host);

            try
            {
                await emailDomain.AddBreachedEmail(value);
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex.Message);
            }

            return CreatedAtAction(nameof(this.Get), new { email = value }, value);
        }
    }
}
