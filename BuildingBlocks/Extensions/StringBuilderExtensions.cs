// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.StringBuilderExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Text;
using BuildingBlocks.Common;

namespace BuildingBlocks.Extensions;

public static class StringBuilderExtensions
{
  public static StringBuilder AppendMultiple(this StringBuilder builder, string text, uint count)
  {
    Ensure.NotNull<StringBuilder>(builder, nameof (builder));
    Ensure.NotNull<string>(text, nameof (text));
    for (int index = 0; (long) index < (long) count; ++index)
      builder.Append(text);
    return builder;
  }

  public static StringBuilder AppendMultiple(
    this StringBuilder builder,
    char character,
    uint count)
  {
    Ensure.NotNull<StringBuilder>(builder, nameof (builder));
    for (int index = 0; (long) index < (long) count; ++index)
      builder.Append(character);
    return builder;
  }

  public static StringBuilder AppendSpace(this StringBuilder builder, uint count)
  {
    return builder.AppendMultiple(' ', count);
  }
}