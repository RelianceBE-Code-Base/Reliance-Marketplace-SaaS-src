using Asp.Versioning;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Services;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

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


    private SubscriptionService _subscriptionService = null;
    private PlanService _planService = null;
    private UserService _userService;

    public ManageLicence(IJwtUtils jwtUtils, IManageLicenseRepository licenseRepository, ISubscriptionsRepository subscriptionRepository, IPlansRepository planRepository, IUsersRepository userRepository)
    {
        _jwtUtils = jwtUtils;
        _planRepository = planRepository;
        _licenseRepository = licenseRepository;
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
        _userRepository = userRepository;
        _userService = new UserService(_userRepository);
        _subscriptionService = new SubscriptionService(_subscriptionRepository, _planRepository);

    }

    [HttpGet("GetAllSubscribedUsers")]
    public IActionResult GetAllSubscribedUsers()
    {
        try
        {
            var subscribedusers = _licenseRepository.GetAllLicensedUsers();
            return Ok(subscribedusers);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [HttpGet("subscribeduserbyemail")]
    public IActionResult GetSubscribedUserbyEmail(string email, string offerId)
    {
        try
        {
            var subscribeduserbyemail = _licenseRepository.SubscribedUserbyEmail(email,offerId);
            return Ok(subscribeduserbyemail);
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpGet("subscriptioninfo")]
    public IActionResult GetSubscriptionInfo(string subscriptionId)
    {
        try
        {
            var subscriptionInfo = _subscriptionRepository.GetById(new Guid(subscriptionId));
            //var subscriptionInfos = _subscriptionService.GetSubscriptionsBySubscriptionId(new Guid(subscriptionId));

            return Ok(subscriptionInfo);
        }
        catch (Exception)
        {
            throw;
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
            throw new Exception(ex.Message);
        }
    }

    //[HttpGet("getsubscription")]
    //public IActionResult GetSubscription(string subscriptionId, string planId)
    //{
    //    try
    //    {

    //        SubscriptionResultExtension subscriptionDetail = new SubscriptionResultExtension();

    //        var planDetails = _planRepository.GetById(planId);

    //        subscriptionDetail = _subscriptionService.GetPartnerSubscription(this.CurrentUserEmailAddress, subscriptionId).FirstOrDefault();

    //        subscriptionDetail.SubscriptionParameters = _subscriptionService.GetSubscriptionsParametersById(new Guid(subscriptionId), planDetails.PlanGuid);

    //        return Ok(subscriptionDetail);
    //    }catch (Exception)
    //    {
    //        throw new Exception();
    //    }
    //}



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
