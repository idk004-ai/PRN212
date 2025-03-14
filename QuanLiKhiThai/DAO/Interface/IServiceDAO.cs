namespace QuanLiKhiThai.DAO.Interface
{
    public interface IServiceDAO<T>
    {
        IEnumerable<T> GetAll();
        T? GetById(int id);
        bool Add(T entity);
        bool Update(T entity);
        bool Delete(int id);
    }
}
