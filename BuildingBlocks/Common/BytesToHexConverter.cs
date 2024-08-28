// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.BytesToHexConverter
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using BuildingBlocks.Extensions;

namespace BuildingBlocks.Common;

public static class BytesToHexConverter
{
  private const char SpaceCharacter = ' ';
  private static readonly int[] ToHexTable = new int[256]
  {
    3145776,
    3211312,
    3276848,
    3342384,
    3407920,
    3473456,
    3538992,
    3604528,
    3670064,
    3735600,
    4259888,
    4325424,
    4390960,
    4456496,
    4522032,
    4587568,
    3145777,
    3211313,
    3276849,
    3342385,
    3407921,
    3473457,
    3538993,
    3604529,
    3670065,
    3735601,
    4259889,
    4325425,
    4390961,
    4456497,
    4522033,
    4587569,
    3145778,
    3211314,
    3276850,
    3342386,
    3407922,
    3473458,
    3538994,
    3604530,
    3670066,
    3735602,
    4259890,
    4325426,
    4390962,
    4456498,
    4522034,
    4587570,
    3145779,
    3211315,
    3276851,
    3342387,
    3407923,
    3473459,
    3538995,
    3604531,
    3670067,
    3735603,
    4259891,
    4325427,
    4390963,
    4456499,
    4522035,
    4587571,
    3145780,
    3211316,
    3276852,
    3342388,
    3407924,
    3473460,
    3538996,
    3604532,
    3670068,
    3735604,
    4259892,
    4325428,
    4390964,
    4456500,
    4522036,
    4587572,
    3145781,
    3211317,
    3276853,
    3342389,
    3407925,
    3473461,
    3538997,
    3604533,
    3670069,
    3735605,
    4259893,
    4325429,
    4390965,
    4456501,
    4522037,
    4587573,
    3145782,
    3211318,
    3276854,
    3342390,
    3407926,
    3473462,
    3538998,
    3604534,
    3670070,
    3735606,
    4259894,
    4325430,
    4390966,
    4456502,
    4522038,
    4587574,
    3145783,
    3211319,
    3276855,
    3342391,
    3407927,
    3473463,
    3538999,
    3604535,
    3670071,
    3735607,
    4259895,
    4325431,
    4390967,
    4456503,
    4522039,
    4587575,
    3145784,
    3211320,
    3276856,
    3342392,
    3407928,
    3473464,
    3539000,
    3604536,
    3670072,
    3735608,
    4259896,
    4325432,
    4390968,
    4456504,
    4522040,
    4587576,
    3145785,
    3211321,
    3276857,
    3342393,
    3407929,
    3473465,
    3539001,
    3604537,
    3670073,
    3735609,
    4259897,
    4325433,
    4390969,
    4456505,
    4522041,
    4587577,
    3145793,
    3211329,
    3276865,
    3342401,
    3407937,
    3473473,
    3539009,
    3604545,
    3670081,
    3735617,
    4259905,
    4325441,
    4390977,
    4456513,
    4522049,
    4587585,
    3145794,
    3211330,
    3276866,
    3342402,
    3407938,
    3473474,
    3539010,
    3604546,
    3670082,
    3735618,
    4259906,
    4325442,
    4390978,
    4456514,
    4522050,
    4587586,
    3145795,
    3211331,
    3276867,
    3342403,
    3407939,
    3473475,
    3539011,
    3604547,
    3670083,
    3735619,
    4259907,
    4325443,
    4390979,
    4456515,
    4522051,
    4587587,
    3145796,
    3211332,
    3276868,
    3342404,
    3407940,
    3473476,
    3539012,
    3604548,
    3670084,
    3735620,
    4259908,
    4325444,
    4390980,
    4456516,
    4522052,
    4587588,
    3145797,
    3211333,
    3276869,
    3342405,
    3407941,
    3473477,
    3539013,
    3604549,
    3670085,
    3735621,
    4259909,
    4325445,
    4390981,
    4456517,
    4522053,
    4587589,
    3145798,
    3211334,
    3276870,
    3342406,
    3407942,
    3473478,
    3539014,
    3604550,
    3670086,
    3735622,
    4259910,
    4325446,
    4390982,
    4456518,
    4522054,
    4587590
  };
  private static readonly byte[] FromHexTable = new byte[103]
  {
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    (byte) 0,
    (byte) 1,
    (byte) 2,
    (byte) 3,
    (byte) 4,
    (byte) 5,
    (byte) 6,
    (byte) 7,
    (byte) 8,
    (byte) 9,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    (byte) 10,
    (byte) 11,
    (byte) 12,
    (byte) 13,
    (byte) 14,
    (byte) 15,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    (byte) 10,
    (byte) 11,
    (byte) 12,
    (byte) 13,
    (byte) 14,
    (byte) 15
  };
  private static readonly byte[] FromHexTable16 = new byte[103]
  {
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    (byte) 0,
    (byte) 16,
    (byte) 32,
    (byte) 48,
    (byte) 64,
    (byte) 80,
    (byte) 96,
    (byte) 112,
    (byte) 128,
    (byte) 144,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    (byte) 160,
    (byte) 176,
    (byte) 192,
    (byte) 208,
    (byte) 224,
    (byte) 240,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    byte.MaxValue,
    (byte) 160,
    (byte) 176,
    (byte) 192,
    (byte) 208,
    (byte) 224,
    (byte) 240
  };

  public static unsafe string ToHex(byte[] bytes)
  {
    Ensure.NotNull<byte[]>(bytes, nameof (bytes));
    fixed (int* numPtr1 = BytesToHexConverter.ToHexTable)
    fixed (byte* numPtr2 = bytes)
    {
      byte* numPtr3 = numPtr2;
      string hex = new string(' ', bytes.Length << 1);
      IntPtr num;
      if (hex == null)
      {
        num = IntPtr.Zero;
      }
      else
      {
        fixed (char* chPtr = &hex.GetPinnableReference())
          num = (IntPtr) chPtr;
      }
      int* numPtr4 = (int*) num;
      while (*numPtr4 != 0)
        *numPtr4++ = numPtr1[*numPtr3++];
      return hex;
    }
  }

  public static unsafe byte[] FromHex(string hexadecimalInput)
  {
    Ensure.NotNull<string>(hexadecimalInput, nameof (hexadecimalInput));
    if (hexadecimalInput.IsNullOrEmpty())
      return new byte[0];
    if (hexadecimalInput.Length % 2 == 1)
      throw new ArgumentException();
    int num1 = 0;
    int length = hexadecimalInput.Length >> 1;
    IntPtr num2;
    if (hexadecimalInput == null)
    {
      num2 = IntPtr.Zero;
    }
    else
    {
      fixed (char* chPtr = &hexadecimalInput.GetPinnableReference())
        num2 = (IntPtr) chPtr;
    }
    if (*(int*) num2 == 7864368)
    {
      if (hexadecimalInput.Length == 2)
        throw new ArgumentException();
      num1 += 2;
      --length;
    }
    byte[] numArray = new byte[length];
    fixed (byte* numPtr1 = BytesToHexConverter.FromHexTable16)
    fixed (byte* numPtr2 = BytesToHexConverter.FromHexTable)
    fixed (byte* numPtr3 = numArray)
    {
      char* chPtr1 = (char*) (num2 + (IntPtr) num1 * 2);
      byte* numPtr4 = numPtr3;
      while (*chPtr1 != char.MinValue)
      {
        if (*chPtr1 <= 'f')
        {
          byte* numPtr5 = numPtr4;
          byte* numPtr6 = numPtr1;
          char* chPtr2 = chPtr1;
          char* chPtr3 = (char*) ((IntPtr) chPtr2 + 2);
          int index1 = (int) *chPtr2;
          int num3;
          byte num4 = (byte) (num3 = (int) numPtr6[index1]);
          *numPtr5 = (byte) num3;
          if (num4 != byte.MaxValue && *chPtr3 <= 'f')
          {
            byte* numPtr7 = numPtr2;
            char* chPtr4 = chPtr3;
            chPtr1 = (char*) ((IntPtr) chPtr4 + 2);
            int index2 = (int) *chPtr4;
            byte num5;
            if ((num5 = numPtr7[index2]) != byte.MaxValue)
            {
              byte* numPtr8 = numPtr4++;
              int num6 = (int) (byte) ((uint) *numPtr8 + (uint) num5);
              *numPtr8 = (byte) num6;
              continue;
            }
          }
        }
        throw new ArgumentException();
      }
      return numArray;
    }
  }
}