using System;
using System.IO;
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
using Newtonsoft.Json;
using System.Net;

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
            if (string.IsNullOrEmpty(_bucketName))
            {
                logger.LogCritical("Missing configuration for S3 bucket. The AppS3Bucket configuration must be set to a S3 bucket.");
                throw new Exception("Missing configuration for S3 bucket. The AppS3Bucket configuration must be set to a S3 bucket.");
            }

            logger.LogInformation($"Configured to use bucket {_bucketName}");
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
        public async Task<IActionResult> Post([FromForm]CreateFileDto fileDto)
        {
            if (fileDto == null || fileDto.File == null)
            {
                return BadRequest();
            }
            MemoryStream seekableStream = await GetFileStream(fileDto);

            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                CannedACL = S3CannedACL.PublicRead,
                Key = fileDto.File.FileName,
                InputStream = seekableStream
            };

            var response = await _amazonS3.PutObjectAsync(putRequest);
            var fileMetadata = new Metadata
            {
                FileName = fileDto.File.FileName,
                FileSize = fileDto.File.Length,
                ContentType = fileDto.File.ContentType,
                FileUrl = GetFileUrl(fileDto)
            };

            _dbContext.Metadata.Add(fileMetadata);
            await _dbContext.SaveChangesAsync();

            var resultUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}/{fileMetadata.FileName}";
            await _notificationService.PublishAsync(_topicArn, resultUrl);

            _logger.LogInformation($"Uploaded object {fileDto.File.FileName} to bucket {_bucketName}. Request Id: {response.ResponseMetadata.RequestId}");

            return Created(resultUrl, null);
        }

        private static async Task<MemoryStream> GetFileStream(CreateFileDto fileDto)
        {
            var seekableStream = new MemoryStream();
            await fileDto.File.CopyToAsync(seekableStream);
            seekableStream.Position = 0;
            return seekableStream;
        }

        private string GetFileUrl(CreateFileDto fileDto)
        {
            return $"https://{_bucketName}.s3.{RegionEndpoint.USEast2.SystemName}.amazonaws.com/{fileDto.File.FileName}";
        }
    }
}
