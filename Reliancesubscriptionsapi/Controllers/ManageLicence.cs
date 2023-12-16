using Asp.Versioning;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Services;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RelianceSubscriptionsApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]/{v:apiVersion}/")]
public class ManageLicence : ControllerBase
{
    private IJwtUtils _jwtUtils;
    private readonly IManageLicenseRepository _licenseRepository;
    private readonly ISubscriptionsRepository _subscriptionRepository;
    private readonly IPlansRepository _planRepository;
    private readonly IUsersRepository _userRepository;
    private readonly IFulfillmentApiService fulfillApiService;


    private SubscriptionService _subscriptionService = null;
    private PlanService _planService = null;
    private UserService _userService;

    public ManageLicence(IJwtUtils jwtUtils, IManageLicenseRepository licenseRepository, ISubscriptionsRepository subscriptionRepository, IPlansRepository planRepository, IUsersRepository userRepository, IFulfillmentApiService fulfillApiService)
    {
        _jwtUtils = jwtUtils;
        _planRepository = planRepository;
        _licenseRepository = licenseRepository;
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
        _userRepository = userRepository;
        _userService = new UserService(_userRepository);
        _subscriptionService = new SubscriptionService(_subscriptionRepository, _planRepository);
        this.fulfillApiService = fulfillApiService;

    }

    [HttpGet("GetAllSubscribedUsers")]
    public IActionResult GetAllSubscribedUsers()
    {
        try
        {
            var subscribedusers = _licenseRepository.GetAllLicensedUsers();

            if (subscribedusers == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(subscribedusers);
            }
        }
        catch (Exception ex)
        {
            var errorMessage = $"Message: {ex.Message} ({ex.InnerException})";

            return BadRequest(errorMessage);
        }
    }

    [HttpGet("subscribeduserbyemail")]
    public IActionResult GetSubscribedUserbyEmail(string email, string offerId)
    {
        try
        {
            var subscribeduserbyemail = _licenseRepository.SubscribedUserbyEmail(email,offerId);

            if (subscribeduserbyemail == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(subscribeduserbyemail);
            }
           
        }
        catch (Exception ex)
        {
            var errorMessage = $"Message: {ex.Message} ({ex.InnerException})";

            return BadRequest(errorMessage);
        }
    }

    [HttpGet("subscriptioninfo")]
    public IActionResult GetSubscriptionInfo(string subscriptionId)
    {
        try
        {
            var subscriptionInfo = _subscriptionRepository.GetById(new Guid(subscriptionId));

            if (subscriptionInfo == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(subscriptionInfo);
            }
        }
        catch (Exception ex)
        {
            var errorMessage = $"Message: {ex.Message} ({ex.InnerException})";

            return BadRequest(errorMessage);
        }
    }

    [HttpPost("adduser")]
    public IActionResult CreateUser(string userId, string email, string fullname, string subId, string planId, string offerId, string tenantId)
    {
        try
        {

            ManageLicense manageLicense = new ManageLicense()
            {
                UserId = userId,
                EmailAddress = email,
                FullName = fullname,
                SubscriptionId = subId,
                PlanId = planId,
                OfferId = offerId,
                TenantId = tenantId,
                Role = "User",
                AddedDate = DateTime.Now,

            };
            _licenseRepository.Save(manageLicense);
           return Ok(manageLicense);

        }
        catch(Exception ex)
        {
            var errorMessage = $"Message: {ex.Message} ({ex.InnerException})";

            return BadRequest(errorMessage);
        }
    }

    [HttpGet("subscription")]
    //[ValidateAntiForgeryToken]
    public IActionResult GetSubscription(string subscriptionId)
    {

        try
        {

            // Step 1: Get all subscriptions from the API
            var subscriptions = this.fulfillApiService.GetAllSubscriptionAsync().GetAwaiter().GetResult();

            var subscription = subscriptions.Where(x => x.Id == new Guid(subscriptionId)).FirstOrDefault();

            if (subscription == null)
            {
                return NotFound();
            }
            else
            {

                if (subscription.SaasSubscriptionStatus == SubscriptionStatusEnum.Subscribed)
                {
                    subscription.IsActiveSubscription = true;
                }

                return Ok(subscription);
            }

        }
        catch (Exception ex)
        {
            var errorMessage = $"Message: {ex.Message} ({ex.InnerException})";

            return BadRequest(errorMessage);
        }
    }



    [AllowAnonymous]
    [HttpPost]
    [Route("authenticate")]
    public IActionResult Authenticate(IdentityUser usersdata)
    {
        var token = _jwtUtils.GenerateToken(usersdata);

        if (token == null)
        {
            return Unauthorized();
        }

        return Ok(token);
    }
}
