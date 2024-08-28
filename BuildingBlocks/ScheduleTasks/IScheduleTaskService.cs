﻿// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.ScheduleTasks.IScheduleTaskService
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.ScheduleTasks;

public interface IScheduleTaskService
{
  Task DeleteTaskAsync(ScheduleTask task);

  Task<ScheduleTask> GetTaskByIdAsync(int taskId);

  Task<ScheduleTask> GetTaskByTypeAsync(string type);

  Task<IList<ScheduleTask>> GetAllTasksAsync(bool showHidden = false);

  Task InsertTaskAsync(ScheduleTask task);

  Task UpdateTaskAsync(ScheduleTask task);
}