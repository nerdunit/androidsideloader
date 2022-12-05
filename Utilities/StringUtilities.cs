using System.Linq;

namespace AndroidSideloader.Utilities
{
    internal class StringUtilities
    {
        public static string RemoveEverythingAfterFirst(string s, string removeMe)
        {
            int index = s.IndexOf(removeMe);
            if (index > 0)
            {
                s = s.Substring(0, index);
            }

            return s;
        }

        public static string RemoveEverythingAfterLast(string s, string removeMe)
        {
            int index = s.LastIndexOf(removeMe);
            if (index > 0)
            {
                s = s.Substring(0, index);
            }

            return s;
        }

        public static string RemoveEverythingBeforeFirst(string s, string removeMe)
        {
            int index = s.IndexOf(removeMe);
            if (index > 0)
            {
                s = s.Substring(index);
            }

            return s;
        }

        public static string KeepOnlyNumbers(string s)
        {
            string numbers = "0123456789";
            string a = "";
            foreach (char ch in s)
            {
                if (numbers.Contains(ch))
                {
                    a += ch;
                }
            }
            return a;
        }

        public static string RemoveEverythingBeforeLast(string s, string removeMe)
        {
            int index = s.LastIndexOf(removeMe);
            if (index > 0)
            {
                s = s.Substring(index);
            }

            return s;
        }
    }
}
