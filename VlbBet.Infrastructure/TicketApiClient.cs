using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VlbBet.Core;

namespace VlbBet.Infrastructure
{
    public sealed class TicketApiClient : IDisposable
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json;

        public string EndpointCreate { get; set; } = "https://vlb.virsbet.com/ticket/";
        public string EndpointListByGames { get; set; } = "https://vlb.virsbet.com/ticket/games";
        public string EndpointPay { get; set; } = "https://vlb.virsbet.com/ticket/pay";
        public string EndpointCancel { get; set; } = "/ticket/cancel";
        // ============================================

        // Si luego usas auth por header:
        public string? BearerToken { get; set; }

        public TicketApiClient(Uri baseAddress)
        {
            _http = new HttpClient { BaseAddress = baseAddress };

            _json = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        // ===================== HELPERS =====================

        private HttpRequestMessage CreateReq(HttpMethod method, string url, object? body = null)
        {
            var req = new HttpRequestMessage(method, url);

            if (!string.IsNullOrWhiteSpace(BearerToken))
                req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", BearerToken);

            if (body != null)
            {
                var jsonBody = JsonSerializer.Serialize(body, _json);
                req.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            return req;
        }

        private async Task<ApiResult<T>> SendAsync<T>(HttpRequestMessage req)
        {
            using var resp = await _http.SendAsync(req);
            var raw = await resp.Content.ReadAsStringAsync();

            // Normaliza mensajes tipo string u objeto
            var parsed = ParseNodeResponse<T>(raw);

            // Si HTTP NO-OK, devolvemos el raw como msg para debug
            if (!resp.IsSuccessStatusCode)
            {
                parsed.Ok = false;
                if (string.IsNullOrWhiteSpace(parsed.Msg))
                    parsed.Msg = $"HTTP {(int)resp.StatusCode}: {raw}";
            }

            return parsed;
        }

        private ApiResult<T> ParseNodeResponse<T>(string? raw)
        {
            raw ??= "";

            // 1) string JSON:  "Juagada Creada"
            if (raw.TrimStart().StartsWith("\""))
            {
                var msg = SafeJsonDeserialize<string>(raw) ?? raw;
                return new ApiResult<T> { Ok = true, Msg = msg };
            }

            // 2) texto plano: Juagada Creada
            if (raw.Length > 0 && raw[0] != '{' && raw[0] != '[')
                return new ApiResult<T> { Ok = true, Msg = raw.Trim() };

            // 3) objeto JSON
            // Caso típico tuyo: { msg:'ok', ticket:{...} }
            var envelope = SafeJsonDeserialize<ApiEnvelope<T>>(raw);
            if (envelope != null)
            {
                return new ApiResult<T>
                {
                    Ok = true,
                    Msg = envelope.Msg ?? "",
                    Data = envelope.Ticket ?? envelope.Data
                };
            }

            // 4) si no es envelope, quizá el JSON es el objeto directo
            var direct = SafeJsonDeserialize<T>(raw);
            if (direct != null)
                return new ApiResult<T> { Ok = true, Data = direct };

            return new ApiResult<T> { Ok = true, Msg = raw };
        }

        private T SafeJsonDeserialize<T>(string raw)
        {
            try { return JsonSerializer.Deserialize<T>(raw, _json); }
            catch { return default; }
        }

        // ===================== API: CREAR =====================

        public async Task<ApiResult<TicketSendDto>> CreateTicketAsync(
            List<BettedItem> betted,
            decimal amount,
            decimal amountToWin)
        {
            if (betted == null || betted.Count < 3 || betted.Count > 10)
                return ApiResult<TicketSendDto>.Fail("Jugada No Permitida");

            if (amountToWin > 100000)
                return ApiResult<TicketSendDto>.Fail("Monto a ganar Superado");

            // ✅ Ajusta esto a TU AppSession real.
            // Yo usaré el patrón que mencionaste: AppSession tiene point, sessionId, user.

            if (string.IsNullOrWhiteSpace(AppSession.Point))
                return ApiResult<TicketSendDto>.Fail("No tiene Punto Asignado");

            if (AppSession.User == null || string.IsNullOrWhiteSpace(AppSession.User))
                return ApiResult<TicketSendDto>.Fail("Usuario no válido en AppSession");

            var reqBody = new CreateTicketRequest
            {
                User = new ApiUser
                {
                    Point = AppSession.Point,
                    SectionId = AppSession.SessionId,
                    Id = AppSession.User,
                    Level = AppSession.Level,
                    //Credit = new ApiCredit { Balance = s.User.Credit?.Balance ?? 0m }
                },
                Amount = amount,
                AmountToWin = amountToWin,
                Betted = betted.Select(b => new ApiBetted
                {
                    Code = b.Code ?? "",
                    Betted = b.Betted ?? "",
                    Rate = b.Rate
                }).ToList()
            };

            using var req = CreateReq(HttpMethod.Post, EndpointCreate, reqBody);
            return await SendAsync<TicketSendDto>(req);
        }

        // ===================== API: LISTAR TICKETS DEL JUEGO =====================

        // Si tu endpoint devuelve lista, esta firma te sirve.
        public async Task<ApiResult<List<TicketSendDto>>> GetTicketGamesAsync()
        {
            using var req = CreateReq(HttpMethod.Get, EndpointListByGames);
            return await SendAsync<List<TicketSendDto>>(req);
        }

        // ===================== API: PAGAR =====================

        // Tu Node actual usa PayTicketRawAsync(tk,"pay") en tu BetForm.
        // Aquí lo formalizamos: mandamos ticketNumber y action "pay".
        public async Task<ApiResult<string>> PayTicketAsync(string ticketNumber)
        {
            if (string.IsNullOrWhiteSpace(ticketNumber))
                return ApiResult<string>.Fail("Digite el número de ticket.");


            var body = new PayTicketRequest
            {
                User = new ApiUser
                {
                    Point = AppSession.Point,
                    SectionId = AppSession.SessionId,
                    Id = AppSession.User ?? "",
                    Level = AppSession.Level ?? "",
                    //Credit = new ApiCredit { Balance = s.User?.Credit?.Balance ?? 0m }
                },
                TicketNumber = ticketNumber.Trim(),
                Action = "pay"
            };

            using var req = CreateReq(HttpMethod.Post, EndpointPay, body);
            return await SendAsync<string>(req);
        }

        // Si tú necesitas el raw como antes:
        public async Task<string> PayTicketRawAsync(string ticketNumber, string action = "pay")
        {
            var r = await PayTicketAsync(ticketNumber);
            return string.IsNullOrWhiteSpace(r.Msg) ? "ok" : r.Msg;
        }

        // ===================== API: CANCELAR / ELIMINAR =====================

        // Como no me diste la ruta real ni contrato, lo mando como body estándar.
        public async Task<ApiResult<string>> CancelTicketAsync(string ticketNumber, string reason = "cancel")
        {
            if (string.IsNullOrWhiteSpace(ticketNumber))
                return ApiResult<string>.Fail("Digite el número de ticket.");

            var body = new CancelTicketRequest
            {
                User = new ApiUser
                {
                    Point = AppSession.Point,
                    SectionId = AppSession.SessionId,
                    Id = AppSession.User?? "",
                    Level = AppSession.Level ?? "",
                    //Credit = new ApiCredit { Balance = s.User?.Credit?.Balance ?? 0m }
                },
                TicketNumber = ticketNumber.Trim(),
                Reason = reason
            };

            using var req = CreateReq(HttpMethod.Post, EndpointCancel, body);
            return await SendAsync<string>(req);
        }

        public void Dispose() => _http.Dispose();
    }

    // =============== RESULTADOS / ENVELOPE ===============

    public sealed class ApiResult<T>
    {
        public bool Ok { get; set; }
        public string Msg { get; set; } = "";
        public T Data { get; set; }

        public static ApiResult<T> Fail(string msg) => new ApiResult<T> { Ok = false, Msg = msg };
    }

    // Envelope común: { msg, ticket } o { msg, data }
    public sealed class ApiEnvelope<T>
    {
        [JsonPropertyName("msg")]
        public string? Msg { get; set; }

        [JsonPropertyName("ticket")]
        public T Ticket { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }
    }

    // =============== DTOs REQUESTS ===============

    public sealed class CreateTicketRequest
    {
        [JsonPropertyName("user")]
        public ApiUser User { get; set; } = new ApiUser();

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("amountToWin")]
        public decimal AmountToWin { get; set; }

        [JsonPropertyName("betted")]
        public List<ApiBetted> Betted { get; set; } = new List<ApiBetted>();
    }

    public sealed class PayTicketRequest
    {
        [JsonPropertyName("user")]
        public ApiUser User { get; set; } = new ApiUser();

        [JsonPropertyName("ticketNumber")]
        public string TicketNumber { get; set; } = "";

        [JsonPropertyName("action")]
        public string Action { get; set; } = "pay";
    }

    public sealed class CancelTicketRequest
    {
        [JsonPropertyName("user")]
        public ApiUser User { get; set; } = new ApiUser();

        [JsonPropertyName("ticketNumber")]
        public string TicketNumber { get; set; } = "";

        [JsonPropertyName("reason")]
        public string Reason { get; set; } = "cancel";
    }

    public sealed class ApiUser
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("point")]
        public string Point { get; set; } = "";

        [JsonPropertyName("sessionId")]
        public string? SectionId { get; set; }

        [JsonPropertyName("level")]
        public string Level { get; set; } = "";

        [JsonPropertyName("credit")]
        public ApiCredit Credit { get; set; } = new ApiCredit();
    }

    public sealed class ApiCredit
    {
        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }
    }

    public sealed class ApiBetted
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = "";

        [JsonPropertyName("betted")]
        public string Betted { get; set; } = "";

        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }
    }

    // =============== DTOs RESPONSES ===============

    public sealed class TicketSendDto
    {
        [JsonPropertyName("amount")] public decimal Amount { get; set; }
        [JsonPropertyName("betted")] public List<ApiBetted> Betted { get; set; } = new List<ApiBetted> ();
        [JsonPropertyName("code")] public string Code { get; set; } = "";
        [JsonPropertyName("date")] public DateTime Date { get; set; }
        [JsonPropertyName("pv")] public string Pv { get; set; } = "";
        [JsonPropertyName("num")] public int Num { get; set; }
        [JsonPropertyName("codeGames")] public int CodeGames { get; set; }
        [JsonPropertyName("point")] public string Point { get; set; } = "";
        [JsonPropertyName("customer")] public string Customer { get; set; } = "";
        [JsonPropertyName("amountToWin")] public decimal AmountToWin { get; set; }
    }
}
