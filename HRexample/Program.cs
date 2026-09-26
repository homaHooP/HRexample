using HRexample.Services;
using HRexample.Data;
using Amazon.S3;
using Amazon;
using Microsoft.EntityFrameworkCore;
        
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
var s3 = new AmazonS3Client(builder.Configuration["s3:AccessKey"], builder.Configuration["s3:SecretKey"], RegionEndpoint.EUCentral1);
builder.Services.AddTransient<bucketService>(x=> new bucketService(s3, builder.Configuration["s3:BucketName"]));
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions => sqlOptions.EnableRetryOnFailure()));

var app = builder.Build();

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
