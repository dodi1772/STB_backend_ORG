using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using System.Collections.Generic;
using STB_backend.Security;


var builder = WebApplication.CreateBuilder(args);
// A kötelező szolgáltatások hozzáadása a konténerhez.
builder.Services.AddControllers();
var supabaseUrl = builder.Configuration["SUPABASE:URL"];
var supabaseKey = builder.Configuration["SUPABASE:KEY"];

var option = new Supabase.SupabaseOptions { AutoConnectRealtime = true };

var supabaseClient = new Supabase.Client(supabaseUrl, supabaseKey, option);
supabaseClient.Postgrest.Options.SerializeEnumsAsStrings = true;
builder.Services.AddSingleton(supabaseClient);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {

        Description = "Add meg a tokened",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc),
            new List<string>()
        }
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddScoped<IAuthorizationHandler, UserRoleHandler>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://cnzmkmydmpsddsaelnpu.supabase.co/auth/v1";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = true,
            ValidAudience = "authenticated",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.Requirements.Add(new UserRoleRequirement("ADMIN")));
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
var app = builder.Build();
// A HTTP-kérelmek feldolgozása.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();