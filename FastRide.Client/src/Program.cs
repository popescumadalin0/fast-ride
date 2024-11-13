using System;
using System.Text;
using System.Threading.Tasks;
using FastRide.Client.Authentication;
using FastRide.Client.Data;
using FastRide.Server.Sdk;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Development.json", true, true);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        /*options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;*/
    })
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration.GetSection("Google:ClientId").Value!;
        options.ClientSecret = builder.Configuration.GetSection("Google:ClientSecret").Value!;
        options.SaveTokens = true;
    })
    /*.AddJwtBearer("Bearer", options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        /*options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };#1#
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies[".AspNetCore.Cookies"];
                return Task.CompletedTask;
            },
        };
    })*/;

builder.Services.AddControllers();

/*builder.Services.AddScoped<IClaimsTransformation,
    ApplicationUserClaimsTransformation>();*/

/*builder.Services.AddScoped<IHostEnvironmentAuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();*/

builder.Services.AddScoped<AuthenticationApiHttpMessageHandler>();

builder.Services.AddHttpContextAccessor();

var apiUrl = builder.Configuration.GetSection("FastRide:BaseUrl").Value;

builder.Services.AddFastRideApiClient<AuthenticationApiHttpMessageHandler>(new Uri(apiUrl));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapControllers();

app.UseStaticFiles();

app.UseCookiePolicy();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();