using Dapr.Actors.Client;
using ReminderMicroservice_.Actors;
using ReminderMicroservice_.Services.Interfaces;
using ReminderMicroservice_.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddDapr();
builder.Services.AddDaprClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IActorProxyFactory, ActorProxyFactory>();
builder.Services.AddSingleton<IEmailService, EmailService>();

// Add Dapr Actors
builder.Services.AddActors(options =>
{
  options.Actors.RegisterActor<ReminderActor>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCloudEvents();

app.MapControllers();
app.MapActorsHandlers(); // Configure actors handling
app.MapSubscribeHandler();

app.Run();