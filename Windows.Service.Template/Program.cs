
using Windows.Service.Template.Application.Common.Interfaces;
using Windows.Service.Template.Persistence.Jobs;
using Windows.Service.Template.Persistence.Services;
using Quartz;
using Serilog;

namespace Windows.Service.Template;

public class Program
{
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.Async(wt => wt.Console())
            .CreateLogger();

        try
        {
            Log.Information("Starting Test Windows Service");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "There was a problem with the service.");
        }
        finally
        {
            Log.Information("Service successfully stopped");
            Log.CloseAndFlush();
        }

        Console.WriteLine("Press any key to close!");
        Console.ReadKey();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services))
            .UseWindowsService(options =>
            {
                options.ServiceName = "Windows.Service.Template";
            })
            .ConfigureServices((httpContext, services) =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true)
                    .AddJsonFile(
                        $"appsettings.{httpContext.HostingEnvironment.EnvironmentName}.json", true, true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .Build();

                services.AddSingleton(configuration);

                services.AddTransient<IEmailService, EmailService>();

                services.AddQuartz(q =>
                {
                    q.UseMicrosoftDependencyInjectionJobFactory();
                    // Just use the name of your job that you created in the Jobs folder.
                    var jobKey = new JobKey("SendEmailReminderJob");
                    q.AddJob<SendEmailReminderJob>(opts => opts.WithIdentity(jobKey));

                    q.AddTrigger(opts => opts
                            .ForJob(jobKey)
                            .WithIdentity("SendEmailReminderJob-trigger")
                            //This Cron interval can be described as "run every minute" (when second is zero)
                            //.WithCronSchedule("0 0 23 ? * *") //runs every day at 11pm
                            .WithCronSchedule("0 0/2 * ? * *")
                    );
                });
                services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

                //services.AddHostedService<Worker>();
            });
}