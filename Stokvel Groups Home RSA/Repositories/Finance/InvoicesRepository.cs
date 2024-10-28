﻿using Stokvel_Groups_Home_RSA.Data;
using Stokvel_Groups_Home_RSA.Interface.IRepo.Finance;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Repositories.Finance
{
    public class InvoicesRepository : Repository<Invoice>, IInvoicesRepository
    {

        private readonly ApplicationDbContext _context;
        public InvoicesRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
        }
    }
}