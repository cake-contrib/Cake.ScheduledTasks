using System;
using NUnit;
using NUnit.Framework;
using Cake.ScheduledTasks;

namespace Cake.ScheduledTasks.Test
{
    [TestFixture]
    public class TaskManagerTest
    {
        [Test]
        public void ShouldWaitForRunningTask()
        {
            ScheduledTaskManager.SetScheduledTaskEnabled(@"\Test\awdasdas", false);
        }
    }
}
