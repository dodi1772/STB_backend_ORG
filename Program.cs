using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using System.Collections.Generic;


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
            new OpenApiSecuritySchemeReference("Bearer", null, null),
            new List<string>()
        }
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MetadataAddress = "https://cnzmkmydmpsddsaelnpu.supabase.co/auth/v1/.well-known/jwks.json";

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

builder.Services.AddAuthorization();
var app = builder.Build();
// A HTTP-kérelmek feldolgozása.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}
app.MapControllers();
app.Run();