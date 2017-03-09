namespace Xtrade_wp8.Com
{
    class Constants
    {
        // is debug
        public static bool IS_DEBUG = true;

        public static string SERVER_ADDRESS_PROTOCOL = "https://";
        public static string SERVER_ADDRESS = "xforex.com";
        public static string SERVER_ADDRESS_TEST_PREF = "test";
        public static string SERVER_ADDRESS_DEV_PREF = "www";
        public static string SOCKET_ADDRESS_TEST_PREF = "socket-test";
        public static string SOCKET_ADDRESS_DEV_PREF = "socket";

        public static string WP_8_0 = "Microsoft Windows NT 8.0";
        public static string WP_8_1 = "Microsoft Windows NT 8.1";

        // test server
        // public static string MAIN_URL_WP8_1 = "https://test.xforex.com/mobile-project/test/wp8/engine_wp8_nativ.html";     
        // public static string MAIN_URL_WP8_0 = "https://test.xforex.com/mobile-project/test/wp8/engine_wp8_nativ_8_0.html";   /**/

        // RC server
        public static string MAIN_URL_WP8_1 = "https://www.xforex.com/mobile-project/rc/wp8/engine_wp8_nativ.html";
        public static string MAIN_URL_WP8_0 = "https://www.xforex.com/mobile-project/rc/wp8/engine_wp8_nativ_8_0.html";     /**/
        public static string APM_POPUP_URL = "https://www.xforex.com/mobile-project/rc/wp8/apm_popup_wp.html";

        // prod server
        /* public static string MAIN_URL_WP8_1 = "https://www.xforex.com/mobile-project/prod/wp8/engine_wp8_nativ.html ";
         public static string MAIN_URL_WP8_0 = "https://www.xforex.com/mobile-project/prod/wp8/engine_wp8_nativ.html ";     /**/

        public static string LANDSCAPE = "Landscape";
        public static string PORTRAIT = "Portrait";

        public static string FLURRY_API_KEY = "TVQD372GQQYDF6XC5TD5";
        public static string PUSHWOOSH_KEY = "2C53B-C6C67";


        public static int URL_UPLOAD_MAX_TIME = 45;

        public static string PAGE_NAME__GRAPHICS_DEALS = "graphicsDeals";
        public static string PAGE_NAME__GRAPHICS_RATES = "graphicsRates";
        public static string PAGE_NAME__GRAPHICS_ORDERS = "graphicsOrders";
    }
}