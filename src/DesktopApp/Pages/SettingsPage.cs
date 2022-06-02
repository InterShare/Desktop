using System;
using System.Diagnostics;
using DesktopApp.Core;
using Eto.Drawing;
using Eto.Forms;

namespace DesktopApp.Pages
{
    public class SettingsPage : Panel
    {
        private readonly string _version;

        public SettingsPage()
        {
            var changeDownloadPathButton = new Button
            {
                Text = "change"
            };
            changeDownloadPathButton.Click += OnChangeDownloadPathClicked;

            _version = MainForm.VersionService.GetVersion() ?? "unknown";


            var downloadPathTextBox = new TextBox();
            downloadPathTextBox.TextBinding.Bind(Config<ConfigFile>.Values, n => n.DownloadPath);
            downloadPathTextBox.TextChanged += (sender, args) => Config<ConfigFile>.Update();

            var deviceNameTextBox = new TextBox();
            deviceNameTextBox.TextBinding.Bind(Config<ConfigFile>.Values, n => n.DeviceName);
            deviceNameTextBox.TextChanged += (sender, args) => Config<ConfigFile>.Update();

            var layout = new DynamicLayout
            {
                DefaultSpacing = new Size(5, 5),
                Padding = new Padding(15)
            };

            var mainContent = new StackLayout()
            {
                Items =
                {
                    new Label
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = "Download folder:",
                        TextColor = Color.FromGrayscale(0.6f)
                    },
                    new StackLayoutItem(new StackLayout()
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            new StackLayoutItem(downloadPathTextBox, true),
                            changeDownloadPathButton
                        }
                    }, HorizontalAlignment.Stretch),
                    new Label(),
                    new Label
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = "Device name:",
                        TextColor = Color.FromGrayscale(0.6f)
                    },
                    new StackLayoutItem(deviceNameTextBox, HorizontalAlignment.Stretch)
                }
            };


            var aboutBox = new GroupBox()
            {
                Padding = 5,
                Content = new StackLayout(
                    new Label
                    {
                        TextAlignment = TextAlignment.Center,
                        Text = $"Version: {_version}\n",
                        Font = Platform.IsMac ? Fonts.Monospace(10) : Fonts.Monospace(8),
                        TextColor = Color.FromGrayscale(0.6f)
                    },
                    new Label
                    {
                        TextAlignment = TextAlignment.Center,
                        Font = Platform.IsMac ? Fonts.Monospace(10) : Fonts.Monospace(8),
                        Text = "Developer: Julian Baumann",
                        TextColor = Color.FromGrayscale(0.6f)
                    },
                    new Label
                    {
                        Text = "https://intershare.julba.de",
                        Font = Platform.IsMac ? Fonts.Monospace(10) : Fonts.Monospace(8),
                        TextColor = Color.FromGrayscale(0.6f)
                    }
                )
                {
                    HorizontalContentAlignment = HorizontalAlignment.Center
                }
            };

            layout.Add(mainContent, yscale: true, xscale: true);
            layout.Add(aboutBox);

            Content = layout;
        }

        private void OnChangeDownloadPathClicked(object sender, EventArgs e)
        {
            using var folderPicker = new SelectFolderDialog();

            if (folderPicker.ShowDialog(this) == DialogResult.Ok)
            {
                string newDirectoryPath = folderPicker.Directory;

                if (!string.IsNullOrEmpty(newDirectoryPath))
                {
                    Config<ConfigFile>.Values.DownloadPath = newDirectoryPath;
                    Config<ConfigFile>.Update();
                }
            }
        }
    }
}