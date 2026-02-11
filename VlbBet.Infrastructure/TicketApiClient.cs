using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using VlbBet.Core;

namespace VlbBet.Infrastructure
{
    public sealed class TicketApiClient : ApiClientBase
    {
        private const string TicketBase = "ticket/";

        private const string AddEndpoint = TicketBase;              // POST ticket/
        private const string GetGamesEndpoint = TicketBase + "ticket-games"; // POST ticket/games
        private const string PayEndpoint = TicketBase + "pay";        // POST ticket/pay
        private const string LastEndpoint = TicketBase + "last";      // POST ticket/last

        public TicketApiClient(ApiSession session) : base(session) { }

        private void ApplyAuth() => Session.ApplyAuthorizationFromApp();

        private static ApiUser BuildUserFromAppSession()
        {
            // Ajusta los nombres según tu AppSession real.
            // El Node usa: req.body.user.point, req.body.user._id, req.body.user.level, req.body.user.company, req.body.user.credit.balance

            return new ApiUser
            {
                Point = AppSession.Point,
                SectionId = AppSession.SessionId,
                Id = AppSession.UserId,
                Level = AppSession.Level,
            };
        }

        public async Task<TicketsGamesItem[]> GetGamesAsync(CancellationToken ct = default)
        {
            ApplyAuth();

            var req = new { user = BuildUserFromAppSession() };

            // Node retorna array: [{ _id, num, amount, pay }]
            var res = await PostAsync<object, TicketsGamesItem[]>(GetGamesEndpoint, req, ct);
            return res ?? Array.Empty<TicketsGamesItem>();
        }

        public async Task<AddTicketResponse> AddAsync(List<BettedItem> betted, decimal amount, decimal amountToWin, CancellationToken ct = default)
        {
            ApplyAuth();

            if (betted == null || betted.Count < 3 || betted.Count > 10)
                return new AddTicketResponse { Msg = "Jugada No Permitida" };

            if (amountToWin > 100000)
                return new AddTicketResponse { Msg = "Monto a ganar Superado" };

            var req = new AddTicketRequest
            {
                User = BuildUserFromAppSession(),
                Amount = amount,
                AmountToWin = amountToWin,
                Betted = betted.ConvertAll(b => new ApiBetted
                {
                    Code = b.Code ?? "",
                    Betted = b.Betted ?? "",
                    Rate = b.Rate,
                    Option = b.Option
                })
            };

            var raw = await PostRawAsync(AddEndpoint, req, ct);
            return AddTicketResponse.Parse(raw);
        }

        /// <summary>
        /// Pago / Verificación (doble llamada) / Anulación (delete) / Repetir (repeat)
        /// Node decide por req.body.action.includes('pay'/'elete'/'repeat')
        /// </summary>
        public async Task<PayTicketResponse> PayTicketAsync(string ticketNum, string action, CancellationToken ct = default)
        {
            ApplyAuth();

            var req = new
            {
                user = BuildUserFromAppSession(),
                ticket = ticketNum,
                action = action // "pay" o "delete" o "repeat"
            };

            var raw = await PostRawAsync(PayEndpoint, req, ct);
            return PayTicketResponse.Parse(raw);
        }

        public Task<PayTicketResponse> DeleteTicketAsync(string ticketNum, CancellationToken ct = default)
            => PayTicketAsync(ticketNum, "delete", ct); // incluye "elete"

        public Task<PayTicketResponse> VerifyOrPayTicketAsync(string ticketNum, CancellationToken ct = default)
            => PayTicketAsync(ticketNum, "pay", ct);

        public async Task<LastTicketResponse> LastTicketAsync(CancellationToken ct = default)
        {
            ApplyAuth();

            var req = new { user = BuildUserFromAppSession() };
            var raw = await PostRawAsync(LastEndpoint, req, ct);
            return LastTicketResponse.Parse(raw);
        }
    }

    #region DTOs

    public sealed class ApiCredit
    {
        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }
    }

    public sealed class ApiUser
    {
        [JsonPropertyName("point")]
        public string Point { get; set; }

        [JsonPropertyName("sectionId")]
        public string SectionId { get; set; }

        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("company")]
        public string Company { get; set; }

        [JsonPropertyName("level")]
        public string Level { get; set; }

        [JsonPropertyName("credit")]
        public ApiCredit Credit { get; set; }
    }

    public sealed class ApiBetted
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("betted")]
        public string Betted { get; set; }

        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }

        [JsonPropertyName("option")]
        public string Option { get; set; }
    }

    public sealed class AddTicketRequest
    {
        [JsonPropertyName("user")]
        public ApiUser User { get; set; }

        [JsonPropertyName("betted")]
        public List<ApiBetted> Betted { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("amountToWin")]
        public decimal AmountToWin { get; set; }
    }

    public sealed class TicketsGamesItem
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("num")]
        public int Num { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("pay")]
        public string Pay { get; set; } // Node lo manda como string toFixed(2)
    }

    public sealed class TicketSend
    {
        [JsonPropertyName("num")] public int Num { get; set; }
        [JsonPropertyName("code")] public string Code { get; set; }
        [JsonPropertyName("pv")] public string Pv { get; set; }
        [JsonPropertyName("amount")] public decimal Amount { get; set; }
        [JsonPropertyName("amountToWin")] public decimal AmountToWin { get; set; }
        [JsonPropertyName("codeGames")] public int CodeGames { get; set; }
    }

    public sealed class AddTicketResponse
    {
        public string Msg { get; set; }
        public TicketSend Ticket { get; set; }

        public static AddTicketResponse Parse(string raw)
        {
            raw = (raw ?? "").Trim();

            // string JSON ("xxx")
            if (raw.StartsWith("\""))
            {
                var msg = System.Text.Json.JsonSerializer.Deserialize<string>(raw);
                return new AddTicketResponse { Msg = msg ?? raw };
            }

            // plain text
            if (!raw.StartsWith("{") && !raw.StartsWith("["))
                return new AddTicketResponse { Msg = raw };

            // object
            try
            {
                var obj = System.Text.Json.JsonSerializer.Deserialize<AddTicketResponseWire>(raw,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (obj == null) return new AddTicketResponse { Msg = raw };

                return new AddTicketResponse
                {
                    Msg = obj.Msg ?? "ok",
                    Ticket = obj.Ticket
                };
            }
            catch
            {
                return new AddTicketResponse { Msg = raw };
            }
        }

        private sealed class AddTicketResponseWire
        {
            [JsonPropertyName("msg")] public string Msg { get; set; }
            [JsonPropertyName("ticket")] public TicketSend Ticket { get; set; }
        }
    }

    public sealed class WinnerInfo
    {
        [JsonPropertyName("amountToPay")]
        public decimal AmountToPay { get; set; }

        [JsonPropertyName("paid")]
        public bool Paid { get; set; }
    }

    public sealed class PayTicketResponse
    {
        public string Msg { get; set; }           // "verify" / "pagado" / mensajes
        public WinnerInfo Winner { get; set; }    // cuando verifica
        public string Raw { get; set; }

        public static PayTicketResponse Parse(string raw)
        {
            raw = (raw ?? "").Trim();

            // string JSON
            if (raw.StartsWith("\""))
            {
                var msg = System.Text.Json.JsonSerializer.Deserialize<string>(raw);
                return new PayTicketResponse { Msg = msg ?? raw, Raw = raw };
            }

            // plain text
            if (!raw.StartsWith("{") && !raw.StartsWith("["))
                return new PayTicketResponse { Msg = raw, Raw = raw };

            // si viene winner object directamente (verify)
            try
            {
                var winner = System.Text.Json.JsonSerializer.Deserialize<WinnerInfo>(raw,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (winner != null && winner.AmountToPay > 0)
                    return new PayTicketResponse { Msg = "verify", Winner = winner, Raw = raw };
            }
            catch { }

            // si viene {ticket, msg:'pagado'} etc.
            try
            {
                var obj = System.Text.Json.JsonSerializer.Deserialize<PayTicketWire>(raw,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (obj != null && !string.IsNullOrWhiteSpace(obj.Msg))
                    return new PayTicketResponse { Msg = obj.Msg, Raw = raw };
            }
            catch { }

            return new PayTicketResponse { Msg = raw, Raw = raw };
        }

        private sealed class PayTicketWire
        {
            [JsonPropertyName("msg")] public string Msg { get; set; }
        }
    }

    public sealed class LastTicketResponse
    {
        public string Msg { get; set; }
        public string Raw { get; set; }

        public static LastTicketResponse Parse(string raw)
        {
            raw = (raw ?? "").Trim();

            if (raw.StartsWith("\""))
            {
                var msg = System.Text.Json.JsonSerializer.Deserialize<string>(raw);
                return new LastTicketResponse { Msg = msg ?? raw, Raw = raw };
            }

            return new LastTicketResponse { Msg = raw, Raw = raw };
        }
    }

    #endregion
}
