﻿using System;
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

            bool useMdns = Config<ConfigFile>.Values.UseMdnsForDiscovery;

            var useMdnsRadioButton = new RadioButton
            {
                Text = "MDNS-SD",
                Checked = useMdns
            };
            useMdnsRadioButton.CheckedChanged += (sender, args) =>
            {
                Config<ConfigFile>.Values.UseMdnsForDiscovery = true;
                Config<ConfigFile>.Update();
            };

            var useUdpBroadcastsRadioButton = new RadioButton(useMdnsRadioButton)
            {
                Text = "UDP Broadcasts",
                Checked = !useMdns
            };
            useUdpBroadcastsRadioButton.CheckedChanged += (sender, args) =>
            {
                Config<ConfigFile>.Values.UseMdnsForDiscovery = false;
                Config<ConfigFile>.Update();
            };

            Content = new TableLayout
            {
                Spacing = new Size(0, 10),
                Padding = 20,
                Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            new Label
                            {
                                TextAlignment = TextAlignment.Right,
                                Text = "Download folder:",
                                TextColor = Color.FromGrayscale(0.6f)
                            },
                            new TableCell(downloadPathTextBox, true),
                            new TableCell(changeDownloadPathButton, false)
                        }
                    },
                    new TableRow
                    {
                        Cells =
                        {
                            new Label
                            {
                                TextAlignment = TextAlignment.Right,
                                Text = "Device name:",
                                TextColor = Color.FromGrayscale(0.6f)
                            },
                            deviceNameTextBox
                        }
                    },
                    new TableRow
                    {
                        Cells =
                        {
                            new Label
                            {
                                TextAlignment = TextAlignment.Right,
                                Text = "Discovery: ",
                                TextColor = Color.FromGrayscale(0.6f)
                            },
                            new DynamicLayout(useMdnsRadioButton, useUdpBroadcastsRadioButton)
                        }
                    },
                    null,
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell(),
                            new GroupBox()
                            {
                                Padding = 5,
                                Content = new StackLayout(new Label
                                    {
                                        TextAlignment = TextAlignment.Center,
                                        Text = $"Version: {_version}\n",
                                        TextColor = Color.FromGrayscale(0.6f),
                                        Font = SystemFonts.Default(10.0f)
                                    },
                                    new Label
                                    {
                                        TextAlignment = TextAlignment.Center,
                                        Text = "Author: Julian Baumann\nhttps://julian-baumann.com",
                                        TextColor = Color.FromGrayscale(0.6f),
                                        Font = SystemFonts.Default(10.0f)
                                    })
                                {
                                    HorizontalContentAlignment = HorizontalAlignment.Center
                                }
                            }
                        }
                    },
                }
            };
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