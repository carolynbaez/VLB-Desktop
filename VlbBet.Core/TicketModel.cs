// ===== DTOs para hablar con Node (Add) =====

using System.Collections.Generic;
using System.Text.Json.Serialization;
using System;

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

public sealed class ApiUser
{
    [JsonPropertyName("_id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("point")]
    public string Point { get; set; } = "";

    // no lo usa tu Add, pero lo dejaste mencionado
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

// Respuesta: puede ser {msg:'ok', ticket:{...}} o un string (ya manejado arriba)
public sealed class CreateTicketResponse
{
    [JsonPropertyName("msg")]
    public string Msg { get; set; } = "";

    [JsonPropertyName("ticket")]
    public TicketSendDto? Ticket { get; set; }
}

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