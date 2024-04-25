using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Repositories.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Captivlink.Api.Utility.CategorySeeder
{
    public class CategorySeeder : ICategorySeeder
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategorySeeder> _logger;
        public CategorySeeder(ICategoryRepository categoryRepository, ILogger<CategorySeeder> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task SeedCategories()
        {
            var fileName = "Data/DummyData/categories.json";


            var json = await File.ReadAllTextAsync(fileName);
            var categories = JsonConvert.DeserializeObject<List<CategoryModel>>(json);
            var categoriesFromDb = (await _categoryRepository.FindAllAsync(null)).ToList();
            var touchedCategories = new List<Category>();
            foreach (var category in categories)
            {
                await ProcessCategory(category, null, categoriesFromDb, touchedCategories);
            }

            var categoriesToDelete = categoriesFromDb.Select(x => x.Id).Except(touchedCategories.Select(p => p.Id));
            foreach (var categoryGuid in categoriesToDelete)
            {
                await _categoryRepository.DeleteAsync(categoryGuid);
            }

            json = JsonConvert.SerializeObject(categories, Formatting.Indented);
            await File.WriteAllTextAsync(fileName, json);

            _logger.LogInformation("Category seed is completed");
        }

        public async Task ProcessCategory(CategoryModel categoryModel, Category parent, List<Category> categoriesFromDb, List<Category> touchedCategories)
        {
            var existingCategory = categoriesFromDb.FirstOrDefault(x => x.Name == categoryModel.Name && x.Parent == parent);
            if (existingCategory == null)
            {
                existingCategory = await _categoryRepository.AddAsync(new Category
                {
                    Name = categoryModel.Name,
                    Parent = parent
                });
                categoryModel.Id = existingCategory.Id;
            }
            else
            {
                touchedCategories.Add(existingCategory);
            }

            if (categoryModel.Children != null && categoryModel.Children.Any())
            {
                foreach (var child in categoryModel.Children)
                {
                    await ProcessCategory(child, existingCategory, categoriesFromDb, touchedCategories);
                }
            }   
        }
       
    }
}
