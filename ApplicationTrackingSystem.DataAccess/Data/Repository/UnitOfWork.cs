using ApplicationTrackingSystem.Data;
using ApplicationTrackingSystem.DataAccess.Data.Repository.IRepository;
using ApplicationTrackingSystem.Models;
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
            JobPost = new JobPostRepository(context);
            ApplyJob = new ApplyJobRepository(context);
        }
        public IJobPostRepository JobPost { private set; get; }

        public IApplyJobRepository ApplyJob { private set; get; }

        public void Save()
        {
           _context.SaveChanges();
        }
    }
}
