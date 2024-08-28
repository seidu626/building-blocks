// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Helpers.DateTimeHelper
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using System.Collections.ObjectModel;

namespace BuildingBlocks.Helpers;

public class DateTimeHelper : IDateTimeHelper
{
  private readonly 
#nullable disable
    DateTimeSettings _dateTimeSettings;

  public DateTimeHelper(DateTimeSettings dateTimeSettings)
  {
    this._dateTimeSettings = dateTimeSettings;
  }

  protected virtual TimeZoneInfo FindTimeZoneById(string id)
  {
    return TimeZoneInfo.FindSystemTimeZoneById(id);
  }

  public virtual ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones()
  {
    return TimeZoneInfo.GetSystemTimeZones();
  }

  public virtual async Task<DateTime> ConvertToUserTimeAsync(DateTime dt)
  {
    return await this.ConvertToUserTimeAsync(dt, dt.Kind);
  }

  public virtual async Task<DateTime> ConvertToUserTimeAsync(
    DateTime dt,
    DateTimeKind sourceDateTimeKind)
  {
    dt = DateTime.SpecifyKind(dt, sourceDateTimeKind);
    return sourceDateTimeKind == DateTimeKind.Local && TimeZoneInfo.Local.IsInvalidTime(dt) ? dt : TimeZoneInfo.ConvertTime(dt, await this.GetCurrentTimeZoneAsync());
  }

  public virtual DateTime ConvertToUserTime(
    DateTime dt,
    TimeZoneInfo sourceTimeZone,
    TimeZoneInfo destinationTimeZone)
  {
    return sourceTimeZone.IsInvalidTime(dt) ? dt : TimeZoneInfo.ConvertTime(dt, sourceTimeZone, destinationTimeZone);
  }

  public virtual DateTime ConvertToUtcTime(DateTime dt) => this.ConvertToUtcTime(dt, dt.Kind);

  public virtual DateTime ConvertToUtcTime(DateTime dt, DateTimeKind sourceDateTimeKind)
  {
    dt = DateTime.SpecifyKind(dt, sourceDateTimeKind);
    return sourceDateTimeKind == DateTimeKind.Local && TimeZoneInfo.Local.IsInvalidTime(dt) ? dt : TimeZoneInfo.ConvertTimeToUtc(dt);
  }

  public virtual DateTime ConvertToUtcTime(DateTime dt, TimeZoneInfo sourceTimeZone)
  {
    return sourceTimeZone.IsInvalidTime(dt) ? dt : TimeZoneInfo.ConvertTimeToUtc(dt, sourceTimeZone);
  }

  public virtual async Task<TimeZoneInfo> GetCurrentTimeZoneAsync() => this.DefaultTimeZone;

  public virtual TimeZoneInfo DefaultTimeZone
  {
    get
    {
      TimeZoneInfo timeZoneInfo = (TimeZoneInfo) null;
      try
      {
        if (!string.IsNullOrEmpty(this._dateTimeSettings.DefaultTimeZoneId))
          timeZoneInfo = this.FindTimeZoneById(this._dateTimeSettings.DefaultTimeZoneId);
      }
      catch (Exception ex)
      {
      }
      return timeZoneInfo ?? TimeZoneInfo.Local;
    }
  }
}