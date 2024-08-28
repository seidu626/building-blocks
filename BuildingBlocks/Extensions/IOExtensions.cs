// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.IOExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using BuildingBlocks.Exceptions;

namespace BuildingBlocks.Extensions;

public static class IOExtensions
{
  public static bool WaitForUnlock(this FileInfo file, int timeoutMs = 1000)
  {
    Guard.AgainstNull((object) file, nameof (file));
    TimeSpan timeSpan = TimeSpan.FromMilliseconds(50.0);
    double num = Math.Floor((double) timeoutMs / timeSpan.TotalMilliseconds);
    try
    {
      for (int index = 0; (double) index < num; ++index)
      {
        if (!file.IsFileLocked())
          return true;
        Task.Delay(timeSpan).Wait();
      }
      return false;
    }
    catch
    {
      return false;
    }
  }

  public static bool IsFileLocked(this FileInfo file)
  {
    if (file == null)
      return false;
    FileStream fileStream = (FileStream) null;
    try
    {
      fileStream = file.Open((FileMode) 3, (FileAccess) 3, (FileShare) 0);
    }
    catch (IOException ex)
    {
      return true;
    }
    finally
    {
      ((Stream) fileStream)?.Close();
    }
    return false;
  }
}