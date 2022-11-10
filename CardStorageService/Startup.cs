using AutoMapper;
using CardStorageService.Data;
using CardStorageService.Mappings;
using CardStorageService.Models.Requests;
using CardStorageService.Models.Validators;
using CardStorageService.Services;
using CardStorageService.Services.Impl;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

namespace CardStorageService
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
            #region Configure gRPC

            services.AddGrpc();

            #endregion

            #region Configure FluentValidator

            services.AddScoped<IValidator<AuthenticationRequest>, AuthenticationRequestValidator>();
            services.AddScoped<IValidator<CreateCardRequest>, CreateCardRequestValidator>();
            services.AddScoped<IValidator<CreateClientRequest>, CreateClientRequestValidator>();

            #endregion

            #region Configure Mapper

            var mapperConfiguration = new MapperConfiguration(mp => mp.AddProfile(new MappingProfile()));
            var mapper = mapperConfiguration.CreateMapper();
            services.AddSingleton(mapper);

            #endregion

            #region Configure Options Services

            services.Configure<DatabaseOptions>(options =>
            {
                Configuration.GetSection("Settings:DatabaseOptions").Bind(options);
            });

            #endregion

            #region Configure Services

            services.AddSingleton<IAuthenticateService, AuthenticateService>();

            services.AddScoped<IClientRepositoryService, ClientRepository>();
            services.AddScoped<ICardRepositoryService, CardRepository>();

            #endregion

            #region Configure Logging

            services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.All | HttpLoggingFields.RequestQuery;
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
                logging.RequestHeaders.Add("Authorization");
                logging.RequestHeaders.Add("X-Real-IP");
                logging.RequestHeaders.Add("X-Forwarded-For");
            });

            #endregion

            #region Configure EF DBContext Service (CardStorageService Database)

            services.AddDbContext<CardStorageServiceDbContext>(options =>
            {
                options.UseSqlServer(Configuration["Settings:DatabaseOptions:ConnectionString"]);
            });

            #endregion

            services.AddControllers();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new
                TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthenticateService.SecretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CardStorageService", Version = "v1" });
                c.CustomOperationIds(SwaggerUtils.OperationIdProvider);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme(Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CardStorageService v1"));
            }

            app.UseRouting();

            app.UseWhen(ctx => ctx.Request.ContentType != "application/grpc",
                builder =>
                {
                    builder.UseHttpLogging();
                });

            //app.UseHttpLogging();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ClientService>();
                endpoints.MapControllers();
            });
        }
    }
}
