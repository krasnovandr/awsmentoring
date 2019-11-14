using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Module6.Controllers
{
    public class CreateSubscriptionDto
    {
        public string Email { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly IAmazonSimpleNotificationService _notificationService;
        private readonly string _topicArn;

        public SubscriptionController(
            IConfiguration configuration,
            IAmazonSimpleNotificationService notificationService)
        {
            _notificationService = notificationService;

            _topicArn = configuration[Startup.SnsTopicArn];
        }

        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _notificationService.ListSubscriptionsByTopicAsync(_topicArn);


                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
          
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CreateSubscriptionDto createSubscription)
        {
            var result = await _notificationService.SubscribeAsync(_topicArn, "email", createSubscription.Email);

            return Ok(result);
        }

        [HttpDelete("{email}")]
        public async Task<IActionResult> Delete(string email)
        {
            var allSubscriptions = await _notificationService.ListSubscriptionsByTopicAsync(_topicArn);

            var targetSubscription = allSubscriptions.Subscriptions.FirstOrDefault(s => s.Endpoint == email);

            if (targetSubscription == null)
            {
                return NotFound();
            }

            var result = await _notificationService.UnsubscribeAsync(targetSubscription.SubscriptionArn);

            return Ok(result);
        }
    }
}