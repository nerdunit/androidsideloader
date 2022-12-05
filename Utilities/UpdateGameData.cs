namespace AndroidSideloader.Utilities
{
    internal class UpdateGameData
    {
        public UpdateGameData(string gameName, string packageName, ulong installedVersionInt)
        {
            GameName = gameName;
            Packagename = packageName;
            InstalledVersionInt = installedVersionInt;
        }
        public string GameName { get; set; }
        public string Packagename { get; set; }

        public ulong InstalledVersionInt { get; set; }
    }
}
