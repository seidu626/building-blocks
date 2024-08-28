// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.Interfaces.IConfigReader
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Common.Interfaces;

public interface IConfigReader
{
  FileInfo ConfigFile { get; }

  Dictionary<string, string> Settings { get; }

  bool TryRead(string key, out IDictionary<string, string> values);

  bool TryRead(string key, out string value);

  bool TryRead(string key, out short value);

  bool TryRead(string key, out int value);

  bool TryRead(string key, out long value);

  bool TryRead(string key, out ushort value);

  bool TryRead(string key, out uint value);

  bool TryRead(string key, out ulong value);

  bool TryRead(string key, out float value);

  bool TryRead(string key, out double value);

  bool TryRead(string key, out Decimal value);

  bool TryRead(string key, out bool value);

  bool TryReadStringAsCSV(string key, string separator, out string[] value);

  bool TryGetTicks(string key, out TimeSpan value);

  bool TryGetMilliseconds(string key, out TimeSpan value);

  bool TryGetSeconds(string key, out TimeSpan value);

  bool TryGetMinutes(string key, out TimeSpan value);

  bool TryGetHours(string key, out TimeSpan value);

  bool TryGetDays(string key, out TimeSpan value);

  bool TryGetWeeks(string key, out TimeSpan value);

  bool TryRead(string key, string formatSpecifier, out DateTime value);

  bool TryRead(string key, out FileInfo value);

  bool TryRead(string key, out DirectoryInfo value);

  bool TryRead(string key, out Uri value);
}