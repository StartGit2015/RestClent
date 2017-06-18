using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RestClientPackage
{
    public class RestClient : HttpClient
    {
        public void InjectAuthorizationToken(string token)
        {
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<T> GetAsync<T>(string uri)
        {            
            HttpResponseMessage response = null;

            response = await GetAsync(uri).ConfigureAwait(false);

            var jsonContent = await response.Content.ReadAsStringAsync();
            
            return await GetReponseObject<T>(response);
        }

        public async Task<U> PutAsync<T, U>(string uri, T obj)
        {
            var json = JsonConvert.SerializeObject(obj);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;
            response = await PutAsync(uri, content);

            return await GetReponseObject<U>(response);
        }

        private async Task<T> GetReponseObject<T>(HttpResponseMessage response)
        {
            var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };
            response.EnsureSuccessStatusCode();
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                using (var reader = new StreamReader(stream))
                {
                    using (var json = new JsonTextReader(reader))
                    {
                        return serializer.Deserialize<T>(json);
                    }
                }
            }
        }
    }
}
