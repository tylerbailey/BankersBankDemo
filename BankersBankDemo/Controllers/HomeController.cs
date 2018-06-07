using BankersBankDemo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace BankersBankDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Processes a CSV file POSTed from the browser and returns a JSON array of objects 
        /// representing the data contained with the CSV file. 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadFile()
        {           
            int rowNumber = 0;
            List<TableViewModel> tableItems = new List<TableViewModel>();
            List<int> errorRows = new List<int>();
            HttpPostedFileBase file;
            JsonResultWithStatus jsonResult;
            String line;

            try
            {
                //Get the file from the Request if it actually exists
                file = Request.Files.AllKeys.Length > 0 ? Request.Files[0] : null;

                if (file != null && file.ContentLength > 0)
                {
                    using (StreamReader reader = new StreamReader(file.InputStream))
                    {
                        while (!reader.EndOfStream)
                        {
                            try
                            {
                                rowNumber++;
                                line = reader.ReadLine();
                                String[] row = line.Split(',');
                                //Only process rows that contain 14 columns, else add the row number in the error list
                                if (row.Length == 14)
                                {
                                   
                                    tableItems.Add(CreateTableViewModel(row));
                                }
                                else
                                {
                                    errorRows.Add(rowNumber);
                                }
                            }
                                //Inner catch for any issues that arise from processing the file without stopping the
                                //other rows from processing and logging the troublesome row
                            catch (Exception innerException)
                            {
                                errorRows.Add(rowNumber);
                            }
                        }
                    }
                }
                WriteResultsToLogFile(tableItems.Count(), errorRows.Count, errorRows);
                jsonResult = new JsonResultWithStatus(true,tableItems);
            }
            catch (Exception ex)
            {
                //File read completely failed so return a false response to display an error on the front 
                jsonResult = new JsonResultWithStatus(false, tableItems);
            }
            
            
            return Content(JsonConvert.SerializeObject(jsonResult));
        }

        /// <summary>
        /// Parses the strings from the string array into an object
        /// </summary>
        /// <param name="row">The read row from the file split on the comma seperator int a string array</param>
        /// <returns>A viewmodel object reprsenting a row in the table</returns>
        public TableViewModel CreateTableViewModel(String[] row)
        {
            TableViewModel tempVM = new TableViewModel();
            tempVM.AccountNumber = FormatNumbers(row[0]);
            tempVM.LastNameCompanyName = row[1];
            tempVM.FirstName = FormatNumbers(row[2]);
            tempVM.MiddleName = FormatPhoneNumber(row[3]);
            tempVM.Address1 = row[4];
            tempVM.Address2 = row[5];
            tempVM.City = row[6];
            tempVM.State = row[7];
            tempVM.Zip = FormatNumbers(row[8]);
            tempVM.PimaryNumber = FormatPhoneNumber(row[9]);
            tempVM.SecondaryNumber = FormatPhoneNumber(row[10]);
            tempVM.DateAccountOpened = row[11];
            tempVM.CurrentBalance = "$ "+FormatNumbers(row[12]);
            tempVM.AccountType = row[13];

            return tempVM;

        }

        /// <summary>
        /// Writes to an existing or creates a new log file the success and failures of the 
        /// file import. 
        /// </summary>
        /// <param name="successCount">The rows that were processed succesfully</param>
        /// <param name="failureCount">The rowsw that failed to process</param>
        /// <param name="errorRows">A list of the row numbers that failed to process</param>
        private void WriteResultsToLogFile(int successCount, int failureCount, List<int> errorRows)
        {
            DateTime currentTime = DateTime.UtcNow;
            String desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);   //For the sake of ease I'm writting this log to the desktop
            String fileName = currentTime.ToString("MM-dd-yyyy") + " File Import Log.txt";

            using (StreamWriter writer = new StreamWriter(System.IO.File.Open(Path.Combine(desktopPath, fileName), System.IO.FileMode.Append)))
            {
                writer.WriteLine(currentTime + " Successful rows: " + successCount);
                writer.WriteLine(currentTime + " Failed rows: " + failureCount);
                StringBuilder appendFailedRows = new StringBuilder();
                foreach (int errorRowNumber in errorRows)
                {
                    appendFailedRows.Append(" #" + errorRowNumber);
                }
                writer.WriteLine(currentTime + " Failed row numbers:" + appendFailedRows.ToString());
                writer.WriteLine();
            }
        }

        /// <summary>
        /// Formats a given number in a string. Reading from the file is padding the numbers with leading zeros.
        /// In the event the converstion fails then the original given string is returned.
        /// </summary>
        /// <param name="number">The number to format</param>
        /// <returns>The formatted number</returns>
        private String FormatNumbers(String number)
        {
            String formattedNumber = "";
            int convertedNumber;
            bool success = int.TryParse(number, out convertedNumber);
            formattedNumber = success ? convertedNumber.ToString() : number;
            return formattedNumber;
        }

        /// <summary>
        /// Converts a string phone number to a ###-###-#### or ###-#### format. If 
        /// the phone number is not 7 or 10 digits then the original given string is returned.
        /// </summary>
        /// <param name="phoneNumber">The phone number to format</param>
        /// <returns>A formatted phone number or the original given string</returns>        
        private String FormatPhoneNumber(String phoneNumber)
        {
            StringBuilder formattedPhoneNumber = new StringBuilder();
            if (phoneNumber.Length == 7)
            {
                formattedPhoneNumber.Append(phoneNumber.Substring(0, 3) + "-" + phoneNumber.Substring(3, 4));
              
            }
            else if (phoneNumber.Length == 10)
            {
                formattedPhoneNumber.Append(phoneNumber.Substring(0, 3) + "-" + phoneNumber.Substring(3, 3) + "-" + phoneNumber.Substring(6, 4));
            }
            else
            {
                formattedPhoneNumber.Append(phoneNumber);
            }
            return formattedPhoneNumber.ToString();
        }
        
    }
}