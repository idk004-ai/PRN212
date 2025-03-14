using Microsoft.Extensions.DependencyInjection;
using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuanLiKhiThai.DAO
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void NavigateTo<T>() where T : Window
        {
            var window = _serviceProvider.GetRequiredService<T>();
            window.Show();
        }

        public void NavigateTo<T, TParameter>(TParameter parameter) where T : Window
        {
            try
            {
                var factoryType = typeof(Func<,>).MakeGenericType(typeof(TParameter), typeof(T));
                if (_serviceProvider.GetService(factoryType) is Delegate factory)
                {
                    var window = factory.DynamicInvoke(parameter) as T;
                    if (window != null)
                    {
                        window.Show();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                LogNavigationError<T>($"Factory method error:  {ex.Message}");
            }

            // Generic approach that attempts to find a constructor with the parameter type
            try
            {
                var constructor = typeof(T).GetConstructor(new[] { typeof(TParameter) });
                if (constructor != null)
                {
                    // Create instance with the parameter
                    var window = (T)constructor.Invoke(new object[] { parameter });
                    window.Show();
                    return;
                }
            }
            catch (Exception ex)
            {
                LogNavigationError<T>(ex.Message);
            }

            // Fallback to standard creation if no matching constructor
            LogNavigationError<T>($"No constructor found that accepts parameter of type {typeof(TParameter).Name}");
            NavigateTo<T>();
        }

        public void NavigateTo(string viewName)
        {
            Type? type = Type.GetType($"QuanLiKhiThai.{viewName}");
            if (type != null)
            {
                var window = (Window)_serviceProvider.GetRequiredService(type);
                window.Show();
            }
            else
            {
                LogNavigationError(viewName, "View not found or not a Window");
            }
        }

        public void NavigateTo(string viewName, object parameter)
        {
            // Implementation for string-based navigation with parameters
            // This would require a more complex approach to match parameters with constructors
            Type? type = Type.GetType($"QuanLiKhiThai.{viewName}");
            if (type != null)
            {
                try
                {
                    // Try to find a constructor that accepts the parameter type
                    var constructor = type.GetConstructor(new[] { parameter.GetType() });
                    if (constructor != null)
                    {
                        var window = (Window)constructor.Invoke(new[] { parameter });
                        window.Show();
                        return;
                    }

                    // If no constructor found, try to get from service provider
                    LogNavigationError(viewName, $"No constructor found that accepts parameter of type {parameter.GetType().Name}");
                    NavigateTo(viewName);
                }
                catch (Exception ex)
                {
                    LogNavigationError(viewName, ex.Message);
                }
            }
            else
            {
                LogNavigationError(viewName, "View not found or not a Window");
            }
        }

        private void LogNavigationError<T>(string message)
        {
            LogNavigationError(typeof(T).Name, message);
        }

        private void LogNavigationError(string viewName, string message)
        {
            Log newLog = new Log
            {
                UserId = UserContext.Current.UserId,
                Action = $"Navigate to {viewName} failed: {message}",
                Timestamp = DateTime.Now
            };
            //App.GetService<ILogDAO>().Add(newLog);
            // You could also show a message box here for debugging
            // MessageBox.Show($"Navigation error: {message}", "Navigation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
