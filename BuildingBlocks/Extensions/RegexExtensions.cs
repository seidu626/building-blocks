// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.RegexExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using System.Text;
using System.Text.RegularExpressions;

namespace BuildingBlocks.Extensions;

public static class RegexExtensions
{
  public static 
#nullable disable
    string ReplaceGroup(this Regex regex, string input, string groupName, string replacement)
  {
    return regex.ReplaceGroupInternal(input, replacement, (Func<Match, Group>) (m => m.Groups[groupName]));
  }

  public static string ReplaceGroup(
    this Regex regex,
    string input,
    int groupNum,
    string replacement)
  {
    return regex.ReplaceGroupInternal(input, replacement, (Func<Match, Group>) (m => m.Groups[groupNum]));
  }

  private static string ReplaceGroupInternal(
    this Regex regex,
    string input,
    string replacement,
    Func<Match, Group> groupGetter)
  {
    return regex.Replace(input, (MatchEvaluator) (match =>
    {
      Group group = groupGetter(match);
      StringBuilder stringBuilder = new StringBuilder();
      int startIndex = 0;
      foreach (Capture capture in group.Captures.Cast<Capture>())
      {
        int num = capture.Index + capture.Length - match.Index;
        int length = capture.Index - match.Index - startIndex;
        stringBuilder.Append(match.Value.Substring(startIndex, length));
        stringBuilder.Append(replacement);
        startIndex = num;
      }
      stringBuilder.Append(match.Value.Substring(startIndex));
      return stringBuilder.ToString();
    }));
  }
}