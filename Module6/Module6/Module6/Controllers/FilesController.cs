using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Amazon.S3;
using Amazon.S3.Model;
using Module6.DbModels;
using Module6.ApiModels;
using Amazon;
using Amazon.SimpleNotificationService;

namespace Module6.Controllers
{
    [Route("api/[controller]")]
    public class FilesController : Controller
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly IAmazonSimpleNotificationService _notificationService;
        private readonly ILogger _logger;
        private readonly string _bucketName;
        private readonly string _topicArn;
        private readonly string _apiEndpoint;
        private readonly FileMetadataContext _dbContext;

        public FilesController(
            IConfiguration configuration,
            ILogger<FilesController> logger,
            IAmazonS3 s3Client,
            IAmazonSimpleNotificationService notificationService,
            FileMetadataContext context)
        {
            _logger = logger;
            _amazonS3 = s3Client;
            _notificationService = notificationService;
            _dbContext = context;

            _bucketName = configuration[Startup.AppS3BucketKey];
            _topicArn = configuration[Startup.SnsTopicArn];
            _apiEndpoint = configuration[Startup.ApiEndpoint];
            if (string.IsNullOrEmpty(_bucketName))
            {
                logger.LogCritical("Missing configuration for S3 bucket. The AppS3Bucket configuration must be set to a S3 bucket.");
                throw new Exception("Missing configuration for S3 bucket. The AppS3Bucket configuration must be set to a S3 bucket.");
            }

            logger.LogInformation($"Configured to use bucket {_bucketName}");
            logger.LogInformation($"Configured to use topic {_topicArn}");
            logger.LogInformation($"Configured to use endpoint {_apiEndpoint}");
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_dbContext.Metadata);
        }

        [HttpGet("{name}", Name = "GetById")]
        public async Task<IActionResult> Get(string name)
        {
            var result = await _dbContext.Metadata
                .FirstOrDefaultAsync(v => v.FileName == name);

            return Ok(result);
        }

        [HttpGet("random")]
        public async Task<IActionResult> GetRandom()
        {
            var result = await _dbContext.Metadata.ToListAsync();

            var random = new Random();
            int index = random.Next(result.Count);

            return Ok(result[index]);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateFileDto fileDto)
        {
            if (fileDto?.Data == null)
            {
                return BadRequest();
            }

            var imageDataByteArray = Convert.FromBase64String(fileDto.Data);
            var seekableStream = new MemoryStream(imageDataByteArray)
            {
                Position = 0
            };


            await UploadImageToS3(seekableStream, fileDto.ImageName);

            var fileMetadata = await UploadMetadataToRdc(imageDataByteArray, fileDto.ImageName);

            var resultUrl = BuildFileUrl(fileDto.ImageName);

            await _notificationService.PublishAsync(_topicArn, resultUrl);

            _logger.LogInformation($"Uploaded object {fileDto.ImageName} to bucket {_bucketName}");

            return Created(resultUrl, null);
        }

        private string BuildFileUrl(string fileName)
        {
            return $"{_apiEndpoint}{Request.Path}/{fileName}";
        }

        private async Task<Metadata> UploadMetadataToRdc(byte[] stream, string fileName)
        {
            var fileMetadata = new Metadata
            {
                FileName = fileName,
                FileSize = stream.Length,
                ContentType = MediaTypeNames.Image.Jpeg,
                FileUrl = GetFileUrl(fileName)
            };

            _dbContext.Metadata.Add(fileMetadata);
            await _dbContext.SaveChangesAsync();
            return fileMetadata;
        }

        private async Task<PutObjectResponse> UploadImageToS3(MemoryStream stream, string fileName)
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                CannedACL = S3CannedACL.PublicRead,
                Key = fileName,
                InputStream = stream
            };

            var response = await _amazonS3.PutObjectAsync(putRequest);
            return response;
        }

        private static MemoryStream GetFileStream(CreateFileDto fileDto)
        {
            var imageDataByteArray = Convert.FromBase64String(fileDto.Data);
            var seekableStream = new MemoryStream(imageDataByteArray)
            {
                Position = 0
            };

            return seekableStream;
        }

        private string GetFileUrl(string fileName)
        {
            return $"https://{_bucketName}.s3.{RegionEndpoint.USEast2.SystemName}.amazonaws.com/{fileName}";
        }
    }
}
