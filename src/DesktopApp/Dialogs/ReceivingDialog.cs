using System;
using System.Threading;
using System.Timers;
using DesktopApp.Helpers;
using Eto.Drawing;
using Eto.Forms;
using SMTSP.Entities;
using SMTSP.Entities.Content;
using Timer = System.Timers.Timer;

namespace DesktopApp.Dialogs
{
    public class ReceivingDialog : Dialog
    {
        private readonly Progress<long> _progress;
        private readonly ProgressBar _progressBar;
        private readonly Spinner _progressSpinner;
        private readonly Label _progressBarText;
        private readonly SmtspContent _content;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ReceivingDialog(Progress<long> progress, SmtspContent content, CancellationTokenSource cancellationToken)
        {
            _progress = progress;
            _content = content;
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

            _progressSpinner = new Spinner
            {
                Height = 30,
                Width = 30,
                Enabled = true
            };

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
                    _content is SmtspFileContent ? _progressBar : _progressSpinner,
                    null
                }
            };

            _progress.ProgressChanged += ProgressOnProgressChanged;

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += CloseDialogClicked;

            NegativeButtons.Add(AbortButton);
        }

        private void ProgressOnProgressChanged(object sender, long bytesProcessed)
        {
            if (_content is SmtspFileContent fileContent)
            {
                Application.Instance.Invoke(() =>
                {
                    try
                    {
                        double progressValue = ((bytesProcessed / (double) fileContent.FileSize) * 100);
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