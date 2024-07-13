using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentEmail.Core.Interfaces;

namespace InspireWebApp.SpaBackend.Emails;

public class AppEmailTemplateRenderer : ITemplateRenderer
{
    public string Parse<T>(string templatePath, T model, bool isHtml = true)
    {
        if (model is not IDictionary<string, string> dictionary)
            throw new AggregateException($"Only {typeof(IDictionary<string, string>)} is supported as model");

        var template = LoadTemplate(templatePath);

        foreach ((var key, var value) in dictionary) template = template.Replace($"[({key})]", value);

        return template;
    }

    public Task<string> ParseAsync<T>(string template, T model, bool isHtml = true)
    {
        throw new Exception("Not supported");
    }

    private static string LoadTemplate(string file)
    {
        var stream = typeof(AppEmailTemplateRenderer).Assembly
            .GetManifestResourceStream("InspireWebApp.SpaBackend.Emails.Templates." + file);

        if (stream == null) throw new ArgumentException($"{file} does not exist or is not embedded");

        return new StreamReader(stream).ReadToEnd();
    }
}