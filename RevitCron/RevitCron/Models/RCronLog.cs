using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DougKlassen.Revit.Cron.Models
{
    class RCronLog //todo: use singleton pattern
    {
        private StringBuilder logText;

        public String Text
        {
            get
            {
                return logText.ToString();
            }
        }

        public RCronLog()
        {
            logText = new StringBuilder(String.Empty);
        }

        public RCronLog(String text)
        {
            logText = new StringBuilder(text);
        }

        public void AppendLine(String text)
        {
            logText.AppendLine(text);
        }

        public void AppendLine(String text, params String[] args)
        {
            logText.AppendLine(String.Format(text, args));
        }
    }
}
