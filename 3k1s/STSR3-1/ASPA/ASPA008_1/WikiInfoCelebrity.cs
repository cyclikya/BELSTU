using System.Text.Json;
using System;

namespace ASPA008_1
{
    public class WikiInfoCelebrity
    {
        HttpClient client;
        Dictionary<string, string> wikiReferens { get; set; }
        string wikiURI;
        
        private WikiInfoCelebrity(string fullname)
        {
            this.client = new HttpClient();
            this.client.Timeout = TimeSpan.FromSeconds(10);
            this.client.DefaultRequestHeaders.Add("User-Agent", "CelebritiesDictionary/1.0 (https://example.com/contact)");
            this.wikiReferens = new Dictionary<string, string>();
            this.wikiURI = string.Format("https://en.wikipedia.org/w/api.php?action=opensearch&search={0}&format=json&limit=10", Uri.EscapeDataString(fullname));
        }

        public static async Task<Dictionary<string, string>> GetRefereces(string fullname)
        {
            WikiInfoCelebrity info = new WikiInfoCelebrity(fullname);
            try
            {
                HttpResponseMessage message = await info.client.GetAsync(info.wikiURI);
                if (message.IsSuccessStatusCode)
                {
                    string jsonContent = await message.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(jsonContent))
                    {
                        using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                        {
                            JsonElement root = doc.RootElement;
                            if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() >= 4)
                            {
                                // Wikipedia OpenSearch API возвращает массив: [searchTerm, [titles], [descriptions], [urls]]
                                JsonElement titles = root[1];
                                JsonElement urls = root[3];
                                
                                if (titles.ValueKind == JsonValueKind.Array && urls.ValueKind == JsonValueKind.Array)
                                {
                                    int count = Math.Min(titles.GetArrayLength(), urls.GetArrayLength());
                                    int maxReferences = 3; // Фиксированное количество ссылок
                                    for (int i = 0; i < count && info.wikiReferens.Count < maxReferences; i++)
                                    {
                                        string title = titles[i].GetString() ?? string.Empty;
                                        string url = urls[i].GetString() ?? string.Empty;
                                        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(url) && !info.wikiReferens.ContainsKey(title))
                                        {
                                            info.wikiReferens.Add(title, url);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем пустой словарь
                System.Diagnostics.Debug.WriteLine($"Error fetching Wikipedia data: {ex.Message}");
            }
            finally
            {
                info.client?.Dispose();
            }
            return info.wikiReferens;
        }
    }
}
