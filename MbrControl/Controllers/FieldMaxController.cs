using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace MbrControl.Controllers
{
    public class FieldMaxController
    {
        [DllImport("C:\\Windows\\SysWOW64\\FieldMax2Lib.dll", CharSet = CharSet.Auto)]
        public static extern Int32 fm2LibOpenDriver(Int16 index);
        [DllImport("C:\\Windows\\SysWOW64\\FieldMax2Lib.dll", CharSet = CharSet.Ansi)]
        public static extern UInt16 fm2LibGetSerialNumber(Int32 h, StringBuilder returnBuffer, ref Int16 pSize);
        [DllImport("C:\\Windows\\SysWOW64\\FieldMax2Lib.dll", CharSet = CharSet.Auto)]
        public static extern UInt16 fm2LibCloseDriver(Int32 h);
        [DllImport("C:\\Windows\\SysWOW64\\FieldMax2Lib.dll", CharSet = CharSet.Ansi)]
        public static extern UInt16 fm2LibGetData(Int32 h, byte[] returnBuffer, ref Int16 count);
        [DllImport("C:\\Windows\\SysWOW64\\FieldMax2Lib.dll", CharSet = CharSet.Ansi)]
        public static extern UInt16 fm2LibSync(Int32 h);
        [DllImport("C:\\Windows\\SysWOW64\\FieldMax2Lib.dll", CharSet = CharSet.Ansi)]
        public static extern UInt16 fm2LibPackagedSendReply(Int32 h, String command, StringBuilder returnBuffer, ref Int16 size);

        static public Int32 OpenDriver()
        {
            //TODO: place into a lock
            var h = fm2LibOpenDriver(0);

            StringBuilder SerialNumber = new StringBuilder();
            Int16 i = 16;
            FieldMaxController.fm2LibGetSerialNumber(h, SerialNumber, ref i);

            return h;
        }
        static public float SingleMeasurement()
        {
            Int32 h = OpenDriver();

            fm2LibSync(h);

            byte[] buffer = new byte[64];
            Int16 returnCount = 0;
            while(returnCount == 0)
            {
                Thread.Sleep(50);
                returnCount = 8;
                fm2LibGetData(h, buffer, ref returnCount);
            }
            byte[] floatBuffer = buffer.Take(4).ToArray();
            fm2LibCloseDriver(h);
            return System.BitConverter.ToSingle(floatBuffer, 0);
            
        }

        static public void SetWavelengthCorrection(int wavelength_nm)
        {
            Int32 h = OpenDriver();
            String command = "WOO" + wavelength_nm.ToString();
            Int16 size = 100;
            StringBuilder returnBuffer = new StringBuilder(size);
            fm2LibPackagedSendReply(h, command, returnBuffer, ref size);

            fm2LibCloseDriver(h);
        }
    }
}