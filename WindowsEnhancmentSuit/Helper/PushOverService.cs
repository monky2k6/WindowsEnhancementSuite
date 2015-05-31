using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsEnhancementSuit.Properties;

namespace WindowsEnhancementSuit.Helper
{
    public class PushOverService
    {
        private const string pushOverUrl = @"https://api.pushover.net/1/messages.json";
        private const string apiToken = @"aXfkHFBZqvH48dYuFU3k4DBodfKKNA";

        private WebClient webClient;
        public PushOverService()
        {
            this.webClient = new WebClient();
        }

        public void SendMessageFromClipboard()
        {
            var userKey = Settings.Default.PushOverUserKey;
            if (String.IsNullOrWhiteSpace(userKey.Trim())) return;

            if (!Clipboard.ContainsText()) return;

            var text = Clipboard.GetText(TextDataFormat.Text);
            if (String.IsNullOrWhiteSpace(text.Trim())) return;

            var pushMessage = new NameValueCollection
                {
                    {"token", apiToken},
                    {"user", userKey},
                    {"title", "Windows Enhancement Suit"},
                    {"message", text}
                };
            try
            {
                this.webClient.UploadValues(pushOverUrl, pushMessage);
            }
            catch (Exception)
            {
            }
            
        }
    }
}
