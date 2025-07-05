using DripOut.Application.Common.Settings;
using DripOut.Application.BusinessLogic;
using DripOut.Domain.Models;
using DripOut.Application.DTOs;
using DripOut.Infrastructure.Implementation;
using DripOut.Persistence;
using DripOut.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.IdentityModel.Tokens;
using System.Security.Principal;
using DripOut.Application.Interfaces;
using DripOut.Application.Interfaces.ReposInterface;
using DripOut.Application.Interfaces.Services;
var builder = WebApplication.CreateBuilder(args);

builder.Configuration
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// Add services to the container.
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddTransient<IJWTService, JWTService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IMailService, MailService>();

//Mapping JWTSettings To class
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings"));
//Mapping CloudinarySettings To class
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
//Mapping MailSettings To class
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
//Mapping GoogleAuth To class
builder.Services.Configure<Authentication_google>(builder.Configuration.GetSection("Authentication:Google"));

builder.Configuration
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);


builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped(typeof(IBaseRepository<>) , typeof(BaseRepository<>) );
//builder.Services.AddScoped<IProductRepository,ProductRepository>();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
		options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);



builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{

	//options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
	options.User.RequireUniqueEmail = true;
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireUppercase = true;
	options.Password.RequireNonAlphanumeric = true;
	options.Password.RequiredLength = 8;
	options.SignIn.RequireConfirmedEmail = true;
	options.Tokens.EmailConfirmationTokenProvider=TokenOptions.DefaultEmailProvider;

})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme =
	options.DefaultChallengeScheme =
	options.DefaultForbidScheme =
	options.DefaultScheme =
	options.DefaultSignInScheme =
	options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.RequireHttpsMetadata = true;
	options.SaveToken = true;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidIssuer = builder.Configuration["JWTSettings:Issuer"],
		ValidateAudience = true,
		ValidAudience = builder.Configuration["JWTSettings:Audience"],
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(
			System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:SignInKey"]!)
		),
		ValidateLifetime = true,
		ClockSkew = TimeSpan.Zero
	};
});
//configure the time of confirmation token
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
{
	opt.TokenLifespan = TimeSpan.FromMinutes(30); 
});


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

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
