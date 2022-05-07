using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web;
using DesktopApp.Core;
using DesktopApp.Dialogs;
using Eto.Forms;
using Eto.Drawing;

namespace DesktopApp.Pages
{
    public class StartPage : Panel
    {
        private GroupBox _dropBox;
        private static readonly Label _addressLabel = new Label
        {
            TextAlignment = TextAlignment.Right,
            TextColor = Color.FromGrayscale(0.6f),
            Text = ""
        };

        public StartPage()
        {
            var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

            layout.Add(CreateDropBox(), yscale: true);
            layout.Add(_addressLabel, yscale: false);

            Content = layout;

            if (Config<ConfigFile>.Values?.WasOpenedBefore == false)
            {
                MessageBox.Show(this, "PLEASE NOTE:\nThis is a pre-release software.\nAt this time there is no update-mechanism implemented, so please check the website regularly for updates", MessageBoxButtons.OK, MessageBoxType.Warning);
                Config<ConfigFile>.Values.WasOpenedBefore = true;
                Config<ConfigFile>.Update();
            }
        }

        public static void ChangeDeviceInfo(ushort port)
        {
            _addressLabel.Text = $"{GetIpAddress()}:{port}";
        }

        private static string GetIpAddress()
        {
            try
            {
                return IpAddress.GetIpAddress();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private GroupBox CreateDropBox()
        {
            var sendFileButton = new Button
            {
                Text = "Send File"
            };

            sendFileButton.Click += OpenFilePicker;

            var sendClipboardButton = new Button
            {
                Text = "Send Clipboard"
            };

            sendClipboardButton.Click += SendFromClipboard;

            _dropBox = new GroupBox()
            {
                // Due to a bug in ETO.Forms regarding drag-drop on linux, we disable it here
                AllowDrop = !Platform.IsGtk,
                Content = new StackLayout()
                {
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Items =
                    {
                        null,
                        new StackLayoutItem(sendFileButton),
                        null,
                        new StackLayoutItem(sendClipboardButton),
                        null
                    }
                }
            };

            _dropBox.DragEnter += GroupBoxOnDragEnter;
            _dropBox.DragLeave += DropBoxOnDragLeave;
            _dropBox.DragDrop += DropBoxOnDragDrop;

            return _dropBox;
        }

        private void DropBoxOnDragLeave(object sender, DragEventArgs e)
        {
            // e.Effects = DragEffects.None;
            _dropBox.BackgroundColor = Color.Parse("transparent");
        }

        private void GroupBoxOnDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragEffects.Move;
            _dropBox.BackgroundColor = Color.Parse("#3880ff");
        }

        private async void DropBoxOnDragDrop(object sender, DragEventArgs dropEvent)
        {
            try
            {
                // dropEvent.Effects = DragEffects.None;
                _dropBox.BackgroundColor = Color.Parse("transparent");

                if (dropEvent.Data.ContainsUris)
                {
                    var uris = dropEvent?.Data?.Uris;

                    if (uris == null || uris?.Length <= 0)
                    {
                        return;
                    }

                    string filePath = HttpUtility.UrlDecode(uris[0].AbsolutePath);

                    FileAttributes fileAttributes = File.GetAttributes(filePath);

                    if (fileAttributes.HasFlag(FileAttributes.Directory))
                    {
                        return;
                    }

                    var dialog = new SendDialog(filePath)
                    {
                        DisplayMode = DialogDisplayMode.Attached
                    };

                    await dialog.ShowModalAsync(MainForm.Reference);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private async void OpenFilePicker(object sender, EventArgs e)
        {
            using var fileDialog = new OpenFileDialog();
            fileDialog.MultiSelect = false;

            if (fileDialog.ShowDialog(MainForm.Reference) == DialogResult.Ok)
            {
                var dialog = new SendDialog(fileDialog.FileName)
                {
                    DisplayMode = DialogDisplayMode.Attached
                };

                await dialog.ShowModalAsync(MainForm.Reference);
            }
        }

        private async void SendFromClipboard(object sender, EventArgs e)
        {
            var clipboard = Clipboard.Instance;
            Stream? content = null;
            string? contentName = null;

            if (clipboard.ContainsText)
            {
                var text = clipboard.Text;
                content = new MemoryStream(Encoding.UTF8.GetBytes(text));
            }

            if (content != null)
            {
                var dialog = new SendDialog(content, contentName!)
                {
                    DisplayMode = DialogDisplayMode.Attached
                };
                
                await dialog.ShowModalAsync(MainForm.Reference);
            }
        }
    }
}