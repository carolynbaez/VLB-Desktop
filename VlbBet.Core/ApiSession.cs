using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using VlbBet.Core;

namespace VlbBet.Infrastructure
{
    public sealed class ApiSession : IDisposable
    {
        public HttpClient Http { get; }
        public JsonSerializerOptions Json { get; }

        public ApiSession(Uri baseAddress)
        {
            if (baseAddress == null) throw new ArgumentNullException(nameof(baseAddress));

            Http = new HttpClient { BaseAddress = baseAddress };

            Json = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        /// <summary>
        /// Aplica header Authorization (SIN Bearer) desde AppSession.SessionId.
        /// Debe llamarse antes de cada request si el token puede cambiar.
        /// </summary>
        public void ApplyAuthorizationFromApp()
        {
            // Ajusta aquí si tu SessionId está en otro lado:
            var token = AppSession.SessionId;

            Http.DefaultRequestHeaders.Remove("authorization");

            if (!string.IsNullOrWhiteSpace(token))
                Http.DefaultRequestHeaders.TryAddWithoutValidation("authorization", token.Trim());
        }

        public void Dispose() => Http.Dispose();
    }
}
