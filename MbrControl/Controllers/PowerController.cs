using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Web.Hosting;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace MbrControl.Controllers
{
    [RoutePrefix("api/power")]
    public class PowerController : ApiController
    {
        static bool BisectionControl(Action<double> variable_set, Func<double> process_read, 
            double set_point, double low, double high, double tolerance)
        {
            double step = (high - low) / 2.0;
            double variable_target = low + step;
            while(true)
            {
                
                variable_set(variable_target);
                double new_value = process_read();
                if (Math.Abs(new_value-set_point) <= tolerance)
                {
                    //Found a suitable variable
                    return true;
                }
                else
                {
                    if(new_value < set_point)
                    {
                        //Variable is too low, step up
                        step = Math.Abs(step / 2.0);
                    }
                    else if (new_value > set_point)
                    {
                        //Variable is too high, step down
                        step = -1.0 * Math.Abs(step / 2.0);
                    }
                }
                if (Math.Abs(step) <= (high-low)/1000.0)
                {
                    return false;
                }
                variable_target = variable_target + step;
            }
        }

        [Route("set_power_bisection/{power_watts:double}")]
        [HttpGet]
        public IHttpActionResult SetPowerBisection(double power_watts)
        {
            //Be sure to add a trailing slash when calling this.
            Action<double> variable_set = (double v) =>
            {
                //Set noise eater modulation voltage
                SR7265Controller.SetDAC(1, v);
                Thread.Sleep(2000);
            };
            Func<double> process_read = () =>
            {
                //Get the actual power reading
                return FieldMaxController.SingleMeasurement();
            };
            double w = WavelengthController.GetLightfieldWavelength();
            FieldMaxController.SetWavelengthCorrection((int)Math.Floor(w));
            Thread.Sleep(2000);
            if (BisectionControl(variable_set, process_read, power_watts, 0.0, 2.5, 0.5e-6))
            {
                return Ok("Power set point found within tolerance");
            }
            else
            {
                return Ok("Failed to find set point");
            }
        }
    }
}