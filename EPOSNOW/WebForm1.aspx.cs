using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Data;
using System.Globalization;
using System.Net;
using Newtonsoft.Json;

namespace EPOSNOW
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        private const string URL = "https://api.eposnowhq.com";
        protected void Page_Load(object sender, EventArgs e)
        {
            //var currentDate = DateTime.Now;
            //var timeBegin = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 09, 01, 01);

            ////var midnightTime = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 00, 00, 00);
            ////var midTicks = midnightTime.TimeOfDay.Ticks;
            //var newTime = timeBegin;
            //for (int i = 0; i < 100; i++)
            //{
            //    newTime = newTime.AddSeconds(1);
            //    var hour = newTime.Hour;
            //    var ticks = newTime.TimeOfDay.Ticks;
            //    if (hour < 9 || ticks == 0)
            //    {
            //        newTime = timeBegin;
            //    }

            //}

        }

        private string SendPostRequest(string value)
        {
            var url = "https://api.eposnowhq.com/api/v4/Transaction";
            var request = WebRequest.Create(url);

            request.Method = "POST";
            const string key = "QzQ5RTJCMDY5RTcyNDc0QjkxOUQ1RkQzOUJCQzk0RTY6M0JGMTIzQkUyNzYxNEQ1NjlCNEJFQTU5NEEzRTRENEY=";
            request.Headers.Add("Authorization", "Basic " + key);
            request.ContentType = "application/json";
            using (var steamWriter = new StreamWriter(request.GetRequestStream()))
            {
                steamWriter.Write(value);
                steamWriter.Flush();
                steamWriter.Close();
                var response = (HttpWebResponse)request.GetResponse();
                using (var steamReader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
                {
                    var resultResponse = steamReader.ReadToEnd();
                    return resultResponse;
                }
            }
        }
        protected void ImportCsv(object sender, EventArgs e)
        {
            string csvPath = Server.MapPath("~/Files/") + Path.GetFileName(FileUpload1.PostedFile.FileName);
            FileUpload1.SaveAs(csvPath);
            StreamReader reader = new StreamReader(File.OpenRead(csvPath));
            int rowCounter = 0;
            string[] dateValues = new string[33];
            var requiredHour = 12;
            var currentDate = DateTime.Now;
            var timeBegin = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, requiredHour, 01, 01);
            //var midnightTime = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 00, 00, 00);
            //var midTicks = midnightTime.TimeOfDay.Ticks;
            var newTime = timeBegin;
            while (!reader.EndOfStream)
            {
                rowCounter++;
                string line = reader.ReadLine();
                if (!String.IsNullOrWhiteSpace(line))
                {
                    if (rowCounter == 1)
                    {
                        dateValues = line.Split(',');
                    }
                    else
                    {
                        string[] values = line.Split(',');

                        int counter = -1;
                        var productID = 0;
                        foreach (var value in values)
                        {
                            //var str = "{" +
                            //              "\"StatusId\":1," +
                            //              "\"DateTime\": \"2019-07-25\"" +
                            //              ",\"TransactionItems\":[" +
                            //              "{" +
                            //                  "\"ProductId\":12425557," +
                            //                  "\"UnitPrice\":2.00," +

                            //                  "\"TaxGroupId\":5556," +
                            //                  "\"Quantity\":1," +
                            //                  "\"DiscountAmount\":0" +
                            //              "}" +
                            //              "]," +
                            //              "\"Tenders\":[" +
                            //              "{" +
                            //                      "\"TenderTypeId\":5691," +
                            //                      "\"Amount\":2.00," +
                            //                      "\"Changegiven\":0" +
                            //                  "}" +
                            //                  "]" +
                            //              "}";
                            var str = "{" +
"\"StatusId\":1," +
"\"DateTime\": \"{date}\"" +
",\"TransactionItems\":[" +
"{" +
"\"ProductId\":{productid}," +
"\"UnitPrice\":{amount}," +

"\"TaxGroupId\":5556," +
"\"Quantity\":1," +
"\"DiscountAmount\":0" +
"}" +
"]," +
"\"Tenders\":[" +
"{" +
"\"TenderTypeId\":5691," +
"\"Amount\":{amount}," +
"\"Changegiven\":0" +
"}" +
"]" +
"}";
                            counter++;
                            if (counter == 0)
                            {
                                productID = Int32.Parse(value);
                            }
                            else
                            {
                                var hour = newTime.Hour;
                                var ticks = newTime.TimeOfDay.Ticks;
                                if (hour < requiredHour || ticks == 0)
                                {
                                    newTime = timeBegin;
                                }
                                var amount = decimal.Parse(value.TrimStart('$'));
                                var date = DateTime.Parse(dateValues[counter] + " " + newTime.Hour + ":" + newTime.Minute + ":" + newTime.Second);
                                str = str.Replace("{productid}", productID.ToString());
                                str = str.Replace("{date}", date.ToString());
                                str = str.Replace("{amount}", amount.ToString(CultureInfo.InvariantCulture));
                                var result = SendPostRequest(str);
                                newTime = newTime.AddSeconds(1);

                            }
                        }
                    }
                }
            }
        }
    }
}