using Microsoft.AspNetCore.Authorization;
using ServiceA.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add services to the container.
builder.Services.AddHttpContextAccessor(); // ลงทะเบียน IHttpContextAccessor

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "UserView",
        policy => policy.Requirements.Add(new PermissionRequirement("Manage Users", "View"))
    );
    options.AddPolicy(
        "UserCreate",
        policy => policy.Requirements.Add(new PermissionRequirement("Manage Users", "Create"))
    );
    options.AddPolicy(
        "UserEdit",
        policy => policy.Requirements.Add(new PermissionRequirement("Manage Users", "Update"))
    );
});
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
await app.RunAsync();
