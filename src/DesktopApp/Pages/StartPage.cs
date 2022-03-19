using System;
using System.IO;
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

        public StartPage()
        {
            Content = new TableLayout()
            {
                Padding = 20,
                Rows =
                {
                    CreateDropBox()
                }
            };

            if (!Config<ConfigFile>.Values.WasOpenedBefore)
            {
                MessageBox.Show(this, "PLEASE NOTE:\nThis is a pre-release software.\nAt this time there is no update-mechanism implemented, so please check the website regularly for updates", MessageBoxButtons.OK, MessageBoxType.Warning);
                Config<ConfigFile>.Values.WasOpenedBefore = true;
                Config<ConfigFile>.Update();
            }
        }

        private GroupBox CreateDropBox()
        {
            var sendFileButton = new Button
            {
                Text = "Send File"
            };

            sendFileButton.Click += OpenFilePicker;

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
    }
}