using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BokApi.Data;

namespace BokApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Railway (och andra containerhostar) anger porten via PORT
            var port = Environment.GetEnvironmentVariable("PORT");
            if (!string.IsNullOrEmpty(port))
            {
                builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
            }

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:4200",
                            "https://bokapi.netlify.app"
                          )
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.ParameterLocation.Header,
                    Description = "Skriv in din JWT-token här (utan \"Bearer\" framför)."
                });

                options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
                {
                    [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
                });
            });

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(GetConnectionString(builder.Configuration)));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Skapar/uppdaterar tabellerna i Railway-databasen vid start
            using (var scope = app.Services.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
            }

            app.UseForwardedHeaders();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                // Railway terminerar TLS framför appen, så omdirigering behövs bara lokalt
                app.UseHttpsRedirection();
            }

            app.UseCors("AllowAngularApp");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }

        // Railway ger DATABASE_URL (postgres://user:pass@host:port/db), som Npgsql inte läser direkt
        private static string? GetConnectionString(IConfiguration configuration)
        {
            var url = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (string.IsNullOrEmpty(url))
            {
                return configuration.GetConnectionString("DefaultConnection");
            }

            var uri = new Uri(url);
            var userInfo = uri.UserInfo.Split(':', 2);
            return new Npgsql.NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.Port > 0 ? uri.Port : 5432,
                Database = uri.AbsolutePath.TrimStart('/'),
                Username = Uri.UnescapeDataString(userInfo[0]),
                Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : null,
                SslMode = Npgsql.SslMode.Prefer
            }.ConnectionString;
        }
    }
}