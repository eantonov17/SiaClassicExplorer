//# SiaClassicExplorer
//** An Explorer for SiaClassic blockchain in C# and .Net Framework **
//* Copyright(C) 2018-2019 Eugene Antonov*
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of version 3 of the GNU General Public License
//as published by the Free Software Foundation.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.If not, see<https://www.gnu.org/licenses/>.

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
