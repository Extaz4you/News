
using Microsoft.EntityFrameworkCore;
using News.Backend.Articles.Db;
using News.Backend.Articles.Services;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace News.Backend.Articles
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddStackExchangeRedisCache(o =>
            {
                o.Configuration = builder.Configuration["RedisConfig"];
                o.InstanceName = "News_Articles_";
            });

            builder.Host.UseSerilog((context, config) =>
            {
                config
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("App", context.HostingEnvironment.ApplicationName);

                config.WriteTo.File(
                    path: context.Configuration["FileLog:Path"] ?? "./logs/log-.json",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    formatter: new CompactJsonFormatter());
                config.WriteTo.Console();
                if (!string.IsNullOrEmpty(context.Configuration["Seq:Url"]))
                {
                    config.WriteTo.Seq(
                        serverUrl: context.Configuration["Seq:Url"],
                        apiKey: context.Configuration["Seq:ApiKey"]);
                }
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ArticlesContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("Connection")));
            builder.Services.AddScoped<ArticleService>(); 
            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
