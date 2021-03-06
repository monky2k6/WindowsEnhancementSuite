﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using WindowsEnhancementSuite.Helper.Windows;

namespace WindowsEnhancementSuite.Helper
{
    public static class Utils
    {
        public static bool GetFreePath(string fileName, string fileExtension, out string availableName)
        {
            availableName = "";

            if(String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(fileExtension)) return false;

            var currentPath = WindowsMethods.GetCurrentExplorerPath();
            if (!String.IsNullOrWhiteSpace(currentPath))
            {
                for (byte i = 0; i < 99; i++)
                {
                    var filePath = Path.Combine(currentPath, String.Format(@"{0}{1}.{2}", fileName, i, fileExtension));
                    if (!File.Exists(filePath))
                    {
                        availableName = filePath;
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsProcessRunning()
        {
            var currentProcess = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(currentProcess.ProcessName);

            return processes.Any(p => p.Id != currentProcess.Id);
        }

        public static string DecodeUrl(Uri url)
        {
            string path = url.LocalPath.Replace("+", "%2B");
            return HttpUtility.UrlDecode(path, Encoding.Default);
        }
    }
}
