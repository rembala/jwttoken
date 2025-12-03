using AspnetCoreRestApi.Configurations;
using AspnetCoreRestApi.Core.IConfiguration;
using AspnetCoreRestApi.Data;
using AspnetCoreRestApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);

var tokenValidationParms = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    RequireExpirationTime = false,
};

builder.Services.AddSingleton(tokenValidationParms);

builder.Services.AddApiVersioning(opt =>
{
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.DefaultApiVersion = ApiVersion.Default;
    opt.ReportApiVersions = true;

    opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddCorrelationIdManager();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = tokenValidationParms;
});

builder.Services.AddIdentity<IdentityUser, IdentityRole> (options =>
{
    options.SignIn.RequireConfirmedAccount = true;    
}).AddEntityFrameworkStores<ApiDbContext>();

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCorrelationIdMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();
