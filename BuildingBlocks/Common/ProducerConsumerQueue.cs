// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.ProducerConsumerQueue`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using System.Collections.Concurrent;

namespace BuildingBlocks.Common;

public sealed class ProducerConsumerQueue<T> : IDisposable
{
  private readonly 
#nullable disable
    BlockingCollection<T> _queue;

  public ProducerConsumerQueue(Action<T> consumer, uint maxConcurrencyLevel)
    : this(consumer, maxConcurrencyLevel, -1)
  {
  }

  public ProducerConsumerQueue(
    Action<T> consumer,
    uint maxConcurrencyLevel,
    uint boundedCapacity)
    : this(consumer, maxConcurrencyLevel, (int) boundedCapacity)
  {
  }

  private ProducerConsumerQueue(
    Action<T> consumer,
    uint maxConcurrencyLevel,
    int boundedCapacity)
  {
    Ensure.NotNull<Action<T>>(consumer, nameof (consumer));
    Ensure.That(maxConcurrencyLevel > 0U, "maxConcurrencyLevel should be greater than zero.");
    Ensure.That(boundedCapacity != 0, "boundedCapacity should be greater than zero.");
    this._queue = boundedCapacity < 0 ? new BlockingCollection<T>() : new BlockingCollection<T>(boundedCapacity);
    this.MaximumConcurrencyLevel = maxConcurrencyLevel;
    this.Completion = this.Configure(consumer);
  }

  public uint MaximumConcurrencyLevel { get; }

  public int Capacity => this._queue.BoundedCapacity;

  public uint PendingCount => (uint) this._queue.Count;

  public T[] PendingItems => this._queue.ToArray();

  public Task<bool> Completion { get; }

  public event EventHandler<ProducerConsumerQueueException> OnException;

  public void Add(T item) => this.Add(item, CancellationToken.None);

  public void Add(T item, CancellationToken cancellationToken)
  {
    try
    {
      this._queue.Add(item, cancellationToken);
    }
    catch (Exception ex)
    {
      EventHandler<ProducerConsumerQueueException> onException = this.OnException;
      if (onException == null)
        return;
      onException((object) this, new ProducerConsumerQueueException("Exception occurred when adding item.", ex));
    }
  }

  public bool TryAdd(T item) => this.TryAdd(item, TimeSpan.Zero, CancellationToken.None);

  public bool TryAdd(T item, TimeSpan timeout)
  {
    return this.TryAdd(item, timeout, CancellationToken.None);
  }

  public bool TryAdd(T item, TimeSpan timeout, CancellationToken cancellationToken)
  {
    try
    {
      return this._queue.TryAdd(item, (int) timeout.TotalMilliseconds, cancellationToken);
    }
    catch (Exception ex)
    {
      EventHandler<ProducerConsumerQueueException> onException = this.OnException;
      if (onException != null)
        onException((object) this, new ProducerConsumerQueueException("Exception occurred when adding item.", ex));
      return false;
    }
  }

  public void CompleteAdding() => this._queue.CompleteAdding();

  public void Dispose() => this._queue?.Dispose();

  private Task<bool> Configure(Action<T> consumer)
  {
    Task task1 = Task.Factory.StartNew((Action) (() =>
    {
      OrderablePartitioner<T> source = Partitioner.Create<T>(this._queue.GetConsumingEnumerable(), EnumerablePartitionerOptions.NoBuffering);
      ParallelOptions parallelOptions = new ParallelOptions();
      parallelOptions.MaxDegreeOfParallelism = (int) this.MaximumConcurrencyLevel;
      Action<T> body = new Action<T>(WrapConsumer);
      Parallel.ForEach<T>((Partitioner<T>) source, parallelOptions, body);
    }), CancellationToken.None, (TaskCreationOptions) 10, TaskScheduler.Default);
    TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
    task1.ContinueWith((Action<Task>) (task => tcs.SetResult(false)), (TaskContinuationOptions) 589824);
    task1.ContinueWith((Action<Task>) (task => tcs.SetResult(true)), (TaskContinuationOptions) 917504);
    return tcs.Task;

    void WrapConsumer(T x)
    {
      try
      {
        consumer(x);
      }
      catch (Exception ex)
      {
        EventHandler<ProducerConsumerQueueException> onException = this.OnException;
        if (onException == null)
          return;
        onException((object) this, new ProducerConsumerQueueException("Exception occurred.", ex));
      }
    }
  }
}