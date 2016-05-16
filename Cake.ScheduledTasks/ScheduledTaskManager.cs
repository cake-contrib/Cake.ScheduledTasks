/*
The MIT License (MIT)

Copyright (c) 2016 hawker-am

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Cake.ScheduledTasks
{
    /// <summary>
    /// Exposes operations carried out on the Windows Task Scheduler
    /// </summary>
    public class ScheduledTaskManager
    {

        /// <summary>
        /// Starts the specified scheduled task
        /// </summary>
        /// <param name="taskName">Name of the task</param>
        public static void StartScheduledTask(string taskName)
        {
            Console.WriteLine("Starting task " + taskName);

            var output = RunScheduledTaskCommand("/Run /TN \"" + taskName + "\"");

            if (output.Contains("SUCCESS"))
                Console.WriteLine(taskName + " has been started.");
            else
                throw new Exception("Error starting scheduled task " + taskName);
        }

        /// <summary>
        /// Stops the specified scheduled task
        /// </summary>
        /// <param name="taskName">Name of the task</param>
        public static void StopScheduledTask(string taskName)
        {
            Console.WriteLine("Stopping task " + taskName);

            var output = RunScheduledTaskCommand("/End /TN \"" + taskName + "\"");


            if (output.Contains("SUCCESS"))
                Console.WriteLine(taskName + " has been stopped.");
            else
                Console.WriteLine("Error stopping scheduled task " + taskName);
        }

       /// <summary>
       /// Enables or disables the specified scheduled task depending on the enabled param. If the task is running
       /// function waits 30 seconds for it to finish otherwise throws an exception.
       /// </summary>
       /// <param name="taskName">Name of the task</param>
       /// <param name="enabled">If set to true, enables the task otherwise disables it</param>
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


        /// <summary>
        /// Starts all the tasks in the specified task folder
        /// </summary>
        /// <param name="folderName">Name of task folder</param>
        public static void StartScheduledTaskByFolder(string folderName)
        {
            var tasks = GetScheduledTasksByFolder(folderName);

            if (tasks.Count == 0)
                return;

            foreach (var task in tasks)
                StartScheduledTask(task.Key);
        }

        /// <summary>
        /// Stops all the tasks in the specified task folder
        /// </summary>
        /// <param name="folderName">Name of the task folder</param>
        public static void StopScheduledTaskByFolder(string folderName)
        {
            var tasks = GetScheduledTasksByFolder(folderName);

            if (tasks.Count == 0)
                return;

            foreach (var task in tasks)
                StopScheduledTask(task.Key);
        }
        
        /// <summary>
        /// Enables or disables all the tasks in the specified task folder depnding on the enabled param
        /// </summary>
        /// <param name="folderName">Name of the task folder</param>
        /// <param name="enabled">If set to true, enables all tasks otherwise disables them.</param>
        public static void SetScheduledTaskEnabledByFolder(string folderName, bool enabled = true)
        {

            Console.WriteLine((enabled ? "Enabling" : "Disabling") + " all tasks in folder " + folderName);
            var tasks = GetScheduledTasksByFolder(folderName);

            if (tasks.Count == 0)
                return;

            foreach (var task in tasks)
                SetScheduledTaskEnabled(task.Key, enabled);
        }

        /// <summary>
        /// Function to run the windows schtasks command with the specified arguments
        /// </summary>
        /// <param name="arguments">Arguments to run the command with</param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns a dictionary list of all scheduled tasks within a task folder
        /// </summary>
        /// <param name="folderName">Name of the task folder</param>
        /// <returns>Dictionary of task names and their corresponding status</returns>
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

                    try
                    {
                        list.Add(lastTask, false);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error adding key: " + lastTask);
                        throw;
                    }
                    
                }

                if (line.Contains("Status") && line.Contains("Running") && lastTask != string.Empty)
                    list[lastTask] = true;
                else
                    lastTask = string.Empty;

            }
            return list;
        }

        /// <summary>
        /// Gets whether the specified task is running or not
        /// </summary>
        /// <param name="taskName">Name of the scheduled task</param>
        /// <returns>True if the task is running, false if it isn't</returns>
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