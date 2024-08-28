using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Sentinel.CustomMiddleware;

public class SentinelHealthCheckMiddleware
{
    private readonly RequestDelegate _next;

    public SentinelHealthCheckMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync(await RenderHealthCheckView(context));
        }
        else
        {
            await _next(context);
        }
    }

    private async Task<string> RenderHealthCheckView(HttpContext context)
    {
        //var viewEngine = context.RequestServices.GetService<IRazorViewEngine>();
        //var tempDataProvider = context.RequestServices.GetService<ITempDataProvider>();
        //  var viewResult = viewEngine.FindView(new ActionContext(context, new RouteData(), new ActionDescriptor()), "HealthCheck", false);

        var viewEngine = context.RequestServices.GetService<IRazorViewEngine>();
        var tempDataProvider = context.RequestServices.GetService<ITempDataProvider>();
        var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());

        var location = "~/Views/HealthCheck.cshtml";
        var viewResult = viewEngine.GetView(location, location, false);



        if (!viewResult.Success)
        {
            throw new InvalidOperationException("Unable to find HealthCheck view.");
        }

        var model = new HealthCheckModel
        {
            Status = "Application is running",
            Timestamp = DateTime.UtcNow
        };

        using (var writer = new StringWriter())
        {
            var viewContext = new ViewContext(
                new ActionContext(context, new RouteData(), new ActionDescriptor()),
                viewResult.View,
                 new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                 {
                     Model = model
                 },
                new TempDataDictionary(context, tempDataProvider),
                writer,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return writer.ToString();
        }
    }
}


