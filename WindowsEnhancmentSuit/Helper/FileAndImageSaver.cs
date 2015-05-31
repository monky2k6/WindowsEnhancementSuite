using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WindowsEnhancementSuit.Properties;

namespace WindowsEnhancementSuit.Helper
{
    public class FileAndImageSaver
    {
        public void SaveClipboardInFile()
        {
            if (Clipboard.ContainsText())
            {
                this.saveClipboardTextToFile();
                return;
            }

            if (Clipboard.ContainsImage())
            {
                this.saveClipboardImageToFile();
            }
        }

        private void saveClipboardImageToFile()
        {
            var imageData = Clipboard.GetImage();
            if (imageData == null) return;

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
        }

        private void saveClipboardTextToFile()
        {
            string textFilePath;
            if (Utils.GetFreePath(@"Clipboard", "txt", out textFilePath))
            {
                try
                {
                    var clipboardText = Clipboard.GetText(TextDataFormat.Text);
                    using (var streamWriter = new StreamWriter(textFilePath))
                    {
                        streamWriter.Write(clipboardText);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }
    }
}
