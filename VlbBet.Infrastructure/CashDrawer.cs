using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VlbBet.Core;

namespace VlbBet.Infrastructure
{
    /// <summary>
    /// API Client para Cuadre de Caja / Venta.
    /// BaseAddress recomendado: https://vlb.virsbet.com
    ///
    /// Nota:
    /// - /venta es UI (React). Los endpoints JSON reales suelen ser /venta/... o /api/venta/... según tu Node.
    /// - Estos paths quedan configurables para que no tengas que tocar código al cambiar rutas.
    /// </summary>
    public sealed class CashDrawerApiClient : ApiClientBase
    {
        // ========= ENDPOINTS (AJUSTABLES) =========

        /// <summary>
        /// Endpoint que devuelve el balance/cuadre por rango de fechas.
        /// Ejemplos comunes:
        ///  - "/venta/balance"
        ///  - "/api/venta/balance"
        /// Puede ser GET con query o POST con body; aquí lo hacemos POST.
        /// </summary>
        public string BalancePath { get; set; } = "/venta";

        /// <summary>
        /// Endpoint para listar tickets de un balance por id.
        /// Ejemplos:
        ///  - "/venta/balance/{id}/tickets"
        ///  - "/api/venta/balance/{id}/tickets"
        /// </summary>
        public string BalanceTicketsPathTemplate { get; set; } = "/venta/balance/{0}/tickets";

        /// <summary>
        /// Endpoint para "cerrar" / generar el cuadre por fechas.
        /// Si tu backend usa el mismo BalancePath para generar el cuadre, apunta esto al mismo.
        /// </summary>
        public string CloseCashDrawerPath { get; set; } = "/venta/cuadre";

        /// <summary>
        /// Endpoint para imprimir reporte del cuadre.
        /// Ejemplos:
        ///  - "/venta/cuadre/print"
        ///  - "/venta/balance/print"
        /// </summary>
        public string PrintReportPath { get; set; } = "/venta/cuadre/print";

        /// <summary>
        /// Endpoint de reimpresión / pago tickets existente en tu sistema.
        /// (Ya lo vienes usando con PayTicketRawAsync en TicketApiClient)
        /// </summary>
        public string TicketPayPath { get; set; } = "/ticket/pay";

        public CashDrawerApiClient(ApiSession session) : base(session) { }

        // ========= METODOS =========

        /// <summary>
        /// Consulta el balance/cuadre en rango [from,to].
        /// </summary>
        public Task<CashBalanceDto> GetBalanceAsync(
            DateTime from,
            DateTime to,
            string user = null,
            string point = null,
            CancellationToken ct = default)
        {
            var req = new BalanceRangeRequest
            {
                from = from,
                to = to,
                user = AppSession.User,
                point = point
            };

            return PostAsync<BalanceRangeRequest, CashBalanceDto>(BalancePath, req, ct);
        }

        /// <summary>
        /// Crea / cierra el cuadre con rango [from,to].
        /// Si tu backend no diferencia "consultar" vs "cerrar", usa el mismo endpoint en CloseCashDrawerPath.
        /// </summary>
        public Task<CashBalanceDto> CloseCashDrawerAsync(
            DateTime from,
            DateTime to,
            string user = null,
            string point = null,
            CancellationToken ct = default)
        {
            var req = new BalanceRangeRequest
            {
                from = from,
                to = to,
                user = user,
                point = point
            };

            return PostAsync<BalanceRangeRequest, CashBalanceDto>(CloseCashDrawerPath, req, ct);
        }

        /// <summary>
        /// Lista tickets que pertenecen a un balance por balanceId.
        /// </summary>
        public async Task<List<CashTicketDto>> GetTicketsForBalanceAsync(string balanceId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(balanceId))
                throw new ArgumentException("balanceId es requerido.", nameof(balanceId));

            var url = string.Format(BalanceTicketsPathTemplate, Uri.EscapeDataString(balanceId.Trim()));

            // Si tu ApiClientBase NO tiene GetAsync<T>, entonces cambia esto a PostAsync<SomeReq, List<CashTicketDto>>
            // y manda el id en el body. Aquí asumo que tienes GetAsync.
            var list = await GetAsync<List<CashTicketDto>>(url, ct).ConfigureAwait(false);
            return list ?? new List<CashTicketDto>();
        }

        /// <summary>
        /// Reimprime un lote de tickets (uno a uno) usando TicketPayPath con action="repeat".
        /// </summary>
        public async Task<PrintBatchResult> PrintTicketsAsync(IEnumerable<string> tickets, CancellationToken ct = default)
        {
            if (tickets == null) throw new ArgumentNullException(nameof(tickets));

            var result = new PrintBatchResult();

            var uniq = tickets
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var tk in uniq)
            {
                try
                {
                    var payload = new { ticket = tk, action = "repeat" };

                    // Ideal: PostRawAsync (para cuando el backend devuelve texto o json libre).
                    // Si tu ApiClientBase no tiene PostRawAsync, usa el fallback comentado abajo.
                    var raw = await PostRawAsync(TicketPayPath, payload, ct).ConfigureAwait(false);

                    result.Success.Add(new PrintTicketResult
                    {
                        Ticket = tk,
                        RawResponse = raw
                    });
                }
                catch (Exception ex)
                {
                    result.Failed.Add(new PrintTicketResult
                    {
                        Ticket = tk,
                        Error = ex.Message
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Imprime todos los tickets que pertenecen a un balance (consulta lista y luego reimprime).
        /// </summary>
        public async Task<PrintBatchResult> PrintTicketsForBalanceAsync(string balanceId, CancellationToken ct = default)
        {
            var list = await GetTicketsForBalanceAsync(balanceId, ct).ConfigureAwait(false);
            var nums = list.Select(x => x?.Num).Where(x => !string.IsNullOrWhiteSpace(x));
            return await PrintTicketsAsync(nums, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Dispara la impresión del REPORTE de cuadre por rango de fechas.
        /// Devuelve "raw" para debug (ok/error).
        /// </summary>
        public Task<string> PrintReportAsync(DateTime from, DateTime to, string user = null, string point = null, CancellationToken ct = default)
        {
            var req = new BalanceRangeRequest
            {
                from = from,
                to = to,
                user = user,
                point = point
            };
            return PostRawAsync(PrintReportPath, req, ct);
        }

        /// <summary>
        /// Dispara impresión del REPORTE por balanceId.
        /// </summary>
        public Task<string> PrintReportByBalanceIdAsync(string balanceId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(balanceId))
                throw new ArgumentException("balanceId es requerido.", nameof(balanceId));

            var req = new { id = balanceId.Trim() };
            return PostRawAsync(PrintReportPath, req, ct);
        }

        // ========= REQUEST MODELS (internos) =========
        // (No inventan DTOs de respuesta; solo body de request)

        private sealed class BalanceRangeRequest
        {
            public DateTime from { get; set; }
            public DateTime to { get; set; }
            public string user { get; set; }
            public string point { get; set; }
        }
    }
}
