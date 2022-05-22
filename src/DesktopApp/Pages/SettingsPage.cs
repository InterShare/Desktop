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

            const string link = "https://intershare.julba.de";

            var websiteLink = new LinkButton()
            {
                Text = link
            };

            websiteLink.Click += delegate
            {
                Process.Start(link);
            };

            var aboutBox = new GroupBox()
            {
                Padding = 5,
                Content = new StackLayout(
                    new Label
                    {
                        TextAlignment = TextAlignment.Center,
                        Text = $"Version: {_version}\n",
                        TextColor = Color.FromGrayscale(0.6f)
                    },
                    new Label
                    {
                        TextAlignment = TextAlignment.Center,
                        Text = "Developer: Julian Baumann",
                        TextColor = Color.FromGrayscale(0.6f)
                    },
                    websiteLink
                )
                {
                    HorizontalContentAlignment = HorizontalAlignment.Center
                }
            };

            layout.Add(mainContent, yscale: true, xscale: true);
            layout.Add(aboutBox);

            Content = layout;
            // layout.Add(_addressLabel, yscale: false);
            //
            // Content = new TableLayout
            // {
            //     Spacing = new Size(0, 10),
            //     Padding = 20,
            //     Rows =
            //     {
            //         new TableRow
            //         {
            //             Cells =
            //             {
            //                 new Label
            //                 {
            //                     VerticalAlignment = VerticalAlignment.Center,
            //                     TextAlignment = TextAlignment.Right,
            //                     Text = "Download folder: ",
            //                     TextColor = Color.FromGrayscale(0.6f)
            //                 },
            //                 new TableCell(downloadPathTextBox, true),
            //                 new TableCell(changeDownloadPathButton, false)
            //             }
            //         },
            //         new TableRow
            //         {
            //             Cells =
            //             {
            //                 new Label
            //                 {
            //                     VerticalAlignment = VerticalAlignment.Center,
            //                     TextAlignment = TextAlignment.Right,
            //                     Text = "Device name: ",
            //                     TextColor = Color.FromGrayscale(0.6f)
            //                 },
            //                 deviceNameTextBox
            //             }
            //         },
            //         null,
            //         new TableRow
            //         {
            //             Cells =
            //             {
            //                 new TableCell(),
            //                 new GroupBox()
            //                 {
            //                     Padding = 5,
            //                     Content = new StackLayout(new Label
            //                         {
            //                             TextAlignment = TextAlignment.Center,
            //                             Text = $"Version: {_version}\n",
            //                             TextColor = Color.FromGrayscale(0.6f),
            //                             Font = SystemFonts.Default(Platform.IsMac ? 10.0f : 7.0f)
            //                         },
            //                         new Label
            //                         {
            //                             TextAlignment = TextAlignment.Center,
            //                             Text = "Author: Julian Baumann\nhttps://julian-baumann.com",
            //                             TextColor = Color.FromGrayscale(0.6f),
            //                             Font = SystemFonts.Default(Platform.IsMac ? 10.0f : 7.0f)
            //                         })
            //                     {
            //                         HorizontalContentAlignment = HorizontalAlignment.Center
            //                     }
            //                 }
            //             }
            //         }
            //     }
            // };
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