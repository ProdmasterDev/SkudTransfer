using Microsoft.EntityFrameworkCore;
using SkudTransfer.Transfers;
using SkudTransferApi.Contexts;
using SkudTransferApi.Transfers;
using SkudWebApplication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped(typeof(ControllerTransfer));
builder.Services.AddScoped(typeof(WorkerGroupTransfer));
builder.Services.AddScoped(typeof(WorkerTransfer));
builder.Services.AddScoped(typeof(CardTransfer));
builder.Services.AddScoped(typeof(EventTransfer));
builder.Services.AddScoped(typeof(AccessTransfer));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(AppMappingProfile));
builder.Services.AddDbContext<OldSkudContext>(ConfigureOldContextConnection);
builder.Services.AddDbContext<NewSkudContext>(ConfigureNewContextConnection);


void ConfigureOldContextConnection(DbContextOptionsBuilder options)
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Old"))
        .EnableSensitiveDataLogging();
}

void ConfigureNewContextConnection(DbContextOptionsBuilder options)
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("New"))
        .EnableSensitiveDataLogging();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
