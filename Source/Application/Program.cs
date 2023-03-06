using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Starting host ...");

try
{
	var builder = WebApplication.CreateBuilder(args);

	builder.Host.UseSerilog((hostBuilderContext, serviceProvider, loggerConfiguration) =>
	{
		loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration);
		loggerConfiguration.ReadFrom.Services(serviceProvider);
	});

	builder.Services.AddRazorPages();

	var app = builder.Build();

	app.UseDeveloperExceptionPage();
	app.UseStaticFiles();
	app.UseRouting();
	app.MapRazorPages();

	app.Run();
}
catch(Exception exception)
{
	Log.Fatal(exception, "Host terminated unexpectedly.");
}
finally
{
	Log.Information("Stopping host ...");
	Log.CloseAndFlush();
}