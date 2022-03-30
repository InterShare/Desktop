using System;
using System.Threading;
using System.Timers;
using DesktopApp.Helpers;
using Eto.Drawing;
using Eto.Forms;
using SMTSP.Entities;
using Timer = System.Timers.Timer;

namespace DesktopApp.Dialogs
{
    public class ReceivingDialog : Dialog
    {
        private readonly Progress<long> _progress;
        private readonly ProgressBar _progressBar;
        private readonly Label _progressBarText;
        private readonly SmtsFile _file;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ReceivingDialog(Progress<long> progress, SmtsFile file, CancellationTokenSource cancellationToken)
        {
            _progress = progress;
            _file = file;
            _cancellationTokenSource = cancellationToken;
            Title = "Receiving File";
            MinimumSize = new Size(SizeHelper.GetSize(300), SizeHelper.GetSize(300));

            _progressBar = new ProgressBar
            {
                MinValue = 0,
                MaxValue = 100
            };
            _progressBar.Indeterminate = false;
            _progressBar.Value = 0;

            _progressBarText = new Label
            {
                TextAlignment = TextAlignment.Center,
                Text = "Receiving file...\n0.00%",
                TextColor = Color.FromGrayscale(0.6f),
                Font = SystemFonts.Bold()
            };

            Content = new TableLayout
            {
                Padding = 10,
                Rows =
                {
                    _progressBarText,
                    null,
                    _progressBar,
                    null
                }
            };

            _progress.ProgressChanged += ProgressOnProgressChanged;

            AbortButton = new Button { Text = "C&ancel" };
            AbortButton.Click += CloseDialogClicked;

            NegativeButtons.Add(AbortButton);
        }

        private void ProgressOnProgressChanged(object sender, long bytesProcessed)
        {
            Application.Instance.Invoke(() =>
            {
                try
                {
                    double progressValue = ((bytesProcessed / (double) _file.FileSize) * 100);
                    _progressBar.Value = (int) progressValue;
                    _progressBarText.Text = $"Receiving file...\n{progressValue:0.00}%";

                    if (progressValue >= 100)
                    {
                        _progress.ProgressChanged -= ProgressOnProgressChanged;
                        _progressBar.Value = 100;
                        Thread.Sleep(500);
                        CloseDialog();
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            });
        }

        private void CloseDialogClicked(object sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();
            CloseDialog();
        }

        private void CloseDialog()
        {
            Application.Instance.Invoke(Close);
        }
    }
}