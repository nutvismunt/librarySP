using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models.JobDTO
{
    public class JobSchedule
    {
        public Type JobType { get; }
        public string CronExpression { get; }
        public JobSchedule (Type jobType, string cronExpression)
        {
            JobType = jobType;
            CronExpression = cronExpression;

        }
    }
}
