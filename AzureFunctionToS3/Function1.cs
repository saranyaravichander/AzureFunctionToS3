using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon.Internal;
using Amazon;
using Amazon.S3.Model;

namespace AzureFunctionToS3
{
    public static class Function1
    {
        [FunctionName("AzureToS3")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var client = new AmazonS3Client(
                Environment.GetEnvironmentVariable("accessKey"),
                Environment.GetEnvironmentVariable("secretKey"),
                RegionEndpoint.USEast1);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string responseMessage = "This HTTP triggered function executed successfully";

            var putRequest = new PutObjectRequest
                {
                BucketName = Environment.GetEnvironmentVariable("existingBucketName"),
                Key = Environment.GetEnvironmentVariable("keyName"),
                ContentBody = requestBody,
                
            };

            PutObjectResponse resp = await client.PutObjectAsync(putRequest);
        
            log.LogInformation("Copy completed");

            return new OkObjectResult(responseMessage);
        }
    }
}
