using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;

namespace BuildingBlocks.Utilities;

public static class ParallelAsync
{
    /// <summary>
    /// https://medium.com/@alex.puiu/parallel-foreach-async-in-c-36756f8ebe62
    /// </summary>
    /// <param name="source"></param>
    /// <param name="body"></param>
    /// <param name="maxDegreeOfParallelism"></param>
    /// <param name="scheduler"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task AsyncParallelForEach<T>(this IAsyncEnumerable<T> source, Func<T, Task> body, int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded, TaskScheduler scheduler = null)
    {
        var options = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism
        };
        if (scheduler != null)
            options.TaskScheduler = scheduler;

        var block = new ActionBlock<T>(body, options);

        await foreach (var item in source)
            block.Post(item);

        block.Complete();
        await block.Completion;
    }
        
    public static Task ParallelForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> funcBody, int maxDoP = 4)
    {
        async Task AwaitPartition(IEnumerator<T> partition)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    await Task.Yield();
                    await funcBody(partition.Current)
                        .ConfigureAwait(false);
                }
            }
        }

        return Task.WhenAll(
            Partitioner
                .Create(source)
                .GetPartitions(maxDoP)
                .AsParallel()
                .Select(AwaitPartition));
    }

    public static Task ParallelForEachAsync<T1, T2>(this IEnumerable<T1> source, Func<T1, T2, Task> funcBody, T2 secondInput, int maxDoP = 4)
    {
        async Task AwaitPartition(IEnumerator<T1> partition)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    await Task.Yield();
                    await funcBody(partition.Current, secondInput).ConfigureAwait(false);
                }
            }
        }

        return Task.WhenAll(
            Partitioner
                .Create(source)
                .GetPartitions(maxDoP)
                .AsParallel()
                .Select(AwaitPartition));
    }

    public static Task ParallelForEachAsync<T1, T2, T3>(this IEnumerable<T1> source, Func<T1, T2, T3, Task> funcBody, T2 secondInput, T3 thirdInput, int maxDoP = 4)
    {
        async Task AwaitPartition(IEnumerator<T1> partition)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    await Task.Yield();
                    await funcBody(partition.Current, secondInput, thirdInput).ConfigureAwait(false);
                }
            }
        }

        return Task.WhenAll(
            Partitioner
                .Create(source)
                .GetPartitions(maxDoP)
                .AsParallel()
                .Select(AwaitPartition));
    }

    public static Task ParallelForEachAsync<T1, T2, T3, T4>(this IEnumerable<T1> source, Func<T1, T2, T3, T4, Task> funcBody, T2 secondInput, T3 thirdInput, T4 fourthInput, int maxDoP = 4)
    {
        async Task AwaitPartition(IEnumerator<T1> partition)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    await Task.Yield();
                    await funcBody(partition.Current, secondInput, thirdInput, fourthInput).ConfigureAwait(false);
                }
            }
        }

        return Task.WhenAll(
            Partitioner
                .Create(source)
                .GetPartitions(maxDoP)
                .AsParallel()
                .Select(AwaitPartition));
    }
}