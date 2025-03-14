using System.Windows;

namespace QuanLiKhiThai.DAO.Interface
{
    public interface INavigationService
    {
        void NavigateTo<T>() where T : Window;
        void NavigateTo<T, TParameter>(TParameter parameter) where T : Window;
        void NavigateTo(string viewName);
        void NavigateTo(string viewName, object parameter);
    }
}
