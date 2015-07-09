using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MbrControl.Models
{
    public class MbrCalibration
    {
        public const string Calibrating = "Calibrating";
        public const string Calibrated = "Calibrated";
        public const string NotCalibrated = "NotCalibrated";

        public string State { get; set; }
        public double Slope { get; set; }
        public double Intercept { get; set; }
        public double RmsError { get; set; }
        public string CalibrationFile { get; set; }

        public MbrCalibration()
        {
            State = MbrCalibration.NotCalibrated;
            Slope = Double.NaN;
            Intercept = Double.NaN;
            CalibrationFile = "";
        }
        public bool IsCalibrated()
        {
            return State == Calibrated;
        }
        public bool IsCalibrating()
        {
            return State == Calibrating;
        }
    }
}