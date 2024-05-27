﻿using System;
using System.Windows.Forms;

namespace Athena_R
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
			Application.SetDefaultFont(new System.Drawing.Font("SimSun", 9F));
            Application.Run(new MainForm());
        }
    }
}
