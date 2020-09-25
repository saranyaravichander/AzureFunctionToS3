using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ADLStoS3
{
    public static class Function1
    {
        [FunctionName("ALStoS3")]
        public static async Task Run([BlobTrigger("sqlparquet/{name}", Connection = "ADLS")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            var client = new AmazonS3Client(
                Environment.GetEnvironmentVariable("accessKey"),
                Environment.GetEnvironmentVariable("secretKey"),
                RegionEndpoint.USEast1);

            string requestBody = await new StreamReader(myBlob).ReadToEndAsync();

            var putRequest = new PutObjectRequest
            {
                BucketName = Environment.GetEnvironmentVariable("existingBucketName"),
                Key = Environment.GetEnvironmentVariable("keyName") + name,
                ContentBody = requestBody,

            };

            PutObjectResponse resp = await client.PutObjectAsync(putRequest);

            log.LogInformation("Copy completed");

        }
    }
}
