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
        public double K0 { get; set; }
        public double K1 { get; set; }
        public double K2 { get; set; }
        public double RmsError { get; set; }
        public string CalibrationFile { get; set; }

        public MbrCalibration()
        {
            State = MbrCalibration.NotCalibrated;
            K0 = Double.NaN;
            K1 = Double.NaN;
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
        public double WavelengthForPosition(double z)
        {
            return z * z * K2 + z * K1 + K0;
        }
        public double PositionForWavelength(double w)
        {
            double a = Math.Sqrt(K1 * K1 - 4 * K0 * K2 + 4 * K2 * w);
            return (-1.0*K1 - a) / (2 * K2);
        }
    }
}