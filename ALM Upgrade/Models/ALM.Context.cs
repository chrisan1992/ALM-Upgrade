﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ALM_Upgrade.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;

    using System.Linq;
    
    public partial class ALMEntities : DbContext
    {
        public ALMEntities()
            : base("name=ALMEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Customer_Project> Customer_Project { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<Project_Email> Project_Email { get; set; }
        public DbSet<ProjectList> ProjectList { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<UsersPermissions> UsersPermissions { get; set; }
        public DbSet<database_firewall_rules> database_firewall_rules { get; set; }
    
        public virtual int AddUser(string pLogin, string pPassword, string email)
        {
            var pLoginParameter = pLogin != null ?
                new ObjectParameter("pLogin", pLogin) :
                new ObjectParameter("pLogin", typeof(string));
    
            var pPasswordParameter = pPassword != null ?
                new ObjectParameter("pPassword", pPassword) :
                new ObjectParameter("pPassword", typeof(string));
    
            var emailParameter = email != null ?
                new ObjectParameter("email", email) :
                new ObjectParameter("email", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AddUser", pLoginParameter, pPasswordParameter, emailParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> AutenticateUser(string username, string pass)
        {
            var usernameParameter = username != null ?
                new ObjectParameter("username", username) :
                new ObjectParameter("username", typeof(string));
    
            var passParameter = pass != null ?
                new ObjectParameter("pass", pass) :
                new ObjectParameter("pass", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("AutenticateUser", usernameParameter, passParameter);
        }
    
        public virtual int UpdateUserPassword(Nullable<int> userID, string newPassword)
        {
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var newPasswordParameter = newPassword != null ?
                new ObjectParameter("NewPassword", newPassword) :
                new ObjectParameter("NewPassword", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdateUserPassword", userIDParameter, newPasswordParameter);
        }
    }
}
