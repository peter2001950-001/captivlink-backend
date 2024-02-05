using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Captivlink.Api.Utility
{
    public class TrimInputStrings : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var (key, value) in context.ActionArguments.ToList())
            {
                if (value is string val)
                {
                    if (!string.IsNullOrEmpty(val))
                    {
                        context.ActionArguments[key] = val.Trim();
                    }

                    continue;
                }

                var argType = value.GetType();
                if (!argType.IsClass)
                {
                    continue;
                }

                TrimAllStringsInObject(value, argType);
            }
        }

        private void TrimAllStringsInObject(object arg, Type argType)
        {
            var stringProperties = argType.GetProperties()
                .Where(p => p.PropertyType == typeof(string));

            foreach (var stringProperty in stringProperties)
            {
                var currentValue = stringProperty.GetValue(arg, null) as string;
                if (!string.IsNullOrEmpty(currentValue))
                {
                    stringProperty.SetValue(arg, currentValue.Trim(), null);
                }
            }
        }
    }
}
