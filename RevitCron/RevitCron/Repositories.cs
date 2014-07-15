using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DougKlassen.Revit.Cron.Models;

namespace DougKlassen.Revit.Cron.Repositories
{
    public interface IrCronTasksRepo
    {
        public rCronTask GetRCronTask();

        public void PutRCronTask(rCronTask);
    }
}
