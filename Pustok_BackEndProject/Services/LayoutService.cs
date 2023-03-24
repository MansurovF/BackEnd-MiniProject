﻿using Microsoft.EntityFrameworkCore;
using Pustok_BackEndProject.DataAccessLayer;
using Pustok_BackEndProject.Interfaces;
using Pustok_BackEndProject.Models;

namespace Pustok_BackEndProject.Services
{
    public class LayoutService:ILayoutService
    {
        private readonly AppDbContext _context;

        public LayoutService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _context.Categories
                .Include(c => c.Children.Where(c => c.IsDeleted == false))
                .Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();
        }

        public async Task<IDictionary<string, string>> GetSettings()
        {
            IDictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

            return settings;
        }
    }
}
