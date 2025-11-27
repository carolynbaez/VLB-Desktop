using System;
using System.Threading.Tasks;
using VlbBet.Core;

namespace VlbBet.Infrastructure
{
    // Mantiene el "uso" original (LoginAsync(req) y listo)
    public sealed class AuthApiClient : ApiClientBase, IDisposable
    {
        // Igual que antes: puedes construirlo "como siempre"
        // (si quieres, también puedes dejar solo el constructor con ApiSession)
        public AuthApiClient()
            : this(new ApiSession("https://vlb.virsbet.com"))
        {
            _ownsSession = true;
        }

        public AuthApiClient(ApiSession session) : base(session)
        {
        }

        private readonly bool _ownsSession = false;

        // MISMA FIRMA que tu clase original
        public async Task<AuthResponse> LoginAsync(AuthRequest req)
        {
            if (req == null) throw new ArgumentNullException(nameof(req));

            // MISMO comportamiento: POST JSON a /login y deserializa AuthResponse
            var result = await PostAsync<AuthRequest, AuthResponse>("/login", req).ConfigureAwait(false);

            // tu versión original devolvía "result" (aunque creaba "response").
            // lo dejamos igual: devolvemos lo que viene del backend.
            return result;
        }

        // Mantiene Dispose como antes
        public void Dispose()
        {
            // Solo cerramos la sesión si este AuthApiClient la creó internamente
            if (_ownsSession)
            {
                Session?.Dispose();
            }
        }
    }
}
