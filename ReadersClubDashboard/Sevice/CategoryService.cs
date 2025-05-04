using ReadersClubCore.Data;
using ReadersClubCore.Models;
using Microsoft.EntityFrameworkCore;



namespace ReadersClubDashboard.Service
{
    public class CategoryService
    {
        private readonly ReadersClubContext _context;

        public CategoryService(ReadersClubContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<bool> CategoryExistsAsync(string name)
        {
            return await _context.Categories
                .AnyAsync(c => c.Name == name && !c.IsDeleted);
        }

        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                category.IsDeleted = true;
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
        }

    }
}
