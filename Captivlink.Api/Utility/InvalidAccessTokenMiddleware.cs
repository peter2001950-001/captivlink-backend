using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Captivlink.Api.Utility
{
    public class InvalidAccessTokenMiddleware
    {
            private readonly RequestDelegate next;

            public InvalidAccessTokenMiddleware(RequestDelegate next)
            {
                this.next = next;
            }

            public async Task InvokeAsync(HttpContext httpContext)
            {
                await next(httpContext);

                if (IsTokenInvalidResponse(httpContext) &&
                    !HasResponseCORSHeaders(httpContext))
                {
                    var origin = httpContext.Request.Headers["Referer"].ToString();

                    if (!string.IsNullOrEmpty(origin))
                    {
                        if (origin.EndsWith('/'))
                        {
                            origin = origin.Remove(origin.Length - 1);
                        }

                        //Add default CORS headers values, so the browser won't generate CORS error
                        httpContext.Response.Headers.Add("access-control-allow-origin", origin);
                        httpContext.Response.Headers.Add("access-control-allow-headers", "*");
                        httpContext.Response.Headers.Add("access-control-allow-methods", "*");
                        httpContext.Response.Headers.Add("access-control-allow-credentials", "true");
                        httpContext.Response.Headers.Add("access-control-expose-headers", "content-disposition");
                    }
                }
            }

            private bool IsTokenInvalidResponse(HttpContext httpContext)
            {
                return httpContext.Response.Headers.ContainsKey("WWW-Authenticate") &&
                       httpContext.Response.Headers["WWW-Authenticate"].Any(x => x.Contains("invalid_token"));
            }

            private bool HasResponseCORSHeaders(HttpContext httpContext)
            {
                return httpContext.Response.Headers.ContainsKey("access-control-allow-origin") ||
                       httpContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin");
            }
        }
}
