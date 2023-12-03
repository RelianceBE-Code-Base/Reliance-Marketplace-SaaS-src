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
    /// <summary>
    /// Gets all known users.
    /// </summary>
    /// <returns>
    /// All known users.
    /// </returns>
    public IEnumerable<ManageLicense> GetAllLicensedUsers(string subscriptionId);

    /// <summary>
    /// Gets the known user detail.
    /// </summary>
    /// <param name="emailAddress">The email address.</param>
    /// <param name="roleId">The role identifier.</param>
    /// <returns>
    /// An instance of KnownUser.
    /// </returns>
    //KnownUsers GetKnownUserDetail(string emailAddress, int roleId);

    /// <summary>
    /// Adds the know users from application configuration.
    /// </summary>
    /// <param name="knownUsers">The known users.</param>
    //void AddKnowUsersFromAppConfig(string knownUsers);

    /// <summary>
    /// Saves all known users.
    /// </summary>
    /// <returns>The number of modified records.</returns>
    public int SaveAllManageLicenses(IEnumerable<ManageLicense> manageLicense, string subscriptionId);

    /// <summary>
    /// Saves all known users.
    /// </summary>
    /// <returns>The number of modified records.</returns>
    public int RemoveAllManageLicenses(IEnumerable<ManageLicense> manageLicense, string subscriptionId);

    public int Save(ManageLicense manageLicense);
}
