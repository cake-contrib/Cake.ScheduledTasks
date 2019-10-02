
# Cake.ScheduledTasks

Windows task scheduler addin for Cake.

## CakeAliases

Wrapper around ScheduledTaskManager

## ScheduledTaskManager

Exposes operations carried out on the Windows Task Scheduler

### M:Cake.ScheduledTasks.GetScheduledTasksByFolder(folderName)

Returns a dictionary list of all scheduled tasks within a task folder

| Name | Description |
| ---- | ----------- |
| folderName | *System.String*<br>Name of the task folder |

#### Returns

Dictionary of task names and their corresponding status

### M:Cake.ScheduledTasks.GetScheduledTaskStatus(taskName)

Gets whether the specified task is running or not

| Name | Description |
| ---- | ----------- |
| taskName | *System.String*<br>Name of the scheduled task |

#### Returns

True if the task is running, false if it isn't

### M:Cake.ScheduledTasks.RunScheduledTaskCommand(arguments)

Function to run the windows schtasks command with the specified arguments

| Name | Description |
| ---- | ----------- |
| arguments | *System.String*<br>Arguments to run the command with |

#### Returns

### M:Cake.ScheduledTasks.SetScheduledTaskEnabled(taskName, enabled)

Enables or disables the specified scheduled task depending on the enabled param. If the task is running function waits 30 seconds for it to finish otherwise throws an exception.

| Name | Description |
| ---- | ----------- |
| taskName | *System.String*<br>Name of the task |
| enabled | *System.Boolean*<br>If set to true, enables the task otherwise disables it |

### M:Cake.ScheduledTasks.SetScheduledTaskEnabledByFolder(folderName, enabled)

Enables or disables all the tasks in the specified task folder depnding on the enabled param

| Name | Description |
| ---- | ----------- |
| folderName | *System.String*<br>Name of the task folder |
| enabled | *System.Boolean*<br>If set to true, enables all tasks otherwise disables them. |

### M:Cake.ScheduledTasks.StartScheduledTask(taskName)

Starts the specified scheduled task

| Name | Description |
| ---- | ----------- |
| taskName | *System.String*<br>Name of the task |

### M:Cake.ScheduledTasks.StartScheduledTaskByFolder(folderName)

Starts all the tasks in the specified task folder

| Name | Description |
| ---- | ----------- |
| folderName | *System.String*<br>Name of task folder |

### M:Cake.ScheduledTasks.StopScheduledTask(taskName)

Stops the specified scheduled task

| Name | Description |
| ---- | ----------- |
| taskName | *System.String*<br>Name of the task |

### M:Cake.ScheduledTasks.StopScheduledTaskByFolder(folderName)

Stops all the tasks in the specified task folder

| Name | Description |
| ---- | ----------- |
| folderName | *System.String*<br>Name of the task folder |
