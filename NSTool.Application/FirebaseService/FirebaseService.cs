using Firebase.Database;
using Firebase.Database.Query;
using FirebaseAdmin;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSTool.Domain.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTool.Application.FirebaseService
{
    public class FirebaseService : IFirebaseService
    {
        private readonly IFirebaseClient _firebaseClient;
        public FirebaseService(IConfiguration configuration)
        {
            var config = new FirebaseConfig
            {
                AuthSecret = configuration["Firebase:AuthSecret"],
                BasePath = configuration["Firebase:DatabaseUrl"]
            };

            _firebaseClient = new FireSharp.FirebaseClient(config);
        }

        public async Task DeleteDataAsync(string path)
        {
            await _firebaseClient.DeleteAsync(path);
        }

        public async Task<List<T>> GetAllDataAsync<T>(string path)
        {
            FirebaseResponse response = await _firebaseClient.GetAsync(path);

            // Check if the response body is "null" or empty
            if (response.Body == "null" || string.IsNullOrWhiteSpace(response.Body))
            {
                return new List<T>();
            }

            // Deserialize the JSON response into a list
            var data = JsonConvert.DeserializeObject<List<T>>(response.Body);

            return data ?? new List<T>();
        }

        public async Task<Dictionary<string, T>> GetAllDataWithKeysAsync<T>(string path)
        {
            FirebaseResponse response = await _firebaseClient.GetAsync(path);

            if (response.Body == "null" || string.IsNullOrWhiteSpace(response.Body))
            {
                return new Dictionary<string, T>();
            }

            // Attempt to deserialize as a dictionary
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, T>>(response.Body) ?? new Dictionary<string, T>();
            }
            catch (JsonSerializationException)
            {
                // Handle the case where data might be an array
                var dataList = JsonConvert.DeserializeObject<List<T>>(response.Body);
                var dataDict = dataList.Select((item, index) => new { item, index })
                                       .ToDictionary(x => x.index.ToString(), x => x.item);
                return dataDict;
            }
        }

        public async Task InsertDataWithKeyAsync<T>(string path, string key, T data)
        {
            var fullPath = $"{path}/{key}";
            await _firebaseClient.SetAsync(fullPath, data);
        }


        public async Task SaveDataAsync(string path, object data)
        {
            SetResponse response = await _firebaseClient.SetAsync(path, data);
        }
    }
}
