// Program.cs
using System;
using System.Windows.Forms;

namespace Computer_Graphics_Programming___Blue_Meteorite
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
