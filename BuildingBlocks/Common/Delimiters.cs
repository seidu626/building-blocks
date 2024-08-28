// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.Delimiters
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Common;

public static class Delimiters
{
  public static char[] Comma { get; } = new char[1]{ ',' };

  public static char[] Dot { get; } = new char[1]{ '.' };

  public static char[] SemiColon { get; } = new char[1]
  {
    ';'
  };

  public static char[] Colon { get; } = new char[1]{ ':' };

  public static char[] Space { get; } = new char[1]{ ' ' };

  public static char[] Tab { get; } = new char[1]{ '\t' };

  public static char[] Pipe { get; } = new char[1]{ '|' };
}