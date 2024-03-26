using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Captivlink.Application.Categories.Results;
using Captivlink.Infrastructure.Domain;

namespace Captivlink.Application.Categories.Mappings
{
    public class CategoryMapping : Profile
    {
        public CategoryMapping()
        {
            CreateMap<List<Category>, List<CategoryResult>>().ConvertUsing<CategoryEntityToResultConverter>();
        }
    }
}
