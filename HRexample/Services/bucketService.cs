using Amazon.S3;
using Amazon.S3.Model;    
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace HRexample.Services
{
    public class bucketService(IAmazonS3 s3, string bucketName)
    {
        private async Task<MemoryStream> DownloadAsync(string key)
        {
            using var response = await s3.GetObjectAsync(bucketName, key);
            var ms = new MemoryStream();
            await response.ResponseStream.CopyToAsync(ms);
            ms.Position = 0;
            return ms;
        }
        public async Task<string> ReadPdfTextAsync(string key)
        {
            using var ms = await DownloadAsync(key);
            using var pdf = PdfDocument.Open(ms);

            var sb = new StringBuilder();
            foreach (Page page in pdf.GetPages())
                sb.AppendLine(page.Text);

            return sb.ToString();
        }
        public async Task<string> ReadTextAsync(string key)
        {
            await using var stream = await DownloadAsync(key);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
        public async Task UploadObjectAsync(string key, Stream content, string? contentType = null)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = content,
                ContentType = contentType ?? "application/octet-stream",
            };
            await s3.PutObjectAsync(request);
        }
        public async Task<string> GenerateLinkAsync(string key)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = key,
                Expires = DateTime.UtcNow.Add(60 * TimeSpan.FromMinutes(1))
            };
            return s3.GetPreSignedURL(request);
        }
    }
}
