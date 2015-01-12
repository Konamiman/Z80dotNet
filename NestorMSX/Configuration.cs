using Konamiman.NestorMSX.Misc;

namespace Konamiman.NestorMSX
{
    /// <summary>
    /// Configuration to be passed to MsxEmulationEnvironment.
    /// </summary>
    public class Configuration
    {
        [Mandatory]
        public string BiosFile { get; set; }

        [Mandatory]
        public string KeymapFile { get; set; }

        [Mandatory]
        public string ColorsFile { get; set; }

        public decimal CpuSpeedInMHz { get; set; }

        public decimal VdpFrequencyMultiplier { get; set; }

        public decimal DisplayZoomLevel { get; set; }

        public int HorizontalMarginInPixels { get; set; }

        public int VerticalMarginInPixels { get; set; }

        public string FilesystemBaseLocation { get; set; }

        [Mandatory]
        public string DiskRomFile { get; set; }

        public string Slot2RomFile { get; set; }

        [Mandatory]
        public string CopyKey { get; set; }

        [Mandatory]
        public string PasteKey { get; set; }
    }
}
