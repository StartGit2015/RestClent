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
        /// <summary>
        /// Injects the authorization token to header
        /// </summary>
        /// <param name="token">access token</param>
        public void InjectAuthorizationToken(string token)
        {
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Method to invoke GET operation
        /// </summary>
        /// <typeparam name="T">Response type</typeparam>
        /// <param name="uri">target url</param>
        /// <returns>Task of type T</returns>
        public async Task<T> GetAsync<T>(string uri)
        {            
            HttpResponseMessage response = null;

            response = await GetAsync(uri).ConfigureAwait(false);

            var jsonContent = await response.Content.ReadAsStringAsync();
            
            return await GetReponseObject<T>(response);
        }

        /// <summary>
        /// Method to invoke POST operation
        /// </summary>
        /// <typeparam name="T">Request type</typeparam>
        /// <typeparam name="U">Response type</typeparam>
        /// <param name="uri">target url</param>
        /// <param name="obj">Request object</param>
        /// <returns>Task of type U</returns>
        public async Task<U> PostAsync<T, U>(string uri, T obj)
        {
            var json = JsonConvert.SerializeObject(obj);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;
            response = await PutAsync(uri, content);

            return await GetReponseObject<U>(response);
        }

        /// <summary>
        /// Method to invokde PUT operation
        /// </summary>
        /// <typeparam name="T">Request type</typeparam>
        /// <typeparam name="U">Response type</typeparam>
        /// <param name="uri">Target url</param>
        /// <param name="obj">Request object</param>
        /// <returns>Task of type U</returns>
        public async Task<U> PutAsync<T, U>(string uri, T obj)
        {
            var json = JsonConvert.SerializeObject(obj);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;
            response = await PutAsync(uri, content);

            return await GetReponseObject<U>(response);
        }

        /// <summary>
        /// Method to invoke DELETE operation
        /// </summary>
        /// <typeparam name="T">Response type</typeparam>
        /// <param name="uri">target url</param>
        /// <returns>Task of type T</returns>
        public async Task<T> DeleteAsync<T>(Uri uri)
        {
            HttpResponseMessage response = null;

            response = await DeleteAsync(uri);

            return await GetReponseObject<T>(response);            
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
