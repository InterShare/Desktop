using System;

namespace DesktopApp.Entities
{
    public class Contact
    {
        public string ContactId { get; set; }
        public string PublicKey { get; set; }
        public bool ConfirmReceivedContent { get; set; }
        public bool SyncClipboard { get; set; }
    }
}