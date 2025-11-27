using System;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using VlbBet.Infrastructure;

namespace VlbBet.App
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppState.Api = new ApiSession("https://vlb.virsbet.com");
            Application.ApplicationExit += (s, e) =>
            {
                if (AppState.Api != null) AppState.Api.Dispose();
            };

            Application.Run(new LoginForm());
        }
    }
}
