using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using VlbBet.Core;

namespace VlbBet.Infrastructure
{
    public sealed class TicketApiClient : ApiClientBase
    {
        public TicketApiClient(ApiSession session) : base(session) { }

        // ========= ENDPOINTS (según tu base "ticket/") =========
        private const string EP_ADD = "ticket/";
        private const string EP_GET_GAMES = "ticket/tickets-games"; 
        private const string EP_PAY = "ticket/pay";           

        // ================== PUBLIC API ==================

        public async Task<CreateTicketResponse> AddAsync(
            List<BettedItem> betted,
            decimal amount,
            decimal amountToWin,
            CancellationToken ct = default)
        {
            Session.RefreshAuthorization();

            if (betted == null || betted.Count < 3 || betted.Count > 10)
                return new CreateTicketResponse { Msg = "Jugada No Permitida" };

            if (amountToWin > 100000)
                return new CreateTicketResponse { Msg = "Monto a ganar Superado" };

            var req = new AddTicketRequest
            {
                User = ApiUser.FromAppSession(),
                Amount = amount,
                AmountToWin = amountToWin,
                Betted = betted.Select(b => new ApiBetted
                {
                    Code = b.Code ?? "",
                    Betted = b.Betted ?? "",
                    Rate = b.Rate
                }).ToList()
            };

            var raw = await PostRawAsync(EP_ADD, req, ct).ConfigureAwait(false);
            return ParseCreateTicketResponse(raw);
        }

        /// <summary>
        /// Controller GetGames: requiere req.body.user.point
        /// Devuelve array: [{_id,num,amount,pay}]
        /// </summary>
        public async Task<List<TicketSummary>> GetGamesAsync(CancellationToken ct = default)
        {
            Session.RefreshAuthorization();

            var req = new UserEnvelopeRequest { User = ApiUser.FromAppSession() };
            var raw = await PostRawAsync(EP_GET_GAMES, req, ct).ConfigureAwait(false);

            // si el backend devuelve [] o string
            var list = TryDeserialize<List<TicketSummary>>(raw);
            if (list != null) return list;

            // string plano (error o mensaje)
            var msg = ParseAsString(raw);
            if (!string.IsNullOrWhiteSpace(msg) && msg != "[]")
                throw new Exception(msg);

            return new List<TicketSummary>();
        }

        /// <summary>
        /// PayTicket con action:
        /// - "pay"    -> paga (requiere doble verificación; 1ra llamada devuelve winner, 2da paga)
        /// - "delete" -> anula ticket (en tu Node hace includes('elete'))
        /// - "repeat" -> repite (para D6); lo dejo por si lo reutilizas
        /// </summary>
        public async Task<ApiMsgResponse> PayTicketAsync(string ticketNum, string action = "pay", CancellationToken ct = default)
        {
            Session.RefreshAuthorization();

            var req = new PayTicketRequest
            {
                User = ApiUser.FromAppSession(),
                Ticket = (ticketNum ?? "").Trim(),
                Action = action
            };

            var raw = await PostRawAsync(EP_PAY, req, ct).ConfigureAwait(false);

            // PayTicket a veces devuelve:
            // - string: "Ticket No Ganador", "Ticket Caducado", etc
            // - objeto winner: { state,factor,amountWinner,amountToPay,paid,... }
            // - objeto {ticket, msg:'pagado'} o { msg:'Ticket Anulado', balance: ... }
            // Para WinForms normalmente quieres msg, y si viene winner se lo ponemos en DataRaw.
            var resp = new ApiMsgResponse();

            // 1) si viene winner obj
            var winner = TryDeserialize<TicketWinnerState>(raw);
            if (winner != null && (winner.AmountToPay > 0 || winner.AmountWinner > 0 || winner.State))
            {
                resp.Msg = "verify";     // primera fase: verificación
                resp.Winner = winner;
                return resp;
            }

            // 2) si viene json {msg:'...'}
            var obj = TryDeserialize<ApiMsgResponse>(raw);
            if (obj != null && (!string.IsNullOrWhiteSpace(obj.Msg) || !string.IsNullOrWhiteSpace(obj.Message)))
                return NormalizeMsg(obj);

            // 3) string plano
            resp.Msg = ParseAsString(raw) ?? raw ?? "Respuesta vacía";
            return NormalizeMsg(resp);
        }

        /// <summary>
        /// Anula ticket (acción delete).
        /// </summary>
        public Task<ApiMsgResponse> DeleteTicketAsync(string ticketNum, CancellationToken ct = default)
            => PayTicketAsync(ticketNum, "delete", ct);

        /// <summary>
        /// Obtiene el último ticket del punto: controller LastTicket.
        /// </summary>
        //public async Task<LastTicketResponse> LastTicketAsync(CancellationToken ct = default)
        //{
        //    Session.RefreshAuthorization();

        //    var req = new UserEnvelopeRequest { User = ApiUser.FromAppSession() };
        //    var raw = await PostRawAsync(EP_LAST, req, ct).ConfigureAwait(false);

        //    // Puede devolver ticket completo o string de error
        //    var tk = TryDeserialize<LastTicketResponse>(raw);
        //    if (tk != null && tk.Num > 0) return tk;

        //    var msg = ParseAsString(raw);
        //    if (!string.IsNullOrWhiteSpace(msg))
        //        return new LastTicketResponse { Msg = msg };

        //    return new LastTicketResponse { Msg = raw ?? "Respuesta vacía" };
        //}

        // ================== PARSERS & HELPERS ==================

        private CreateTicketResponse ParseCreateTicketResponse(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return new CreateTicketResponse { Msg = "Respuesta vacía" };

            // "texto"
            var asString = ParseAsString(raw);
            if (!string.IsNullOrWhiteSpace(asString) && (asString.Length < 200 || !raw.TrimStart().StartsWith("{")))
            {
                // cuando Node devuelve 'Juagada Creada' o error string
                return new CreateTicketResponse { Msg = asString };
            }

            // { msg:'ok', ticket:{...} }
            var obj = TryDeserialize<CreateTicketResponse>(raw);
            if (obj != null) return NormalizeCreate(obj);

            return new CreateTicketResponse { Msg = raw };
        }

        private static CreateTicketResponse NormalizeCreate(CreateTicketResponse r)
        {
            if (r == null) return new CreateTicketResponse { Msg = "Respuesta vacía" };
            if (string.IsNullOrWhiteSpace(r.Msg) && !string.IsNullOrWhiteSpace(r.Message)) r.Msg = r.Message;
            if (string.IsNullOrWhiteSpace(r.Msg)) r.Msg = "ok";
            return r;
        }

        private static ApiMsgResponse NormalizeMsg(ApiMsgResponse r)
        {
            if (r == null) return new ApiMsgResponse { Msg = "Respuesta vacía" };
            if (string.IsNullOrWhiteSpace(r.Msg) && !string.IsNullOrWhiteSpace(r.Message)) r.Msg = r.Message;
            if (string.IsNullOrWhiteSpace(r.Msg)) r.Msg = "ok";
            return r;
        }

        private T TryDeserialize<T>(string raw)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(raw)) return default;
                var t = raw.TrimStart();
                if (!t.StartsWith("{") && !t.StartsWith("[")) return default;
                if (LooksLikeHtml(raw)) return default;
                return JsonSerializer.Deserialize<T>(raw, Json);
            }
            catch { return default; }
        }

        private static string ParseAsString(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            var t = raw.Trim();

            // JSON string: "texto"
            if (t.StartsWith("\""))
            {
                try { return JsonSerializer.Deserialize<string>(t); }
                catch { return t.Trim('"'); }
            }

            // texto plano
            if (!t.StartsWith("{") && !t.StartsWith("["))
                return t;

            return null;
        }

        public async Task<Ticket> GetTicketGamesAsync()
        {
            throw new NotImplementedException();
        }
    }

    // ================== REQUESTS ==================

    public sealed class UserEnvelopeRequest
    {
        [JsonPropertyName("user")]
        public ApiUser User { get; set; } = new ApiUser();
    }

    public sealed class AddTicketRequest
    {
        [JsonPropertyName("user")] public ApiUser User { get; set; } = new ApiUser();
        [JsonPropertyName("betted")] public List<ApiBetted> Betted { get; set; } = new List<ApiBetted>();
        [JsonPropertyName("amount")] public decimal Amount { get; set; }
        [JsonPropertyName("amountToWin")] public decimal AmountToWin { get; set; }
    }

    public sealed class PayTicketRequest
    {
        [JsonPropertyName("user")] public ApiUser User { get; set; } = new ApiUser();
        [JsonPropertyName("ticket")] public string Ticket { get; set; } = "";
        [JsonPropertyName("action")] public string Action { get; set; } = "pay";
    }

    // ================== DTOs ==================

    public sealed class ApiBetted
    {
        [JsonPropertyName("code")] public string Code { get; set; } = "";
        [JsonPropertyName("betted")] public string Betted { get; set; } = "";
        [JsonPropertyName("rate")] public decimal Rate { get; set; }
    }

    public sealed class ApiUser
    {
        // Node usa _id
        [JsonPropertyName("_id")] public string Id { get; set; } = "";
        [JsonPropertyName("point")] public string Point { get; set; } = "";
        [JsonPropertyName("sectionId")] public string SectionId { get; set; } = "";

        [JsonPropertyName("level")] public string Level { get; set; } = "";

        public static ApiUser FromAppSession()
        {
            return new ApiUser
            {
                Id = AppSession.UserId,
                Point = AppSession.Point,
                SectionId = AppSession.SessionId,
                Level = AppSession.Level
            };
        }
    }

    public sealed class TicketSummary
    {
        [JsonPropertyName("_id")] public string Id { get; set; }
        [JsonPropertyName("num")] public int Num { get; set; }
        [JsonPropertyName("amount")] public decimal Amount { get; set; }

        // en Node lo manda como string: amountToWin.toFixed(2)
        [JsonPropertyName("pay")] public string Pay { get; set; }

        [JsonIgnore]
        public decimal PayDecimal
        {
            get
            {
                decimal v;
                return decimal.TryParse(Pay, out v) ? v : 0m;
            }
        }
    }

    public sealed class CreateTicketResponse
    {
        [JsonPropertyName("msg")] public string Msg { get; set; } = "";
        [JsonPropertyName("message")] public string Message { get; set; } = "";

        [JsonPropertyName("ticket")] public TicketSendDto Ticket { get; set; }
    }

    public sealed class TicketSendDto
    {
        [JsonPropertyName("amount")] public decimal Amount { get; set; }
        [JsonPropertyName("betted")] public List<ApiBetted> Betted { get; set; }
        [JsonPropertyName("code")] public string Code { get; set; }
        [JsonPropertyName("date")] public DateTime Date { get; set; }
        [JsonPropertyName("pv")] public string Pv { get; set; }
        [JsonPropertyName("num")] public int Num { get; set; }
        [JsonPropertyName("codeGames")] public int CodeGames { get; set; }
        [JsonPropertyName("point")] public string Point { get; set; }
        [JsonPropertyName("customer")] public string Customer { get; set; }
        [JsonPropertyName("amountToWin")] public decimal AmountToWin { get; set; }
    }

    public sealed class ApiMsgResponse
    {
        [JsonPropertyName("msg")] public string Msg { get; set; } = "";
        [JsonPropertyName("message")] public string Message { get; set; } = "";

        // extras para respuestas del PayTicket
        [JsonPropertyName("balance")] public decimal? Balance { get; set; }
        [JsonPropertyName("ticket")] public object Ticket { get; set; } // a veces devuelve {ticket:[], msg:userUpdateNewBalance}
        [JsonIgnore] public TicketWinnerState Winner { get; set; }
    }

    public sealed class TicketWinnerState
    {
        [JsonPropertyName("state")] public bool State { get; set; }
        [JsonPropertyName("factor")] public int Factor { get; set; }
        [JsonPropertyName("amountWinner")] public decimal AmountWinner { get; set; }
        [JsonPropertyName("amountToPay")] public decimal AmountToPay { get; set; }
        [JsonPropertyName("paid")] public bool Paid { get; set; }
        [JsonPropertyName("jackpot")] public object Jackpot { get; set; }
    }

    public sealed class LastTicketResponse
    {
        // cuando es ok, el backend devuelve el ticket full (tiene num)
        [JsonPropertyName("num")] public int Num { get; set; }

        // si viene texto
        [JsonIgnore] public string Msg { get; set; }
    }
}
