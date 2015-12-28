using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WindowsEnhancementSuite.Extensions;
using WindowsEnhancementSuite.Properties;

namespace WindowsEnhancementSuite.Helper
{
    public class FileAndImageSaver
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

            new Action(() =>
            {
                try
                {
                    string imageFilePath;
                    switch (Settings.Default.ImageSaveFormat)
                    {
                        case 0:
                            if (Utils.GetFreePath(@"Clipboard", "png", out imageFilePath))
                            {
                                imageData.Save(imageFilePath, ImageFormat.Png);
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

                                    imageData.Save(imageFilePath, encoder, parameters); 
                                }
                            }
                            break;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                }
            }).RunAsStaThread();

            return true;
        }

        private bool saveClipboardTextToFile()
        {
            var clipboardText = Clipboard.GetText(TextDataFormat.Text);
            if (String.IsNullOrWhiteSpace(clipboardText)) return false;

            new Action(() =>
            {
                try
                {
                    string textFilePath;
                    if (Utils.GetFreePath(@"Clipboard", "txt", out textFilePath))
                    {
                        using (var streamWriter = new StreamWriter(textFilePath))
                        {
                            streamWriter.Write(clipboardText);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                }                
            }).RunAsStaThread();

            return true;
        }
    }
}
