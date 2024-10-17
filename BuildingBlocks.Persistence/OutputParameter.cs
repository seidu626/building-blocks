namespace BuildingBlocks.Persistence;

public class OutputParameter<T>
{
    public T Value { get; set; }
    public static implicit operator T(OutputParameter<T> output) => output.Value;
    public static implicit operator OutputParameter<T>(T value) => new OutputParameter<T> { Value = value };
}
