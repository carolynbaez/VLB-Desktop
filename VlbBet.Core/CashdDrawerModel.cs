using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace VlbBet.Core
{
    public class CashBalanceDto
    {
        // Id del balance/cuadre (si existe en tu API)
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        // Punto / banca
        public string Point { get; set; }

        // Usuario/cuadrador
        public string User { get; set; }

        // Totales típicos (pueden variar según tu backend)
        public decimal TotalSales { get; set; }      // total vendido
        public decimal TotalPaid { get; set; }       // total pagado
        public decimal Net { get; set; }             // neto

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        // Captura cualquier cosa extra sin romper
        [JsonExtensionData]
        public Dictionary<string, JsonElement> Extra { get; set; }
    }

    public class CashTicketDto
    {
        // En tu tabla de React se usa item.num
        public string Num { get; set; }

        // A veces viene _id en el listado
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        public decimal Amount { get; set; }
        public decimal Pay { get; set; }

        public DateTime? Date { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement> Extra { get; set; }
    }

    public class PrintBatchResult
    {
        public List<PrintTicketResult> Success { get; } = new List<PrintTicketResult>();
        public List<PrintTicketResult> Failed { get; } = new List<PrintTicketResult>();
    }

    public class PrintTicketResult
    {
        public string Ticket { get; set; }
        public string RawResponse { get; set; }
        public string Error { get; set; }
    }
}
