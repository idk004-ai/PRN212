using QuanLiKhiThai.DAO.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiKhiThai.DAO
{
    public class StationInspectorDAO : IStationInspectorDAO
    {
        bool IServiceDAO<StationInspector>.Add(StationInspector entity)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.StationInspectors.Add(entity);
                return db.SaveChanges() > 0;
            }
        }

        bool IServiceDAO<StationInspector>.Delete(int id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<StationInspector> IServiceDAO<StationInspector>.GetAll()
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.StationInspectors.ToList();
            }
        }

        StationInspector? IServiceDAO<StationInspector>.GetById(int id)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.StationInspectors.Find(id);
            }
        }

        StationInspector? IStationInspectorDAO.GetByStationAndInspectorId(int stationId, int inspectorId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.StationInspectors
                    .FirstOrDefault(si => si.StationId == stationId && si.InspectorId == inspectorId);
            }
        }

        IEnumerable<StationInspector> IStationInspectorDAO.GetByStationId(int stationId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.StationInspectors
                    .Where(si => si.StationId == stationId)
                    .ToList();
            }
        }

        bool IServiceDAO<StationInspector>.Update(StationInspector entity)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.StationInspectors.Update(entity);
                return db.SaveChanges() > 0;
            }
        }
    }
}
