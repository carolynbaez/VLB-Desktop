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
