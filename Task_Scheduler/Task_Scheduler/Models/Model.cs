using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Task_Scheduler
{
    public enum TaskStates
    {
        ToDo,
        InProgress,
        ForReview,
        Completed,
        OnHold,
        Canceled,
    }

    public class Model
    {
        public const string sqlDbConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\chamb\OneDrive\Desktop\Task_Scheduler\Task_Scheduler\Task_Scheduler\Task_Scheduler\Data\TaskSchedulerDB.mdf;Integrated Security=True;Connect Timeout=30";
        public static List<string> GetTaskStageNames()
        {
            List<string> returnValues = new List<string>();

            foreach (var item in Enum.GetNames(typeof(TaskStates)))
            {
                returnValues.Add(item.ToString());
            }
            
            return returnValues;
        }
    }
}
