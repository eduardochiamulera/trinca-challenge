using Domain;
using CrossCutting;
using Microsoft.Extensions.Hosting;
using Serverless_Api.Middlewares;
using Services;

var host = new HostBuilder()
    .ConfigureServices(services =>
    {
        services.AddEventStore();
        services.AddServicesDependencies();
		services.AddDomainDependencies();
    })
    .ConfigureFunctionsWorkerDefaults(builder => builder.UseMiddleware<AuthMiddleware>())
    .Build();

host.Run();
