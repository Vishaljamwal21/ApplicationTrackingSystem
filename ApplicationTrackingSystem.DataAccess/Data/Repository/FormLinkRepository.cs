﻿using ApplicationTrackingSystem.Data;
using ApplicationTrackingSystem.DataAccess.Data.Repository.IRepository;
using ApplicationTrackingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTrackingSystem.DataAccess.Data.Repository
{
    public class FormLinkRepository : Repository<FormLinks>, IFormLinkRepository
    {
        private readonly ApplicationDbContext _context;
        public FormLinkRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
