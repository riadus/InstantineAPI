using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using InstantineAPI.AutoMapper;
using InstantineAPI.Core;
using InstantineAPI.Core.Database;
using InstantineAPI.Core.Domain;
using InstantineAPI.Core.Email;
using InstantineAPI.Core.Photos;
using InstantineAPI.Data;
using InstantineAPI.Database;
using InstantineAPI.Database.Repositories;
using InstantineAPI.Domain;
using InstantineAPI.Email;
using InstantineAPI.Formatter;
using InstantineAPI.Middelware;
using InstantineAPI.Photos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace InstantineAPI
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
            var csvFormatterOptions = new CsvFormatterOptions();
            services.AddMvc(options =>
            {
                options.InputFormatters.Add(new CsvInputFormatter(csvFormatterOptions));
                options.OutputFormatters.Add(new CsvOutputFormatter(csvFormatterOptions));
                options.FormatterMappings.SetMediaTypeMappingForFormat("csv", Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("text/csv"));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(
                x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddCors();
            services.AddEntityFrameworkSqlite()
                .AddDbContext<InstantineDbContext>(x => x.UseSqlite("Data Source=InstantineDbContext.db"));

            services.AddSwaggerGen(options =>
            {
                options.DocInclusionPredicate((version, apiDescription) =>
                {
                    var values = apiDescription.RelativePath
                        .Split('/')
                        .Select(v => v.Replace("v{version}", version));

                    apiDescription.RelativePath = string.Join("/", values);

                    var versionParameter = apiDescription.ParameterDescriptions.SingleOrDefault(p => p.Name == "version");

                    if (versionParameter != null)
                    {
                        apiDescription.ParameterDescriptions.Remove(versionParameter);
                    }

                    return true;
                });

                options.DescribeAllEnumsAsStrings();
                options.DescribeStringEnumsInCamelCase();
                options.SwaggerDoc("v1", new Info { Title = "Instantine API", Version = "v1" });

                // Swagger 2.+ support
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                options.AddSecurityRequirement(security);
            });

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new DtoMappingProfile());
            });
            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<ICodeGenerator, CodeGenerator>();
            services.AddTransient<IPhotoService, PhotoService>();
            services.AddSingleton<IFtpService, FtpService>();
            services.AddTransient<IReactionService, ReactionService>();
            services.AddTransient<IAlbumService, AlbumService>();
            services.AddTransient<IConstants, Constants>();
            services.AddTransient<IPermissionsService, PermissionsService>();
            services.AddTransient<IClock>(x => new Clock(() => DateTime.UtcNow));
            services.AddTransient<IGuid>(x => new GuidGenerator(Guid.NewGuid));
            services.AddTransient<IPasswordService, PasswordService>();
            services.AddTransient<IEncryptionService, EncryptionService>();
            services.AddTransient<IRandomStringGenerator, RandomStringGenerator>();

            services.RegisterRepositories();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, IUserService userService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Instantine API V1");
            });
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();

            await userService.RegisterDefaultAdmin();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static void RegisterRepositories(this IServiceCollection service)
        {
            service.AddTransient<IUnitOfWork, UnitOfWork>();
            service.AddTransient<IRepository<Photo>, PhotosRepository>();
            service.AddTransient<IRepository<Like>, LikesRepository>();
            service.AddTransient<IRepository<Comment>, CommentsRepository>();
            service.AddTransient<IRepository<User>, UsersRepository>();
            service.AddTransient<IRepository<Album>, AlbumRepository>();
        }
    }
}
