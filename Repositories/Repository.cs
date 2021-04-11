using ExemploMeetingHangfire.Repositories.Interfaces;
using ExemploMeetingHangfire.Contexts;
using ExemploMeetingHangfire.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExemploMeetingHangfire.Enums;

namespace ExemploMeetingHangfire.Repositories
{
    public class Repository<T> : IRepository<T> where T : EntidadeBase
    {
        private readonly EFContext _context;
        public Repository(EFContext context)
        {
            _context = context;
        }

        public IQueryable<T> Query()
        {
            return _context.Set<T>().AsQueryable();
        }

        public IEnumerable<T> GetAllActive()
        {
            return Query().Where(x => x.Ativo);
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public T FindById(Guid id)
        {
            return Query().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<T> FindByIds(IEnumerable<Guid> ids)
        {
            return Query().Where(x => ids.Contains(x.Id));
        }

        public async Task RemoveAsync(T model) 
        {
            await SingleOperation(model, EntityState.Deleted);
        }

        public async Task RemoveAsync(IEnumerable<T> models)
        {
            await MultipleOperations(models, EntityState.Deleted);
        }

        public async Task UpdateAsync(T model)
        {
            await SingleOperation(model, EntityState.Modified);
        }

        public async Task UpdateAsync(IEnumerable<T> models)
        {
            await MultipleOperations(models, EntityState.Modified);
        }

        public async Task InsertAsync(T model)
        {
            await SingleOperation(model, EntityState.Added);
        }

        public async Task InsertAsync(IEnumerable<T> models) 
        {
            await MultipleOperations(models, EntityState.Added);
        }

        private async Task MultipleOperations(IEnumerable<T> models, EntityState state, bool autoDetect = false)
        {
            if (models is null || !models.Any())
                return;

            _context.ChangeTracker.AutoDetectChangesEnabled = autoDetect;

            foreach(var model in models)
                Set(model, state);

            await _context.SaveChangesAsync();
        }

        private async Task SingleOperation(T model, EntityState state, bool autoDetect = false)
        {
            if (model is null)
                return;

            _context.ChangeTracker.AutoDetectChangesEnabled = autoDetect;

            Set(model, state);

            await _context.SaveChangesAsync();
        }

        private void Set(T model, EntityState state)
        {
            if (state == EntityState.Added)
                _context.Add(model);
            else if (state == EntityState.Modified)
                _context.Update(model);
            else if (state == EntityState.Deleted)
                _context.Remove(model);
        }
    }
}
