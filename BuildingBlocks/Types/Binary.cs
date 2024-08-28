namespace BuildingBlocks.Types;

public class Binary
{
    /// <summary>
    /// Convert an object to a Byte Array, using Protobuf.
    /// </summary>
    public static byte[] Serialize(object obj)
    {
        if (obj == null)
            return null;

        using var stream = new MemoryStream();

        System.Text.Json.JsonSerializer.Serialize(stream, obj);

        return stream.ToArray();
    }

    /// <summary>
    /// Convert a byte array to an Object of T, using Protobuf.
    /// </summary>
    public static T? Deserialize<T>(byte[] arrBytes)
    {
        using var stream = new MemoryStream();

        // Ensure that our stream is at the beginning.
        stream.Write(arrBytes, 0, arrBytes.Length);
        stream.Seek(0, SeekOrigin.Begin);

        return System.Text.Json.JsonSerializer.Deserialize<T>(stream);
    }
}