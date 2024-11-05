using System;

namespace BuildingBlocks.Common
{
    public static class UnitConverter
    {
        private const double KiloBytes = 1024.0;
        private const double MegaBytes = 1048576.0;
        private const double GigaBytes = 1073741824.0;
        private const double TeraBytes = 1099511627776.0;

        public static string Humanize(double bytes)
        {
            double absBytes = Math.Abs(bytes);

            return absBytes switch
            {
                >= GigaBytes => $"{bytes / GigaBytes:#,#.##} GBytes",
                >= MegaBytes => $"{bytes / MegaBytes:#,#.##} MBytes",
                >= KiloBytes => $"{bytes / KiloBytes:#,#.##} KBytes",
                _ => $"{bytes:#,#} Bytes"
            };
        }

        public static double BytesToMegaBytes(this double bytes) => bytes / MegaBytes;

        public static double BytesToGigaBytes(this double bytes) => bytes / GigaBytes;

        public static double KiloBytesToMegaBytes(this double kiloBytes) => kiloBytes / KiloBytes;

        public static double MegaBytesToGigaBytes(this double megaBytes) => megaBytes / KiloBytes;

        public static double MegaBytesToTeraBytes(this double megaBytes) => megaBytes / TeraBytes;

        public static double GigaBytesToMegaBytes(this double gigaBytes) => gigaBytes * KiloBytes;

        public static double GigaBytesToTeraBytes(this double gigaBytes) => gigaBytes / KiloBytes;

        public static double TeraBytesToMegaBytes(this double teraBytes) => teraBytes * MegaBytes;

        public static double TeraBytesToGigaBytes(this double teraBytes) => teraBytes * KiloBytes;
    }
}