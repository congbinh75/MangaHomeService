using MangaHomeService.Models;
using MangaHomeService.Models.Entities;
using MangaHomeService.Policies;
using MangaHomeService.Services;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IChapterService, ChapterService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IPageService, PageService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IReadingListService, ReadingListService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ITitleService, TitleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenInfoProvider, TokenInfoProvider>();

builder.Services.AddSingleton<IAuthorizationRequirement, EmailConfirmedRequirement>();
builder.Services.AddSingleton<IAuthorizationRequirement, NotBannedRequirement>();

builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? ""))
    };
});
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddRequirements(new EmailConfirmedRequirement())
        .Build();
    options.AddPolicy("EmailConfirmedRequirement", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.AddRequirements(new EmailConfirmedRequirement());
    });
});
builder.Services.AddDbContextFactory<MangaHomeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MangaHome")));
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("vi-VN") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
