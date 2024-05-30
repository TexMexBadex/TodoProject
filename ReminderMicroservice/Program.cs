using System.Runtime.InteropServices.ComTypes;
using Dapr;
using Dapr.Actors.Runtime;
using Dapr.Actors;
using ReminderMicroservice.Actors;using ReminderMicroservice.Models;
using ReminderMicroservice.Services;
using ReminderMicroservice.Services.Interfaces;
using ReminderMicroservice.Services.ReminderMicroservice.Services;

var builder = WebApplication.CreateBuilder(args);

// Register actors
builder.Services.AddActors(options =>
{
  options.Actors.RegisterActor<ReminderActor>();

  options.ReentrancyConfig = new ActorReentrancyConfig()
  {
    Enabled = true,
    MaxStackDepth = 32,
  };
  
});

// Add services to the container
builder.Services.AddControllers().AddDapr();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IReminderService, ReminderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseCloudEvents();

app.UseEndpoints(endpoints =>
{
  endpoints.MapSubscribeHandler();
  endpoints.MapControllers();
  endpoints.MapActorsHandlers();
});


app.Run();