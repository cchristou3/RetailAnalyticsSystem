using System.Collections.Generic;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace InspireWebApp.SpaBackend.Emails;

public interface IAuthMailer
{
    Task<SendResponse> SendPasswordReset(string email, string callbackUrl);
    Task<SendResponse> SendEmailConfirm(string email, string callbackUrl);
}

public class AuthMailer : IAuthMailer
{
    private readonly IFluentEmailFactory _emailFactory;

    public AuthMailer(IFluentEmailFactory emailFactory)
    {
        _emailFactory = emailFactory;
    }

    public async Task<SendResponse> SendPasswordReset(string email, string callbackUrl)
    {
        Dictionary<string, string> mapDict = new()
        {
            ["callbackUrl"] = callbackUrl
        };

        return await _emailFactory.Create()
            .To(email)
            .Subject("InspireWebApp - Reset Password.")
            .UsingTemplate("password_reset.html", mapDict)
            .SendAsync();
    }

    public async Task<SendResponse> SendEmailConfirm(string email, string callbackUrl)
    {
        Dictionary<string, string> mapDict = new()
        {
            ["callbackUrl"] = callbackUrl
        };

        return await _emailFactory.Create()
            .To(email)
            .Subject("InspireWebApp - Activate your Account.")
            .UsingTemplate("account_activation.html", mapDict)
            .SendAsync();
    }
}