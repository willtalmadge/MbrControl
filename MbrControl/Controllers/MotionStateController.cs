using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CommandInterfaceConexCC;

namespace MbrControl.Controllers
{
    [RoutePrefix("api/motionstate")]
    public class MotionStateController : ApiController
    {
        [Route("")]
        public IHttpActionResult GetState()
        {
            if (IsMoving())
            {
                return Ok("Moving");
            }
            else
            {
                return Ok("Ready to move");
            }

        }
        public static bool IsMoving()
        {
            string errorCode = "";
            string errorString = "";
            string controllerState = "";

            ConnexCcLock.DoJobOnSerial((CC, addr) =>
            {
                CC.TS(addr, out errorCode, out controllerState, out errorString);

                return true;
            });


            if (controllerState == "28")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
