var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("Order", c => c.BaseAddress = new Uri("http://localhost:5211"));
builder.Services.AddHttpClient("Inventory", c => c.BaseAddress = new Uri("http://localhost:5115"));
builder.Services.AddHttpClient("Notify", c => c.BaseAddress = new Uri("http://localhost:5215"));


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