// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Persistence.Extensions.ExceptionExtensions
// Assembly: BuildingBlocks.Persistence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 98D21131-3851-47B7-9AEB-AB489CBD4F40
// Assembly location: C:\Users\420919\Repositories\Decompiled\BuildingBlocks.Persistence.dll

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

#nullable disable
namespace BuildingBlocks.Persistence.Extensions
{
  public static class ExceptionExtensions
  {
    public static bool IsUniquenessViolationException(this DbUpdateException exception)
    {
      if (!(((Exception) exception)?.InnerException is SqlException innerException))
        innerException = ((Exception) exception)?.InnerException?.InnerException as SqlException;
      if (innerException == null)
        return false;
      switch (innerException.Number)
      {
        case 547:
        case 2601:
        case 2627:
          return true;
        default:
          return false;
      }
    }

    public static bool IsDeadlockException(this SqlException exception)
    {
      return exception != null && exception.Number == 1205;
    }

    public static bool IsAlreadyAttachedEntityException(this InvalidOperationException exception)
    {
      return exception != null && exception.HResult == -2146233079;
    }
  }
}
