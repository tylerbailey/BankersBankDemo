using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankersBankDemo.Models
{
    /// <summary>
    /// Class respresenting a JSON response. Returns a status that can be used to determine 
    /// if the process to generate the result was completed successfully along with a result
    /// to hold any related objects 
    /// </summary>
    public class JsonResultWithStatus
    {
        public JsonResultWithStatus(bool status, Object result)
        {
            Status = status;
            Result = result;
        }

        public bool Status { get; set; }
        public Object Result { get; set; }
    }
}