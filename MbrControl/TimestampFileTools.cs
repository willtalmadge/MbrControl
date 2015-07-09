using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace MbrControl
{
    public class TimestampFileTools
    {
        static string base_path = "C:\\Users\\labuser\\Dropbox\\Data";

        public static string PathForDay()
        {
            DateTime date = DateTime.Now;
            return String.Format("\\{0}\\{1}\\{2}\\",
                date.Year.ToString("D4"),
                date.Month.ToString("D2"),
                date.Day.ToString("D2"));

        }
        public static string Timestamp()
        {
            DateTime date = DateTime.Now;
            return String.Format("{0}{1}{2}",
                date.Hour.ToString("D2"),
                date.Minute.ToString("D2"),
                date.Second.ToString("D2")
                );
        }
        public static string FilePathNow(string filename, string extension)
        {
            return String.Format("{0}{1}{2}_{3}.{4}",
                base_path,
                PathForDay(),
                filename,
                Timestamp(),
                extension
                );
        }
        public static void LogStringArray(string fullPath, string[] values)
        {
            using (StreamWriter outfile = new StreamWriter(fullPath, true))
            {
                string header_string = string.Join(",", values);
                outfile.WriteLine(header_string);
                outfile.Flush();
            }
        }
        public static void LogDoubleArray(string fullPath, double[] values)
        {
            String[] s = new String[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                s[i] = values[i].ToString("e6");
            }
            LogStringArray(fullPath, s);
        }
    }
}