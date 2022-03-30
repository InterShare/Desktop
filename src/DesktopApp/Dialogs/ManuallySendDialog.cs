using System;
using DesktopApp.Helpers;
using Eto.Drawing;
using Eto.Forms;

namespace DesktopApp.Dialogs
{
    public class ManuallySendDialog : Dialog
    {
        public delegate void SendClickedDelegate(string address, ushort port);

        private readonly TextBox _addressTextBox = new TextBox
        {
            PlaceholderText = "192.168.1.120:42420"
        };

        public event SendClickedDelegate SendClicked = delegate {  };

        public ManuallySendDialog()
        {
            Title = "Enter address manually";
            MinimumSize = new Size(SizeHelper.GetSize(300), SizeHelper.GetSize(100));

            var layout = new DynamicLayout
            {
                DefaultSpacing = new Size(5, 5),
                Padding = new Padding(10)
            };

            var sendButton = new Button
            {
                Text = "Send"
            };

            sendButton.Click += SendButtonOnClick;

            _addressTextBox.KeyDown += TextBoxOnKeyDown;

            layout.Add(_addressTextBox, yscale: false);
            layout.Add(sendButton, yscale: false);

            Content = layout;

            AbortButton = new Button { Text = "C&ancel" };
            AbortButton.Click += CloseDialog;

            NegativeButtons.Add(AbortButton);
        }

        private void TextBoxOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Keys.Enter)
            {
                Send();
            }
        }

        private void SendButtonOnClick(object sender, EventArgs e)
        {
            Send();
        }

        private void Send()
        {
            CloseDialog();

            string text = _addressTextBox.Text;

            if (!string.IsNullOrEmpty(text))
            {
                string ipAddress = text;
                ushort port = 42420;

                if (text.Contains(":"))
                {
                    ipAddress = text.Split(':')[0];
                    port = ushort.Parse(text.Split(':')[1]);
                }

                SendClicked.Invoke(ipAddress, port);

            }
        }

        private void CloseDialog(object? sender = null, EventArgs? args = null)
        {
            try
            {
                Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}