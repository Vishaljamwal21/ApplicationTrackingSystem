using ApplicationTrackingSystem.Data;
using ApplicationTrackingSystem.DataAccess.Data.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTrackingSystem.DataAccess.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context=context;
        }

        public void Save()
        {
           _context.SaveChanges();
        }
    }
}
