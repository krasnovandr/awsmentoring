using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module6.DbModels;

namespace Module6
{
    public class Startup
    {
        public const string AppS3BucketKey = "AppS3Bucket";

        public const string SnsTopicArn = "SnsTopicArn";

        public const string ApiEndpoint = "ApiEndpoint";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Add S3 to the ASP.NET Core dependency injection framework.
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<Amazon.S3.IAmazonS3>();
            services.AddAWSService<IAmazonSimpleNotificationService>();

            var connectionString = Configuration["DefaultConnection"];

            services.AddDbContext<FileMetadataContext>(options => options.UseSqlServer(connectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
