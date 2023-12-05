using BookStore_CNN.Models;

namespace BookStore_CNN.Repository
{
    public interface ICategoriesRepository
    {
        Category Add(Category category);
        Category Update(Category category);
        Category Delete (int idCategory);

        Category GetCategory(int idCategory);

        IEnumerable<Category> GetAllCategories();
    }
}
