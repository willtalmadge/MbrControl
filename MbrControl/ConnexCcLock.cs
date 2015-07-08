using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommandInterfaceConexCC;

namespace MbrControl
{
    public class ConnexCcLock
    {
        private static object lockObj = new Object();

        static public void DoJobOnSerial(Func<ConexCC, int, bool> job)
        {
            lock (lockObj)
            {
                var CC = new ConexCC();
                int addr = 1;
                CC.OpenInstrument("COM15");
                job(CC, addr);
                CC.CloseInstrument();
            }
        }
    }
}