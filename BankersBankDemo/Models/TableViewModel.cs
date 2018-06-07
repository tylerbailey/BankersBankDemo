using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankersBankDemo.Models
{
    /// <summary>
    /// Class that represents a row from the read spreadsheet for the front end 
    /// </summary>
    public class TableViewModel
    {
        public String AccountNumber { get; set; }
        public String LastNameCompanyName { get; set; }
        public String FirstName { get; set; }
        public String MiddleName { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        public String Zip { get; set; }
        public String PimaryNumber { get; set; }
        public String SecondaryNumber { get; set; }
        public String DateAccountOpened { get; set; }
        public String CurrentBalance { get; set; }
        public String AccountType { get; set; }
    }
}