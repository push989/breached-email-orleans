using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GrainInterfaces;
using Orleans;
using Orleans.Providers;

namespace Grains
{
    public class EmailDomainState
    {
        public List<string> BreachedEmails { get; set; }
    }

    [StorageProvider(ProviderName = "blobStore")]
    public class EmailDomainGrain : Grain<EmailDomainState>, IEmailDomain
    {
        public override Task OnActivateAsync()
        {
            if (State.BreachedEmails == null)
            {
                State.BreachedEmails = new List<string>();
            }

            RegisterTimer(async (state) => await base.WriteStateAsync(), null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));

            return base.OnActivateAsync();
        }

        public Task<bool> CheckEmailBreached(string email)
        {
            return Task.FromResult(State.BreachedEmails.Contains(email));
        }

        public Task<List<string>> GetBreachedEmails()
        {
            return Task.FromResult(State.BreachedEmails);
        }

        public Task AddBreachedEmail(string email)
        {
            if (State.BreachedEmails.Contains(email))
            {
                return Task.FromException(new ArgumentException("Email is already marked as breached."));
            }

            State.BreachedEmails.Add(email);

            return Task.CompletedTask;
        }
    }
}
