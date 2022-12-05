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
            if (!MainForm.DonorApps.Equals(""))
            {
                string[] gameListSplited = MainForm.DonorApps.Split(new[] { '\n' });
                foreach (string game in gameListSplited)
                {
                    if (game.Length > 1)
                    {
                        string[] splitGame = game.Split(';');
                        donorGames.Add(splitGame);
                    }
                }
            }


        }
        public static void initNewApps()
        {
            newApps.Clear();
            if (!DonorsListViewForm.newAppsForList.Equals(""))
            {
                string[] newListSplited = DonorsListViewForm.newAppsForList.Split(new[] { '\n' });
                foreach (string game in newListSplited)
                {
                    if (game.Length > 1)
                    {
                        string[] splitGame = game.Split(';');
                        newApps.Add(splitGame);
                    }
                }
            }


        }
    }
}
