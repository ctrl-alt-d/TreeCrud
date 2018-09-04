using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeCrud.DataLayer.Models;

namespace TreeCrud.DataLayer.Data
{
    public class UnitatsRepository : IUnitatsRepository
    {

        private readonly AligaContext _context;

        public UnitatsRepository(AligaContext context)
        {
            _context = context;
        }

        public Task<List<Unitat>> GetChildrenAsync(int id)
        {
            return Task.FromResult(_context.Unitats.Where(x => x.ParentId == id).ToList());
        }

        public async Task<Unitat> GetNodeAsync(int id) => await _context.Unitats.FindAsync(id);

        public Task<List<Unitat>> GetRoot()
        {
            return Task.FromResult(_context.Unitats.Where(x => x.ParentId == null).ToList());
        }
    }

}