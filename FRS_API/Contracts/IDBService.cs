using FRS_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS_API.Contracts
{
    public interface IDBService
    {
        #region Users
        
        public Task<User> AddUserAsync(User user);

        public Task<List<User>> GetAllUsersAsync();

        #endregion

        public Task<List<Item>> GetAllItemsAsync();

        public Task<int> AddTransaction(Transaction transaction);

        public Task<List<Transaction>> GetTransactions(int userId);

        public Task<bool> IsUserAuthenticated(int userId, string personId);

        public Task<string> GetUserVoiceId(int userId);

        public Task<string> UploadBlobToContainer(string containerName, string blobName, string uploadLocation, string storageConnectionString);
    }
}
