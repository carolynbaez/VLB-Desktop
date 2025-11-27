using System;
using System.Threading.Tasks;
using VlbBet.Core;

namespace VlbBet.Infrastructure
{
    public sealed class AuthApiClient : ApiClientBase, IDisposable
    {

        public AuthApiClient()
            : this(new ApiSession("https://vlb.virsbet.com"))
        {
            _ownsSession = true;
        }

        public AuthApiClient(ApiSession session) : base(session)
        {
        }

        private readonly bool _ownsSession = false;

        public async Task<AuthResponse> LoginAsync(AuthRequest req)
        {
            if (req == null) throw new ArgumentNullException(nameof(req));

            var result = await PostAsync<AuthRequest, AuthResponse>("/login", req).ConfigureAwait(false);

        
            return result;
        }

        public void Dispose()
        {
            if (_ownsSession)
            {
                Session?.Dispose();
            }
        }
    }
}
