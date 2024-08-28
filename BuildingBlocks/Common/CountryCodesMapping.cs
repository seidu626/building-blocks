﻿// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.CountryCodesMapping
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Common;

public static class CountryCodesMapping
{
  private static readonly Dictionary<string, string> CountryCodeMapping = CountryCodesMapping.InitializeMapping();

  public static Dictionary<string, string> Mappings
  {
    get
    {
      return new Dictionary<string, string>((IDictionary<string, string>) CountryCodesMapping.CountryCodeMapping, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }

  public static bool TryGetCountryName(string code, out string countryName)
  {
    Ensure.NotNullOrEmptyOrWhiteSpace(code);
    return CountryCodesMapping.CountryCodeMapping.TryGetValue(code, out countryName);
  }

  private static Dictionary<string, string> InitializeMapping()
  {
    return new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "AFG",
        "Afghanistan"
      },
      {
        "ALB",
        "Albania"
      },
      {
        "ARE",
        "U.A.E."
      },
      {
        "ARG",
        "Argentina"
      },
      {
        "ARM",
        "Armenia"
      },
      {
        "AUS",
        "Australia"
      },
      {
        "AUT",
        "Austria"
      },
      {
        "AZE",
        "Azerbaijan"
      },
      {
        "BEL",
        "Belgium"
      },
      {
        "BGD",
        "Bangladesh"
      },
      {
        "BGR",
        "Bulgaria"
      },
      {
        "BHR",
        "Bahrain"
      },
      {
        "BIH",
        "Bosnia and Herzegovina"
      },
      {
        "BLR",
        "Belarus"
      },
      {
        "BLZ",
        "Belize"
      },
      {
        "BOL",
        "Bolivia"
      },
      {
        "BRA",
        "Brazil"
      },
      {
        "BRN",
        "Brunei Darussalam"
      },
      {
        "CAN",
        "Canada"
      },
      {
        "CHE",
        "Switzerland"
      },
      {
        "CHL",
        "Chile"
      },
      {
        "CHN",
        "People's Republic of China"
      },
      {
        "COL",
        "Colombia"
      },
      {
        "CRI",
        "Costa Rica"
      },
      {
        "CZE",
        "Czech Republic"
      },
      {
        "DEU",
        "Germany"
      },
      {
        "DNK",
        "Denmark"
      },
      {
        "DOM",
        "Dominican Republic"
      },
      {
        "DZA",
        "Algeria"
      },
      {
        "ECU",
        "Ecuador"
      },
      {
        "EGY",
        "Egypt"
      },
      {
        "ESP",
        "Spain"
      },
      {
        "EST",
        "Estonia"
      },
      {
        "ETH",
        "Ethiopia"
      },
      {
        "FIN",
        "Finland"
      },
      {
        "FRA",
        "France"
      },
      {
        "FRO",
        "Faroe Islands"
      },
      {
        "GBR",
        "United Kingdom"
      },
      {
        "GEO",
        "Georgia"
      },
      {
        "GRC",
        "Greece"
      },
      {
        "GRL",
        "Greenland"
      },
      {
        "GTM",
        "Guatemala"
      },
      {
        "HKG",
        "Hong Kong S.A.R."
      },
      {
        "HND",
        "Honduras"
      },
      {
        "HRV",
        "Croatia"
      },
      {
        "HUN",
        "Hungary"
      },
      {
        "IDN",
        "Indonesia"
      },
      {
        "IND",
        "India"
      },
      {
        "IRL",
        "Ireland"
      },
      {
        "IRN",
        "Iran"
      },
      {
        "IRQ",
        "Iraq"
      },
      {
        "ISL",
        "Iceland"
      },
      {
        "ISR",
        "Israel"
      },
      {
        "ITA",
        "Italy"
      },
      {
        "JAM",
        "Jamaica"
      },
      {
        "JOR",
        "Jordan"
      },
      {
        "JPN",
        "Japan"
      },
      {
        "KAZ",
        "Kazakhstan"
      },
      {
        "KEN",
        "Kenya"
      },
      {
        "KGZ",
        "Kyrgyzstan"
      },
      {
        "KHM",
        "Cambodia"
      },
      {
        "KOR",
        "Korea"
      },
      {
        "KWT",
        "Kuwait"
      },
      {
        "LAO",
        "Lao P.D.R."
      },
      {
        "LBN",
        "Lebanon"
      },
      {
        "LBY",
        "Libya"
      },
      {
        "LIE",
        "Liechtenstein"
      },
      {
        "LKA",
        "Sri Lanka"
      },
      {
        "LTU",
        "Lithuania"
      },
      {
        "LUX",
        "Luxembourg"
      },
      {
        "LVA",
        "Latvia"
      },
      {
        "MAC",
        "Macao S.A.R."
      },
      {
        "MAR",
        "Morocco"
      },
      {
        "MCO",
        "Principality of Monaco"
      },
      {
        "MDV",
        "Maldives"
      },
      {
        "MEX",
        "Mexico"
      },
      {
        "MKD",
        "Macedonia (FYROM)"
      },
      {
        "MLT",
        "Malta"
      },
      {
        "MNE",
        "Montenegro"
      },
      {
        "MNG",
        "Mongolia"
      },
      {
        "MYS",
        "Malaysia"
      },
      {
        "NGA",
        "Nigeria"
      },
      {
        "NIC",
        "Nicaragua"
      },
      {
        "NLD",
        "Netherlands"
      },
      {
        "NOR",
        "Norway"
      },
      {
        "NPL",
        "Nepal"
      },
      {
        "NZL",
        "New Zealand"
      },
      {
        "OMN",
        "Oman"
      },
      {
        "PAK",
        "Islamic Republic of Pakistan"
      },
      {
        "PAN",
        "Panama"
      },
      {
        "PER",
        "Peru"
      },
      {
        "PHL",
        "Republic of the Philippines"
      },
      {
        "POL",
        "Poland"
      },
      {
        "PRI",
        "Puerto Rico"
      },
      {
        "PRT",
        "Portugal"
      },
      {
        "PRY",
        "Paraguay"
      },
      {
        "QAT",
        "Qatar"
      },
      {
        "ROU",
        "Romania"
      },
      {
        "RUS",
        "Russia"
      },
      {
        "RWA",
        "Rwanda"
      },
      {
        "SAU",
        "Saudi Arabia"
      },
      {
        "SCG",
        "Serbia and Montenegro (Former)"
      },
      {
        "SEN",
        "Senegal"
      },
      {
        "SGP",
        "Singapore"
      },
      {
        "SLV",
        "El Salvador"
      },
      {
        "SRB",
        "Serbia"
      },
      {
        "SVK",
        "Slovakia"
      },
      {
        "SVN",
        "Slovenia"
      },
      {
        "SWE",
        "Sweden"
      },
      {
        "SYR",
        "Syria"
      },
      {
        "TAJ",
        "Tajikistan"
      },
      {
        "THA",
        "Thailand"
      },
      {
        "TKM",
        "Turkmenistan"
      },
      {
        "TTO",
        "Trinidad and Tobago"
      },
      {
        "TUN",
        "Tunisia"
      },
      {
        "TUR",
        "Turkey"
      },
      {
        "TWN",
        "Taiwan"
      },
      {
        "UKR",
        "Ukraine"
      },
      {
        "URY",
        "Uruguay"
      },
      {
        "USA",
        "United States"
      },
      {
        "UZB",
        "Uzbekistan"
      },
      {
        "VEN",
        "Bolivarian Republic of Venezuela"
      },
      {
        "VNM",
        "Vietnam"
      },
      {
        "YEM",
        "Yemen"
      },
      {
        "ZAF",
        "South Africa"
      },
      {
        "ZWE",
        "Zimbabwe"
      }
    };
  }
}