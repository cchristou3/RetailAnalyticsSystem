using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using FluentEmail.Core.Interfaces;
using FluentEmail.MailKitSmtp;
using InspireWebApp.SpaBackend.Data;
using InspireWebApp.SpaBackend.Emails;
using InspireWebApp.SpaBackend.Features.Identity;
using InspireWebApp.SpaBackend.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace InspireWebApp.SpaBackend;

public static class Bootstrapper
{
    public static WebApplication BuildApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
            app.Services.GetRequiredService<IMapper>()
                .ConfigurationProvider
                .AssertConfigurationIsValid();

        ConfigurePipeline(app);

        return app;
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        
        builder.Logging.AddConsole();

        services.AddDbContext<ApplicationDbContext>(contextBuilder =>
        {
            contextBuilder.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                sqlServerBuilder => { sqlServerBuilder.UseNodaTime(); }
            );
        });

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;

                // Simplify the requirements for dev/testing/demo
                options.Password.RequiredLength = 2;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddSignInManager()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                // options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddCookie(IdentityConstants.ApplicationScheme, options =>
            {
                options.LoginPath = "/auth/login";
                options.Events.OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync;

                static Func<RedirectContext<CookieAuthenticationOptions>, Task> HandleApiRequest(
                    int statusCode,
                    Func<RedirectContext<CookieAuthenticationOptions>, Task> original
                )
                {
                    return redirectContext =>
                    {
                        if (redirectContext.HttpContext.IsApiRequest())
                        {
                            redirectContext.Response.StatusCode = statusCode;
                            return Task.CompletedTask;
                        }

                        // TODO: Add a parameter to force a full page reload
                        // instead of internal angular router navigation
                        return original(redirectContext);
                    };
                }

                options.Events.OnRedirectToLogin = HandleApiRequest(
                    StatusCodes.Status401Unauthorized, options.Events.OnRedirectToLogin
                );
                options.Events.OnRedirectToAccessDenied = HandleApiRequest(
                    StatusCodes.Status403Forbidden, options.Events.OnRedirectToAccessDenied
                );
            });

        services.AddScoped<SignInManager<ApplicationUser>, ApplicationSignInManager>();

        if (builder.Environment.IsDevelopment()) services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddAutoMapper(
            config => { config.AddCollectionMappers(); },
            typeof(Bootstrapper).Assembly
        );

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        if (builder.Environment.IsDevelopment())
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerDocument();
        }

        // In production, the Angular files will be served from this directory
        services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientAppDist"; });

        ConfigureEmailServices(services, builder.Configuration);
    }

    private static void ConfigureEmailServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAuthMailer, AuthMailer>();

        services.AddSingleton<ITemplateRenderer, AppEmailTemplateRenderer>();

        EmailConfiguration emailConf = new();
        configuration.GetSection("Email").Bind(emailConf);

        services.AddFluentEmail(
            emailConf.DefaultFromEmail, emailConf.DefaultFromName
        );

        if (emailConf.SmtpServerHost != null && emailConf.SmtpServerPort != null)
        {
            // TODO: Check that the below comment is still true.
            // For reasons unknown to me, the ISenders are registered as scoped by default
            // while the IFluentEmail(Factory) itself above is using transient lifetime
            // which the service container injected.
            // This means that IFluentEmail(Factory) fails to fetch the ISenders when injected into
            // a singleton (which our mailers are, since they handle their own concurrency)

            if (emailConf.SmtpLogin != null && emailConf.SmtpPassword != null)
                services.AddTransient<ISender>(x => new MailKitSender(new SmtpClientOptions
                {
                    Server = emailConf.SmtpServerHost,
                    Port = emailConf.SmtpServerPort.Value,

                    User = emailConf.SmtpLogin,
                    Password = emailConf.SmtpPassword,

                    RequiresAuthentication = true
                }));
            else
                /*builder.AddMailKitSender(new SmtpClientOptions
                {
                    Server = emailConf.SmtpServerHost,
                    Port = emailConf.SmtpServerPort.Value,
                });*/
                services.AddTransient<ISender>(x => new MailKitSender(new SmtpClientOptions
                {
                    Server = emailConf.SmtpServerHost,
                    Port = emailConf.SmtpServerPort.Value
                }));
        }
    }

    private static void ConfigurePipeline(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }

        app.UseHttpsRedirection();

        // Note: the position of this middleware is important.
        // 1) We want to avoid the auth/controllers pipeline when in the end we will just serve a static file
        // 2) UseSpa() causes a rewrite of the request to the index.html, which breaks serving of static assets
        if (!app.Environment.IsDevelopment()) app.UseSpaStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
    }
}