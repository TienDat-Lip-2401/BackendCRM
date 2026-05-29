using RedmineApp;
using RedmineApp.MiddleWares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddHttpContextAccessor();
var allowOrigins = builder.Configuration.GetSection("AllowOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", policy =>
    {
        if (allowOrigins.Length > 0)
        {
            // Nếu có cấu hình domain cụ thể
            policy.WithOrigins(allowOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials(); // Cho phép gửi Token/Cookie
        }
        else
        {
            // Nếu chưa có cấu hình (thường dùng lúc Dev)
            // LƯU Ý: Nếu dùng AllowAnyOrigin thì KHÔNG ĐƯỢC dùng AllowCredentials
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }

        // Luôn cho phép FE đọc Header này nếu bạn có xử lý file
        policy.WithExposedHeaders("Content-Disposition");
    });
});
var app = builder.Build();
app.UseCors("AllowOrigin");
app.UseSwagger();
app.UseSwaggerUI();


// Configure the HTTP request pipeline.
app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseMiddleware<CustomAuthorizeMiddleware>();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
