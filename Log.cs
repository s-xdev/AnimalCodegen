using System.Net.Http;
using System.Text;
using System;
using MelonLoader;


namespace AnimalCodegen.Logging
{
    public static class WebhookLogger
    {   
        private static HttpClient httpClient = new HttpClient();
        public static string WebhookUrl = "";

        public static async Task SendMessage(string message)
        {
            var Payload = new
            {
                content = message
            };

            var JsonPayload = System.Text.Json.JsonSerializer.Serialize(Payload);
            var content = new StringContent(JsonPayload, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(WebhookUrl, content);
            }
            catch (Exception ex) {
                MelonLogger.Msg($"Failed to post to webhook. {ex.Message}");
            };
        }
        
    }
}
