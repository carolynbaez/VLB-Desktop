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

        /// <summary>
        /// Obtiene el balance de caja entre fechas y usuario.
        /// POST: { init:"yyyy-MM-dd", end:"yyyy-MM-dd", user:{ _id:"...", point:"..." } }
        /// </summary>

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

            // Si el backend responde un string JSON: "Favor Seleccionar Intervalo de fecha"
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


        private async Task<CashDrawerBalanceDto> ReadAsDto(HttpResponseMessage response, CancellationToken ct)
        {
            var raw = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"HTTP {(int)response.StatusCode}: {raw}");

            if (string.IsNullOrWhiteSpace(raw))
                throw new InvalidOperationException("Respuesta vacía del servidor.");

            try
            {
                using var doc = JsonDocument.Parse(raw);
                var kind = doc.RootElement.ValueKind;

                // Si el backend devuelve array u otra cosa, lo reportamos con claridad
                if (kind != JsonValueKind.Object)
                    throw new InvalidOperationException($"La respuesta no es un objeto JSON (root={kind}).\n\nJSON RAW:\n{raw}");

                // Si viene envuelto { data: {...} }
                if (doc.RootElement.TryGetProperty("data", out var dataEl) && dataEl.ValueKind == JsonValueKind.Object)
                {
                    var dtoFromData = dataEl.Deserialize<CashDrawerBalanceDto>(_json);
                    if (dtoFromData == null) throw new InvalidOperationException("DTO nulo en data.");
                    return dtoFromData;
                }

                // Directo { betted, paid, deposito, ... }
                var dto = doc.RootElement.Deserialize<CashDrawerBalanceDto>(_json);
                if (dto == null) throw new InvalidOperationException("DTO nulo.");
                return dto;
            }
            catch (Exception ex)
            {
                // ✅ Esto te dirá EXACTAMENTE lo que vino del API
                throw new InvalidOperationException(
                    $"No se pudo convertir el JSON a CashDrawerBalanceDto.\n\nDetalle: {ex.Message}\n\nJSON RAW:\n{raw}", ex);
            }
        }

        public void Dispose() => _http?.Dispose();

        private sealed class ApiEnvelope<T>
        {
            [JsonPropertyName("ok")]
            public bool Ok { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }

            [JsonPropertyName("data")]
            public T Data { get; set; }
        }
    }

    public sealed class CashDrawerBalanceRequest
    {
        [JsonPropertyName("init")]
        public DateTime init { get; set; }

        [JsonPropertyName("end")]
        public DateTime end { get; set; }

        [JsonPropertyName("user")]
        public UserReq User { get; set; }
    }

    public sealed class UserReq
    {
        [JsonPropertyName("_id")]
        public string _Id { get; set; }

        [JsonPropertyName("point")]
        public string Point { get; set; }
    }

    public sealed class CashDrawerBalanceDto
    {
        [JsonPropertyName("betted")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? Betted { get; set; }

        [JsonPropertyName("winned")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? Winned { get; set; }

        [JsonPropertyName("paid")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? Paid { get; set; }

        [JsonPropertyName("balance")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? Balance { get; set; }

        [JsonPropertyName("deposito")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? Deposito { get; set; }

        [JsonPropertyName("retiro")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? Retiro { get; set; }

        [JsonPropertyName("balancePlayer")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? BalancePlayer { get; set; }

        [JsonPropertyName("total")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? Total { get; set; }

        [JsonPropertyName("point")]
        public string Point { get; set; }

        [JsonPropertyName("tickets")]
        public JsonElement Tickets { get; set; }
    }
}
