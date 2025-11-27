//using MQTTnet;
//using MQTTnet.Client;
//using MQTTnet.Client.Options;
//using System;
//using System.Text;
//using System.Text.Json;
//using System.Threading;
//using System.Threading.Tasks;

//namespace VlbBet.Infrastructure
//{
//    public sealed class MqttGameClient : IDisposable
//    {
//        private readonly string _host;
//        private readonly int _port;
//        private readonly IMqttClient _client;
//        private readonly IMqttClientOptions _options;

//        private static readonly JsonSerializerOptions _json = new JsonSerializerOptions
//        {
//            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
//            PropertyNameCaseInsensitive = true
//        };

//        public event EventHandler<GamesLineMessage> GamesLineReceived;
//        public event EventHandler<HourMessage> HourReceived;
//        public event EventHandler<bool> ConnectionChanged;

//        public MqttGameClient(string host, int port = 1883)
//        {
//            _host = host;
//            _port = port;

//            var factory = new MqttFactory();
//            _client = factory.CreateMqttClient();

//            // ✅ ClientId único (evita que el broker bote tu app si abres 2 instancias)
//            var clientId = "vlb-bet-winforms-" + Guid.NewGuid().ToString("N");

//            _options = new MqttClientOptionsBuilder()
//                .WithClientId(clientId)
//                .WithTcpServer(_host, _port)
//                .WithCleanSession()
//                .Build();

//            _client.UseConnectedHandler(async e =>
//            {
//                ConnectionChanged?.Invoke(this, true);

//                // Suscribirse cuando conecte
//                await _client.SubscribeAsync("vlb/games-line");
//                await _client.SubscribeAsync("vlb/hour");

//                // Por si en algún sitio publican con slash inicial
//                await _client.SubscribeAsync("/vlb/games-line");
//                await _client.SubscribeAsync("/vlb/hour");
//            });

//            _client.UseDisconnectedHandler(e =>
//            {
//                ConnectionChanged?.Invoke(this, false);
//            });

//            _client.UseApplicationMessageReceivedHandler(e =>
//            {
//                try
//                {
//                    var topic = (e.ApplicationMessage.Topic ?? "").Trim();
//                    if (topic.StartsWith("/")) topic = topic.Substring(1);

//                    var payloadBytes = e.ApplicationMessage.Payload ?? Array.Empty<byte>();
//                    var payload = Encoding.UTF8.GetString(payloadBytes);

//                    if (topic.Equals("vlb/games-line", StringComparison.OrdinalIgnoreCase))
//                    {
//                        GamesLineMessage msg = null;

//                        // 1) Intento normal
//                        try
//                        {
//                            msg = JsonSerializer.Deserialize<GamesLineMessage>(payload, _json);
//                        }
//                        catch
//                        {
//                            // 2) Fallback: al menos sacamos active y num para que el UI reaccione
//                            try
//                            {
//                                using var doc = JsonDocument.Parse(payload);
//                                var root = doc.RootElement;

//                                bool active = root.TryGetProperty("active", out var a) && a.ValueKind == JsonValueKind.True;
//                                int num = root.TryGetProperty("num", out var n) && n.TryGetInt32(out var v) ? v : 0;

//                                msg = new GamesLineMessage
//                                {
//                                    Active = active,
//                                    Num = num,
//                                    Games = new System.Collections.Generic.List<VlbBet.Core.Game>()
//                                };
//                            }
//                            catch { /* ignorar */ }
//                        }

//                        if (msg != null) GamesLineReceived?.Invoke(this, msg);
//                        return;
//                    }

//                    if (topic.Equals("vlb/hour", StringComparison.OrdinalIgnoreCase))
//                    {
//                        HourMessage msg = null;

//                        try
//                        {
//                            msg = JsonSerializer.Deserialize<HourMessage>(payload, _json);
//                        }
//                        catch
//                        {
//                            try
//                            {
//                                using var doc = JsonDocument.Parse(payload);
//                                var root = doc.RootElement;

//                                long hour = root.TryGetProperty("hour", out var h) && h.TryGetInt64(out var v) ? v : 0;
//                                msg = new HourMessage { Hour = hour };
//                            }
//                            catch { /* ignorar */ }
//                        }

//                        if (msg != null) HourReceived?.Invoke(this, msg);
//                        return;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine("Error MQTT: " + ex.Message);
//                }
//            });
//        }

//        public async Task ConnectAsync(CancellationToken token = default)
//        {
//            if (!_client.IsConnected)
//                await _client.ConnectAsync(_options, token);
//        }

//        public void Dispose()
//        {
//            try
//            {
//                if (_client != null)
//                {
//                    if (_client.IsConnected)
//                        _client.DisconnectAsync().GetAwaiter().GetResult();

//                    _client.Dispose();
//                }
//            }
//            catch { }
//        }
//    }
//}
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace VlbBet.Infrastructure
{
    public class MqttGameClient : IDisposable
    {
        private readonly string _host;
        private readonly int _port;
        private readonly IMqttClient _client;
        private readonly IMqttClientOptions _options;

        private static readonly JsonSerializerOptions _json = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public event EventHandler<GamesLineMessage> GamesLineReceived;
        public event EventHandler<HourMessage> HourReceived;

        public event EventHandler<bool> ConnectionChanged;

        public MqttGameClient(string host, int port = 1883)
        {
            _host = host;
            _port = port;

            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();

            _options = new MqttClientOptionsBuilder()
                .WithClientId("vlb-bet-winforms")
                .WithTcpServer(_host, _port)
                .WithCleanSession()
                .Build();

            _client.UseConnectedHandler(async e =>
            {
                ConnectionChanged?.Invoke(this, true);

                await _client.SubscribeAsync("vlb/games-line");
                await _client.SubscribeAsync("vlb/hour");
            });

            _client.UseDisconnectedHandler(e =>
            {
                ConnectionChanged?.Invoke(this, false);
            });

            _client.UseApplicationMessageReceivedHandler(e =>
            {
                try
                {
                    var topic = e.ApplicationMessage.Topic;
                    var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload ?? Array.Empty<byte>());

                    if (string.IsNullOrWhiteSpace(payload))
                        return;

                    if (topic == "vlb/games-line")
                    {
                        var msg = JsonSerializer.Deserialize<GamesLineMessage>(payload, _json);

                        // ✅ IMPORTANTÍSIMO: hidratar líneas/pitchers aquí
                        msg?.Normalize();

                        if (msg != null) GamesLineReceived?.Invoke(this, msg);
                    }
                    else if (topic == "vlb/hour")
                    {
                        var msg = JsonSerializer.Deserialize<HourMessage>(payload, _json);
                        if (msg != null) HourReceived?.Invoke(this, msg);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error MQTT: " + ex);
                }
            });
        }

        public async Task ConnectAsync(CancellationToken token = default)
        {
            if (!_client.IsConnected)
                await _client.ConnectAsync(_options, token);
        }

        public void Dispose()
        {
            try
            {
                if (_client != null)
                {
                    if (_client.IsConnected)
                        _client.DisconnectAsync().GetAwaiter().GetResult();

                    _client.Dispose();
                }
            }
            catch { }
        }
    }
}
