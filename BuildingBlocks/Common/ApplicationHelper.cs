// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.ApplicationHelper
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BuildingBlocks.Common;

public static class ApplicationHelper
{
  public static TimeSpan GetProcessStartupDuration()
  {
    return DateTime.Now.Subtract(Process.GetCurrentProcess().StartTime);
  }

  public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

  public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

  public static bool IsOSX => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

  public static OSPlatform OSPlatform => ApplicationHelper.GetOSPlatform();

  public static bool IsProcessLargeAddressAware()
  {
    using (Process currentProcess = Process.GetCurrentProcess())
      return ApplicationHelper.IsLargeAddressAware(currentProcess.MainModule?.FileName);
  }

  internal static bool IsLargeAddressAware(string file)
  {
    Ensure.NotNullOrEmptyOrWhiteSpace(file);
    FileInfo fileInfo = new FileInfo(file);
    Ensure.Exists(fileInfo);
    using (FileStream fileStream = File.Open(((FileSystemInfo) fileInfo).FullName, (FileMode) 3, (FileAccess) 1, (FileShare) 3))
    {
      using (BinaryReader binaryReader = new BinaryReader((Stream) fileStream))
      {
        if (binaryReader.ReadInt16() != (short) 23117)
          return false;
        binaryReader.BaseStream.Position = 60L;
        int num = binaryReader.ReadInt32();
        binaryReader.BaseStream.Position = (long) num;
        if (binaryReader.ReadInt32() != 17744)
          return false;
        binaryReader.BaseStream.Position += 18L;
        return ((int) binaryReader.ReadInt16() & 32) == 32;
      }
    }
  }

  private static OSPlatform GetOSPlatform()
  {
    if (ApplicationHelper.IsWindows)
      return OSPlatform.Windows;
    if (ApplicationHelper.IsLinux)
      return OSPlatform.Linux;
    return ApplicationHelper.IsOSX ? OSPlatform.OSX : OSPlatform.Create("UNKNOWN");
  }
}