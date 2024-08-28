// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.ProcessHelper
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using System.Diagnostics;

namespace BuildingBlocks.Common;

public static class ProcessHelper
{
  public static 
#nullable disable
    Task<ProcessExecutionResult> ExecuteAsync(
      ProcessStartInfo processInfo,
      CancellationToken cToken = default (CancellationToken))
  {
    Ensure.NotNull<ProcessStartInfo>(processInfo, nameof (processInfo));
    processInfo.UseShellExecute = false;
    processInfo.CreateNoWindow = true;
    processInfo.RedirectStandardOutput = true;
    processInfo.RedirectStandardError = true;
    Process process = new Process()
    {
      EnableRaisingEvents = true,
      StartInfo = processInfo
    };
    TaskCompletionSource<ProcessExecutionResult> tcs = new TaskCompletionSource<ProcessExecutionResult>();
    List<string> standardOutput = new List<string>();
    List<string> standardError = new List<string>();
    TaskCompletionSource<string[]> standardOutputResults = new TaskCompletionSource<string[]>();
    process.OutputDataReceived += (DataReceivedEventHandler) ((sender, args) =>
    {
      if (args.Data != null)
        standardOutput.Add(args.Data);
      else
        standardOutputResults.SetResult(standardOutput.ToArray());
    });
    TaskCompletionSource<string[]> standardErrorResults = new TaskCompletionSource<string[]>();
    process.ErrorDataReceived += (DataReceivedEventHandler) ((sender, args) =>
    {
      if (args.Data != null)
        standardError.Add(args.Data);
      else
        standardErrorResults.SetResult(standardError.ToArray());
    });
    process.Exited += (EventHandler) ((sender, args) => tcs.TrySetResult(new ProcessExecutionResult(process, standardOutputResults.Task.Result, standardErrorResults.Task.Result)));
    using (cToken.Register((Action) (() =>
           {
             tcs.TrySetCanceled();
             try
             {
               if (process.HasExited)
                 return;
               process.Kill();
             }
             catch (InvalidOperationException ex)
             {
             }
           })))
    {
      cToken.ThrowIfCancellationRequested();
      if (!process.Start())
        tcs.TrySetException((Exception) new InvalidOperationException("Failed to start the process."));
      process.BeginOutputReadLine();
      process.BeginErrorReadLine();
      return tcs.Task;
    }
  }

  public static Task<ProcessExecutionResult> ExecuteAsync(
    string processPath,
    string args,
    CancellationToken cToken = default (CancellationToken))
  {
    Ensure.NotNullOrEmptyOrWhiteSpace(processPath);
    return ProcessHelper.ExecuteAsync(new ProcessStartInfo(processPath, args), cToken);
  }

  public static Task<ProcessExecutionResult> ExecuteAsync(
    FileInfo processPath,
    string args,
    CancellationToken cToken = default (CancellationToken))
  {
    Ensure.NotNull<FileInfo>(processPath, nameof (processPath));
    return ProcessHelper.ExecuteAsync(new ProcessStartInfo(((FileSystemInfo) processPath).FullName, args), cToken);
  }
}