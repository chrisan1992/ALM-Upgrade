//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Users
    {
        public Users()
        {
            this.UsersPermissions = new HashSet<UsersPermissions>();
        }
    
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Emailpass { get; set; }
    
        public virtual ICollection<UsersPermissions> UsersPermissions { get; set; }
    }
}
