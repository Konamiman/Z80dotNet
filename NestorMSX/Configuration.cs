namespace Konamiman.NestorMSX
{
    /// <summary>
    /// Configuration to be passed to MsxEmulationEnvironment.
    /// </summary>
    public class Configuration
    {
        public string BiosFile { get; set; }

        public string KeymapFile { get; set; }

        public string ColorsFile { get; set; }

        public decimal CpuSpeedInMHz { get; set; }

        public decimal VdpFrequencyMultiplier { get; set; }

        public decimal DisplayZoomLevel { get; set; }

        public int HorizontalMarginInPixels { get; set; }

        public int VerticalMarginInPixels { get; set; }

        public string FilesystemBaseLocation { get; set; }

        public string DiskRomFile { get; set; }

        public string Slot2RomFile { get; set; }

        public string CopyKey { get; set; }

        public string PasteKey { get; set; }
    }
}
