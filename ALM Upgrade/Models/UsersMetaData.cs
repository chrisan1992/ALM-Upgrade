using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ALM_Upgrade.Models
{

    [MetadataType(typeof(UsersMetaData))]
    public partial class Users
    {

    }

    public class UsersMetaData
    {
        [Required(ErrorMessage ="Email is required.")]
        [RegularExpression(@"^([\w\.\-]+)@(hpe.com|microfocus.com)", ErrorMessage ="The email doesn't have the correct format. Must be a valid email address and belong to @hpe.com or @microfocus.com")]
        public String Email { get; set; }

        [Required(ErrorMessage ="Username is required")]
        public String UserName { get; set; }

        [Required(ErrorMessage ="The password is required")]
        public String Emailpass { get; set; }
    }

    
}