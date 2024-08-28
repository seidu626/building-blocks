// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.ProcessExecutionResult
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Diagnostics;

namespace BuildingBlocks.Common;

public sealed class ProcessExecutionResult : IDisposable
{
  private readonly Process _process;

  internal ProcessExecutionResult(
    Process process,
    string[] standardOutput,
    string[] standardError)
  {
    this._process = Ensure.NotNull<Process>(process, nameof (process));
    this.PID = this._process.Id;
    this.ExecutionTime = this._process.ExitTime - this._process.StartTime;
    this.StandardOutput = Ensure.NotNull<string[]>(standardOutput, nameof (standardOutput));
    this.StandardError = Ensure.NotNull<string[]>(standardError, nameof (standardError));
  }

  public int PID { get; }

  public TimeSpan ExecutionTime { get; }

  public string[] StandardOutput { get; }

  public string[] StandardError { get; }

  public T ReadProcessInfo<T>(Func<Process, T> selector) => selector(this._process);

  public void Dispose() => this._process?.Dispose();
}