using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.Hosting;
using MbrControl.Models;

namespace MbrControl.Controllers
{
    [RoutePrefix("api/wavelength")]
    public class WavelengthController : ApiController
    {
        static string base_path = "C:\\Users\\labuser\\Dropbox\\Data";
        static double calibration_start = 10.0;
        static double calibration_end = 20.0;
        static int calibration_points = 10;

        string PathForDay()
        {
            DateTime date = DateTime.Now;
            return String.Format("\\{0}\\{1}\\{2}\\",
                date.Year.ToString("D4"),
                date.Month.ToString("D2"),
                date.Day.ToString("D2"));

        }
        string Timestamp()
        {
            DateTime date = DateTime.Now;
            return String.Format("{0}{1}{2}",
                date.Hour.ToString("D2"),
                date.Minute.ToString("D2"),
                date.Second.ToString("D2")
                );
        }
        string CalibrationPathNow(string filename, string extension)
        {
            return String.Format("{0}{1}{2}_{3}.{4}",
                base_path,
                PathForDay(),
                filename,
                Timestamp(),
                extension
                );
        }
        [Route("{targetWavelength_nm:double}")]
        [HttpGet]
        public IHttpActionResult GotoWavelength(double targetWavelength_nm)
        {
            if (WebApiApplication.calibration.IsCalibrated()) {
                double target_z = (targetWavelength_nm - WebApiApplication.calibration.Intercept)/WebApiApplication.calibration.Slope;
                if (PositionController.TrySetPosition(target_z))
                {
                    return Ok("Moving to wavelength");
                }
                else
                {
                    return Ok("Motor not ready");
                }
            }
            else
            {
                return Ok("Not calibrated");
            }
        }
        [Route("calibrate")]
        [HttpGet]
        public IHttpActionResult Calibrate()
        {
            WebApiApplication.calibration.State = MbrCalibration.Calibrating;
            string path = CalibrationPathNow("mbr_calibration", "csv");

            var V = Vector<double>.Build;

            var z = V.Dense(calibration_points+1);
            var w = V.Dense(calibration_points+1);

            Action<CancellationToken> asyncTask = (cancellationToken) =>
                {
                    using (StreamWriter outfile = new StreamWriter(path))
                    {
                        outfile.WriteLine("{0}, {1}", "Displacement (mm)", "Wavelength (nm)");
                        outfile.Flush();
                        double delta_z = (calibration_end - calibration_start) / calibration_points;
                        for (int i = 0; i < calibration_points+1; i++)
                        {
                            double target = calibration_start + delta_z * i;
                            PositionController.TrySetPosition(target);
                    
                            //Block while moving
                            while(MotionStateController.IsMoving())
                            {
                                Thread.Sleep(1000);
                            }
                            Thread.Sleep(100);

                            WebResponse response = WebRequest.Create("http://192.168.1.87:5000/wavelength_at_max").GetResponse();
                            StreamReader reader = new StreamReader(response.GetResponseStream());
                            string wavelength = reader.ReadToEnd();
                            
                            outfile.WriteLine("{0}, {1}", target.ToString("F5"), wavelength);
                            outfile.Flush();
                            z[i] = target;

                            w[i] = Convert.ToDouble(wavelength);
                        }
                        Tuple<double, double> fit = Fit.Line(z.ToArray(), w.ToArray());
                        var model = z*fit.Item2 + fit.Item1;
                        var dif = model - w;
                        double rms = Math.Sqrt(dif.DotProduct(dif)/(calibration_points+1));
                        WebApiApplication.calibration.Slope = fit.Item2;
                        WebApiApplication.calibration.Intercept = fit.Item1;
                        WebApiApplication.calibration.State = MbrCalibration.Calibrated;
                        WebApiApplication.calibration.RmsError = rms;
                    }
                };

            HostingEnvironment.QueueBackgroundWorkItem(asyncTask);

            return Ok("Calibrating");
        }
        [Route("is_calibrated")]
        [HttpGet]
        public IHttpActionResult IsCalibrated()
        {
            return Ok(WebApiApplication.calibration.IsCalibrated());
        }
        [Route("calibration")]
        [HttpGet]
        public IHttpActionResult GetCalibration()
        {
            return Ok(WebApiApplication.calibration);
        }
    }
}
