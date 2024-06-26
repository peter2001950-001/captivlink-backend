﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Captivlink.Api.Utility.Swagger
{
    public class SwaggerDefaultOptions : IConfigureOptions<SwaggerGenOptions>
    {
        

        public void Configure(SwaggerGenOptions options)
        {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = ApiTitle, Version = "v1" });
                options.CustomSchemaIds(i => i.FullName);
                return;

        }

        protected virtual string ApiTitle => "undefined";
    }
}
