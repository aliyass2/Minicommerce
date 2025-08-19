using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Minicommerce.Application;
using Minicommerce.Infrastructure;
using Minicommerce.Infrastructure.Data;
using Minicommerce.Infrastructure.Data.Seed;
using Minicommerce.WebApi.Middleware;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("TokenKey not found");
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // ✅ THIS IS THE KEY FIX - Handle API authentication properly
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                // Prevent the default redirect to login page
                context.HandleResponse();
                
                // Return 401 instead of redirect for API requests
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                
                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    error = "Unauthorized",
                    message = "Valid JWT token required"
                });
                
                return context.Response.WriteAsync(result);
            },
            OnMessageReceived = context =>
            {
                var token = context.Token ?? context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("✅ Token validated successfully");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"❌ Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };
    });
    builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddScoped<RoleSeeder>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost",          // Android emulator
                "http://10.0.2.2",          // Android emulator host machine
                "http://127.0.0.1",         // Local development
                "https://localhost",        // HTTPS localhost
                "http://localhost:3000",    // Common React dev port
                "http://localhost:8080",    // Common development port
                "http://localhost:8081",    // Expo/React Native Metro bundler
                "exp://127.0.0.1:19000",    // Expo development
                "exp://localhost:19000"     // Expo development
            )
            .SetIsOriginAllowed(origin => true) // Allow any origin for development (remove in production)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Important for authentication
    });
    
    // Alternative: Named policy for more control
    options.AddPolicy("AndroidPolicy", policy =>
    {
              policy.WithOrigins(
                "http://localhost",          // Android emulator
                "http://10.0.2.2",          // Android emulator host machine
                "http://127.0.0.1",         // Local development
                "https://localhost",        // HTTPS localhost
                "http://localhost:3000",    // Common React dev port
                "http://localhost:8080",    // Common development port
                "http://localhost:8081",    // Expo/React Native Metro bundler
                "exp://127.0.0.1:19000",    // Expo development
                "exp://localhost:19000"     // Expo development
              )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();


app.Run();
