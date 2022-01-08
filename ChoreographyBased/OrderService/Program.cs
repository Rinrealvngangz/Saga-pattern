using System.Collections.Generic;
using Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;
using OrderService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
IConfiguration Configuration =  builder.Configuration;
var connecstring = Configuration["ConnectString"];
builder.Host.ConfigureServices((_, service) =>
{
    service.AddSingleton<IConnectionProvider>(new ConnectionProvider("amqp://guest:guest@localhost:5672"));
    service.AddSingleton<IPublisher<OrderDetail,List<int>,int>, Publisher<OrderDetail,List<int>,int>>();
    service.AddSingleton<ISubscribe<List<CustomerResponse>>, Subscriber<List<CustomerResponse>>>();
    service.AddSingleton<ICreateOrder>(x =>
        new CreateOrder(connecstring, x.GetService<ILogger<OrderCreate>>()));
    
    service.AddSingleton<IDeleteOrder>(x =>
        new DeleteOrder(connecstring, x.GetService<ILogger<DeleteOrder>>()));
    
    service.AddSingleton<IUpdateProduct>(x =>
        new UpdateProduct(connecstring, x.GetService<ILogger<UpdateProduct>>()));
    service.AddHostedService<OrderBackground>();
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