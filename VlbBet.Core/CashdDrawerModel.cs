using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace VlbBet.Core
{
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
    public class PrintTicketResult
    {
        public string Ticket { get; set; }
        public string RawResponse { get; set; }
        public string Error { get; set; }
    }
}
