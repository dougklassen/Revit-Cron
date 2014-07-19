using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DougKlassen.Revit.Cron
{
    public sealed class RCronLog
    {
        private static readonly RCronLog instance = new RCronLog();

        private StringBuilder logText;

        private RCronLog()
        {
            logText = new StringBuilder(String.Empty);
        }

        public static RCronLog Instance
        {
            get
            {
                return instance;
            }
        }

        public StringBuilder LogText
        {
            get
            {
                return logText;
            }
        }
    }
}
