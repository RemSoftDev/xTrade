namespace Xtrade_wp8.Com.VO
{
    class ChartsData
    {
        public static string LANDSCAPE = "_landscape";

        public static string PORTRAIT = "_portrait";

        public ChartsData(string pUlr = "", string pOrientation = "", int pWidthPortrait = 0, int pHeghtPortrait = 0, int pWidthLandscape = 0, int pHeghtLandscape = 0)
        {
            Url = pUlr;
            Orientation = pOrientation;
            LandscapeWidth = pWidthLandscape;
            LandscapeHeight = pHeghtLandscape;
            PortraitWidth = pWidthPortrait;
            PortraitHeight = pHeghtPortrait;
        }

        public string Url { get; set; }

        public string Orientation { get; set; }

        public int PortraitWidth { get; set; }

        public int PortraitHeight { get; set; }

        public int LandscapeWidth { get; set; }

        public int LandscapeHeight { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }
    }
}