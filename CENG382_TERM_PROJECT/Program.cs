using CENG382_TERM_PROJECT.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using CENG382_TERM_PROJECT.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.0
builder.Services.AddRazorPages(options =>
{
    options.Conventions.ConfigureFilter(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
	
var keysDirectoryPath = Path.Combine(AppContext.BaseDirectory, "Keys");
if (!Directory.Exists(keysDirectoryPath))
{
    Directory.CreateDirectory(keysDirectoryPath);
}

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysDirectoryPath))
    .SetApplicationName("CENG382_TERM_PROJECT");

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "Keys")))
    .SetApplicationName("CENG382_TERM_PROJECT");
	
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;
    });
	
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("InstructorOnly", policy => policy.RequireRole("Instructor"));
});
	
	builder.Services.AddHttpContextAccessor();
	builder.Services.AddScoped<ISessionService, SessionService>();
	builder.Services.AddScoped<IInstructorService, InstructorService>();
	builder.Services.AddScoped<IPaginationService, PaginationService>();
	builder.Services.AddScoped<IPasswordService, PasswordService>();
	builder.Services.AddScoped<ITermService, TermService>();
	builder.Services.AddScoped<IClassroomService, ClassroomService>();
    builder.Services.AddScoped<IRecurringReservationService, RecurringReservationService>();
    builder.Services.AddMemoryCache();
    builder.Services.AddHttpClient();
    builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<IPublicHolidayService, PublicHolidayService>();
    builder.Services.AddHttpClient<PublicHolidayApiClient>();
    builder.Services.AddScoped<IFeedbackService, FeedbackService>();
    builder.Services.AddScoped<ISystemLogService, SystemLogService>();
    builder.Services.AddSession(options =>
                    {
                        options.IdleTimeout = TimeSpan.FromMinutes(30);
                        options.Cookie.HttpOnly = true;
                        options.Cookie.IsEssential = true;
	                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                        options.Cookie.SameSite = SameSiteMode.Strict;
                    });

    var app = builder.Build();

// timeslot seed i�lemi
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    CENG382_TERM_PROJECT.Data.Seed.TimeSlotSeeder.Seed(dbContext);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    await next();
});


app.UseSession();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
