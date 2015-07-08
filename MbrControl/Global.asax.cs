using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using MbrControl.Models;

namespace MbrControl
{
    public class WebApiApplication : System.Web.HttpApplication
    {

        static public MbrCalibration calibration;

        protected void Application_Start()
        {
            calibration = new MbrCalibration();

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
