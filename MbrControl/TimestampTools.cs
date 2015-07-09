using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MbrControl
{
    public class TimestampTools
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
        public static string CalibrationPathNow(string filename, string extension)
        {
            return String.Format("{0}{1}{2}_{3}.{4}",
                base_path,
                PathForDay(),
                filename,
                Timestamp(),
                extension
                );
        }
    }
}