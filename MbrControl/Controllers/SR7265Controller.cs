using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NationalInstruments.NI4882;
using System.Threading;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace MbrControl.Controllers
{
    [RoutePrefix("api/sr7265")]
    public class SR7265Controller : ApiController
    {
        public const byte CommandCompleteMask = 1;
        public const byte DataAvailableMask = 128;

        [Route("average_adc/{n:int}/{samples:int}")]
        [HttpGet]
        public IHttpActionResult GetAverageADC(int n, int samples)
        {
            return Ok(AverageADC(n, samples));
        }
        static public double AverageADC(int n, int samples)
        {

            Device device = new Device(0, 12, 0);
            device.SetEndOnEndOfString = true;
            device.SetEndOnWrite = true;

            var V = Vector<double>.Build;
            var data = V.Dense(samples);
            for (int i = 0; i < samples; i++)
            {
                device.Write("ADC.2");
                string response = device.ReadString();
                data[i] = Convert.ToDouble(response);
            }

            device.Dispose();
            return data.Average();
        }
        static public void SetDAC(int n, double v)
        {
            Device device = new Device(0, 12, 0);
            device.SetEndOnEndOfString = true;
            device.SetEndOnWrite = true;

            device.Write(String.Format("DAC. {0} {1}", n.ToString(), v.ToString("F4")));

            device.Dispose();
        }
        static private string ReplaceCommonEscapeSequences(string s)
        {
            return s.Replace("\\n", "\n").Replace("\\r", "\r");
        }

        static private string InsertCommonEscapeSequences(string s)
        {
            return s.Replace("\n", "\\n").Replace("\r", "\\r");
        }

    }
}