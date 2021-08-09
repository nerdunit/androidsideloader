using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AndroidSideloader
{
    class rcloneFolder
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string ModTime { get; set; }

    }

    class SideloaderRCLONE
    {
        public static List<string> RemotesList = new List<string>();

        public static string RcloneGamesFolder = "Quest Games";

        //This shit sucks but i'll switch to programatically adding indexes from the gamelist txt sometimes maybe

        public static int GameNameIndex = 0;
        public static int ReleaseNameIndex = 1;
        public static int ReleaseAPKPathIndex = 2;
        public static int PackageNameIndex = 3;
        public static int VersionCodeIndex = 4;
        public static int VersionNameIndex = 5;

        public static List<string> gameProperties = new List<string>();
        /* Game Name
         * Release Name
         * Release APK Path
         * Package Name
         * Version Code
         * Version Name
         */
        public static List<string[]> games = new List<string[]>();

        public static string Nouns = Environment.CurrentDirectory + "\\nouns";
        public static string ThumbnailsFolder = Environment.CurrentDirectory + "\\thumbnails";
        public static string NotesFolder = Environment.CurrentDirectory + "\\notes";

        public static void UpdateNouns(string remote)
        {
            RCLONE.runRcloneCommand($"sync \"{remote}:{RcloneGamesFolder}/.meta/nouns\" \"{Nouns}\"");  
        }   
        
        public static void UpdateGamePhotos(string remote)
        {
            RCLONE.runRcloneCommand($"sync \"{remote}:{RcloneGamesFolder}/.meta/thumbnails\" \"{ThumbnailsFolder}\"");  
        }

        public static void UpdateGameNotes(string remote)
        {

            RCLONE.runRcloneCommand($"sync \"{remote}:{RcloneGamesFolder}/.meta/notes\" \"{NotesFolder}\"");
        }

        public static void RefreshRemotes()
        {
            RemotesList.Clear();
            var remotes = RCLONE.runRcloneCommand("listremotes").Output.Split('\n');

            Logger.Log("Loaded following remotes: ");
            foreach (string r in remotes)
            {
                if (r.Length > 1)
                {
                    var remote = r.Remove(r.Length - 1);
                    if (remote.Contains("mirror"))
                    {
                        Logger.Log(remote);
                        RemotesList.Add(remote);
                    }
                }
            }
        }

        public static void initGames(string remote)
        {
            gameProperties.Clear();
            games.Clear();
            string tempGameList = RCLONE.runRcloneCommand($"cat \"{remote}:{RcloneGamesFolder}/GameList.txt\"").Output;    
            if (MainForm.debugMode)
                File.WriteAllText("GamesList.txt", tempGameList);
            string gamePropertiesLine = Utilities.StringUtilities.RemoveEverythingAfterFirst(tempGameList, "\n");

            foreach (string gameProperty in gamePropertiesLine.Split(';'))
            {
                gameProperties.Add(gameProperty);
            }
            gameProperties.Add("Modified Date");


            tempGameList = Utilities.StringUtilities.RemoveEverythingBeforeFirst(tempGameList, "\n");

            List<rcloneFolder> gameFolders = JsonConvert.DeserializeObject<List<rcloneFolder>>(RCLONE.runRcloneCommand($"lsjson \"{remote}:{RcloneGamesFolder}\"").Output);


            foreach (string game in tempGameList.Split('\n'))
            {
                if (game.Length > 1)
                {
                    string[] splitGame = game.Split(';');

                    //gameFolder.find();

                    var gameFolder = gameFolders.Find((predicate) => predicate.Path == splitGame[1]);
                    //splitGame[6] = gameFolder.ModTime;
                    Array.Resize(ref splitGame, splitGame.Length + 1);
                    splitGame[splitGame.Length - 1] = gameFolder.ModTime;
                    games.Add(splitGame);
                }
            }

            //Output
            //Console.WriteLine("Headers:");
            //foreach (string s in gameProperties)
            //{
            //    Console.WriteLine($"gameProperty: {s}");
            //}



            foreach (string[] s in games)
            {
                //string output = "";
                //for (int i = 0; i < gameProperties.Count; i++)
                //    output += s[i] + " ";

            }




        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public static long GetFolderSize(string FolderName, string remote)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            try
            {
                dynamic results = JsonConvert.DeserializeObject<dynamic>(RCLONE.runRcloneCommand($"size \"{remote}:{RcloneGamesFolder}/{FolderName}\" --json").Output);
                long gameSize = results.bytes.ToObject<long>();
                return gameSize / 1000000;
            }
            catch { return 0; }
        }

        public static void updateConfig(string remote)
        {
            string localHash = "";
            try { localHash = File.ReadAllText(Environment.CurrentDirectory + "\\rclone\\hash.txt"); } catch { } //file may not exist

            string hash = RCLONE.runRcloneCommand($"md5sum \"{remote}:Quest Homebrew/Sideloading Methods/1. Rookie Sideloader - VRP Edition/VRP.download.config\"").Output;
            try { hash = hash.Substring(0, hash.LastIndexOf(" ")); } catch { return; } //remove stuff after hash

            Debug.WriteLine("The local file hash is " + localHash + " and the current a file hash is " + hash);

            if (!string.Equals(localHash, hash))
            {
                RCLONE.runRcloneCommand(string.Format($"copy \"{remote}:Quest Homebrew/Sideloading Methods/1. Rookie Sideloader - VRP Edition/VRP.download.config\" \"{Environment.CurrentDirectory}\\rclone\""));
                RCLONE.killRclone();
                File.WriteAllText(Environment.CurrentDirectory + "\\rclone\\hash.txt", hash);
            }
        }
    }
}
