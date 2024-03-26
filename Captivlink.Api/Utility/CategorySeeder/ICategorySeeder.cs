using System.Threading.Tasks;

namespace Captivlink.Api.Utility.CategorySeeder
{
    public interface ICategorySeeder
    {
        Task SeedCategories();
    }
}
