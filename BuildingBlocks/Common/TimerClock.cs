// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.TimerClock
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using BuildingBlocks.Common.Interfaces;

namespace BuildingBlocks.Common;

public sealed class TimerClock : ITimerClock, IDisposable
{
  private DateTimeOffset _nextSchedule;

  private DateTimeOffset NextSchedule
  {
    get
    {
      Thread.MemoryBarrier();
      return this._nextSchedule;
    }
    set
    {
      this._nextSchedule = value;
      Thread.MemoryBarrier();
    }
  }

  public 
#nullable disable
    IClock Clock { get; }

  public event EventHandler<EventArgs> Tick;

  public TimeSpan TickInterval { get; }

  public TimerClock(TimeSpan interval)
  {
    this.Clock = (IClock) Common.Clock.Instance;
    this.TickInterval = interval;
    this.NextSchedule = DateTimeOffset.MaxValue;
    TimerClock.InternalTimer.Tick += new EventHandler<EventArgs>(this.OnTick);
  }

  public bool Enabled
  {
    get => this.NextSchedule != DateTimeOffset.MaxValue;
    set
    {
      this.NextSchedule = value ? this.Clock.Now.Add(this.TickInterval) : DateTimeOffset.MaxValue;
    }
  }

  public void Dispose()
  {
    this.NextSchedule = DateTimeOffset.MaxValue;
    TimerClock.InternalTimer.Tick -= new EventHandler<EventArgs>(this.OnTick);
  }

  public override string ToString()
  {
    return "Interval: " + this.TickInterval.ToString() + " - DateTime: " + this.Clock.Now.ToString("yyyy-MM-dd HH:mm.ss.fff");
  }

  private void OnTick(object sender, EventArgs args)
  {
    if (this.NextSchedule == DateTimeOffset.MaxValue)
      return;
    DateTimeOffset now = this.Clock.Now;
    if (now < this.NextSchedule)
      return;
    this.NextSchedule = now.Add(this.TickInterval);
    ThreadPool.UnsafeQueueUserWorkItem((WaitCallback) (_ =>
    {
      EventHandler<EventArgs> tick = this.Tick;
      if (tick == null)
        return;
      tick((object) this, EventArgs.Empty);
    }), (object) null);
  }

  private static class InternalTimer
  {
    internal static event EventHandler<EventArgs> Tick;

    static InternalTimer()
    {
      AsyncFlowControl asyncFlowControl = ExecutionContext.SuppressFlow();
      new Thread((ThreadStart) (() =>
      {
        SpinWait spinWait = new SpinWait();
        while (true)
        {
          EventHandler<EventArgs> tick;
          do
          {
            spinWait.SpinOnce();
            tick = TimerClock.InternalTimer.Tick;
          }
          while (tick == null);
          tick((object) null, EventArgs.Empty);
        }
      }))
      {
        Priority = ((ThreadPriority) 4),
        Name = nameof (InternalTimer),
        IsBackground = true
      }.Start();
      asyncFlowControl.Undo();
    }
  }
}