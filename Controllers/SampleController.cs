using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;

namespace MinioSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly IMinioClient _minio;
        private readonly string _bucketName = "mybucket";
        public SampleController(IMinioClient minio)
        {
            _minio = minio;
        }

        [HttpGet("download/{objectName}")]
        public async Task<IActionResult> Download(string objectName)
        {
            var exists = await _minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
            if (!exists) return NotFound($"Bucket '{_bucketName}' not found.");

            Response.ContentType = "application/octet-stream";
            Response.Headers.Add("Content-Disposition", $"attachment; filename={objectName}");

            await _minio.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName)
                    .WithCallbackStream(async (stream, ct) =>
                    {
                        var buffer = new byte[81920];
                        int bytesRead;
                        while (!HttpContext.RequestAborted.IsCancellationRequested &&
                               (bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
                        {
                            await Response.Body.WriteAsync(buffer, 0, bytesRead, ct);
                            await Response.Body.FlushAsync(ct);
                        }
                    })
            );
            return new EmptyResult();
        }
    }
}
