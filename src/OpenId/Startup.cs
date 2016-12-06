using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OpenId
{
    public class Startup
    {

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddMvc();

            services.AddEntityFramework()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            services.AddOpenIddict<ApplicationDbContext>()
                   // Register the ASP.NET Core MVC binder used by OpenIddict.
                   // Note: if you don't call this method, you won't be able to
                   // bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
                   .AddMvcBinders()

                   // Enable the token endpoint (required to use the password flow).
                   .EnableTokenEndpoint("/connect/token")

                   // Allow client applications to use the grant_type=password flow.
                   .AllowPasswordFlow()

                   // During development, you can disable the HTTPS requirement.
                   .DisableHttpsRequirement()

                   // Register a new ephemeral key, that is discarded when the application
                   // shuts down. Tokens signed using this key are automatically invalidated.
                   // This method should only be used during development.
                   .AddEphemeralSigningKey();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseIdentity();

            app.UseOAuthValidation();

            app.UseOpenIdConnectServer(options => {
                options.Provider = new AuthorizationProvider();

                // Enable the authorization and token endpoints.
                options.AuthorizationEndpointPath = "/connect/authorize";
                options.TokenEndpointPath = "/connect/token";
                options.AllowInsecureHttp = true;

                // Register a new ephemeral key, that is discarded when the application
                // shuts down. Tokens signed using this key are automatically invalidated.
                // This method should only be used during development.
                options.SigningCredentials.AddEphemeralKey();
                
            });

            app.UseMvc();
            
            app.UseCors(builder =>  // <- ADD THIS
               builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowAnyOrigin()
           );

            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

    
        }
    }
}
