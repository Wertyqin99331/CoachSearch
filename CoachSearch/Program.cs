using System.Reflection;
using System.Text;
using CoachSearch.Data;
using CoachSearch.Data.Entities;
using CoachSearch.Repositories.Customer;
using CoachSearch.Repositories.Like;
using CoachSearch.Repositories.Review;
using CoachSearch.Repositories.Trainer;
using CoachSearch.Repositories.TrainingProgram;
using CoachSearch.Services.FileUploadService;
using CoachSearch.Services.Token;
using CoachSearch.Services.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	 .AddControllers()
	 .AddNewtonsoftJson();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options
		/*
		.UseSqlServer(builder.Configuration.GetConnectionString("Db"))
		*/
		.UseNpgsql(builder.Configuration.GetConnectionString("Db"))
		.UseLazyLoadingProxies();
});
builder.Services.AddIdentity<ApplicationUser, IdentityRole<long>>(options =>
	{
		options.Password.RequiredUniqueChars = 0;
		options.Password.RequireUppercase = false;
		options.Password.RequiredLength = 6;
		options.Password.RequireNonAlphanumeric = false;
		options.Password.RequireLowercase = false;
		options.Password.RequireDigit = false;
	})
	.AddRoles<IdentityRole<long>>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new()
		{
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateLifetime = false,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["Jwt:Issuer"]!,
			ValidAudience = builder.Configuration["Jwt:Audience"]!,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
		};
	});

builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ITrainerRepository, TrainerRepository>();
builder.Services.AddScoped<ITrainingProgramRepository, TrainingProgramRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
	option.SwaggerDoc("v1", new OpenApiInfo { Title = "CoachSearch", Version = "v1" });
	option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Description = "Please enter a valid token",
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		BearerFormat = "JWT",
		Scheme = "Bearer"
	});
	option.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
            Array.Empty<string>()
        }
	});
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	option.IncludeXmlComments(xmlPath);
});

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(corsOptions => corsOptions
	.AllowAnyHeader()
	.AllowAnyMethod()
	.SetIsOriginAllowed((host) => true)
	.AllowCredentials()
);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


/*var folderPath = Path.Combine(builder.Environment.ContentRootPath, "Images");*/

var folderPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
if (!Directory.Exists(folderPath))
	Directory.CreateDirectory(folderPath);

/*app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(
		Path.Combine(builder.Environment.ContentRootPath, "Images")),
	RequestPath = "/Images"
});*/
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

