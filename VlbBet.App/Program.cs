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
            Application.Run(new LoginForm());
        }
    }
}
