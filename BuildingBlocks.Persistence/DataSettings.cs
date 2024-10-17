// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Persistence.DataSettings
// Assembly: BuildingBlocks.Persistence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 98D21131-3851-47B7-9AEB-AB489CBD4F40
// Assembly location: C:\Users\420919\Repositories\Decompiled\BuildingBlocks.Persistence.dll

using BuildingBlocks.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using BuildingBlocks.Data;

#nullable disable
namespace BuildingBlocks.Persistence
{
  public class DataSettings : IConfig
  {
    public DataSettings()
    {
      this.RawDataSettings = (IDictionary<string, string>) new Dictionary<string, string>();
    }

    public string DatabaseName { get; set; }

    public string DefaultSchema { get; set; }

    [JsonProperty(PropertyName = "DataConnectionString")]
    public string ConnectionString { get; set; }

    [JsonConverter(typeof (StringEnumConverter))]
    public DataProviderType DataProvider { get; set; }

    public int? SQLCommandTimeout { get; set; }

    public IDictionary<string, string> RawDataSettings { get; }

    [JsonIgnore]
    public bool IsValid
    {
      get
      {
        return this.DataProvider != DataProviderType.Unknown && !string.IsNullOrEmpty(this.ConnectionString);
      }
    }

    public Version AppVersion { get; set; }

    public bool IsSqlServer => this.DataProvider == DataProviderType.SqlServer;

    public string MigrationsHistoryTableName { get; set; }

    public string MigrationsHistoryTableSchema { get; set; }

    protected virtual string SerializeSettings()
    {
      return string.Format("AppVersion: {0}{3}DataProvider: {1}{3}DataConnectionString: {2}{3}", (object) this.AppVersion.ToString(), (object) this.DataProvider, (object) this.ConnectionString, (object) Environment.NewLine);
    }
  }
}
