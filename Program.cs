using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

// 1. CORS zerbitzua gehitue
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection ez da aurkitu appsettings.json fitxategian.");

builder.Services.AddSingleton(connectionString);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 2. CORS middleware-a aktibatu (app.UseAuthorization() baino lehenago jarri behar da)
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Run();