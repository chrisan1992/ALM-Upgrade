using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ALM_Upgrade.Models
{

    [MetadataType(typeof(Customer_ProjectMetaData))]
    public partial class Customer_Project
    {

    }

    public class Customer_ProjectMetaData
    {
        [Display(Name ="Customer Name")]
        [Required(ErrorMessage = "Customer name is required.")]
        public String customer_name { get; set; }

        [Display(Name = "Target URL")]
        [Required(ErrorMessage = "URL is required.")]
        [RegularExpression(@"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?(/)?$",ErrorMessage ="The URL is not valid")]
        public String customer_url { get; set; }

        [Display(Name = "SD Number")]
        public String service_id { get; set; }
    }
}