using MangaHomeService;
using MangaHomeService.Models;
using MangaHomeService.Policies;
using MangaHomeService.Services;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;

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

builder.Services.AddTransient<ITokenInfoProvider, TokenInfoProvider>();

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
        .AddRequirements(new NotBannedRequirement())
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
builder.Services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResources));
    });

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

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json"; // Set content type to JSON

        var stringLocalizer = app.Services.GetService<IStringLocalizer<SharedResources>>();
        var errorMessage = stringLocalizer?[Constants.ERR_UNEXPECTED_ERROR].Value ?? Constants.DefaultErrorMessage;

        var responseObject = new { message = errorMessage };
        var jsonResponse = JsonSerializer.Serialize(responseObject);

        await context.Response.WriteAsync(jsonResponse);
    });
});

app.Run();
