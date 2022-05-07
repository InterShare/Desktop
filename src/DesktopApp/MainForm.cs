using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DesktopApp.Core;
using DesktopApp.Dialogs;
using DesktopApp.Dto;
using DesktopApp.Entities;
using DesktopApp.Extensions;
using DesktopApp.Helpers;
using DesktopApp.Pages;
using DesktopApp.Services;
using Eto.Drawing;
using Eto.Forms;
using SMTSP;
using SMTSP.Advertisement;
using SMTSP.Core;
using SMTSP.Discovery.Entities;
using SMTSP.Entities;
using SMTSP.Entities.Content;

namespace DesktopApp
{
    public sealed class MainForm : Form
    {
        private Advertiser? _advertiser;
        private readonly StartPage _startPage;
        private readonly SettingsPage _settingsPage;
        private readonly ContactsPage _contactsPage;

        public static MainForm Reference { get; set; }
        public static IVersionService VersionService { get; private set; }

        public MainForm(IVersionService versionService)
        {
            VersionService = versionService;

            Config<ConfigFile>.Initialize("InterShare");
            AppCore.Initialize();

            Reference = this;

            Title = "InterShare";
            Maximizable = false;
            MinimumSize = new Size(SizeHelper.GetSize(400), SizeHelper.GetSize(350));
            Menu = new MenuBar();
            ToolBar = CreateToolbar();

            _startPage = new StartPage();
            _settingsPage = new SettingsPage();
            _contactsPage = new ContactsPage();

            Content = _startPage;

            Application.Instance.Terminating += OnClosed;

            Start();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            _advertiser?.StopAdvertising();
            _advertiser?.Dispose();
        }

        private ToolBar CreateToolbar()
        {
            var startPageButton = new RadioToolItem()
            {
                Image = Icons.HomeIcon,
                Text = "Start",
                Checked = true
            };
            startPageButton.Click += (sender, args) =>
            {
                Content = _startPage;
            };

            var contactsButton = new RadioToolItem
            {
                Image = Icons.ContactsIcon,
                Text = "Contacts"
            };
            
            contactsButton.Click += (_, __) =>
            {
                Content = _contactsPage;
            };

            var settingsButton = new RadioToolItem()
            {
                Image = Icons.SettingsIcon,
                Text = "Settings",
            };
            
            settingsButton.Click += (sender, args) =>
            {
                Content = _settingsPage;
            };

            return new ToolBar()
            {
                Items  =
                {
                    startPageButton,
                    contactsButton,
                    settingsButton
                }
            };
        }

        private void Start()
        {
            try
            {
                var smtspReceiver = new SmtspReceiver();
                smtspReceiver.StartReceiving();

                smtspReceiver.RegisterTransferRequestCallback(OnTransferRequestCallback);
                smtspReceiver.OnFileReceive += OnContentReceived;

                Config<ConfigFile>.Values.MyDeviceInfo = new DeviceInfo()
                {
                    DeviceId = Config<ConfigFile>.Values.DeviceIdentifier,
                    DeviceName =  Config<ConfigFile>.Values.DeviceName,
                    DeviceType = DeviceTypes.Computer,
                    Port = ushort.Parse(smtspReceiver.Port.ToString())
                };

                StartPage.ChangeDeviceInfo(Config<ConfigFile>.Values.MyDeviceInfo.Port);

                _advertiser = new Advertiser(Config<ConfigFile>.Values.MyDeviceInfo, Config<ConfigFile>.Values.UseMdnsForDiscovery ? DiscoveryTypes.Mdns : DiscoveryTypes.UdpBroadcasts);
                _advertiser.Advertise();
                Config<ConfigFile>.Values.PropertyChanged += ConfigPropertyChanged;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void ConfigPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ConfigFile.UseMdnsForDiscovery))
            {
                _advertiser?.StopAdvertising();
                _advertiser?.Dispose();
                _advertiser = new Advertiser(Config<ConfigFile>.Values.MyDeviceInfo, Config<ConfigFile>.Values.UseMdnsForDiscovery ? DiscoveryTypes.Mdns : DiscoveryTypes.UdpBroadcasts);
                _advertiser.Advertise();
            }
        }

        private async void OnContentReceived(object sender, SmtspContent content)
        {
            if (content is SmtspFileContent fileContent)
            {
                HandleFileContentReceived(fileContent);
            } else if (content is SmtspClipboardContent clipboardContent)
            {
                HandleClipboardContentReceived(clipboardContent);
            }
        }

        private void HandleClipboardContentReceived(SmtspClipboardContent content)
        {
            var clipboard = Clipboard.Instance;
            clipboard.SetDataStream(content.DataStream, DataFormats.Text);
        }
        
        private async void HandleFileContentReceived(SmtspFileContent file)
        {
            try
            {
                string fullPath = Path.Combine(Config<ConfigFile>.Values.DownloadPath, file.FileName);

                var count = 1;

                string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
                string extension = Path.GetExtension(fullPath);
                string path = Path.GetDirectoryName(fullPath)!;
                string newFullPath = fullPath;

                if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                }

                while(File.Exists(newFullPath))
                {
                    var tempFileName = $"{fileNameOnly} ({count++})";
                    newFullPath = Path.Combine(path, tempFileName + extension);
                }

                using FileStream fileStream = File.Create(newFullPath);

                var cancellationTokenSource = new CancellationTokenSource();

                var progress = new Progress<long>();

                await Application.Instance.InvokeAsync(async () =>
                {
                    var dialog = new ReceivingDialog(progress, file, cancellationTokenSource)
                    {
                        DisplayMode = DialogDisplayMode.Attached
                    };

                    await dialog.ShowModalAsync(this);
                });


                try
                {
                    await file.DataStream.CopyToAsyncWithProgress(fileStream, progress, cancellationTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    // ignore
                }

                file.DataStream?.Close();
                file.DataStream?.Dispose();

                if (fileStream.Length == file.FileSize || file.FileSize == -1)
                {
                    await Application.Instance.InvokeAsync(() =>
                    {
                        MessageBox.Show(this, "The file was saved successfully", MessageBoxButtons.OK);
                    });
                }
                else
                {
                    if (!cancellationTokenSource.IsCancellationRequested)
                    {
                        await Application.Instance.InvokeAsync(() =>
                        {
                            MessageBox.Show(this, "File size did not match. The sender may have canceled the transfer", MessageBoxButtons.OK);
                        });
                    }

                    try
                    {
                        File.Delete(newFullPath);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                fileStream.Close();
            }
            catch (Exception exception)
            {

            }
        }

        private Task<bool> OnTransferRequestCallback(TransferRequest transferRequest)
        {
            return Application.Instance.Invoke(() =>
            {
                var contacts = Config<ConfigFile>.Values.Contacts; 
                
                Contact? contact = contacts.SingleOrDefault(x => x.ContactId == transferRequest.SenderId);

                if (contact != null && contact.AlwaysAllowReceivingContent)
                {
                    return Task.FromResult(true);
                }

                var contentIsFile = transferRequest.Content is SmtspFileContent;

                var msgText = contentIsFile
                    ? $"{transferRequest.SenderName}\n wants to send you \"{((SmtspFileContent) transferRequest.Content).FileName}\"\nAccept?"
                    : "{transferRequest.SenderName}\n wants to share the clipboard with you\nAccept?";

                    DialogResult answer = MessageBox.Show(
                    msgText,
                    MessageBoxButtons.YesNo, MessageBoxType.Question, MessageBoxDefaultButton.Yes);

                if (answer == DialogResult.Yes)
                {

                    if (contact == null)
                    {
                        contact = new Contact
                        {
                            ContactId = transferRequest.SenderId,
                            ContactName = transferRequest.SenderName,
                            PublicKey = null,
                            AlwaysAllowReceivingContent = false,
                            SyncClipboard = false
                        };
                        
                        Config<ConfigFile>.Values.Contacts.Add(contact);
                        Config<ConfigFile>.Update();
                    }
                    
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            });
        }
    }
}