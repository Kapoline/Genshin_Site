using System;
using System.Linq;
using LinqToDB.Mapping;
using Newtonsoft.Json;


namespace GenshinPomoyka.Models
{
    [Table(Name = "Account")]
    public class Account
    {
        [Column("user_id")]
        [Column(IsPrimaryKey = true)]
        public Guid Id { get; set; }
        [Column("user_login")]
        public string Email { get; set; }
        
        [Column("user_password")]
        public string Password { get; set; }
        
        [Column("user_nickname")]
        public string Nickname { get; set; }
        
        [Column("user_role")]
        public string Role { get; set; }
    }

    
}