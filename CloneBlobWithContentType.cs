using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Sample
{
  public static class CloneBlobWithContentType
  {
    private static string StorageAccountConnectionString =
        System.Environment.GetEnvironmentVariable("storage_account_connection_string", EnvironmentVariableTarget.Process);
    private static string InputContainerName =
        System.Environment.GetEnvironmentVariable("input_container_name", EnvironmentVariableTarget.Process);
    private static string OutputContainerName =
        System.Environment.GetEnvironmentVariable("output_container_name", EnvironmentVariableTarget.Process);

    [FunctionName("CloneBlobWithContentType")]
    public static async Task Run(
        [BlobTrigger("%input_container_name%/{name}", Connection = "storage_account_connection_string")] Stream blob,
        string name,
        ILogger log)
    {
      log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {blob.Length} Bytes");

      var inBlob = new BlobClient(StorageAccountConnectionString, InputContainerName, name);
      var inProps = await inBlob.GetPropertiesAsync();
      var contentType = inProps.Value.ContentType;

      log.LogInformation($"Copying {name} from {InputContainerName} to {OutputContainerName} with content-type {contentType}");

      var outBlob = new BlobClient(StorageAccountConnectionString, OutputContainerName, name);
      await outBlob.UploadAsync(
        blob,
        new BlobHttpHeaders
        {
          ContentType = contentType
        }
      );
    }
  }
}
