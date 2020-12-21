using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Text;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Repositories.Accounts;
using WebAPI.Repositories.Categories;
using WebAPI.Repositories.Certificates;
using WebAPI.Repositories.Educations;
using WebAPI.Repositories.Persons;
using WebAPI.Repositories.Projects;
using WebAPI.Repositories.ProjectTechnologies;
using WebAPI.Repositories.Skills;
using WebAPI.Repositories.Technologies;
using WebAPI.Repositories.WorkHistories;
using WebAPI.Services.Accounts;
using WebAPI.Services.Categories;
using WebAPI.Services.Certificates;
using WebAPI.Services.Educations;
using WebAPI.Services.ExportFiles;
using WebAPI.Services.Persons;
using WebAPI.Services.Projects;
using WebAPI.Services.ProjectTechnologies;
using WebAPI.Services.Skills;
using WebAPI.Services.Technologies;
using WebAPI.Services.Uploads;
using WebAPI.Services.WorkHistories;
using WebAPI.Services.ImportFiles;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add service and create Policy with options
            services.AddCors();

            services.AddControllers();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation  
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "JWT Token Authentication API",
                    Description = "ASP.NET Core 3.1 Web API"
                });
                // To Enable authorization using Swagger (JWT)  
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });
            });
            //services.AddControllers(x => x.AllowEmptyInputInBodyModelBinding = true);
            services.AddMvc()
                    .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            // Get config string
            services.AddMvc();
            services.AddSingleton<IConfiguration>(Configuration);
            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                };
            });
            services.AddHttpContextAccessor();
            services.AddTransient(typeof(IExportFileService), typeof(ExportFileService));
            services.AddScoped<IPersonRepository>(x => new PersonRepository(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(typeof(IPersonService), typeof(PersonService));
            services.AddScoped<IUploadRepository>(x => new UploadRepository(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(typeof(IUploadService), typeof(UploadService));
            services.AddScoped<IAccountRepository>(x => new AccountRepository(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(typeof(IAccountService), typeof(AccountService));
            services.AddScoped<ICategoryRepository>(x => new CategoryRepository(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(typeof(ICategoryService), typeof(CategoryService));
            services.AddScoped<ITechnologyRepository>(x => new TechnologyRepository(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(typeof(ITechnologyService), typeof(TechnologyService));
            services.AddScoped<ISkillRepository>(x => new SkillRepository(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(typeof(ISkillService), typeof(SkillService));
            services.AddScoped<IProjectRepository>(x => new ProjectRepository(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(typeof(IProjectService), typeof(ProjectService));
            services.AddScoped<IProjectTechnologyRepository>(x => new ProjectTechnologyRepository(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(typeof(IProjectTechnologyService), typeof(ProjectTechnologyService));
            services.AddScoped<IEducationRepository>(x => new EducationRepository(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(typeof(IEducationService), typeof(EducationService));
            services.AddScoped<ICertificateRepository>(x => new CertificateRepository(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(typeof(ICertificateService), typeof(CertificateService));
            services.AddScoped<IWorkHistoryRepository>(x => new WorkHistoryRepository(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(typeof(IWorkHistoryService), typeof(WorkHistoryService));
            services.AddScoped(typeof(IImportService), typeof(ImportService));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            HttpContext.Configure(app.ApplicationServices.
                GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>()
            );
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/api/Error/CreateLog");
            }
            else
            {
                app.UseExceptionHandler("/api/Error/CreateLog");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

            app.UseAuthentication();
            app.UseAuthorization();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                     name: "DefaultApi",
                     pattern: "{controller}/{action}/{id?}");

                endpoints.MapControllerRoute(
                    name: "ActionApi",
                    pattern: "api/{controller}/{action}/{id?}");
            });
        }
    }
}
