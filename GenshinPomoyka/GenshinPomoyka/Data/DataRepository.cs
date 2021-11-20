using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Core;
using GenshinPomoyka.Models;

namespace GenshinPomoyka.Data
{
    public class DataRepository
    {
        private const string connectionString = "Host=localhost;Username=postgres;Password=RiKed1032002;Database=GenshinPomoyka";

        public DataConnection Context
        {
            get
            {
                var builder = new LinqToDbConnectionOptionsBuilder();

                builder.UsePostgreSQL(connectionString);

                return new DataConnection(builder.Build());
            }
        }
        
        private ITable<User> Users => Context.GetTable<User>();
        
        public void Create(object item)
        {
            switch (item.GetType().Name)
            {
                case ("User"):
                    CreateUser((User)item);
                    break;
                default:
                    throw new NotImplementedException();
            };
        }
        
        private void CreateUser(User item)
        {
            Context.Insert(item);
        }
        
        public void Delete(object item)
        {
            Context.Delete(item);
        }
        
        public object GetItem(Guid id, string type)
        {
            return type switch
            {
                "User" => GetUser(id),
                
                _ => throw new NotImplementedException(),
            };
        }
        
        private User GetUser(Guid id)
        {
            return Users.SingleOrDefault(x => x.Id == id);
        }
        
        public IEnumerable<object> GetItemList(string type)
        {
            return type switch
            {
                "User" => GetUserList(),
                //"Service" => GetServiceList(),
                //"TariffPlan" => GetTariffList(),
                _ => throw new NotImplementedException(),
            };
        }
        
        private IEnumerable<User> GetUserList()
        {
            var list = Users.ToList();
            if (list != null)
                return list;
            return null;
        }
        
        public void Update(object item)
        {
            switch (item.GetType().Name)
            {
                case ("User"):
                    UpdateAccount((User)item);
                    break;
                default:
                    throw new NotImplementedException();
            };
        }
        
        private void UpdateAccount(User item)
        {
            Context.Update(item);
        }

        public void Dispose()
        {
            Context.Dispose();
        }
        
        public User GetAccountByEmail(string email)
        {
            return Users.SingleOrDefault(x => x.Email == email);
        }
    }
}