using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTool.Domain.Services.Contract
{
    public interface IFirebaseService
    {
        Task SaveDataAsync(string path, object data);
        Task<List<T>> GetAllDataAsync<T>(string path);

        public Task<Dictionary<string, T>> GetAllDataWithKeysAsync<T>(string path);
        public Task InsertDataWithKeyAsync<T>(string path, string key, T data);
        public Task DeleteDataAsync(string path);
    }
}
