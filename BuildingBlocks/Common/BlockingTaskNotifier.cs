// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.BlockingTaskNotifier
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Diagnostics.Tracing;

namespace BuildingBlocks.Common;

public sealed class BlockingTaskNotifier : EventListener
{
  private static readonly Guid Guid = new Guid("2e5dba47-a3d2-4d16-8ee0-6671ffdcd7b5");
  private static readonly Lazy<BlockingTaskNotifier> Listener = new Lazy<BlockingTaskNotifier>((Func<BlockingTaskNotifier>) (() => new BlockingTaskNotifier()));

  private BlockingTaskNotifier()
  {
  }

  public static event EventHandler<string> OnDetection;

  public static void Start()
  {
    BlockingTaskNotifier blockingTaskNotifier = BlockingTaskNotifier.Listener.Value;
  }

  protected override void OnEventSourceCreated(EventSource eventSource)
  {
    if (!(eventSource.Guid == BlockingTaskNotifier.Guid))
      return;
    this.EnableEvents(eventSource, EventLevel.Informational, (EventKeywords) 3);
  }

  protected override void OnEventWritten(EventWrittenEventArgs eventData)
  {
    if (eventData.EventId != 10 || eventData.Payload == null || eventData.Payload.Count <= 3 || !(eventData.Payload[3] is 1))
      return;
    EventHandler<string> onDetection = BlockingTaskNotifier.OnDetection;
    if (onDetection == null)
      return;
    onDetection((object) this, Environment.StackTrace);
  }
}