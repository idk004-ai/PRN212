using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;

namespace QuanLiKhiThai.Views
{
    /// <summary>
    /// Interaction logic for AppointmentCancellationEmailWindow.xaml
    /// </summary>
    public partial class AppointmentCancellationEmailWindow : Window
    {
        // Properties to store vehicle and appointment information
        private Vehicle _vehicle;
        private InspectionAppointment _appointment;
        private User _station;
        private EmailService _emailService;

        // Property to store cancellation reason entered by user
        public string CancellationReason { get; private set; } = string.Empty;

        // Property to track whether the user confirmed cancellation
        public bool CancellationConfirmed { get; private set; } = false;

        // Dictionary of email templates
        private Dictionary<string, string> _emailTemplates = new Dictionary<string, string>();

        /// <summary>
        /// Constructor with vehicle info and appointment
        /// </summary>
        public AppointmentCancellationEmailWindow(
            Vehicle vehicle, 
            InspectionAppointment appointment)
        {
            InitializeComponent();

            _vehicle = vehicle;
            _appointment = appointment;
            _station = appointment.Station;
            _emailService = new EmailService();

            InitializeTemplates();
            LoadData();

            // Set button event handlers
            btnCancel.Click += BtnCancel_Click;
            btnSendCancel.Click += BtnSendCancel_Click;
            btnPreviewEmail.Click += BtnPreviewEmail_Click;
            cmbCancellationReason.SelectionChanged += CmbCancellationReason_SelectionChanged;
            cmbMessageTemplate.SelectionChanged += cmbMessageTemplate_SelectionChanged;
        }

        /// <summary>
        /// Initialize email templates
        /// </summary>
        private void InitializeTemplates()
        {
            // Safety Concerns template
            _emailTemplates["Safety Concerns"] = @"Dear [Owner Name],

We regret to inform you that your vehicle inspection appointment scheduled for [Date] at [Station Name] has been cancelled.

After reviewing your vehicle's inspection history and current status, our system has flagged potential safety concerns that need to be addressed before proceeding with a standard inspection.

Your vehicle has been noted to have the following issue(s):
- Potential safety concerns based on prior inspections
- Need for specialized inspection procedure

Please contact our customer service at [Station Phone] to discuss these concerns and schedule a specialized pre-inspection assessment.

We apologize for any inconvenience this may cause.

Best regards,
[Station Name] Inspection Team";

            // Multiple Prior Cancellations template
            _emailTemplates["Multiple Prior Cancellations"] = @"Dear [Owner Name],

We regret to inform you that your vehicle inspection appointment scheduled for [Date] at [Station Name] has been cancelled.

Our records indicate that this vehicle has had multiple previous inspection cancellations. Due to this pattern, we are unable to proceed with a standard inspection at this time.

For vehicles with repeated cancellations, we require:
- A pre-inspection assessment
- Additional documentation regarding previous repair work
- Verification of compliance with all safety standards

Please contact our customer service at [Station Phone] to discuss the next steps.

We apologize for any inconvenience this may cause.

Best regards,
[Station Name] Inspection Team";

            // Documentation Issues template
            _emailTemplates["Documentation Issues"] = @"Dear [Owner Name],

We regret to inform you that your vehicle inspection appointment scheduled for [Date] at [Station Name] has been cancelled.

Our review indicates that there are issues with the documentation required for your vehicle's inspection. To proceed with an inspection, we require:
- Complete vehicle registration documentation
- Proof of ownership
- Previous inspection records (if applicable)
- Proper identification documents

Please ensure all documentation is complete and up-to-date before scheduling a new appointment.

If you have any questions, please contact our customer service at [Station Phone].

We apologize for any inconvenience this may cause.

Best regards,
[Station Name] Inspection Team";

            // Custom Message template (blank for user to fill)
            _emailTemplates["Custom Message"] = @"Dear [Owner Name],

We regret to inform you that your vehicle inspection appointment scheduled for [Date] at [Station Name] has been cancelled.

[Enter your custom cancellation message here]

Please contact our customer service at [Station Phone] if you have any questions or would like to schedule a new appointment.

We apologize for any inconvenience this may cause.

Best regards,
[Station Name] Inspection Team";
        }

        /// <summary>
        /// Load vehicle and owner data into form
        /// </summary>
        private void LoadData()
        {
            // Set window title with vehicle info
            Title = $"Cancel Appointment - {_vehicle.PlateNumber}";

            // Load vehicle information
            txtPlateNumber.Text = _vehicle.PlateNumber;
            txtBrandModel.Text = $"{_vehicle.Brand} {_vehicle.Model}";
            txtEngineNo.Text = _vehicle.EngineNumber;

            // Load owner information
            txtOwnerName.Text = _vehicle.Owner.FullName;
            txtOwnerEmail.Text = _vehicle.Owner.Email;
            txtOwnerPhone.Text = _vehicle.Owner.Phone;

            // Load appointment information
            txtStation.Text = _station.FullName;
            txtScheduledDate.Text = _appointment.ScheduledDateTime.ToString("MMM dd, yyyy - hh:mm tt");

            // Set email subject
            txtEmailSubject.Text = $"Important: Cancellation of Inspection for Vehicle {_vehicle.PlateNumber}";

            // Set subtitle
            txtSubtitle.Text = $"Send notification to {_vehicle.Owner.FullName} about appointment cancellation";

            // Set default template
            cmbMessageTemplate.SelectedIndex = 0;
            UpdateEmailTemplate();
        }

        /// <summary>
        /// Update the email content based on selected template
        /// </summary>
        private void UpdateEmailTemplate()
        {
            if (cmbMessageTemplate.SelectedItem is ComboBoxItem selectedItem)
            {
                string? templateName = selectedItem.Content.ToString();
                if (templateName != null && _emailTemplates.TryGetValue(templateName, out string template))
                {
                    // Replace placeholders with actual data
                    string emailContent = template
                        .Replace("[Owner Name]", _vehicle.Owner.FullName)
                        .Replace("[Date]", _appointment.ScheduledDateTime.ToString("MMM dd, yyyy - hh:mm tt"))
                        .Replace("[Station Name]", _station.FullName)
                        .Replace("[Station Phone]", _station.Phone);

                    txtEmailContent.Text = emailContent;
                }
            }
        }

        /// <summary>
        /// Handle template selection change
        /// </summary>
        private void cmbMessageTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEmailTemplate();
        }

        /// <summary>
        /// Update email content when cancellation reason changes
        /// </summary>
        private void CmbCancellationReason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Map cancellation reason to appropriate template
            if (cmbCancellationReason.SelectedItem is ComboBoxItem selectedReason)
            {
                string reasonContent = selectedReason.Content.ToString();

                // Select appropriate template based on reason
                if (reasonContent == "Vehicle has serious safety issues")
                {
                    SetTemplateSelection("Safety Concerns");
                }
                else if (reasonContent == "Multiple previous cancellations")
                {
                    SetTemplateSelection("Multiple Prior Cancellations");
                }
                else if (reasonContent == "Incomplete vehicle documentation")
                {
                    SetTemplateSelection("Documentation Issues");
                }
                else
                {
                    SetTemplateSelection("Custom Message");
                }
            }
        }

        /// <summary>
        /// Set the template selection in the combo box
        /// </summary>
        private void SetTemplateSelection(string templateName)
        {
            foreach (ComboBoxItem item in cmbMessageTemplate.Items)
            {
                if (item.Content.ToString() == templateName)
                {
                    cmbMessageTemplate.SelectedItem = item;
                    break;
                }
            }
        }

        /// <summary>
        /// Handle preview email button click
        /// </summary>
        private void BtnPreviewEmail_Click(object sender, RoutedEventArgs e)
        {

            // Prepare email data
            string senderEmail = _station.FullName;
            string recipientEmail = _vehicle.Owner.Email;
            string subject = txtEmailSubject.Text;
            string body = txtEmailContent.Text;

            // Check if attachments are included
            List<string>? attachments = null;
            if (chkIncludeHistory.IsChecked == true)
            {
                var inspectionHistory = _vehicle.InspectionRecords.ToList();
                if (inspectionHistory.Count > 0)
                {
                    string attachmentPath = _emailService.GenerateInspectionHistoryPdf(_vehicle, inspectionHistory);
                    if (!string.IsNullOrEmpty(attachmentPath))
                    {
                        attachments = new List<string> { attachmentPath };
                    }
                }
            }

            // Create a preview window with all information
            var previewWindow = new EmailPreviewWindow(
                recipientEmail,
                subject,
                body,
                senderEmail,
                false, // isHtml
                attachments);

            // Show preview as dialog
            previewWindow.Owner = this;
            previewWindow.ShowDialog();
        }

        /// <summary>
        /// Handle cancel button click
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            CancellationConfirmed = false;
            Close();
        }

        /// <summary>
        /// Handle send email and cancel button click
        /// </summary>
        private async void BtnSendCancel_Click(object sender, RoutedEventArgs e)
        {
            // Confirm with user
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to cancel this appointment and send the cancellation email?",
                "Confirm Cancellation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Get cancellation reason
                if (cmbCancellationReason.SelectedItem is ComboBoxItem selectedItem)
                {
                    CancellationReason = selectedItem.Content.ToString();

                    if (CancellationReason == "Other (please specify)")
                    {
                        // Use email content as reason if "Other" is selected
                        CancellationReason = txtEmailContent.Text;
                    }
                }

                // Disable the send button to prevent multiple clicks
                btnSendCancel.IsEnabled = false;
                btnSendCancel.Content = "Sending...";

                try
                {
                    // Prepare email data
                    string recipientEmail = _vehicle.Owner.Email;
                    string recipientName = _vehicle.Owner.FullName;
                    string subject = txtEmailSubject.Text;
                    string body = txtEmailContent.Text;
                    bool sendCopyToSender = chkSendCopy.IsChecked ?? false;

                    // Create attachments if history is requested
                    List<string>? attachments = null;
                    if (chkIncludeHistory.IsChecked == true)
                    {
                        var inspectionHistory = _vehicle.InspectionRecords.ToList();
                        if (inspectionHistory.Count > 0)
                        {
                            string attachmentPath = _emailService.GenerateInspectionHistoryPdf(_vehicle, inspectionHistory);
                            if (!string.IsNullOrEmpty(attachmentPath))
                            {
                                attachments = new List<string> { attachmentPath };
                            }
                        }
                    }

                    // Send email
                    var sendResult = await _emailService.SendEmailAsync(
                        recipientEmail: recipientEmail,
                        recipientName: recipientName,
                        subject: subject,
                        body: body,
                        isBodyHtml: false,
                        ccEmails: null,
                        sendCopyToSender: sendCopyToSender,
                        attachmentPaths: attachments);

                    if (sendResult.Success)
                    {
                        // Show success message
                        MessageBox.Show(
                            "Cancellation email has been sent successfully.",
                            "Email Sent",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        CancellationConfirmed = true;
                        Close();
                    }
                    else
                    {
                        // Show error message
                        MessageBox.Show(
                            $"Failed to send email: {sendResult.ErrorMessage}\n\n" +
                            "The appointment will still be cancelled, but you may need to notify the customer manually.",
                            "Email Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        // Still consider cancellation confirmed since we want to proceed with appointment cancellation
                        CancellationConfirmed = true;
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"An error occurred: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    btnSendCancel.IsEnabled = true;
                    btnSendCancel.Content = "Send Email & Cancel";
                }
            }
        }
    }
}
