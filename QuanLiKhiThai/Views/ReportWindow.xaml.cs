using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using OfficeOpenXml;
using System.ComponentModel;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.Context;
using DrawingColor = System.Drawing.Color;

namespace QuanLiKhiThai.Views
{
    public partial class ReportWindow : Window, INotifyPropertyChanged
    {
        // Dependency Injection for database service
        private readonly IVehicleDAO _vehicleDAO;
        private readonly IInspectionRecordDAO _inspectionRecordDAO;


        // Properties for chart bindings
        private SeriesCollection _pieSeries;
        public SeriesCollection PieSeries
        {
            get { return _pieSeries; }
            set
            {
                _pieSeries = value;
                OnPropertyChanged(nameof(PieSeries));
            }
        }

        private SeriesCollection _columnSeries;
        public SeriesCollection ColumnSeries
        {
            get { return _columnSeries; }
            set
            {
                _columnSeries = value;
                OnPropertyChanged(nameof(ColumnSeries));
            }
        }

        private string[] _columnLabels;
        public string[] ColumnLabels
        {
            get { return _columnLabels; }
            set
            {
                _columnLabels = value;
                OnPropertyChanged(nameof(ColumnLabels));
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Database service (you'll need to implement or inject this)
        // private readonly IInspectionService _inspectionService;

        public ReportWindow(IVehicleDAO vehicleDAO, IInspectionRecordDAO inspectionRecordDAO)
        {
            InitializeComponent();

            // Dependency Injection for database service
            this._vehicleDAO = vehicleDAO;
            this._inspectionRecordDAO = inspectionRecordDAO;

            // Set DataContext
            DataContext = this;

            // Initialize charts with empty data
            InitializeCharts();

            // Set default date range (last 30 days)
            datePicker_StartDate.SelectedDate = DateTime.Now.AddDays(-30);
            datePicker_EndDate.SelectedDate = DateTime.Now;
        }

        private DrawingColor ToDrawingColor(Color wpfColor)
        {
            return DrawingColor.FromArgb(wpfColor.A, wpfColor.R, wpfColor.G, wpfColor.B);
        }

        private void InitializeCharts()
        {
            // Initialize pie chart with empty data
            PieSeries = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Passed",
                    Values = new ChartValues<double> { 0 },
                    Fill = new SolidColorBrush(Color.FromRgb(76, 175, 80)), // Green
                    DataLabels = true,
                    LabelPoint = point => $"{point.Y} ({point.Participation:P1})"
                },
                new PieSeries
                {
                    Title = "Failed",
                    Values = new ChartValues<double> { 0 },
                    Fill = new SolidColorBrush(Color.FromRgb(244, 67, 54)), // Red
                    DataLabels = true,
                    LabelPoint = point => $"{point.Y} ({point.Participation:P1})"
                },
                new PieSeries
                {
                    Title = "Pending",
                    Values = new ChartValues<double> { 0 },
                    Fill = new SolidColorBrush(Color.FromRgb(33, 150, 243)), // Blue
                    DataLabels = true,
                    LabelPoint = point => $"{point.Y} ({point.Participation:P1})"
                }
            };

            // Initialize column chart with empty data
            ColumnSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Inspections",
                    Values = new ChartValues<double> { 0 },
                    Fill = new SolidColorBrush(Color.FromRgb(75, 138, 208)), // Blue
                }
            };

            ColumnLabels = new[] { "No Data" };
        }

        private void btnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateDateRange())
                return;

            LoadReportData();
        }

        private bool ValidateDateRange()
        {
            if (!datePicker_StartDate.SelectedDate.HasValue || !datePicker_EndDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select both start and end dates.", "Missing Dates", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (datePicker_StartDate.SelectedDate > datePicker_EndDate.SelectedDate)
            {
                MessageBox.Show("Start date cannot be later than end date.", "Invalid Date Range", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void LoadReportData()
        {
            // Define date range
            DateTime startDate = datePicker_StartDate.SelectedDate.Value;
            DateTime endDate = datePicker_EndDate.SelectedDate.Value.AddDays(1).AddTicks(-1); // Include full end date

            // Get inspection data for selected date range
            // In a real application, you would retrieve this from your database
            List<InspectionRecord> inspections = GetInspectionsFromDatabase(startDate, endDate);

            // Update summary counts
            UpdateSummaryStatistics(inspections);

            // Update charts
            UpdateCharts(inspections, startDate, endDate);

            // Populate data grids
            LoadInspectionDetailsGrid(inspections);
            LoadInspectorPerformanceGrid(inspections);
        }

        private List<InspectionRecord> GetInspectionsFromDatabase(DateTime startDate, DateTime endDate)
        {
            List<InspectionRecord> demoData = _inspectionRecordDAO.GetRecordInDateRange(startDate, endDate);
            return demoData;
        }

        private void UpdateSummaryStatistics(List<InspectionRecord> inspections)
        {
            if (inspections == null)
                return;

            // Calculate summary statistics
            int totalVehicles = inspections.Select(i => i.Vehicle?.PlateNumber).Distinct().Count();
            int totalInspections = inspections.Count;
            int passedCount = inspections.Count(i => i.Result == "Pass");
            int failedCount = inspections.Count(i => i.Result == "Fail");
            int pendingCount = inspections.Count(i => i.Result == "Pending");
            double passRate = totalInspections > 0 ? (double)passedCount / totalInspections * 100 : 0;

            // Update UI
            txtTotalVehicles.Text = totalVehicles.ToString();
            txtTotalInspections.Text = totalInspections.ToString();
            txtPassedCount.Text = passedCount.ToString();
            txtFailedCount.Text = failedCount.ToString();
            txtPendingCount.Text = pendingCount.ToString();
            txtPassRate.Text = $"{passRate:F2}%";
        }

        private void UpdateCharts(List<InspectionRecord> inspections, DateTime startDate, DateTime endDate)
        {
            UpdatePieChart(inspections);
            UpdateBarChart(inspections, startDate, endDate);
        }

        private void UpdatePieChart(List<InspectionRecord> inspections)
        {
            if (inspections == null || inspections.Count == 0)
                return;

            // Group inspections by result
            int passedCount = inspections.Count(i => i.Result == "Pass");
            int failedCount = inspections.Count(i => i.Result == "Fail");
            int pendingCount = inspections.Count(i => i.Result == "Pending");

            // Update pie chart values
            PieSeries[0].Values = new ChartValues<double> { passedCount };
            PieSeries[1].Values = new ChartValues<double> { failedCount };
            PieSeries[2].Values = new ChartValues<double> { pendingCount };
        }

        private void UpdateBarChart(List<InspectionRecord> inspections, DateTime startDate, DateTime endDate)
        {
            if (inspections == null || inspections.Count == 0)
                return;

            // Group inspections by date
            var inspectionsByDate = inspections
                .GroupBy(i => i.InspectionDate)
                .OrderBy(g => g.Key)
                .ToList();

            // Prepare data for chart
            var dailyCounts = new ChartValues<double>();
            var dateLabels = new List<string>();

            // Determine appropriate date format based on date range
            string dateFormat = (endDate - startDate).TotalDays > 7 ? "MM/dd" : "MM/dd";

            // Fill in zeros for missing dates to ensure continuous timeline
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var group = inspectionsByDate.FirstOrDefault(g => g.Key == date);
                int count = group?.Count() ?? 0;

                dailyCounts.Add(count);
                dateLabels.Add(date.ToString(dateFormat));
            }

            // Update column chart
            ColumnSeries[0].Values = dailyCounts;
            ColumnLabels = dateLabels.ToArray();
        }

        private void LoadInspectionDetailsGrid(List<InspectionRecord> inspections)
        {
            dataGridInspections.ItemsSource = null; // Clear first to refresh
            dataGridInspections.ItemsSource = inspections;
        }

        private void LoadInspectorPerformanceGrid(List<InspectionRecord> inspections)
        {
            if (inspections == null || inspections.Count == 0)
            {
                dataGridInspectors.ItemsSource = null;
                return;
            }

            // Group by inspector and calculate statistics
            var inspectorPerformance = inspections
                .GroupBy(i => i.Inspector?.UserId)
                .Select(g => new InspectorPerformanceViewModel
                {
                    InspectorId = g.First().InspectorId,
                    InspectorName = g.Count() > 0 ? g.First().Inspector?.FullName : "Unknown",
                    TotalCount = g.Count(),
                    PassedCount = g.Count(i => i.Result == "Pass"),
                    FailedCount = g.Count(i => i.Result == "Fail")
                })
                .OrderByDescending(i => i.TotalCount)
                .ToList();

            // Calculate pass rates
            foreach (var item in inspectorPerformance)
            {
                item.PassRate = item.TotalCount > 0 ? (double)item.PassedCount / item.TotalCount * 100 : 0;
            }

            dataGridInspectors.ItemsSource = inspectorPerformance;
        }

        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel();
        }

        private void ExportToExcel()
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = $"Inspection_Report_{DateTime.Now:yyyyMMdd}"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                    using (var package = new ExcelPackage())
                    {
                        // Summary worksheet
                        var summarySheet = package.Workbook.Worksheets.Add("Summary");

                        // Report header
                        summarySheet.Cells[1, 1].Value = "Inspection Statistics Report";
                        summarySheet.Cells[1, 1].Style.Font.Size = 16;
                        summarySheet.Cells[1, 1].Style.Font.Bold = true;

                        summarySheet.Cells[2, 1].Value = "Period:";
                        summarySheet.Cells[2, 2].Value = $"{datePicker_StartDate.SelectedDate:dd/MM/yyyy} - {datePicker_EndDate.SelectedDate:dd/MM/yyyy}";

                        // Summary data
                        summarySheet.Cells[4, 1].Value = "Total Vehicles:";
                        summarySheet.Cells[4, 2].Value = txtTotalVehicles.Text;

                        summarySheet.Cells[5, 1].Value = "Total Inspections:";
                        summarySheet.Cells[5, 2].Value = txtTotalInspections.Text;

                        summarySheet.Cells[6, 1].Value = "Passed:";
                        summarySheet.Cells[6, 2].Value = txtPassedCount.Text;

                        summarySheet.Cells[7, 1].Value = "Failed:";
                        summarySheet.Cells[7, 2].Value = txtFailedCount.Text;

                        summarySheet.Cells[8, 1].Value = "Pending:";
                        summarySheet.Cells[8, 2].Value = txtPendingCount.Text;

                        summarySheet.Cells[9, 1].Value = "Pass Rate:";
                        summarySheet.Cells[9, 2].Value = txtPassRate.Text;

                        // Format the summary section
                        using (var range = summarySheet.Cells[4, 1, 9, 1])
                        {
                            range.Style.Font.Bold = true;
                        }

                        // Create Inspection Details worksheet
                        var inspectionsSheet = package.Workbook.Worksheets.Add("Inspection Details");

                        // Headers
                        inspectionsSheet.Cells[1, 1].Value = "Record ID";
                        inspectionsSheet.Cells[1, 2].Value = "Vehicle";
                        inspectionsSheet.Cells[1, 3].Value = "Owner";
                        inspectionsSheet.Cells[1, 4].Value = "Date";
                        inspectionsSheet.Cells[1, 5].Value = "Inspector";
                        inspectionsSheet.Cells[1, 6].Value = "Result";
                        inspectionsSheet.Cells[1, 7].Value = "CO2 Level";
                        inspectionsSheet.Cells[1, 8].Value = "HC Level";
                        inspectionsSheet.Cells[1, 9].Value = "Comments";

                        // Format headers
                        using (var range = inspectionsSheet.Cells[1, 1, 1, 9])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(ToDrawingColor(Color.FromRgb(200, 230, 255)));
                        }

                        // Export data from dataGridInspections
                        var inspections = dataGridInspections.ItemsSource as IEnumerable<InspectionRecord>;
                        if (inspections != null)
                        {
                            int row = 2;
                            foreach (var record in inspections)
                            {
                                inspectionsSheet.Cells[row, 1].Value = record.RecordId;
                                inspectionsSheet.Cells[row, 2].Value = record.Vehicle?.PlateNumber;
                                inspectionsSheet.Cells[row, 3].Value = record.Vehicle?.Owner?.FullName;
                                inspectionsSheet.Cells[row, 4].Value = record.InspectionDate;
                                inspectionsSheet.Cells[row, 4].Style.Numberformat.Format = "dd/MM/yyyy";
                                inspectionsSheet.Cells[row, 5].Value = record.Inspector?.FullName;
                                inspectionsSheet.Cells[row, 6].Value = record.Result;
                                inspectionsSheet.Cells[row, 7].Value = record.Co2emission;
                                inspectionsSheet.Cells[row, 8].Value = record.Hcemission;
                                inspectionsSheet.Cells[row, 9].Value = record.Comments;

                                // Color-code the result column
                                if (record.Result == "Pass")
                                    inspectionsSheet.Cells[row, 6].Style.Font.Color.SetColor(ToDrawingColor(Color.FromRgb(0, 128, 0)));
                                else if (record.Result == "Fail")
                                    inspectionsSheet.Cells[row, 6].Style.Font.Color.SetColor(ToDrawingColor(Color.FromRgb(192, 0, 0)));
                                else if (record.Result == "Pending")
                                    inspectionsSheet.Cells[row, 6].Style.Font.Color.SetColor(ToDrawingColor(Color.FromRgb(0, 0, 192)));

                                row++;
                            }

                            // Auto-fit columns
                            for (int col = 1; col <= 9; col++)
                            {
                                inspectionsSheet.Column(col).AutoFit();
                            }
                        }

                        // Create Inspector Performance worksheet
                        var inspectorsSheet = package.Workbook.Worksheets.Add("Inspector Performance");

                        // Headers
                        inspectorsSheet.Cells[1, 1].Value = "Inspector Name";
                        inspectorsSheet.Cells[1, 2].Value = "Total Inspections";
                        inspectorsSheet.Cells[1, 3].Value = "Passed";
                        inspectorsSheet.Cells[1, 4].Value = "Failed";
                        inspectorsSheet.Cells[1, 5].Value = "Pass Rate";

                        // Format headers
                        using (var range = inspectorsSheet.Cells[1, 1, 1, 5])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(ToDrawingColor(Color.FromRgb(200, 230, 255)));
                        }

                        // Export data from dataGridInspectors
                        var inspectorData = dataGridInspectors.ItemsSource as IEnumerable<InspectorPerformanceViewModel>;
                        if (inspectorData != null)
                        {
                            int row = 2;
                            foreach (var inspector in inspectorData)
                            {
                                inspectorsSheet.Cells[row, 1].Value = inspector.InspectorName;
                                inspectorsSheet.Cells[row, 2].Value = inspector.TotalCount;
                                inspectorsSheet.Cells[row, 3].Value = inspector.PassedCount;
                                inspectorsSheet.Cells[row, 4].Value = inspector.FailedCount;
                                inspectorsSheet.Cells[row, 5].Value = inspector.PassRate / 100; // Export as decimal for Excel percentage formatting
                                inspectorsSheet.Cells[row, 5].Style.Numberformat.Format = "0.00%";

                                row++;
                            }

                            // Auto-fit columns
                            for (int col = 1; col <= 5; col++)
                            {
                                inspectorsSheet.Column(col).AutoFit();
                            }
                        }

                        // Save the Excel package
                        package.SaveAs(new FileInfo(saveDialog.FileName));
                    }

                    MessageBox.Show($"Report successfully exported to Excel.\nLocation: {saveDialog.FileName}",
                                    "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting to Excel: {ex.Message}",
                                   "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    // Support classes

    public class InspectorPerformanceViewModel
    {
        public int InspectorId { get; set; }
        public string InspectorName { get; set; }
        public int TotalCount { get; set; }
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }
        public double PassRate { get; set; }
        public string PassRateString => $"{PassRate:F2}%";
    }
}

