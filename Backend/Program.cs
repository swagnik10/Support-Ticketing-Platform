using Backend.DbConnection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Dependency Injection
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<NHibernate.ISession>(sp =>
{
    return NHibernateHelper.SessionFactory.OpenSession();
});

var app = builder.Build();

Log.Information("Application build scucessfully at {Time}", DateTime.Now);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// NHibernate configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
NHibernateHelper.Configure(connectionString ?? throw new Exception("Connection String can't be null"));

Log.Information("Application starting running at {Time}", DateTime.Now);

app.Run();
