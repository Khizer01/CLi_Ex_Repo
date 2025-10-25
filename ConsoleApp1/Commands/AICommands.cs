using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1
{
    public class AICommands
    {
        private static readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string API_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5:generateContent";
        static AICommands()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public AICommands(string apiKey)
        {
            _apiKey = apiKey;
        }

        // Non-async wrapper method that matches the Action<string[]> delegate signature
        public void AskAI(string[] args)
        {
            Console.WriteLine("Starting AI Chat with Gemini. Type 'exit' to end the conversation.");
            Console.WriteLine("----------------------------------------");

            string userInput;
            do
            {
                Console.Write("You: ");
                userInput = Console.ReadLine();
                
                if (string.IsNullOrEmpty(userInput) || userInput.ToLower() == "exit")
                    break;

                try
                {
                    // Use Task.Run to avoid blocking and get the result synchronously
                    string response = Task.Run(() => SendMessageToGeminiAsync(userInput)).Result;
                    Console.WriteLine("\nAI: " + response);
                    Console.WriteLine("----------------------------------------");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error communicating with AI: {ex.Message}");
                }

            } while (true);

            Console.WriteLine("AI chat session ended.");
        }

        private async Task<string> SendMessageToGeminiAsync(string message)
        {
            // Create the request payload with a smaller object structure
            var requestContent = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = message }
                        }
                    }
                }
            };

            // Serialize with optimized settings
            string jsonRequest = JsonConvert.SerializeObject(requestContent, new JsonSerializerSettings { 
                NullValueHandling = NullValueHandling.Ignore 
            });

            // Create the request with cached URL
            string requestUrl = $"{API_URL}?key={_apiKey}";
            using (var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json"))
            {
                // Send the request with cancellation support
                using (var response = await _httpClient.PostAsync(requestUrl, content).ConfigureAwait(false))
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        return $"API Error: {response.StatusCode} - {jsonResponse}";
                    }
                    
                    // Extract response using JObject for better performance
                    return ParseResponseWithJson(jsonResponse);
                }
            }
        }

        private string ParseResponseWithJson(string jsonResponse)
        {
            try 
            {
                // Use JObject for faster and more reliable parsing
                JObject response = JObject.Parse(jsonResponse);
                
                // Navigate the JSON structure to find the text content
                JToken candidates = response["candidates"];
                if (candidates != null && candidates.Type == JTokenType.Array && candidates.HasValues)
                {
                    JToken content = candidates[0]?["content"];
                    JToken parts = content?["parts"];
                    
                    if (parts != null && parts.Type == JTokenType.Array && parts.HasValues)
                    {
                        string text = parts[0]?["text"]?.ToString();
                        if (!string.IsNullOrEmpty(text))
                        {
                            return text;
                        }
                    }
                }
                
                return "Could not parse AI response.";
            }
            catch
            {
                return "Error parsing AI response.";
            }
        }
    }
}