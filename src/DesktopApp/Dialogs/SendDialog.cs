using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DesktopApp.Core;
using Eto.Drawing;
using Eto.Forms;
using SMTSP;
using SMTSP.Discovery;
using SMTSP.Entities;

namespace DesktopApp.Dialogs
{
    public class SendDialog : Dialog
    {
        private readonly Discovery _discovery;
        private readonly List<DeviceInfo> _devices = new ();
        private readonly ListBox _listBox;
        private readonly ProgressBar _progressBar = new ProgressBar();
        private readonly Label _progressBarLabel = new Label();
        private readonly CancellationTokenSource _sendFileCancellationTokenSource = new CancellationTokenSource();

        private readonly string _filePath;

        public SendDialog(string filePath)
        {
            _discovery = new Discovery(Config<ConfigFile>.Values.MyDeviceInfo, Config<ConfigFile>.Values.UseMdnsForDiscovery ? DiscoveryTypes.Mdns : DiscoveryTypes.UdpBroadcasts);
            _filePath = filePath;

            Title = "Send File";
            MinimumSize = new Size(300, 300);

            _discovery.DiscoveredDevices.CollectionChanged += DiscoveredDevicesOnCollectionChanged;
            _discovery.SendOutLookupSignal();

            _listBox = new ListBox();
            _listBox.MouseDoubleClick += ListBoxOnMouseDoubleClick;

            ShowList();

            AbortButton = new Button { Text = "C&ancel" };
            AbortButton.Click += CloseDialog;

            NegativeButtons.Add(AbortButton);
        }

        private void DiscoveredDevicesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Application.Instance.Invoke(() =>
            {
                _devices.Clear();
                _listBox.Items.Clear();

                if (_discovery?.DiscoveredDevices == null)
                {
                    return;
                }

                foreach (DeviceInfo? deviceInfo in _discovery.DiscoveredDevices)
                {
                    try
                    {
                        _devices.Add(deviceInfo);

                        _listBox.Items.Add(new ListItem
                        {
                            Key = deviceInfo.DeviceId,
                            Tag = deviceInfo,
                            Text = $"{deviceInfo.DeviceName}"
                        });
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
            });
        }

        private void CloseDialog(object sender = null, EventArgs args = null)
        {
            try
            {
                _sendFileCancellationTokenSource.Cancel();
                _discovery.Dispose();
                Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void ShowLoadingSpinner()
        {
            Content = new TableLayout
            {
                Padding = 10,
                Rows =
                {
                    new Spinner
                    {
                        Height = 30,
                        Width = 30,
                        Enabled = true
                    }
                }
            };
        }

        private void ShowProgressbar()
        {
            _progressBar.Indeterminate = false;
            _progressBar.Value = 0;

            _progressBarLabel.Font = SystemFonts.Bold();
            _progressBarLabel.TextAlignment = TextAlignment.Center;
            _progressBarLabel.Text = "Requesting...\n";

            Content = new TableLayout
            {
                Padding = 10,
                Rows =
                {
                    _progressBarLabel,
                    null,
                    _progressBar,
                    null
                }
            };
        }

        private void ShowList()
        {
            var manualButton = new Button();
            manualButton.Text = "Manual";
            manualButton.Click += ManualButtonOnClick;

            Content = new TableLayout
            {
                Padding = 10,
                Rows =
                {
                    new TableLayout
                    {
                        Padding = new Padding(5, 0, 0, 10),
                        Rows =
                        {
                            new TableRow
                            {
                                Cells =
                                {
                                    new Spinner
                                    {
                                        Height = 15,
                                        Width = 15,
                                        Enabled = true
                                    },
                                    new Label
                                    {
                                        Text = " Searching for devices",
                                        Font = SystemFonts.Bold(),
                                        TextColor = Color.FromGrayscale(0.6f)
                                    },
                                    null,
                                    manualButton
                                }
                            }
                        }
                    },
                    _listBox
                }
            };
        }

        private async void ManualButtonOnClick(object sender, EventArgs e)
        {
            var dialog = new ManuallySendDialog()
            {
                DisplayMode = DialogDisplayMode.Attached
            };

            dialog.SendClicked += async delegate(string address, ushort port)
            {
                var deviceInfo = new DeviceInfo
                {
                    IpAddress = address,
                    Port = port
                };

                await SendFile(deviceInfo);
            };

            await dialog.ShowModalAsync(this);
        }

        private void UpdateProgressBar(double progress)
        {
            _progressBarLabel.Text = $"Sending...\n{progress:0.00}%";
            _progressBar.Value = (int) progress;
        }

        private async void ListBoxOnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            string selectedKey = _listBox.SelectedKey;

            if (!string.IsNullOrEmpty(selectedKey))
            {
                DeviceInfo deviceInfo = _devices.Find(device => device.DeviceId == selectedKey);

                if (deviceInfo != null)
                {
                    await SendFile(deviceInfo);
                }
            }
        }

        private async Task SendFile(DeviceInfo deviceInfo)
        {
            try
            {
                ShowLoadingSpinner();
                ShowProgressbar();

                string fileName = Path.GetFileName(_filePath);
                FileStream fileStream = File.OpenRead(_filePath);

                var progress = new Progress<long>();
                progress.ProgressChanged += (sender, bytesProcessed) =>
                {
                    double progressValue = ((bytesProcessed / (double) fileStream.Length) * 100);
                    UpdateProgressBar(progressValue);
                };

                SendFileResponses result = await SmtspSender.SendFile(deviceInfo,
                    new SmtsFile
                    {
                        Name = fileName,
                        DataStream = fileStream,
                        FileSize = fileStream.Length
                    }, Config<ConfigFile>.Values.MyDeviceInfo, progress, _sendFileCancellationTokenSource.Token);

                fileStream.Dispose();

                if (result == SendFileResponses.Denied)
                {
                    MessageBox.Show(this, "The receiver declined the file", "Declined", MessageBoxButtons.OK);
                    ShowList();
                    return;
                }

                CloseDialog();
            }
            catch(Exception exception)
            {
                MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK);
            }
        }
    }
}