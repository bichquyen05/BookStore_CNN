using BookStore_CNN.Models;

namespace BookStore_CNN.Repository
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly BookStoreCaratContext _context;
        public CategoriesRepository(BookStoreCaratContext context)
        {
            _context = context;
        }
        public Category Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return category;
        }

        public Category Delete(int idCategory)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Category> GetAllCategories()
        {
           return _context.Categories;
        }

        public Category GetCategory(int idCategory)
        {
            return _context.Categories.Find(idCategory);
        }

        public Category Update(Category category)
        {
            _context.Update(category);
            _context.SaveChanges();
            return category;
        }
    }
}
