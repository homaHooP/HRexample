using HRexample.Services;
using HRexample.Data;
using Amazon.S3;
using Amazon;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var missingSettings = new[] { "ConnectionStrings:DefaultConnection", "s3:BucketName", "gemini:ApiKey" }
    .Where(key => string.IsNullOrEmpty(builder.Configuration[key]))
    .ToList();
if (missingSettings.Count > 0)
{
    throw new InvalidOperationException("Missing configuration: " + string.Join(", ", missingSettings));
}

var s3 =string.IsNullOrEmpty(builder.Configuration["s3:AccessKey"])
    ? new AmazonS3Client(RegionEndpoint.EUCentral1)
    : new AmazonS3Client(builder.Configuration["s3:AccessKey"], builder.Configuration["s3:SecretKey"], RegionEndpoint.EUCentral1);
var bucketName = builder.Configuration["s3:BucketName"] ?? throw new InvalidOperationException("s3:BucketName is not configured");
builder.Services.AddTransient<bucketService>(x=> new bucketService(s3, bucketName));
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions => sqlOptions.EnableRetryOnFailure()));
var client = new Google.GenAI.Client(null,null,builder.Configuration["gemini:ApiKey"]);
List<string> models = new List<string> { "gemini-3.8-flash", "gemini-3.6-flash", "gemini-3.5-flash-lite" };
builder.Services.AddScoped<GeminiService>(x => new GeminiService(client, x.GetRequiredService<ILogger<GeminiService>>(), models));

var app = builder.Build();

// Застосовує міграції під час запуску (нова БД на AWS створиться автоматично)
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

// Elastic Beanstalk проксує запити через nginx / load balancer
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=JobRequirements}/{action=Index}/{id?}");

app.Run();
