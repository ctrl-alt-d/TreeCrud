using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TreeCrud.DataLayer.Models;

namespace TreeCrud.DataLayer.Data
{
    public interface IUnitatsRepository
    {
        Task<List<Unitat>> GetRootAsync();
        Task<Unitat> GetNodeAsync(int id);
        Task<List<Unitat>> GetChildrenAsync(int id);

        Task Add(Unitat unitat);
    }
}
