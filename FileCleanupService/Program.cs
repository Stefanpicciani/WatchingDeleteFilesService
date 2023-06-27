using FileCleanupServices;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<FileCleanupService>();
    })
    .Build();

await host.RunAsync();
