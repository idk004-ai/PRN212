// Helper/WindowManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace QuanLiKhiThai.Helper
{
    public class WindowManager
    {
        private static Dictionary<Type, Window> _activeWindows = new Dictionary<Type, Window>();

        /// <summary>
        /// Close all active windows except the specified types
        /// </summary>
        /// <param name="excludeTypes">Specified window that don't need to close</param>
        public static void CloseAllExcept(params Type[] excludeTypes)
        {
            var windowsToClose = Application.Current.Windows
                .OfType<Window>()
                .Where(w => !excludeTypes.Contains(w.GetType()))
                .ToList();

            foreach (var window in windowsToClose)
            {
                try
                {
                    window.Close();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Register an active window
        /// </summary>
        public static void RegisterWindow<T>(T window) where T : Window
        {
            var type = typeof(T);
            if (_activeWindows.ContainsKey(type))
            {
                _activeWindows[type] = window;
            }
            else
            {
                _activeWindows.Add(type, window);
            }
        }

        /// <summary>
        /// Get or create an active window
        /// </summary>
        public static T GetOrCreate<T>(Func<T> factory) where T : Window
        {
            var type = typeof(T);

            if (_activeWindows.TryGetValue(type, out var window) &&
                window != null &&
                !((Window)window).IsVisible)
            {
                _activeWindows.Remove(type);
                window = null;
            }

            if (window == null)
            {
                window = factory();
                _activeWindows[type] = window;
                ((Window)window).Closed += (s, e) => {
                    _activeWindows.Remove(type);
                };
            }

            return (T)window;
        }

        public void CloseAllWindowsExcept<T>() where T : Window
        {
            WindowManager.CloseAllExcept(typeof(T));
        }

        public void CloseAllWindows()
        {
            WindowManager.CloseAllExcept(); 
        }

    }
}
