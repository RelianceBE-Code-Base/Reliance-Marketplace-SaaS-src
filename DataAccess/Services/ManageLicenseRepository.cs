using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services;

/// <summary>
/// The Known users data repository.
/// </summary>
/// <seealso cref="IManageLicenseRepository" />
public class ManageLicenseRepository : IManageLicenseRepository
{
    /// <summary>
    /// The this.context.
    /// </summary>
    private readonly SaasKitContext context;

    /// <summary>
    /// The disposed.
    /// </summary>
    private bool disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManageLicenseRepository"/> class.
    /// </summary>
    /// <param name="context">The this.context.</param>
    public ManageLicenseRepository(SaasKitContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Gets all known users.
    /// </summary>
    /// <returns>
    /// All known users.
    /// </returns>
    public IEnumerable<ManageLicense> GetAllLicensedUsersBySub(string subscriptionId)
    {
        return this.context.ManageLicense.Where(x=>x.SubscriptionId == subscriptionId).ToList();
    }

    /// <summary>
    /// Gets the known user detail.
    /// </summary>
    /// <param name="emailAddress">The email address.</param>
    /// <param name="roleId">The role identifier.</param>
    /// <returns>
    /// User detail by email and role.
    /// </returns>
    public IEnumerable<ManageLicense> GetAllLicensedUsers()
    {
        return this.context.ManageLicense.ToList();
    }


    public ManageLicense SubscribedUserbyEmail(string email, string offerId)
    {
       return this.context.ManageLicense.Where(x=>x.EmailAddress == email && x.OfferId == offerId).FirstOrDefault();
    }

    /// <summary>
    /// Adds the specified entities.
    /// </summary>
    /// <param name="entities">The entities.</param>
    /// <returns>
    /// Internal identifier after saving the entity.
    /// </returns>
    public int Save(ManageLicense manageLicense)
    {

            var existingUser = this.context.ManageLicense.Where(s => s.EmailAddress == manageLicense.EmailAddress && s.OfferId == manageLicense.OfferId && s.PlanId == manageLicense.PlanId).FirstOrDefault();
            if (existingUser == null)
            {
                this.context.ManageLicense.Add(manageLicense);
                this.context.SaveChanges();
                return manageLicense.Id;
            }
            else
            {
                return 0;
            }
    }

    /// <summary>
    /// Save all known users.
    /// </summary>
    /// <returns>The number of modified records.</returns>
    public int SaveAllManageLicenses(IEnumerable<ManageLicense> manageLicense, string subscriptionId)
    {
        var usersToAdd = new List<ManageLicense>();
        foreach (var entity in manageLicense)
        {
            var isUserAvaliable = context.ManageLicense.Where(x=>x.EmailAddress == entity.EmailAddress && x.SubscriptionId == subscriptionId).FirstOrDefault();
            if (isUserAvaliable == null)
            {
                entity.AddedDate = DateTime.Now;
                usersToAdd.Add(entity);
            }

        }
        this.context.ManageLicense.AddRange(usersToAdd);
        return this.context.SaveChanges();
    }

    public int RemoveAllManageLicenses(IEnumerable<ManageLicense> manageLicense, string subscriptionId)
    {
        var usersToRemove = new List<ManageLicense>();
        foreach (var entity in context.ManageLicense.ToList())
        {
            var isUserAvaliable = manageLicense.Where(x => x.EmailAddress == entity.EmailAddress && x.SubscriptionId == subscriptionId).FirstOrDefault();
            if (isUserAvaliable == null)
            {
                usersToRemove.Add(entity);
            }

        }
        this.context.ManageLicense.RemoveRange(usersToRemove);
        return this.context.SaveChanges();
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.context.Dispose();
            }
        }

        this.disposed = true;
    }
}
