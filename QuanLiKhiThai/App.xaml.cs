using Microsoft.Extensions.DependencyInjection;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.Helper;
using QuanLiKhiThai.ViewModel;
using QuanLiKhiThai.Views;
using System.Windows;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

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
            services.AddTransient<IStationInspectorDAO, StationInspectorDAO>();

            // register services: Singleton
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<LogsViewManager>();
            services.AddSingleton<LogMonitorHelper>();

            // register windows
            services.AddTransient<MainWindow>();
            services.AddTransient<Login>();
            services.AddTransient<OwnerHome>();
            services.AddTransient<StationHome>();
            services.AddTransient<VehicleHistory>();
            services.AddTransient<RegisterVehicle>();
            services.AddTransient<ScheduleTestWindow>();
            services.AddTransient<NeededCheckVehicleList>();
            services.AddTransient<InspectorVehicleListWindow>();
            services.AddTransient<LogsMonitorWindow>();
            services.AddTransient<ReportWindow>();
            services.AddTransient<Func<int, InspectorManagementWindow>>(provider =>
                (stationId) =>
                {
                    var userDAO = provider.GetRequiredService<IUserDAO>();
                    var stationInspectorDAO = provider.GetRequiredService<IStationInspectorDAO>();
                    var navigationService = provider.GetRequiredService<INavigationService>();
                    return new InspectorManagementWindow(navigationService, userDAO, stationInspectorDAO, stationId);
                });
            services.AddTransient<Func<VehicleCheckViewModel, AssignInspectorWindow>>(provider =>
                (viewModel) => {
                    var userDAO = provider.GetRequiredService<IUserDAO>();
                    var vehicleDAO = provider.GetRequiredService<IVehicleDAO>();
                    var inspectionAppointmentDAO = provider.GetRequiredService<IInspectionAppointmentDAO>();
                    var inspectionRecordDAO = provider.GetRequiredService<IInspectionRecordDAO>();
                    return new AssignInspectorWindow(viewModel, userDAO, vehicleDAO, inspectionAppointmentDAO, inspectionRecordDAO);
                });
            services.AddTransient<Func<InspectionRecord, VehicleInspectionDetailsWindow>>(provider =>
                (inspectionRecord) =>
                {
                    var inspectionRecordDAO = provider.GetRequiredService<IInspectionRecordDAO>();
                    return new VehicleInspectionDetailsWindow(inspectionRecord, inspectionRecordDAO);
                });
            services.AddTransient<Func<VehicleCheckViewModel, VehicleDetailWindow>>(provider =>
                (viewModel) =>
                {
                    var vehicleDAO = provider.GetRequiredService<IVehicleDAO>();
                    return new VehicleDetailWindow(viewModel, vehicleDAO);
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
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Lấy MainWindow từ DI container
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            // Khởi tạo LogsViewManager và mở cửa sổ log nếu cần
            var logsManager = _serviceProvider.GetRequiredService<LogsViewManager>();
            if (logsManager.AutoShowOnStartup)
            {
                logsManager.ShowLogsWindow(forceShow: true);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var logsManager = _serviceProvider.GetRequiredService<LogsViewManager>();
            logsManager.CloseLogsWindow();
            base.OnExit(e);
        }

        public static T GetService<T>() where T : notnull
        {
            return ((App)Current)._serviceProvider.GetRequiredService<T>();
        }
    }

}
