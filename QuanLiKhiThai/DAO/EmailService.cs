using QuanLiKhiThai.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiKhiThai.DAO
{
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly bool _enableSsl;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailService()
        {
            _smtpServer = "smtp.gmail.com";
            _smtpPort = 587;
            _smtpUsername = ConfigurationHelper.GetAppSetting("EmailUsername");
            _smtpPassword = ConfigurationHelper.GetAppSetting("EmailPassword");
            _enableSsl = true;
            _senderEmail = _smtpUsername;
            _senderName = "Vehicle Inspection System";
        }

        // Constructor with custom SMTP settings
        public EmailService(string smtpServer, int smtpPort, string smtpUsername,
                           string smtpPassword, bool enableSsl, string senderEmail, string senderName)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUsername = smtpUsername;
            _smtpPassword = smtpPassword;
            _enableSsl = enableSsl;
            _senderEmail = senderEmail;
            _senderName = senderName;
        }

        public async Task<(bool Success, string ErrorMessage)> SendEmailAsync
            (
                string recipientEmail, 
                string recipientName, 
                string subject, 
                string body, 
                bool isBodyHtml = false, 
                List<string>? ccEmails = null,
                bool sendCopyToSender = false,
                List<string>? attachmentPaths = null
            )
        {
            try
            {
                using (var message = new MailMessage())
                {
                    // Setup sender and receiver
                    message.From = new MailAddress(_senderEmail, _senderName);
                    message.To.Add(new MailAddress(recipientEmail, recipientName));
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = isBodyHtml;

                    // Add CC recipients
                    if (ccEmails != null)
                    {
                        foreach (var email in ccEmails)
                        {
                            if (!string.IsNullOrWhiteSpace(email))
                                message.CC.Add(email);
                        }
                    }

                    // Add sender to BCC if requested
                    if (sendCopyToSender)
                    {
                        message.Bcc.Add(_senderEmail);
                    }

                    // Add attachments
                    if (attachmentPaths != null)
                    {
                        foreach (var path in attachmentPaths)
                        {
                            if (File.Exists(path))
                            {
                                var attachment = new Attachment(path);
                                message.Attachments.Add(attachment);
                            }
                        }
                    }

                    // Configure SMTP client
                    using (var client = new SmtpClient(_smtpServer, _smtpPort))
                    {
                        client.EnableSsl = _enableSsl;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.Timeout = 10000; // 10 seconds timeout

                        // Send email
                        await client.SendMailAsync(message);
                        return (true, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Convert plain text email to simple HTML format
        /// </summary>
        public string ConvertToHtml(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            // Replace line breaks with <br> tags
            string html = plainText.Replace(Environment.NewLine, "<br>");

            // Add basic HTML structure
            html = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; font-size: 14px; line-height: 1.5; }}
    </style>
</head>
<body>
    {html}
</body>
</html>";

            return html;
        }

        #region Support Inspection Record Email

        /// <summary>
        /// Generate HTML for inspection history
        /// </summary>
        public string GenerateInspectionHistoryHtml(List<InspectionRecord> records)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<h2>Vehicle Inspection History</h2>");
            sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse;'>");

            // Header row
            sb.AppendLine("<tr style='background-color: #f0f0f0; font-weight: bold;'>");
            sb.AppendLine("<th>Date</th>");
            sb.AppendLine("<th>Station</th>");
            sb.AppendLine("<th>Inspector</th>");
            sb.AppendLine("<th>Result</th>");
            sb.AppendLine("<th>CO₂ Emission</th>");
            sb.AppendLine("<th>HC Emission</th>");
            sb.AppendLine("<th>Comments</th>");
            sb.AppendLine("</tr>");

            // Data rows
            foreach (var record in records)
            {
                string rowStyle = record.Result == Constants.RESULT_PASSED
                    ? "background-color: #e6ffe6;" // Light green for passed
                    : record.Result == Constants.RESULT_FAILED
                        ? "background-color: #ffe6e6;" // Light red for failed
                        : record.Result == Constants.RESULT_CANCELLED
                            ? "background-color: #f2f2f2; color: #888888;" // Grey for cancelled
                            : string.Empty; // Default

                sb.AppendLine($"<tr style='{rowStyle}'>");
                sb.AppendLine($"<td>{record.InspectionDate?.ToString("yyyy-MM-dd") ?? "N/A"}</td>");
                sb.AppendLine($"<td>{record.Station.FullName}</td>");
                sb.AppendLine($"<td>{record.Inspector.FullName}</td>");
                sb.AppendLine($"<td>{record.Result}</td>");
                sb.AppendLine($"<td>{record.Co2emission}</td>");
                sb.AppendLine($"<td>{record.Hcemission}</td>");
                sb.AppendLine($"<td>{record.Comments}</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");

            return sb.ToString();
        }

        /// <summary>
        /// Generate PDF attachment from inspection history
        /// </summary>
        public string GenerateInspectionHistoryPdf(Vehicle vehicle, List<InspectionRecord> records)
        {
            // This would normally use a PDF library like iText or PDFsharp
            // For this example, we'll just create a text file
            string fileName = $"Inspection_History_{vehicle.PlateNumber}_{DateTime.Now:yyyyMMdd}.txt";
            string filePath = Path.Combine(Path.GetTempPath(), fileName);

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine($"INSPECTION HISTORY FOR VEHICLE {vehicle.PlateNumber}");
                    writer.WriteLine($"Brand: {vehicle.Brand}");
                    writer.WriteLine($"Model: {vehicle.Model}");
                    writer.WriteLine($"Engine Number: {vehicle.EngineNumber}");
                    writer.WriteLine($"Owner: {vehicle.Owner.FullName}");
                    writer.WriteLine(new string('-', 80));

                    foreach (var record in records)
                    {
                        writer.WriteLine($"Date: {record.InspectionDate?.ToString("yyyy-MM-dd") ?? "N/A"}");
                        writer.WriteLine($"Station: {record.Station.FullName}");
                        writer.WriteLine($"Inspector: {record.Inspector.FullName}");
                        writer.WriteLine($"Result: {record.Result}");
                        writer.WriteLine($"CO₂ Emission: {record.Co2emission}");
                        writer.WriteLine($"HC Emission: {record.Hcemission}");
                        writer.WriteLine($"Comments: {record.Comments}");
                        writer.WriteLine(new string('-', 80));
                    }
                }

                return filePath;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
