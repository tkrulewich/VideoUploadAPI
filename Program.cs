using Azure.Storage.Blobs;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
            // get the authority from appsettings.json
            options.Authority = builder.Configuration["IdentityServer:Authority"];
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidAudience = "video_api", // This should match the ApiResource name in IdentityServer4
            };
        });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VideoScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "video_api");
    });
});

builder.Services.AddSingleton(new BlobServiceClient(builder.Configuration["BlobStorage:ConnectionString"]));

builder.Services.AddSingleton<IBlobService, BlobService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
