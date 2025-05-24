using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Server.Services
{
    public static class PushNotifier
    {
        private static readonly HttpClient client = new HttpClient();
        public static List<string> ExpoTokens { get; } = new List<string>();

        public static void RegisterToken(string token)
        {
            if (!ExpoTokens.Contains(token))
                ExpoTokens.Add(token);
        }

        public static async Task SendToAllAsync(string title, string body)
        {
            foreach (var token in ExpoTokens)
            {
                var payload = new
                {
                    to = token,
                    sound = "default",
                    title,
                    body
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                await client.PostAsync("https://exp.host/--/api/v2/push/send", content);
            }
        }
    } 
}
