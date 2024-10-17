// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Persistence.DataSettingsManager
// Assembly: BuildingBlocks.Persistence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 98D21131-3851-47B7-9AEB-AB489CBD4F40
// Assembly location: C:\Users\420919\Repositories\Decompiled\BuildingBlocks.Persistence.dll

using BuildingBlocks.Infrastructure;
using BuildingBlocks.Utilities;
using BuildingBlocks.Utilities.Threading;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Data;

#nullable enable
namespace BuildingBlocks.Persistence
{
  public static class DataSettingsManager
  {
    private static bool? _databaseIsInstalled;
    private static readonly 
    #nullable disable
    ReaderWriterLockSlim s_rwLock = new ReaderWriterLockSlim();
    private static DataSettings s_current = (DataSettings) null;
    private static readonly Func<DataSettings> s_settingsFactory = (Func<DataSettings>) (() => new DataSettings());
    private static bool s_TestMode = false;

    private static DataSettings LoadDataSettingsFromOldFile(string data)
    {
      DataSettings dataSettings = new DataSettings();
      using (StringReader stringReader = new StringReader(data))
      {
        string str1;
        while ((str1 = ((TextReader) stringReader).ReadLine()) != null)
        {
          int length = str1.IndexOf(':');
          if (length != -1)
          {
            string key = str1.Substring(0, length).Trim();
            string str2 = str1;
            int startIndex = length + 1;
            string s = str2.Substring(startIndex, str2.Length - startIndex).Trim();
            switch (key)
            {
              case "DataProvider":
                DataProviderType result1;
                dataSettings.DataProvider = Enum.TryParse<DataProviderType>(s, true, out result1) ? result1 : DataProviderType.Unknown;
                continue;
              case "DataConnectionString":
                dataSettings.ConnectionString = s;
                continue;
              case "SQLCommandTimeout":
                int result2;
                dataSettings.SQLCommandTimeout = new int?(int.TryParse(s, out result2) ? result2 : -1);
                continue;
              default:
                dataSettings.RawDataSettings.Add(key, s);
                continue;
            }
          }
        }
        return dataSettings;
      }
    }

    public static DataSettings Current
    {
      get
      {
        using (LockExtensions.GetUpgradeableReadLock(DataSettingsManager.s_rwLock))
        {
          if (Singleton<DataSettings>.Instance != null)
            DataSettingsManager.s_current = Singleton<DataSettings>.Instance;
          if (DataSettingsManager.s_current != null)
            return DataSettingsManager.s_current;
          using (LockExtensions.GetWriteLock(DataSettingsManager.s_rwLock))
          {
            DataSettingsManager.s_current = DataSettingsManager.s_settingsFactory();
            DataSettingsManager.s_current = DataSettingsManager.LoadSettings(reloadSettings: true);
          }
        }
        return DataSettingsManager.s_current;
      }
    }

    public static async Task<DataSettings> LoadSettingsAsync(
      string filePath = null,
      bool reloadSettings = false,
      IFileProvider fileProvider = null)
    {
      if (!reloadSettings && Singleton<DataSettings>.Instance != null)
        return Singleton<DataSettings>.Instance;
      if (fileProvider == null)
        fileProvider = CommonHelper.DefaultFileProvider;
      if (filePath == null)
        filePath = fileProvider.MapPath(DataSettingsDefaults.FilePath);
      if (!fileProvider.FileExists(filePath))
      {
        filePath = fileProvider.MapPath(DataSettingsDefaults.ObsoleteFilePath);
        if (!fileProvider.FileExists(filePath))
          return new DataSettings();
        DataSettings dataSettings_old = DataSettingsManager.LoadDataSettingsFromOldFile(await fileProvider.ReadAllTextAsync(filePath, Encoding.UTF8));
        await DataSettingsManager.SaveSettingsAsync(dataSettings_old, fileProvider);
        fileProvider.DeleteFile(filePath);
        Singleton<DataSettings>.Instance = dataSettings_old;
        return Singleton<DataSettings>.Instance;
      }
      string str = await fileProvider.ReadAllTextAsync(filePath, Encoding.UTF8);
      if (string.IsNullOrEmpty(str))
        return new DataSettings();
      DataSettings dataSettings = JsonConvert.DeserializeObject<DataSettings>(str);
      string environmentVariable1 = Environment.GetEnvironmentVariable(DataSettingsDefaults.EnvironmentVariableDataConnectionString);
      string environmentVariable2 = Environment.GetEnvironmentVariable(DataSettingsDefaults.EnvironmentVariableDataProvider);
      string environmentVariable3 = Environment.GetEnvironmentVariable(DataSettingsDefaults.EnvironmentVariableSQLCommandTimeout);
      if (!string.IsNullOrEmpty(environmentVariable1))
        dataSettings.ConnectionString = environmentVariable1;
      if (!string.IsNullOrEmpty(environmentVariable2))
        dataSettings.DataProvider = JsonConvert.DeserializeObject<DataProviderType>(environmentVariable2);
      int result;
      if (!string.IsNullOrEmpty(environmentVariable3) && int.TryParse(environmentVariable3, out result))
        dataSettings.SQLCommandTimeout = new int?(result);
      Singleton<DataSettings>.Instance = dataSettings;
      return Singleton<DataSettings>.Instance;
    }

    public static DataSettings LoadSettings(
      string filePath = null,
      bool reloadSettings = false,
      IFileProvider fileProvider = null)
    {
      if (!reloadSettings && Singleton<DataSettings>.Instance != null)
        return Singleton<DataSettings>.Instance;
      if (fileProvider == null)
        fileProvider = CommonHelper.DefaultFileProvider;
      if (filePath == null)
        filePath = fileProvider.MapPath(DataSettingsDefaults.FilePath);
      if (!fileProvider.FileExists(filePath))
      {
        filePath = fileProvider.MapPath(DataSettingsDefaults.ObsoleteFilePath);
        if (!fileProvider.FileExists(filePath))
          return new DataSettings();
        DataSettings settings = DataSettingsManager.LoadDataSettingsFromOldFile(fileProvider.ReadAllText(filePath, Encoding.UTF8));
        DataSettingsManager.SaveSettings(settings, fileProvider);
        fileProvider.DeleteFile(filePath);
        Singleton<DataSettings>.Instance = settings;
        return Singleton<DataSettings>.Instance;
      }
      string str = fileProvider.ReadAllText(filePath, Encoding.UTF8);
      if (string.IsNullOrEmpty(str))
        return new DataSettings();
      DataSettings dataSettings = JsonConvert.DeserializeObject<DataSettings>(str);
      string environmentVariable1 = Environment.GetEnvironmentVariable(DataSettingsDefaults.EnvironmentVariableDataConnectionString);
      string environmentVariable2 = Environment.GetEnvironmentVariable(DataSettingsDefaults.EnvironmentVariableDataProvider);
      string environmentVariable3 = Environment.GetEnvironmentVariable(DataSettingsDefaults.EnvironmentVariableSQLCommandTimeout);
      if (!string.IsNullOrEmpty(environmentVariable1))
        dataSettings.ConnectionString = environmentVariable1;
      if (!string.IsNullOrEmpty(environmentVariable2))
        dataSettings.DataProvider = JsonConvert.DeserializeObject<DataProviderType>(environmentVariable2);
      int result;
      if (!string.IsNullOrEmpty(environmentVariable3) && int.TryParse(environmentVariable3, out result))
        dataSettings.SQLCommandTimeout = new int?(result);
      Singleton<DataSettings>.Instance = dataSettings;
      return Singleton<DataSettings>.Instance;
    }

    public static async Task SaveSettingsAsync(DataSettings settings, IFileProvider fileProvider = null)
    {
      Singleton<DataSettings>.Instance = settings ?? throw new ArgumentNullException(nameof (settings));
      if (fileProvider == null)
        fileProvider = CommonHelper.DefaultFileProvider;
      string str1 = fileProvider.MapPath(DataSettingsDefaults.FilePath);
      fileProvider.CreateFile(str1);
      string str2 = JsonConvert.SerializeObject((object) Singleton<DataSettings>.Instance, (Formatting) 1);
      await fileProvider.WriteAllTextAsync(str1, str2, Encoding.UTF8);
    }

    public static void SaveSettings(DataSettings settings, IFileProvider fileProvider = null)
    {
      Singleton<DataSettings>.Instance = settings ?? throw new ArgumentNullException(nameof (settings));
      if (fileProvider == null)
        fileProvider = CommonHelper.DefaultFileProvider;
      string str1 = fileProvider.MapPath(DataSettingsDefaults.FilePath);
      fileProvider.CreateFile(str1);
      string str2 = JsonConvert.SerializeObject((object) Singleton<DataSettings>.Instance, (Formatting) 1);
      fileProvider.WriteAllText(str1, str2, Encoding.UTF8);
    }

    public static void ResetCache() => DataSettingsManager._databaseIsInstalled = new bool?();

    internal static void SetTestMode(bool isTestMode)
    {
      DataSettingsManager.s_TestMode = isTestMode;
    }

    public static async Task<bool> IsDatabaseInstalledAsync()
    {
      DataSettingsManager._databaseIsInstalled.GetValueOrDefault();
      if (!DataSettingsManager._databaseIsInstalled.HasValue)
        DataSettingsManager._databaseIsInstalled = new bool?(!string.IsNullOrEmpty((await DataSettingsManager.LoadSettingsAsync(reloadSettings: true))?.ConnectionString));
      return DataSettingsManager._databaseIsInstalled.Value;
    }

    public static bool IsDatabaseInstalled()
    {
      DataSettingsManager._databaseIsInstalled.GetValueOrDefault();
      if (!DataSettingsManager._databaseIsInstalled.HasValue)
        DataSettingsManager._databaseIsInstalled = new bool?(!string.IsNullOrEmpty(DataSettingsManager.LoadSettings(reloadSettings: true)?.ConnectionString));
      return DataSettingsManager._databaseIsInstalled.Value;
    }

    public static async Task<int> GetSqlCommandTimeoutAsync()
    {
      return (int?) (await DataSettingsManager.LoadSettingsAsync())?.SQLCommandTimeout ?? -1;
    }

    public static int GetSqlCommandTimeout()
    {
      return (int?) DataSettingsManager.LoadSettings()?.SQLCommandTimeout ?? -1;
    }
  }
}
