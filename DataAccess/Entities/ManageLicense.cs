using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public class ManageLicense
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public string EmailAddress { get; set; }
    public DateTime? AddedDate { get; set; }
    public string FullName { get; set; }
    public string SubscriptionId { get; set; }
    public string PlanId { get; set; }
    public string OfferId { get; set; }
    public string TenantId { get; set;}
    public string Role { get; set;}
}
