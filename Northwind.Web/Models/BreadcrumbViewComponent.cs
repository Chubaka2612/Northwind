using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

public class Breadcrumb
{
    public string Label { get; }
    public string Action { get; }
    public string Controller { get; }
    public object RouteId { get; }

    public Breadcrumb(string label, string action, string controller, object routeId = null)
    {
        Label = label;
        Action = action;
        Controller = controller;
        RouteId = routeId;
    }
}

public class BreadcrumbViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ViewContext viewContext)
    {
        var routeValues = viewContext?.RouteData?.Values;

        string controller = routeValues["controller"]?.ToString();
        string action = routeValues["action"]?.ToString();
        object id = routeValues.ContainsKey("id") ? routeValues["id"] : null;
        string title = viewContext.ViewData["Title"]?.ToString();

        var breadcrumbs = new List<Breadcrumb>
        {
            new Breadcrumb("Home", "Index", "Home")
        };

        if (!string.IsNullOrEmpty(controller))
        {
            if (!string.Equals(action, "Index", StringComparison.OrdinalIgnoreCase))
            {
                breadcrumbs.Add(new Breadcrumb(controller, "Index", controller));
            }

            breadcrumbs.Add(new Breadcrumb(title ?? action, action, controller, id));
        }

        return View("BreadcrumbViewComponent", breadcrumbs);
    }
}
