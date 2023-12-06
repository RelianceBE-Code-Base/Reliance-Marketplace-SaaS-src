using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

/// <summary>
/// Known User Repository.
/// </summary>
/// <seealso cref="System.IDisposable" />
/// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IBaseRepository{Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities.ManageLicense}" />
public interface IManageLicenseRepository
{
   
    public IEnumerable<ManageLicense> GetAllLicensedUsersBySub(string subscriptionId);

    public IEnumerable<ManageLicense> GetAllLicensedUsers();

    public ManageLicense SubscribedUserbyEmail(string email, string offerId);

    /// <summary>
    /// Saves all licensed users.
    /// </summary>
    /// <returns>The number of modified records.</returns>
    public int SaveAllManageLicenses(IEnumerable<ManageLicense> manageLicense, string subscriptionId);

    /// <summary>
    /// remove all licensed users.
    /// </summary>
    /// <returns>The number of modified records.</returns>
    public int RemoveAllManageLicenses(IEnumerable<ManageLicense> manageLicense, string subscriptionId);

    public int Save(ManageLicense manageLicense);
}
