// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Persistence.DataSettingsDefaults
// Assembly: BuildingBlocks.Persistence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 98D21131-3851-47B7-9AEB-AB489CBD4F40
// Assembly location: C:\Users\420919\Repositories\Decompiled\BuildingBlocks.Persistence.dll

#nullable disable
namespace BuildingBlocks.Persistence
{
  public static class DataSettingsDefaults
  {
    public static string ObsoleteFilePath => "~/App_Data/Settings.txt";

    public static string FilePath => "~/dataSettings.json";

    public static string EnvironmentVariableDataConnectionString
    {
      get => "dataSettings__DataConnectionString";
    }

    public static string EnvironmentVariableDataProvider => "dataSettings__DataProvider";

    public static string EnvironmentVariableSQLCommandTimeout => "dataSettings__SQLCommandTimeout";
  }
}
