using FRS_API.Contracts;
using FRS_API.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace FRS_API.Services
{
    public class DBService : IDBService
    {
        private string connectionString;
        public  DBService(string _connectionString)
        {
            connectionString = _connectionString;
        }

        public async Task<User> AddUserAsync(User user)
        {
            var query = $"Insert into [User](Name, Contact,Gender,Balance,Password,AzurePersonId) OUTPUT INSERTED.Id " +
                $"values('{user.Name}','{user.Contact}','{user.Gender}',{user.Balance},'{user.Password}','{user.AzurePersonId}');";
            using (var connection = new SqlConnection(connectionString))
            {
                int userId = 0;
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var result =  await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                    if (result != null)
                    {
                        var success = Int32.TryParse(result.ToString(), out userId);
                    }
                }
                if (userId > 0)
                {
                    query = $"select * from [user] where Id = {userId}";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            var newUser = new User
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Contact = reader.GetString(2),
                                Gender = reader.GetString(3),
                                Balance = reader.GetInt32(4),
                                Password = reader.GetString(5),
                                AzurePersonId = reader.GetString(6)
                            };
                            return newUser;
                        }
                    }
                }
                return null;
            }
        }

        public async Task<List<Item>> GetAllItemsAsync()
        {
            var items = new List<Item>();
            var query = "select * from [Items]";
            using (var connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                    while (reader.Read())
                    {
                        var item = new Item
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Category = reader.GetString(2),
                            Amount = reader.GetInt32(3),
                            Size = reader.GetInt32(4),
                            Color = reader.GetString(5),
                            ImageUrl = reader.GetString(6)
                        };
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var userList = new List<User>();
            var query = "Select * from [User]";
            using (var connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {

                    connection.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var user = new User { Id = reader.GetInt32(0), Name = reader.GetString(1), Contact = reader.GetString(2),
                            Gender = reader.GetString(3), Balance = reader.GetInt32(4) , Password = reader.GetString(5), AzurePersonId = reader.GetString(6) };
                        userList.Add(user);
                    }
                }
            }
            return userList;

        }

        public async Task<int> AddTransaction(Transaction transaction)
        {
            int finalBalance = 0;   
            var query = $"select Balance from [User] where Id = {transaction.UserId};";
            using (var connection = new SqlConnection(connectionString))
            {
                // check Balance
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                    int balance = int.Parse(reader.ToString());
                    finalBalance = balance;
                    if (balance < transaction.Amount)
                    { throw new Exception("Amount is greater than balance"); }

                }
               
                // Deduct Amount from balance;
                query = $"Update [User] set Balance = Balance - {transaction.Amount} where Id = {transaction.UserId};";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    var reader = cmd.ExecuteNonQuery();
                }
                // Add Transaction;
                query = $"Insert into [Transaction_Details] (UserId, Amount,NoOfItems) OUTPUT INSERTED.Id values ({transaction.UserId}," +
                    $"{transaction.Amount},{transaction.NoOfItems});";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    var reader = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                    int transactionId = int.Parse(reader.ToString());
                    return finalBalance - transaction.Amount;
                }
            }
        }

        public async Task<List<Transaction>> GetTransactions(int userId)
        {
            var transactions = new List<Transaction>();
            var query = $"select * from [Transaction_Details] where UserId = {userId}";
            using (var connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                    while (reader.Read())
                    {
                        var transaction = new Transaction
                        {
                            Id = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            Amount = reader.GetInt32(2),
                            NoOfItems = reader.GetInt32(3)
                        };
                        transactions.Add(transaction);
                    }
                }
            }
            return transactions;
        }

        public async Task<bool> IsUserAuthenticated(int userId, string personId)
        {
            var query = $"select AzurepersonId from [User] where Id ={userId};";
            using (var connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                    if (reader.ToString().Equals(personId,StringComparison.OrdinalIgnoreCase))
                    { return true; }
                    return false;
                }
            }
        }
    }
}
