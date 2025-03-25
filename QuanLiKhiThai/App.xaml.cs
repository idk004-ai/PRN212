using Microsoft.Extensions.DependencyInjection;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.Helper;
using QuanLiKhiThai.ViewModel;
using QuanLiKhiThai.Views;
using System;
using System.Windows;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;
        private ExpirationService _expirationService;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // register services: Transient
            services.AddTransient<IUserDAO, UserDAO>();
            services.AddTransient<ILogDAO, LogDAO>();
            services.AddTransient<INotificationDAO, NotificationDAO>();
            services.AddTransient<IVehicleDAO, VehicleDAO>();
            services.AddTransient<IInspectionAppointmentDAO, InspectionAppointmentDAO>();
            services.AddTransient<IInspectionRecordDAO, InspectionRecordDAO>();
            services.AddTransient<Lazy<IInspectionRecordDAO>>(sp =>
                new Lazy<IInspectionRecordDAO>(() => sp.GetRequiredService<IInspectionRecordDAO>()));
            services.AddTransient<IStationInspectorDAO, StationInspectorDAO>();
            services.AddTransient<IInspectionValidator, InspectionValidator>();
            services.AddTransient<IViolationRecordDAO, ViolationRecordDAO>();
            services.AddTransient<ValidationService>();


            // register services: Singleton
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<LogsViewManager>();
            services.AddSingleton<LogMonitorHelper>();
            services.AddSingleton<InspectionRules>();
            services.AddSingleton<IValidationNotifier, MessageBoxValidationNotifier>();
            services.AddSingleton<IValidationLogger, DatabaseValidationLogger>();
            services.AddSingleton<PaginationService>();
            services.AddSingleton<EmailService>();
            services.AddSingleton<ExpirationService>();


            // register windows
            services.AddTransient<MainWindow>();
            services.AddTransient<Login>();
            services.AddTransient<EditProfileWindow>();
            services.AddTransient<OwnerHome>();
            services.AddTransient<StationHome>();
            services.AddTransient<VehicleLookupWindow>();
            services.AddTransient<VehicleHistory>();
            services.AddTransient<RegisterVehicle>();
            services.AddTransient<ScheduleTestWindow>();
            services.AddTransient<NeededCheckVehicleList>();
            services.AddTransient<InspectorVehicleListWindow>();
            services.AddTransient<LogsMonitorWindow>();
            services.AddTransient<ReportWindow>();
            services.AddTransient<VerificationWindow>();
            services.AddTransient<Func<int, InspectorManagementWindow>>(provider =>
                (stationId) =>
                {
                    var userDAO = provider.GetRequiredService<IUserDAO>();
                    var stationInspectorDAO = provider.GetRequiredService<IStationInspectorDAO>();
                    var navigationService = provider.GetRequiredService<INavigationService>();
                    var inspectionRecordDAO = provider.GetRequiredService<IInspectionRecordDAO>();
                    var inspectionAppointment = provider.GetRequiredService<IInspectionAppointmentDAO>();
                    return new InspectorManagementWindow(navigationService, userDAO, stationInspectorDAO, stationId, inspectionRecordDAO, inspectionAppointment);
                });
            services.AddTransient<Func<VehicleCheckViewModel, AssignInspectorWindow>>(provider =>
                (viewModel) => {
                    var userDAO = provider.GetRequiredService<IUserDAO>();
                    var vehicleDAO = provider.GetRequiredService<IVehicleDAO>();
                    var inspectionAppointmentDAO = provider.GetRequiredService<IInspectionAppointmentDAO>();
                    var inspectionRecordDAO = provider.GetRequiredService<IInspectionRecordDAO>();
                    var validationService = provider.GetRequiredService<ValidationService>();
                    return new AssignInspectorWindow(viewModel, userDAO, vehicleDAO, inspectionAppointmentDAO, inspectionRecordDAO, validationService);
                });
            services.AddTransient<Func<(InspectionRecord record, bool isViewMode), VehicleInspectionDetailsWindow>>(provider =>
                (parameters) =>
                {
                    var inspectionRecordDAO = provider.GetRequiredService<IInspectionRecordDAO>();
                    return new VehicleInspectionDetailsWindow(parameters.record, parameters.isViewMode, inspectionRecordDAO);
                });
            services.AddTransient<Func<VehicleCheckViewModel, VehicleDetailWindow>>(provider =>
                (viewModel) =>
                {
                    var vehicleDAO = provider.GetRequiredService<IVehicleDAO>();
                    var inspectionRecordDAO = provider.GetRequiredService<IInspectionRecordDAO>();
                    var inspectionAppointmentDAO = provider.GetRequiredService<IInspectionAppointmentDAO>();
                    var paginationService = provider.GetRequiredService<PaginationService>();
                    var validationService = provider.GetRequiredService<ValidationService>();
                    var logDAO = provider.GetRequiredService<ILogDAO>();
                    return new VehicleDetailWindow(viewModel, vehicleDAO, inspectionRecordDAO, inspectionAppointmentDAO, paginationService, validationService, logDAO);
                });
            services.AddTransient<Func<int, AddInspectorWindow>>(provider => 
                (stationId) =>
                {
                    var userDAO = provider.GetRequiredService<IUserDAO>();
                    var stationInspectorDAO = provider.GetRequiredService<IStationInspectorDAO>();
                    return new AddInspectorWindow(userDAO, stationInspectorDAO, stationId);
                });
            services.AddTransient<Func<(int StationId, int InspectorId), AddInspectorWindow>>(provider =>
                (parameters) =>
                {
                    var userDAO = provider.GetRequiredService<IUserDAO>();
                    var stationInspectorDAO = provider.GetRequiredService<IStationInspectorDAO>();
                    return new AddInspectorWindow(userDAO, stationInspectorDAO, parameters.StationId, parameters.InspectorId);
                });
            services.AddTransient<Func<Vehicle, ViolationRecordWindow>>(provider => 
                (vehicle) =>
                {
                    var iviolationRecordDAO = provider.GetRequiredService<IViolationRecordDAO>();
                    return new ViolationRecordWindow(vehicle, iviolationRecordDAO);
                });
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Lấy MainWindow từ DI container
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();

            //var logsManager = _serviceProvider.GetRequiredService<LogsViewManager>();
            //if (logsManager.AutoShowOnStartup)
            //{
            //    logsManager.ShowLogsWindow(forceShow: true);
            //}

            // Initialize ExpirationService
            _expirationService = _serviceProvider.GetRequiredService<ExpirationService>();
            _expirationService.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //var logsManager = _serviceProvider.GetRequiredService<LogsViewManager>();
            //logsManager.CloseLogsWindow();

            // Stop ExpirationService
            _expirationService?.Stop();

            base.OnExit(e);
        }

        public static T GetService<T>() where T : notnull
        {
            return ((App)Current)._serviceProvider.GetRequiredService<T>();
        }
    }

}
