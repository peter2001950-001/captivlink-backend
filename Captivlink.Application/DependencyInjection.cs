using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Captivlink.Application.Interfaces.ValidatorPipelines;
using Captivlink.Application.Validators;
using Captivlink.Application.Validators.Services;
using Captivlink.Infrastructure;
using FluentValidation.AspNetCore;

namespace Captivlink.Application
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            var thisAssembly = typeof(DependencyInjection).Assembly;

            services.AddAutoMapper(thisAssembly);
            services.AddFluentValidationAutoValidation();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(thisAssembly));
            services.AddRepositories();

            RegisterValidationPipelineCommands(services);
        }

        private static void RegisterValidationPipelineCommands(IServiceCollection services)
        {
            var commandTypes = Assembly.GetAssembly(typeof(DependencyInjection))!
                .GetTypes()
                .Where(type => type.GetInterfaces().ToList().Exists(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition().IsAssignableTo(typeof(IValidatedRequest<>))))
                .Where(type => type is {IsClass: true, IsAbstract: false});

            var pipelineBehaviorGenericType = typeof(IPipelineBehavior<,>);
            var validationBehaviorGenericType = typeof(ValidationBehavior<,>);
            var resultGenericType = typeof(ValueResult<>);

            foreach (var commandType in commandTypes)
            {
                var commandResponseType = commandType.GetInterfaces().First(i =>
                        i.IsGenericType && i.GetGenericTypeDefinition().IsAssignableTo(typeof(IValidatedRequest<>)))
                    .GetGenericArguments()
                    .First();
                var resultType = resultGenericType.MakeGenericType(commandResponseType);
                var pipelineBehaviorInterface = pipelineBehaviorGenericType.MakeGenericType(commandType, resultType);
                var validationBehaviorType =
                    validationBehaviorGenericType.MakeGenericType(commandType, commandResponseType);

                services.AddScoped(pipelineBehaviorInterface, validationBehaviorType);
            }
        }
    }
}