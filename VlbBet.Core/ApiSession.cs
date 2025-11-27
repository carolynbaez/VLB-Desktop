using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace VlbBet.Infrastructure
{
    public static class AppState
    {
        public static ApiSession Api; // sesión compartida (cookies, headers, etc.)
    }
    public sealed class ApiSession : IDisposable
    {
        private readonly HttpClientHandler _handler;

        public HttpClient Http { get; }
        public CookieContainer Cookies { get { return _handler.CookieContainer; } }
        public JsonSerializerOptions Json { get; }
        public Uri BaseAddress { get; }

        public ApiSession(string baseUrl, TimeSpan? timeout = null)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("baseUrl requerido.", nameof(baseUrl));

            BaseAddress = new Uri(baseUrl.TrimEnd('/'));

            _handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer(),
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            Http = new HttpClient(_handler)
            {
                BaseAddress = BaseAddress,
                Timeout = timeout ?? TimeSpan.FromSeconds(30)
            };

            Http.DefaultRequestHeaders.Accept.Clear();
            Http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Http.DefaultRequestHeaders.UserAgent.ParseAdd("VlbBet.WinForms/1.0");

            Json = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        public void AddCookie(string name, string value, string path = "/")
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("cookie name requerido.", nameof(name));
            Cookies.Add(BaseAddress, new Cookie(name, value, path));
        }

        public void Dispose()
        {
            Http?.Dispose();
            _handler?.Dispose();
        }
    }
}
