using labs10_11.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace labs10_11.Data
{
    public interface IUnitOfWork
    {
        IRepository<Person> Persons { get; }
        IRepository<Department> Departments { get; }
        Task SaveAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;

        public IRepository<Person> Persons { get; }
        public IRepository<Department> Departments { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Persons = new Repository<Person>(_context);
            Departments = new Repository<Department>(_context);
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();

        public async Task BeginTransactionAsync() => _transaction = await _context.Database.BeginTransactionAsync();

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
            if (_transaction != null)
                await _transaction.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }
    }
}
