using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace QuanLiKhiThai.Views
{
    public partial class EmailPreviewWindow : Window
    {
        public EmailPreviewWindow(
            string recipient,
            string subject,
            string body,
            string sender = "system@inspectionsystem.com",
            bool isHtml = false,
            List<string>? attachments = null)
        {
            InitializeComponent();

            txtSender.Text = sender;
            txtRecipient.Text = recipient;
            txtSubject.Text = subject;

            // Show attachments if any
            if (attachments != null && attachments.Count > 0)
            {
                attachmentsPanel.Visibility = Visibility.Visible;
                foreach (var attachment in attachments)
                {
                    lstAttachments.Items.Add(System.IO.Path.GetFileName(attachment));
                }
            }

            if (isHtml)
            {
                // If content is HTML, display in WebBrowser
                txtBody.Visibility = Visibility.Collapsed;
                webBrowser.Visibility = Visibility.Visible;
                webBrowser.NavigateToString(body);
            }
            else
            {
                // If plain text, display in TextBox
                txtBody.Text = body;
                webBrowser.Visibility = Visibility.Collapsed;
                txtBody.Visibility = Visibility.Visible;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
