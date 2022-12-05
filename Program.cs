using System;
using System.IO;
using System.Security.Permissions;
using System.Windows.Forms;

namespace AndroidSideloader
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        private static void Main()
        {

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            form = new MainForm();
            Application.Run(form);
            //form.Show();
        }
        public static MainForm form;

        private static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            string date_time = DateTime.Now.ToString("dddd, MMMM dd @ hh:mmtt (UTC)");
            File.WriteAllText(Sideloader.CrashLogPath, $"Date/Time of crash: {date_time}\nMessage: {e.Message}\nData: {e.Data}\nSource: {e.Source}\nTargetSite: {e.TargetSite}");
        }
    }
}