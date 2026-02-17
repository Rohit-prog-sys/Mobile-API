using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MobileAPI.Interface;
using MobileAPI.Models;
using MobileAPI.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#region Services


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()  // Only log Information and higher
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning) // Ignore framework logs below Warning
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)     // Ignore system logs below Warning
    .ReadFrom.Configuration(builder.Configuration)
     .CreateLogger();

builder.Host.UseSerilog();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();



builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});



// -----------------------------
// Switch Service (Bank API)
// -----------------------------

builder.Services.Configure<SwitchSettings>(
    builder.Configuration.GetSection("SwitchSettings"));

builder.Services.AddHttpClient<ISwitchService, SwitchService>((sp, c) =>
{
    var settings = sp.GetRequiredService<IOptions<SwitchSettings>>().Value;
    c.BaseAddress = new Uri(settings.NPCISwitch);
});
builder.Services.AddHttpClient<IAccountService, AccountService>((sp,c )=>
{
    var settings = sp.GetRequiredService<IOptions<SwitchSettings>>().Value;
    c.BaseAddress = new Uri(settings.CBSInterface);
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod());
});

// -----------------------------
// JWT Settings
// -----------------------------
builder.Services.Configure<JWTSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IOTPRepository, OTPRepository>();
//builder.Services.AddScoped<IOTPService,OTPService>();
builder.Services.AddHttpClient<OTPService>();


var jwt = builder.Configuration
                 .GetSection("JwtSettings")
                 .Get<JWTSettings>();


// -----------------------------
// Authentication (JWT)
// -----------------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

#endregion


var app = builder.Build();


#region Middleware

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthentication();   // MUST come before Authorization
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "Mobile API Running Successfully");


#endregion


app.Run();
