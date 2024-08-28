// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Helpers.IDateTimeHelper
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Collections.ObjectModel;

namespace BuildingBlocks.Helpers;

public interface IDateTimeHelper
{
  ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones();

  Task<DateTime> ConvertToUserTimeAsync(DateTime dt);

  Task<DateTime> ConvertToUserTimeAsync(DateTime dt, DateTimeKind sourceDateTimeKind);

  DateTime ConvertToUserTime(
    DateTime dt,
    TimeZoneInfo sourceTimeZone,
    TimeZoneInfo destinationTimeZone);

  DateTime ConvertToUtcTime(DateTime dt);

  DateTime ConvertToUtcTime(DateTime dt, DateTimeKind sourceDateTimeKind);

  DateTime ConvertToUtcTime(DateTime dt, TimeZoneInfo sourceTimeZone);

  Task<TimeZoneInfo> GetCurrentTimeZoneAsync();

  TimeZoneInfo DefaultTimeZone { get; }
}