using Microsoft.EntityFrameworkCore;
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
        public bool Add(StationInspector entity)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.StationInspectors.Add(entity);
                return db.SaveChanges() > 0;
            }
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<StationInspector> GetAll()
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.StationInspectors.ToList();
            }
        }

        public StationInspector? GetById(int id)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.StationInspectors.Find(id);
            }
        }

        public StationInspector? GetByStationAndInspectorId(int stationId, int inspectorId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.StationInspectors
                    .FirstOrDefault(si => si.StationId == stationId && si.InspectorId == inspectorId);
            }
        }

        public IEnumerable<StationInspector> GetByStationId(int stationId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.StationInspectors
                    .Where(si => si.StationId == stationId)
                    .ToList();
            }
        }

        public bool Update(StationInspector entity)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.StationInspectors.Update(entity);
                return db.SaveChanges() > 0;
            }
        }

        public StationInspector? GetByInspectorId(int inspectorId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.StationInspectors
                    .Include(si => si.Station)
                    .FirstOrDefault(si => si.InspectorId == inspectorId);
            }
        }
    }
}
