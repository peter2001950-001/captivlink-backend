using AutoMapper;
using Captivlink.Application.Categories.Results;
using Captivlink.Infrastructure.Domain;

namespace Captivlink.Application.Categories.Mappings
{
    public class CategoryEntityToResultConverter : ITypeConverter<List<Category>, List<CategoryResult>>
    {
        public List<CategoryResult> Convert(List<Category> source, List<CategoryResult> destination, ResolutionContext context)
        {
            return MapCategories(null, source);  
        }

        private List<CategoryResult> MapCategories(Guid? parentId, List<Category> allCategories)
        {
            var result = new List<CategoryResult>();

            foreach (var category in allCategories.Where(x => x.Parent?.Id == parentId))
            {
                var categoryResult = new CategoryResult
                {
                    Id = category.Id,
                    Name = category.Name,
                    Children = MapCategories(category.Id, allCategories)
                };

                result.Add(categoryResult);
            }

            return result;
        }
    }
}
