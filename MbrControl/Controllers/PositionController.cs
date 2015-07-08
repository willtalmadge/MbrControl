using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

using CommandInterfaceConexCC;

namespace MbrControl.Controllers
{
    [RoutePrefix("api/position")]
    public class PositionController : ApiController
    {
        [Route("")]
        public double GetPosition()
        {
            double position=0.0;
            string error="";
            ConnexCcLock.DoJobOnSerial((CC, addr) =>
            {
                CC.TP(addr, out position, out error);
                return true;
            });
            return position;
        }
        [Route("{position_mm:double}")]
        [HttpGet]
        public IHttpActionResult SetPosition(double position_mm)
        {
            //It is necessary to put a trailing slash after the argument in the call
            //for example, api/position/5.1/
            if (WebApiApplication.calibration.IsCalibrating())
            {
                return Ok("Ignored, calibration in process");
            }

            if(TrySetPosition(position_mm))
            {
                return Ok("Moving");
            }
            else
            {
                return Ok("Ignored, motor not ready");
            }


        }
        public static bool TrySetPosition(double position_mm)
        {
            string errorCode = "";
            string errorString = "";
            string controllerState = "";

            ConnexCcLock.DoJobOnSerial((CC, addr) =>
            {
                CC.TS(addr, out errorCode, out controllerState, out errorString);
                return true;
            });


            if ((controllerState == "32") | (controllerState == "33") | (controllerState == "34"))
            {
                double position = 0.0;

                ConnexCcLock.DoJobOnSerial((CC, addr) =>
                {
                    CC.PA_Set(addr, position_mm, out errorString);
                    CC.TP(addr, out position, out errorString);
                    return true;
                });

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
