using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WindowsEnhancementSuite.Helper;
using WindowsEnhancementSuite.Properties;

namespace WindowsEnhancementSuite.Services
{
    public class FileAndImageSaveService
    {
        public bool SaveClipboardInFile()
        {
            if (Clipboard.ContainsText())
            {
                return this.saveClipboardTextToFile();
            }

            if (Clipboard.ContainsImage())
            {
                return this.saveClipboardImageToFile();
            }

            return false;
        }

        private bool saveClipboardImageToFile()
        {
            var imageData = Clipboard.GetImage();
            if (imageData == null) return false;

            ThreadHelper.RunAsStaThread(() =>
            {                
                using (var memoryStream = new MemoryStream())
                {
                    string imageFilePath = String.Empty;
                    switch (Settings.Default.ImageSaveFormat)
                    {
                        case 0:
                            if (Utils.GetFreePath(@"Clipboard", "png", out imageFilePath))
                            {
                                imageData.Save(memoryStream, ImageFormat.Png);
                            }
                            break;
                        case 1:
                            if (Utils.GetFreePath(@"Clipboard", "jpg", out imageFilePath))
                            {
                                var encoder =
                                ImageCodecInfo.GetImageEncoders().FirstOrDefault(e => e.MimeType == @"image/jpeg");
                                if (encoder != null)
                                {
                                    var compression = Settings.Default.JpegCompression > 100 ? (byte)70 : Settings.Default.JpegCompression;
                                    var parameters = new EncoderParameters(1);
                                    parameters.Param[0] = new EncoderParameter(Encoder.Quality, (long)compression);

                                    imageData.Save(memoryStream, encoder, parameters);
                                }
                            }
                            break;
                    }

                    if (String.IsNullOrWhiteSpace(imageFilePath)) return;
                    try
                    {
                        using (var fileStream = new FileStream(imageFilePath, FileMode.Create))
                        {
                            memoryStream.WriteTo(fileStream);
                        }                        
                    }
                    catch (UnauthorizedAccessException)
                    {
                        string tmpFile = Path.GetTempFileName();
                        using (var fileStream = new FileStream(tmpFile, FileMode.Create))
                        {
                            memoryStream.WriteTo(fileStream);
                        } 
                        UacAssistService.TryAsAdmin(UacAssistCommand.MOVE, tmpFile, imageFilePath);
                    } 
                }
            });

            return true;
        }

        private bool saveClipboardTextToFile()
        {
            var clipboardText = Clipboard.GetText(TextDataFormat.Text);
            if (String.IsNullOrWhiteSpace(clipboardText)) return false;

            ThreadHelper.RunAsStaThread(() =>
            {
                string textFilePath;
                if (Utils.GetFreePath(@"Clipboard", "txt", out textFilePath))
                {
                    try
                    {
                        writeTextToFile(textFilePath, clipboardText);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        string tmpFile = Path.GetTempFileName();
                        writeTextToFile(tmpFile, clipboardText);
                        UacAssistService.TryAsAdmin(UacAssistCommand.MOVE, tmpFile, textFilePath);
                    }
                }                
            });

            return true;
        }

        private void writeTextToFile(string path, string text)
        {
            using (var streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(text);
            }
        }
    }
}
