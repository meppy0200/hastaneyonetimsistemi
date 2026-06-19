using System;
using System.Windows.Forms;
using HastaneBilgiSistemi.Data;
using HastaneBilgiSistemi.Forms;

namespace HastaneBilgiSistemi
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Db.Initialize();
            Application.Run(new MainForm());
        }
    }
}
