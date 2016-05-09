using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Cake.ScheduledTasks
{
    public class ScheduledTaskManager
    {
        public static void StartScheduledTask(string taskName)
        {
            Console.WriteLine("Starting task " + taskName);

            var output = RunScheduledTaskCommand("/Run /TN \"" + taskName + "\"");

            if (output.Contains("SUCCESS"))
                Console.WriteLine(taskName + " has been started.");
            else
                throw new Exception("Error starting scheduled task " + taskName);
        }

        public static void StopScheduledTask(string taskName)
        {
            Console.WriteLine("Stopping task " + taskName);

            var output = RunScheduledTaskCommand("/End /TN \"" + taskName + "\"");


            if (output.Contains("SUCCESS"))
                Console.WriteLine(taskName + " has been stopped.");
            else
                Console.WriteLine("Error stopping scheduled task " + taskName);
        }

       
        public static void SetScheduledTaskEnabled(string taskName, bool enabled = true)
        {
            Console.WriteLine((enabled ? "Enabling" : "Disabling") + " task " + taskName);

            if (!enabled)
            {
                var tries = 0;
                const int maximumTries = 30;

                // Wait for the scheduled task to finish, check once every second for 30 seconds
                while (GetScheduledTaskStatus(taskName) && tries < maximumTries)
                {
                    ++tries;
                    Thread.Sleep(1000);
                }

                if(tries == 30)
                    throw new Exception("Error disabling scheduled task " + taskName + " - did not finish running within " + maximumTries + " seconds");
                
                Console.WriteLine(taskName + " has been stopped");
            }

            var output = RunScheduledTaskCommand("/Change /TN \"" + taskName + "\" /" + (enabled ? "ENABLE" : "DISABLE"));

            if (output.Contains("SUCCESS"))
                Console.WriteLine(taskName + " has been " + (enabled ? "enabled" : "disabled") + ".");
            else
                throw new Exception("Error " + (enabled ? "enabling" : "disabling") + " scheduled task" + taskName);

        }


       
        public static void StartScheduledTaskByFolder(string folderName)
        {
            var tasks = GetScheduledTasksByFolder(folderName);

            if (tasks.Count == 0)
                return;

            foreach (var task in tasks)
                StartScheduledTask(task.Key);
        }

       
        public static void StopScheduledTaskByFolder(string folderName)
        {
            var tasks = GetScheduledTasksByFolder(folderName);

            if (tasks.Count == 0)
                return;

            foreach (var task in tasks)
                StopScheduledTask(task.Key);
        }
        
        public static void SetScheduledTaskEnabledByFolder(string folderName, bool enabled = true)
        {

            Console.WriteLine((enabled ? "Enabling" : "Disabling") + " all tasks in folder " + folderName);
            var tasks = GetScheduledTasksByFolder(folderName);

            if (tasks.Count == 0)
                return;

            foreach (var task in tasks)
                SetScheduledTaskEnabled(task.Key, enabled);
        }

        private static string RunScheduledTaskCommand(string arguments)
        {
            string output;

            using (var process = Process.Start(new ProcessStartInfo
            {
                FileName = "schtasks",
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false
            }))
            {
                process.Start();


                output = process.StandardOutput.ReadToEnd();

                process.WaitForExit();
            }

            return output;
        }

        private static Dictionary<string, bool> GetScheduledTasksByFolder(string folderName)
        {
            var output = RunScheduledTaskCommand("/query /fo list");

            var lines = output.Split('\n');

            var list = new Dictionary<string, bool>();

            var lastTask = "";
            foreach (var line in lines)
            {
                if (line.Contains("TaskName:") && line.Contains(folderName))
                {
                    lastTask = line.Substring(10).Trim();
                    list.Add(lastTask, false);
                }

                if (line.Contains("Status") && line.Contains("Running") && lastTask != string.Empty)
                    list[lastTask] = true;
                else
                    lastTask = string.Empty;

            }
            return list;
        }

        private static bool GetScheduledTaskStatus(string taskName)
        {
            var output = RunScheduledTaskCommand("/query /tn \"" + taskName + "\" /fo list");

            var lines = output.Split('\n');


            var foundName = false;

            foreach (var line in lines)
            {
                if (line.Contains("TaskName:") && line.Contains(taskName))
                {
                    foundName = true;
                    continue;
                }


                if (line.Contains("Status") && line.Contains("Running") && foundName)
                    return true;  
            }

            return false;
        }
    }
}