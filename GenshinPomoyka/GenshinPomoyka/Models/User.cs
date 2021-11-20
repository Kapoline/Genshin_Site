using System;
using System.Linq;
using LinqToDB.Mapping;


namespace GenshinPomoyka.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("User")]
    public class User
    {
        [System.ComponentModel.DataAnnotations.Schema.Column("user_id"),PrimaryKey]
        public Guid Id { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column("user_login")]
        public string Email { get; set; }
        
        [Column("user_password")]
        public string Password { get; set; }
        
        [Column("user_nickname")]
        public string Nickname { get; set; }
        
        [Column("user_role")]
        public Roles Role { get; set; }
        
    }

    public enum Roles
    {
        Admin,
        User,
    }
}