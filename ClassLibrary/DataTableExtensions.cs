using System;
using System.Data;

namespace ClassLibrary
{
    public static class Extensions
    {
        public static DateTime TimeStamp0_DateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        public static string TimeStampToString(this string timeStamp, string format = "yyyy/MM/dd HH:mm:ss")
        {
            var unixTimeStamp = double.Parse(timeStamp);
            return TimeStamp0_DateTime.AddSeconds(unixTimeStamp).ToString(format);
        }

        public static void AddTimeColumn(this System.Data.DataTable dt, string dst, string src)
        {
            dt.Columns.Add(dst, typeof(string));
            foreach (DataRow dr in dt.Rows)
            {
                dr[dst] = TimeStampToString(dr[src].ToString());
            }
        }
    }
}
