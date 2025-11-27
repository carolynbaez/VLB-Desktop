using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace VlbBet.Infrastructure
{
    public abstract class ApiClientBase
    {
        protected ApiSession Session { get; }

        protected HttpClient Http { get { return Session.Http; } }
        protected JsonSerializerOptions Json { get { return Session.Json; } }

        protected ApiClientBase(ApiSession session)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
        }

        protected Task<TRes> GetAsync<TRes>(string endpoint, CancellationToken ct = default)
        {
            endpoint = NormalizeEndpoint(endpoint);
            return SendGetAsync<TRes>(endpoint, ct);
        }

        protected Task<TRes> PostAsync<TReq, TRes>(string endpoint, TReq req, CancellationToken ct = default)
        {
            endpoint = NormalizeEndpoint(endpoint);
            return SendPostAsync<TReq, TRes>(endpoint, req, ct);
        }

        protected Task<string> PostRawAsync<TReq>(string endpoint, TReq req, CancellationToken ct = default)
        {
            endpoint = NormalizeEndpoint(endpoint);
            return SendPostRawAsync(endpoint, req, ct);
        }

        private async Task<TRes> SendGetAsync<TRes>(string endpoint, CancellationToken ct)
        {
            using (var res = await Http.GetAsync(endpoint, ct).ConfigureAwait(false))
            {
                var body = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                EnsureSuccess(res, body, endpoint);

                if (string.IsNullOrWhiteSpace(body)) return default(TRes);
                if (LooksLikeHtml(body))
                    throw new Exception("Respuesta HTML (SPA). Estás llamando una ruta de FRONT, no un endpoint JSON: " + endpoint);

                return JsonSerializer.Deserialize<TRes>(body, Json);
            }
        }

        private async Task<TRes> SendPostAsync<TReq, TRes>(string endpoint, TReq req, CancellationToken ct)
        {
            var json = JsonSerializer.Serialize(req, Json);

            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            using (var res = await Http.PostAsync(endpoint, content, ct).ConfigureAwait(false))
            {
                var body = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                EnsureSuccess(res, body, endpoint);

                if (string.IsNullOrWhiteSpace(body)) return default(TRes);
                if (LooksLikeHtml(body))
                    throw new Exception("Respuesta HTML (SPA). Estás llamando una ruta de FRONT, no un endpoint JSON: " + endpoint);

                return JsonSerializer.Deserialize<TRes>(body, Json);
            }
        }

        private async Task<string> SendPostRawAsync<TReq>(string endpoint, TReq req, CancellationToken ct)
        {
            var json = JsonSerializer.Serialize(req, Json);

            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            using (var res = await Http.PostAsync(endpoint, content, ct).ConfigureAwait(false))
            {
                var body = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                EnsureSuccess(res, body, endpoint);

                if (LooksLikeHtml(body))
                    throw new Exception("Respuesta HTML (SPA). Estás llamando una ruta de FRONT, no un endpoint JSON: " + endpoint);

                return body;
            }
        }

        private static void EnsureSuccess(HttpResponseMessage res, string body, string endpoint)
        {
            if (res.IsSuccessStatusCode) return;
            throw new Exception("HTTP " + (int)res.StatusCode + " (" + res.ReasonPhrase + ") en " + endpoint + " - " + body);
        }

        private static string NormalizeEndpoint(string endpoint)
        {
            if (string.IsNullOrWhiteSpace(endpoint)) return "/";
            endpoint = endpoint.Trim();
            if (!endpoint.StartsWith("/")) endpoint = "/" + endpoint;
            return endpoint;
        }

        protected static bool LooksLikeHtml(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            var t = s.TrimStart();
            return t.StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase)
                   || t.StartsWith("<html", StringComparison.OrdinalIgnoreCase)
                   || t.Contains("You need to enable JavaScript", StringComparison.OrdinalIgnoreCase);
        }
    }
}

