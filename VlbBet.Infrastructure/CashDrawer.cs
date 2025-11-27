using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using VlbBet.Core;

namespace VlbBet.Infrastructure
{
    public sealed class CashDrawerApiClient : IDisposable
    {
        private const string BalanceEndpoint = "https://vlb.virsbet.com/run/venta";

        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json;

        public CashDrawerApiClient(Uri baseAddress)
        {
            if (baseAddress == null) throw new ArgumentNullException(nameof(baseAddress));

            if (!baseAddress.AbsoluteUri.EndsWith("/"))
                baseAddress = new Uri(baseAddress.AbsoluteUri + "/");

            _http = new HttpClient { BaseAddress = baseAddress };

            _json = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };
        }

        public async Task<CashDrawerBalanceDto> GetBalanceAsync(
    DateTime init,
    DateTime end,
    string userId,
    string userPoint,
    CancellationToken ct = default)
        {
            var req = new CashDrawerBalanceRequest
            {
                init = init,
                end = end,
                User = new UserReq { _Id = userId, Point = userPoint }
            };

            var body = JsonSerializer.Serialize(req, _json);
            using var content = new StringContent(body, Encoding.UTF8, "application/json");

            // Header Authorization
            _http.DefaultRequestHeaders.Remove("Authorization");
            _http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", AppSession.SessionId);

            using var response = await _http.PostAsync(BalanceEndpoint, content).ConfigureAwait(false);

            var raw = (await response.Content.ReadAsStringAsync().ConfigureAwait(false))?.Trim() ?? "";

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"HTTP {(int)response.StatusCode}: {raw}");

            if (raw.StartsWith("\"") && raw.EndsWith("\""))
            {
                var msg = JsonSerializer.Deserialize<string>(raw, _json) ?? raw;
                throw new InvalidOperationException(msg);
            }

            try
            {
                var dto = JsonSerializer.Deserialize<CashDrawerBalanceDto>(raw, _json);
                if (dto == null) throw new InvalidOperationException("Respuesta inválida (DTO nulo).");
                return dto;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"No se pudo convertir el JSON a CashDrawerBalanceDto.\n\nDetalle: {ex.Message}\n\nJSON RAW:\n{raw}", ex);
            }
        }

        public void Dispose() => _http?.Dispose();
    }

  
}
