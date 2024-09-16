using BankAPI.DAL;
using BankAPI.Services.Implementations;
using BankAPI.Services.Interfaces;
using BankAPI.Utility;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigurationBuilder configuration = new();

builder.Services.AddDbContext<BankingDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")));
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Banking API doc",
        Version = "v1",
        Description = "We were crazy enough to build a Bank API",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Ayrat N.",
            Email = "nizamutdinov.airat02@gmail.com",
            Url = new Uri("https://github.com/NizamAir")

        }
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseSwagger();

app.UseSwaggerUI(x =>
{
    var prefix = string.IsNullOrEmpty(x.RoutePrefix) ? "." : "..";
    x.SwaggerEndpoint($"{prefix}/swagger/v1/swagger.json", "Banking API doc");
});

app.MapControllers();

app.Run();
