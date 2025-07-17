using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViroCureDAL.basic;

namespace DataAccess
{
    public class AccountDAO : GenericRepository<Account>
    {
        public AccountDAO(MyNewStoreDbContext context) : base(context)
        {
        }

        public async Task<Account> LoginAsync(string email, string password)
        {
            return await _context.Accounts.Include(p=> p.Role). FirstOrDefaultAsync(e => e.Email == email && e.Password == password);
        }

        public async Task<Account> GetByEmailAsync(string email)
        {
            return await _context.Accounts.FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<Account> GetByIdWithRoleAsync(int id)
        {
            return await _context.Accounts.Include(p => p.Role).FirstOrDefaultAsync(e => e.AccountId == id);
        }

        public async Task AddAsync(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

    }
}
