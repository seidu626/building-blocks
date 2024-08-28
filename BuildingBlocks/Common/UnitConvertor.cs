// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.UnitConverter
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Runtime.CompilerServices;

namespace BuildingBlocks.Common;

public static class UnitConverter
{
  private const double KiloBytes = 1024.0;
  private const double MegaBytes = 1048576.0;
  private const double GigaBytes = 1073741824.0;

  public static string Humanize(double bytes)
  {
    double num = Math.Abs(bytes);
    if (num >= 1073741824.0)
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
      interpolatedStringHandler.AppendFormatted<double>(bytes / 1073741824.0, "#,#.##");
      interpolatedStringHandler.AppendLiteral(" GBytes");
      return interpolatedStringHandler.ToStringAndClear();
    }
    if (num >= 1048576.0)
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
      interpolatedStringHandler.AppendFormatted<double>(bytes / 1048576.0, "#,#.##");
      interpolatedStringHandler.AppendLiteral(" MBytes");
      return interpolatedStringHandler.ToStringAndClear();
    }
    if (num >= 1024.0)
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
      interpolatedStringHandler.AppendFormatted<double>(bytes / 1024.0, "#,#.##");
      interpolatedStringHandler.AppendLiteral(" KBytes");
      return interpolatedStringHandler.ToStringAndClear();
    }
    DefaultInterpolatedStringHandler interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(6, 1);
    interpolatedStringHandler1.AppendFormatted<double>(bytes, "#,#");
    interpolatedStringHandler1.AppendLiteral(" Bytes");
    return interpolatedStringHandler1.ToStringAndClear();
  }

  public static double BytesToMegaBytes(this double bytes) => bytes / 1048576.0;

  public static double BytesToGigaBytes(this double bytes) => bytes / 1048576.0 / 1024.0;

  public static double KiloBytesToMegaBytes(this double kiloBytes) => kiloBytes / 1024.0;

  public static double MegaBytesToGigaBytes(this double megaBytes) => megaBytes / 1024.0;

  public static double MegaBytesToTeraBytes(this double megaBytes) => megaBytes / 1048576.0;

  public static double GigaBytesToMegaBytes(this double gigaBytes) => gigaBytes * 1024.0;

  public static double GigaBytesToTeraBytes(this double gigaBytes) => gigaBytes / 1024.0;

  public static double TeraBytesToMegaBytes(this double teraBytes) => teraBytes * 1048576.0;

  public static double TeraBytesToGigaBytes(this double teraBytes) => teraBytes * 1024.0;
}