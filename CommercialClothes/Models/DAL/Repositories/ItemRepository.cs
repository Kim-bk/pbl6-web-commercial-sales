using CommercialClothes.Models.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComercialClothes.Models;
using ComercialClothes.Models.DAL;
using CommercialClothes.Models.DAL.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace CommercialClothes.Models.DAL.Repositories
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        public ItemRepository(DbFactory dbFactory) : base(dbFactory)
        {
        }

        public async Task<List<Item>> GetItemById(int idItem)
        {
            return await DbSet.Where(it => it.Id == idItem).ToListAsync();
        }
    }
}

