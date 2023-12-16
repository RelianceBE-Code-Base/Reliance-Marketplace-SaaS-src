using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Azure.Identity;
using System;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Services;
using Microsoft.Marketplace.SaaS;

var builder = WebApplication.CreateBuilder(args);

// Uncomment this lines of code when going live

var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

// Add services to the container.
builder.Services.AddDbContext<SaasKitContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var config = new SaaSApiClientConfiguration()
{
    AdAuthenticationEndPoint = builder.Configuration["SaaSApiConfiguration:AdAuthenticationEndPoint"],
    ClientId = builder.Configuration["SaaSApiConfiguration:ClientId"],
    ClientSecret = builder.Configuration["SaaSApiConfiguration:ClientSecret"],
    MTClientId = builder.Configuration["SaaSApiConfiguration:MTClientId"],
    FulFillmentAPIBaseURL = builder.Configuration["SaaSApiConfiguration:FulFillmentAPIBaseURL"],
    FulFillmentAPIVersion = builder.Configuration["SaaSApiConfiguration:FulFillmentAPIVersion"],
    GrantType = builder.Configuration["SaaSApiConfiguration:GrantType"],
    Resource = builder.Configuration["SaaSApiConfiguration:Resource"],
    SaaSAppUrl = builder.Configuration["SaaSApiConfiguration:SaaSAppUrl"],
    SignedOutRedirectUri = builder.Configuration["SaaSApiConfiguration:SignedOutRedirectUri"],
    TenantId = builder.Configuration["SaaSApiConfiguration:TenantId"],
    Environment = builder.Configuration["SaaSApiConfiguration:Environment"]
};
var creds = new ClientSecretCredential(config.TenantId.ToString(), config.ClientId.ToString(), config.ClientSecret);


builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    x.DefaultAuthenticateScheme = OpenIdConnectDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    var Key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]);
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Key)
    };
})
.AddOpenIdConnect(options =>
{
    options.Authority = $"{config.AdAuthenticationEndPoint}/common/v2.0";
    options.ClientId = config.MTClientId;
    options.ResponseType = OpenIdConnectResponseType.IdToken;
    options.CallbackPath = "/Home/Index";
    options.SignedOutRedirectUri = config.SignedOutRedirectUri;
    options.TokenValidationParameters.NameClaimType = ClaimConstants.CLAIM_NAME; //This does not seem to take effect on User.Identity. See Note in CustomClaimsTransformation.cs
    options.TokenValidationParameters.ValidateIssuer = false;
})
;



if (!Uri.TryCreate(config.FulFillmentAPIBaseURL, UriKind.Absolute, out var fulfillmentBaseApi))
{
    fulfillmentBaseApi = new Uri("https://marketplaceapi.microsoft.com/api");
}

builder.Services
    .AddSingleton<IFulfillmentApiService>(new FulfillmentApiService(new MarketplaceSaaSClient(fulfillmentBaseApi, creds), config, new FulfillmentApiClientLogger()))
    .AddSingleton<SaaSApiClientConfiguration>(config);



builder.Services.AddApiVersioning(x =>
{
    x.DefaultApiVersion = new ApiVersion(1, 0);
    x.AssumeDefaultVersionWhenUnspecified = true;
    x.ReportApiVersions = true;
});

//JSON Serializer
builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling =
       Newtonsoft.Json.ReferenceLoopHandling.Ignore).AddNewtonsoftJson(options =>
    options.SerializerSettings.ContractResolver = new DefaultContractResolver());

builder.Services.AddScoped<ISubscriptionsRepository, SubscriptionsRepository>();
builder.Services.AddScoped<IPlansRepository, PlansRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<ISubscriptionLogRepository, SubscriptionLogRepository>();
builder.Services.AddScoped<IApplicationLogRepository, ApplicationLogRepository>();
builder.Services.AddScoped<IApplicationConfigRepository, ApplicationConfigRepository>();
builder.Services.AddScoped<IOffersRepository, OffersRepository>();
builder.Services.AddScoped<IOfferAttributesRepository, OfferAttributesRepository>();
builder.Services.AddScoped<IPlanEventsMappingRepository, PlanEventsMappingRepository>();
builder.Services.AddScoped<IEventsRepository, EventsRepository>();
builder.Services.AddScoped<IManageLicenseRepository, ManageLicenseRepository>();
builder.Services.AddScoped<IJwtUtils, JwtUtils>();

var app = builder.Build();


if (string.IsNullOrEmpty(app.Configuration.GetValue<String>("KEY")))
    throw new Exception("Error");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
        
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
