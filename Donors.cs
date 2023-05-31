using System.Collections.Generic;

namespace AndroidSideloader
{
    internal class Donors
    {
        public static int GameNameIndex = 0;
        public static int PackageNameIndex = 1;
        public static int VersionCodeIndex = 2;
        public static int UpdateOrNew = 3;
        /* Game Name
        * Package Name
        * Version Code
        * Update or New app
        */
        public static List<string> newAppProperties = new List<string>();
        public static List<string> donorGameProperties = new List<string>();

        public static List<string[]> donorGames = new List<string[]>();
        public static List<string[]> newApps = new List<string[]>();
        public static void initDonorGames()
        {
            donorGameProperties.Clear();
            donorGames.Clear();
            if (!string.IsNullOrEmpty(MainForm.DonorApps))
            {
                string[] gameListSplited = MainForm.DonorApps.Split('\n');
                foreach (string game in gameListSplited)
                {
                    if (game.Length > 1)
                    {
                        donorGames.Add(game.Split(';'));
                    }
                }
            }
        }

        public static void initNewApps()
        {
            newApps.Clear();
            if (!string.IsNullOrEmpty(DonorsListViewForm.newAppsForList))
            {
                string[] newListSplited = DonorsListViewForm.newAppsForList.Split('\n');
                foreach (string game in newListSplited)
                {
                    if (game.Length > 1)
                    {
                        newApps.Add(game.Split(';'));
                    }
                }
            }
        }
    }
}
