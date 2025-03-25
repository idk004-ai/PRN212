using Microsoft.EntityFrameworkCore;
using QuanLiKhiThai.DAO.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiKhiThai.DAO
{
    public class ViolationRecordDAO : IViolationRecordDAO
    {
        public bool Add(ViolationRecord entity)
        {
            using (var context = new QuanLiKhiThaiContext())
            {
                context.ViolationRecords.Add(entity);
                return context.SaveChanges() > 0;
            }
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ViolationRecord> GetAll()
        {
            using (var context = new QuanLiKhiThaiContext())
            {
                return context.ViolationRecords
                    .Include(iv => iv.Vehicle)
                    .Include(iv => iv.Officer)
                    .ToList();
            }
        }

        public ViolationRecord? GetById(int id)
        {
            using (var context = new QuanLiKhiThaiContext())
            {
                return context.ViolationRecords
                    .Include(iv => iv.Vehicle)
                    .Include(iv => iv.Officer)
                    .FirstOrDefault(iv => iv.Id == id);
            }
        }

        public bool Update(ViolationRecord entity)
        {
            using (var context = new QuanLiKhiThaiContext())
            {
                context.ViolationRecords.Update(entity);
                return context.SaveChanges() > 0;
            }
        }
    }
}
