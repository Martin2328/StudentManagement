var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var conn = builder.Configuration.GetConnectionString("DefaultConnection")!;

// Add DbContext
builder.Services.AddDbContext<StudentManagement.Data.StudentContext>(options =>
    Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseSqlServer(options, conn));

// Register repository; EF DbContext will be injected by DI
builder.Services.AddScoped<StudentManagement.Data.IStudentRepository, StudentManagement.Data.EfStudentRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add request logging middleware
app.UseMiddleware<StudentManagement.Middleware.RequestLoggingMiddleware>();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
