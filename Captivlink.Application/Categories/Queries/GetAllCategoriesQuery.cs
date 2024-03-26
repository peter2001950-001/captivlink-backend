using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Application.Categories.Results;
using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;

namespace Captivlink.Application.Categories.Queries
{
    public class GetAllCategoriesQuery: IQuery<List<CategoryResult>?>
    {
    }
}
