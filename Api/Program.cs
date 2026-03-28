using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Vulpes.Perpendicularity.Api.Configuration;
using Vulpes.Perpendicularity.Core.RegistrationExtensions;
using Vulpes.Perpendicularity.Infrastructure.Mongo;
using Vulpes.Perpendicularity.Infrastructure.RegistrationExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new LowercaseParameterTransformer()));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

MongoConfigurator.Configure();

// Dependencies.
_ = builder.Services
    .InjectInfrastructure()
    .InjectDomain();

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