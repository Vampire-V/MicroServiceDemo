using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add Authentication
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            ),
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// เพิ่ม Reverse Proxy จาก YARP
builder
    .Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy")); // โหลดจาก appsettings.json

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

// Middleware ดึง Claims และ Forward ไปใน Header
app.Use(
    async (context, next) =>
    {
        if (context.User.Identity is { IsAuthenticated: true })
        {
            var claims = context.User.Claims.ToDictionary(c => c.Type, c => c.Value);

            // Forward Claims ใน Header
            foreach (var claim in claims)
            {
                context.Request.Headers[$"X-Claim-{claim.Key}"] = claim.Value;
            }
        }

        await next();
    }
);

// กำหนด Middleware สำหรับ Reverse Proxy
app.MapReverseProxy();

await app.RunAsync();
