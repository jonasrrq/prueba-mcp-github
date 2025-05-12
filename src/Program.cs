using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MigratedProject.Services;
using MigratedProject.Tools;
using MigratedProject.Utils;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly();
    })
    .ConfigureLogging(logging =>
    {
        logging.AddConsole(consoleLogOptions =>
        {
            // Configure all logs to go to stderr
            consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
        });
    });

await builder.Build().RunAsync();
