using System;
using System.Collections.Generic;

namespace Captivlink.Api.Utility.CategorySeeder
{
    public class CategoryModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public List<CategoryModel> Children { get; set; }
    }
}
