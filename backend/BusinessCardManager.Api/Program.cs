using BusinessCardManager.Api.Data;
using BusinessCardManager.Api.Exporting.Interfaces;
using BusinessCardManager.Api.Exporting.Services;
using BusinessCardManager.Api.Importing.Interfaces;
using BusinessCardManager.Api.Importing.Parsers;
using BusinessCardManager.Api.Importing.Qr;
using BusinessCardManager.Api.Importing.Services;
using BusinessCardManager.Api.Importing.Validators;
using BusinessCardManager.Api.Services;
using BusinessCardManager.Api.Validators;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IBusinessCardValidator, BusinessCardValidator>();
builder.Services.AddScoped<IBusinessCardPayloadParser, BusinessCardPayloadParser>();
builder.Services.AddScoped<IQrCodeReader, ZxingQrCodeReader>();
builder.Services.AddScoped<IImportFileValidator, ImportFileValidator>();
builder.Services.AddScoped<IImportedBusinessCardValidator, ImportedBusinessCardValidator>();
builder.Services.AddScoped<IBusinessCardImportService, BusinessCardImportService>();
builder.Services.AddScoped<IBusinessCardService, BusinessCardService>();
builder.Services.AddScoped<CsvBusinessCardFileWriter>();
builder.Services.AddScoped<XmlBusinessCardFileWriter>();
builder.Services.AddScoped<IBusinessCardExportService, BusinessCardExportService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AngularClient");
app.MapControllers();

app.Run();
