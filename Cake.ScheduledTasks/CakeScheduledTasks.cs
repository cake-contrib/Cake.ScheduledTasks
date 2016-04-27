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
using System.Linq;
using Cake.Core;
using Cake.Core.Annotations;


namespace Cake.ScheduledTasks
{
    public static class CakeScheduledTasks
    {
        [CakeMethodAlias]
        public static void StartScheduledTask(this ICakeContext context, string taskName)
        {
            var output = RunScheduledTaskCommand("/Run /TN \"" + taskName + "\"");

            if(output.Contains("SUCCESS"))
                Console.WriteLine(taskName + " has been started.");
            else
                throw new Exception("Error starting scheduled task " + taskName);
        }

        [CakeMethodAlias]
        public static void StopScheduledTask(this ICakeContext context, string taskName)
        {
            var output = RunScheduledTaskCommand("/End /TN \"" + taskName + "\"");

         
            if(output.Contains("SUCCESS"))
                Console.WriteLine(taskName + " has been stopped.");
            else
                Console.WriteLine("Error stopping scheduled task " + taskName);
        }

        [CakeMethodAlias]
        public static void SetScheduledTaskEnabled(this ICakeContext context, string taskName, bool enabled = true)
        {
            if (!enabled)
            {
                var endOutput = RunScheduledTaskCommand("/End /TN \"" + taskName + "\"");

                if(endOutput.Contains("SUCCESS"))
                    Console.WriteLine(taskName + " has been stopped");
                else
                    throw new Exception("Error disabling scheduled task " + taskName);
            }

            var output = RunScheduledTaskCommand("/Change /TN \"" + taskName + "\" /" + (enabled ? "ENABLE" : "DISABLE"));

            if(output.Contains("SUCCESS"))
                Console.WriteLine(taskName + " has been " + (enabled ? "enabled" : "disabled") + ".");
            else
                throw new Exception("Error " + (enabled ? "enabling" : "disabling") + " scheduled task" + taskName);
                
        }


        [CakeMethodAlias]
        public static void StartScheduledTaskByFolder(this ICakeContext context, string folderName)
        {
            var tasks = GetScheduledTasksByFolder(folderName);

            if (tasks.Count == 0)
                return;

            foreach (var task in tasks)
                StartScheduledTask(context, task);
        }

        [CakeMethodAlias]
        public static void StopScheduledTaskByFolder(this ICakeContext context, string folderName)
        {
            var tasks = GetScheduledTasksByFolder(folderName);

            if (tasks.Count == 0)
                return;

            foreach (var task in tasks)
                StopScheduledTask(context, task);
        }


        [CakeMethodAlias]
        public static void SetScheduledTaskEnabledByFolder(this ICakeContext context, string folderName, bool enabled = true)
        {
            var tasks = GetScheduledTasksByFolder(folderName);

            if (tasks.Count == 0)
                return;

            foreach (var task in tasks)
                SetScheduledTaskEnabled(context, task, enabled);
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

        private static List<string> GetScheduledTasksByFolder(string folderName)
        {
            var output = RunScheduledTaskCommand("/query /fo list");

            var lines = output.Split('\n');

            return (from line 
                    in lines
                    where line.Contains("TaskName:") && line.Contains(folderName)
                    select line.Substring(10).Trim())
                    .ToList();
        }
    }


}
