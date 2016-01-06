using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using WindowsEnhancementSuite.Properties;

namespace WindowsEnhancementSuite.Helper
{
    public class FileCreateHelper
    {
        public bool CreateAndOpenTextfile()
        {
            ThreadHelper.RunAsStaThread(() =>
            {
                try
                {
                    string textFilePath;
                    if (Utils.GetFreePath(@"NewTextFile", "txt", out textFilePath))
                    {
                        File.Create(textFilePath).Dispose();

                        if (String.IsNullOrWhiteSpace(Settings.Default.TextApplication))
                        {
                            Process.Start(textFilePath);
                        }
                        else
                        {
                            Process.Start(Settings.Default.TextApplication, textFilePath);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (Win32Exception)
                {
                }
            });

            return true;
        }
    }
}
