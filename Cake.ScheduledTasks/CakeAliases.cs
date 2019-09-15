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


using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.ScheduledTasks
{
    /// <summary>
    /// Wrapper around ScheduledTaskManager
    /// </summary>
    public static class CakeAliases
    {
        [CakeMethodAlias]
        public static void StartScheduledTask(this ICakeContext context, string taskName) => ScheduledTaskManager.StartScheduledTask(taskName);

        [CakeMethodAlias]
        public static void StopScheduledTask(this ICakeContext context, string taskName) => ScheduledTaskManager.StopScheduledTask(taskName);

        [CakeMethodAlias]
        public static void SetScheduledTaskEnabled(this ICakeContext context, string taskName, bool enabled = true) => ScheduledTaskManager.SetScheduledTaskEnabled(taskName, enabled);

        [CakeMethodAlias]
        public static void StartScheduledTaskByFolder(this ICakeContext context, string folderName) => ScheduledTaskManager.StartScheduledTaskByFolder(folderName);

        [CakeMethodAlias]
        public static void StopScheduledTaskByFolder(this ICakeContext context, string folderName) => ScheduledTaskManager.StopScheduledTaskByFolder(folderName);

        [CakeMethodAlias]
        public static void SetScheduledTaskEnabledByFolder(this ICakeContext context, string folderName, bool enabled = true) => ScheduledTaskManager.SetScheduledTaskEnabledByFolder(folderName, enabled);
    }


}
