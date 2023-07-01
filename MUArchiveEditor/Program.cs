using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinecraftUArchiveExplorer
{
    internal static class Program
    {
        public static string API_URL = "http://api.pckstudio.xyz";

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] Args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            if(Args.Length < 1)
                Application.Run(new FormMain());
            else
                Application.Run(new FormMain(Args[0]));
        }
    }
}