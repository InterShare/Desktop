using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace DesktopApp.Entities
{
    public class Contact
    {
        private readonly BehaviorSubject<bool> _alwaysAllowReceivingContent = new (false);
        private readonly BehaviorSubject<bool> _syncClipboardSubject = new (false);
        
        public string ContactId { get; set; }
        public string ContactName { get; set; }
        public string PublicKey { get; set; }
        public bool SyncClipboard
        {
            get => _syncClipboardSubject.Value;
            set
            {
                if (SyncClipboard != value)
                {
                    _syncClipboardSubject.OnNext(value);
                }
            }
        }
        public bool AlwaysAllowReceivingContent
        {
            get => _alwaysAllowReceivingContent.Value;
            set
            {
                if (AlwaysAllowReceivingContent != value)
                {
                    _alwaysAllowReceivingContent.OnNext(value);
                }
            }
        }

        public IObservable<bool> AlwaysAllowReceivingContentObservable => _alwaysAllowReceivingContent;
        public IObservable<bool> SyncClipboardObservable => _syncClipboardSubject;

    }
}