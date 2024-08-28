// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.AssemblyExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;
using BuildingBlocks.Common;

namespace BuildingBlocks.Extensions;

public static class AssemblyExtensions
{
  public static string GetFrameworkVersion(this Assembly assembly)
  {
    Ensure.NotNull<Assembly>(assembly, nameof (assembly));
    TargetFrameworkAttribute frameworkAttribute = assembly.GetCustomAttributes(true).OfType<TargetFrameworkAttribute>().FirstOrDefault<TargetFrameworkAttribute>();
    return frameworkAttribute == null ? ".NET 2, 3 or 3.5" : frameworkAttribute.FrameworkName;
  }

  public static DirectoryInfo GetAssemblyLocation(this Assembly assembly)
  {
    Ensure.NotNull<Assembly>(assembly, nameof (assembly));
    return new DirectoryInfo(Path.GetDirectoryName(assembly.Location) ?? throw new InvalidOperationException());
  }

  public static DirectoryInfo GetAssemblyCodeBase(this Assembly assembly)
  {
    Ensure.NotNull<Assembly>(assembly, nameof (assembly));
    return new DirectoryInfo(Path.GetDirectoryName(new Uri(assembly.Location).LocalPath));
  }

  public static bool IsOptimized(this Assembly assembly)
  {
    Ensure.NotNull<Assembly>(assembly, nameof (assembly));
    object[] customAttributes = assembly.GetCustomAttributes(typeof (DebuggableAttribute), false);
    if (customAttributes.Length == 0)
      return true;
    foreach (Attribute attribute in customAttributes)
    {
      if (attribute is DebuggableAttribute debuggableAttribute)
        return !debuggableAttribute.IsJITOptimizerDisabled;
    }
    return false;
  }

  public static bool Is32Bit(this Assembly assembly)
  {
    Ensure.NotNull<Assembly>(assembly, nameof (assembly));
    string location = assembly.Location;
    if (location.IsNullOrEmptyOrWhiteSpace())
      location = assembly.Location;
    Uri uri = new Uri(location);
    Ensure.That(uri.IsFile, "Assembly location is not a file.");
    return AssemblyName.GetAssemblyName(uri.LocalPath).ProcessorArchitecture == ProcessorArchitecture.X86;
  }

  public static bool IsAssemblyLargeAddressAware(this Assembly assembly)
  {
    Ensure.NotNull<Assembly>(assembly, nameof (assembly));
    return ApplicationHelper.IsLargeAddressAware(assembly.Location);
  }
}