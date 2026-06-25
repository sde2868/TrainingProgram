using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data;
using TraineeManagement.Models;
using TraineeManagement.Services;
using TraineeManagement.Interfaces;
using SubmissionProcessor.Services;
using SubmissionProcessor.Configuration;

using SubmissionProcessor;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("DefaultConnection")));
});

builder.Services.Configure<StorageSettings>(builder.Configuration.GetSection("Storage"));
builder.Services.Configure<ProcessingOptions>(builder.Configuration.GetSection(ProcessingOptions.SectionName));

builder.Services.AddScoped<IFileStorageService, LocalFileStorageServices>();
builder.Services.AddScoped<ISubmissionProcessingService, SubmissionProcessingService>();

builder.Services.AddHostedService<Worker>();

builder.Services.AddHttpClient<ITrainingDirectoryClient, TrainingDirectoryClient>(
    client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["TrainingDirectory:BaseUrl"]!);
        client.Timeout = TimeSpan.FromSeconds(2);
    }).AddStandardResilienceHandler();
    
var host = builder.Build();
host.Run();
