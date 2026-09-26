using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace HRexample.Services
{
    public class bucketService(IAmazonS3 s3, string bucketName)
    {
        public async Task<List<S3Object>> ListObjectsAsync()
        {
            var response = await s3.ListObjectsV2Async(new ListObjectsV2Request { BucketName = bucketName });
            var objects = response.S3Objects ?? new List<S3Object>();
            return objects;
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
        public async Task DownloadObjectAsync(string key, string destinationPath)
        {
            var response = await s3.GetObjectAsync(new GetObjectRequest { BucketName = bucketName, Key = key });
            await response.WriteResponseStreamToFileAsync(destinationPath, false, default);
        }
        public async Task DeleteObjectAsync(string key)
        {
            await s3.DeleteObjectAsync(new DeleteObjectRequest { BucketName = bucketName, Key = key });
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
