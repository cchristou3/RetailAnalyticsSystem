using Microsoft.AspNetCore.Mvc;

namespace InspireWebApp.SpaBackend.Helpers;

public static class UrlHelpers
{
    public static string RelativeToAbsolute(this IUrlHelper urlHelper, string relative)
    {
        var request = urlHelper.ActionContext.HttpContext.Request;

        return string.Concat(
            request.Scheme,
            "://",
            request.Host.ToUriComponent(),
            relative
        );
    }
}