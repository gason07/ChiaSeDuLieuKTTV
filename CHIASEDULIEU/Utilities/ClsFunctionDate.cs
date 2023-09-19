using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHIASEDULIEU.Utilities
{
    public class ClsFunctionDate
    {
        /// <summary>
        /// Hàm format kiểu Datetime về dạng dd/mm/yyyy
        /// </summary>
        /// <param name="dateTime1"></param>
        /// <returns></returns>
        public static string ShowDateGrid(DateTime? dateTime1)
        {
            string str = string.Empty;
            if (dateTime1 != null)
            {
                var dateTime = (DateTime)dateTime1;
                str = dateTime.ToString("dd/MM/yyyy");
            }
            if (str == "01/01/0001")
            {
                return "";
            }
            else
            {
                return str;
            }
        }
        // dinh dang sDate = dd/mm/yyyy
        /// <summary>
        /// Hàm chuyển chuỗi nhập dd/mm/yyyy; MM/yyyy; yyyy => Datetime
        /// </summary>
        /// <param name="sDate"> chuỗi nhập</param>
        /// <param name="sStartEnd"> "S" or "E"</param>
        /// <returns></returns>
        public static DateTime ConvertDate(string sDate, string sStartEnd)
        {
            var datetime = new DateTime();
            if (sDate != "")
            {
                if (sDate.Length >= 8 && sDate.Length <= 10) // dd/MM/yyyy; d/M/yyyy
                {
                    // sDate = sDate.Substring(0, 10);
                    string[] s = sDate.Split('/');
                    int Year = int.Parse(s[2]);
                    int Mont = int.Parse(s[1]);
                    int Day = int.Parse(s[0]);
                    datetime = new DateTime(Year, Mont, Day);
                }
                if (sDate.Length == 6 || sDate.Length == 7) // nhập MM/yyyy , M/yyyy
                {
                    string[] s = sDate.Split('/');
                    int Year = int.Parse(s[1]);
                    int Mont = int.Parse(s[0]);
                    if (sStartEnd == "S") // StartDate
                    {
                        datetime = new DateTime(Year, Mont, 1);
                    }
                    if (sStartEnd == "E") // EndDate
                    {
                        int lastDayOfMonth = DateTime.DaysInMonth(Year, Mont);
                        datetime = new DateTime(Year, Mont, lastDayOfMonth);
                    }
                }
                if (sDate.Length == 4) // Nhập yyyy
                {
                    int Year = int.Parse(sDate);
                    if (sStartEnd == "S") // StartDate
                    {
                        datetime = new DateTime(Year, 1, 1);
                    }
                    if (sStartEnd == "E") // EndDate
                    {
                        int lastDayOfMonth = DateTime.DaysInMonth(Year, 12);
                        datetime = new DateTime(Year, 12, lastDayOfMonth); // Ngày cuối cùng của năm
                    }
                }
                return datetime;
            }
            else
                return datetime;
        }

        public static DateTime ConvertDate(string sDate)
        {
            var datetime = new DateTime();
            if (sDate != "")
            {
                string[] s = sDate.Split('/');
                int Year = int.Parse(s[2]);
                int Mont = int.Parse(s[1]);
                int Day = int.Parse(s[0]);
                datetime = new DateTime(Year, Mont, Day);
                return datetime;
            }
            else
                return datetime;
        }
    }
}