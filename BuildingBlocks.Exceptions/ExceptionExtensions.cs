// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Exceptions.ExceptionExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BuildingBlocks.Exceptions;

public static class ExceptionExtensions
{
  public static Stacky PrettifyStackTrace(this Exception ex)
  {
    Stacky stacky = new Stacky()
    {
      ExceptionMessage = ex.Message,
      ExceptionType = ex.GetType().ToString()
    };
    ExceptionExtensions.ParseStackTrace(ex, stacky);
    return stacky;
  }

  public static Stacky PrettifyStackTraceWithParameters(this Exception ex, params object[] args)
  {
    Stacky stacky = new Stacky()
    {
      ExceptionMessage = ex.Message,
      ExceptionType = ex.GetType().ToString(),
      MethodArguments = ((IEnumerable<ParameterInfo>) new StackTrace(ex, false).GetFrame(0).GetMethod().GetParameters()).Select<ParameterInfo, string>((Func<ParameterInfo, string>) (p => p.Name)).Zip((IEnumerable<object>) args, (Name, Value) => new
      {
        Name = Name,
        Value = Value
      }).ToDictionary(x => x.Name, x => x.Value)
    };
    ExceptionExtensions.ParseStackTrace(ex, stacky);
    return stacky;
  }

  private static void ParseStackTrace(Exception ex, Stacky stacky)
  {
    if (string.IsNullOrEmpty(ex.StackTrace))
      return;
    string[] strArray1 = ex.StackTrace.Split(Constants.Stacky.NewLineArray, StringSplitOptions.RemoveEmptyEntries);
    int num = 0;
    DefaultInterpolatedStringHandler interpolatedStringHandler;
    for (int index = 0; index < strArray1.Length; ++index)
    {
      if (index == 0)
      {
        string[] strArray2 = strArray1[index].Split(Constants.Stacky.InArray, StringSplitOptions.RemoveEmptyEntries);
        if (strArray2.Length != 0)
          stacky.Method = strArray2[0].Split(Constants.Stacky.AtArray, StringSplitOptions.RemoveEmptyEntries)[0];
        string[] strArray3;
        if (strArray2.Length <= 1)
          strArray3 = new string[2]
          {
            strArray2[0],
            string.Empty
          };
        else
          strArray3 = strArray2[1].Split(Constants.Stacky.LineArray, StringSplitOptions.RemoveEmptyEntries);
        string[] strArray4 = strArray3;
        stacky.FileName = strArray4[0].Contains(".cs") ? strArray4[0] : "SystemException";
        int result;
        if (int.TryParse(strArray4[1], out result))
          stacky.Line = result;
        List<string> stackLines = stacky.StackLines;
        interpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 2);
        interpolatedStringHandler.AppendFormatted<int>(index);
        interpolatedStringHandler.AppendLiteral(": ");
        interpolatedStringHandler.AppendFormatted(stacky.Method);
        string stringAndClear = interpolatedStringHandler.ToStringAndClear();
        stackLines.Add(stringAndClear);
      }
      else if (strArray1[index].StartsWith("---"))
      {
        ++num;
        stacky.StackLines.Add(string.Format("=== Sub-stack {0} ===", (object) num));
      }
      else
      {
        List<string> stackLines = stacky.StackLines;
        string str1 = strArray1[index];
        interpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 1);
        interpolatedStringHandler.AppendFormatted<int>(index);
        interpolatedStringHandler.AppendLiteral(": @ ");
        string stringAndClear = interpolatedStringHandler.ToStringAndClear();
        string str2 = str1.Replace("   at ", stringAndClear);
        stackLines.Add(str2);
      }
    }
  }
}