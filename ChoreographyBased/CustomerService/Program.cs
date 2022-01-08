using System.Collections.Generic;
using Core;
using CustomerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
IConfiguration Configuration =  builder.Configuration;
var connectString = Configuration["ConnectString"];
builder.Host.ConfigureServices((_, service) =>
{
    service.AddSingleton<IConnectionProvider>(new ConnectionProvider("amqp://guest:guest@localhost:5672"));
    service.AddSingleton<IPublisher<CustomerResponse,List<CustomerResponse>,CustomerResponse>, 
                        Publisher<CustomerResponse,List<CustomerResponse>,CustomerResponse>>();
    service.AddSingleton<ISubscribe<List<int>>, Subscriber<List<int>>>();
    service.AddSingleton<ICustomerCreditCard>(x =>
        new CustomerCreditCard(connectString, x.GetService<ILogger<CustomerCreditCard>>()));
    service.AddHostedService<CustomerBackground>();

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

app.MapControllers();

app.Run();