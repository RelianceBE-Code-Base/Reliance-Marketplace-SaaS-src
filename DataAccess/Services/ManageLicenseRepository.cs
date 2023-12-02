using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services
{
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
        public IEnumerable<ManageLicenses> GetAllLicensedUsers(string subscriptionId)
        {
            return this.context.ManageLicenses.Where(x=>x.SubscriptionId == subscriptionId).ToList();
        }

        /// <summary>
        /// Gets the known user detail.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>
        /// User detail by email and role.
        /// </returns>
        //public KnownUsers GetKnownUserDetail(string emailAddress, int roleId)
        //{
        //    return this.context.KnownUsers.Where(s => s.UserEmail == emailAddress && s.RoleId == roleId).FirstOrDefault();
        //}

        /// <summary>
        /// Adds the know users from application configuration.
        /// </summary>
        /// <param name="knownUsers">The known users.</param>
        //public void AddUsers(string knownUsers)
        //{
        //    var existingUsers = this.context.KnownUsers;
        //    if (existingUsers != null && existingUsers.ToList().Count() == 0)
        //    {
        //        List<string> knownUsersList = knownUsers.Split(',').ToList();
        //        foreach (var user in knownUsersList)
        //        {
        //            var users = new KnownUsers()
        //            {
        //                UserEmail = user.Trim(),
        //                RoleId = 1, // Publisher Admin
        //            };
        //            this.context.KnownUsers.Add(users);
        //            this.context.SaveChanges();
        //        }
        //    }
        //}

        /// <summary>
        /// Adds the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns>
        /// Internal identifier after saving the entity.
        /// </returns>
        public int Save(ManageLicenses manageLicenses)
        {

                var existingUser = this.context.ManageLicenses.Where(s => s.EmailAddress == manageLicenses.EmailAddress).FirstOrDefault();
                if (existingUser == null)
                {
                    this.context.ManageLicenses.Add(manageLicenses);
                    this.context.SaveChanges();
                    return manageLicenses.Id;
                }
                else
                {
                    return 0;
                }
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        //public void Remove(KnownUsers entity)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Save all known users.
        /// </summary>
        /// <returns>The number of modified records.</returns>
        public int SaveAllManageLicenses(IEnumerable<ManageLicenses> manageLicenses, string subscriptionId)
        {
            var usersToAdd = new List<ManageLicenses>();
            foreach (var entity in manageLicenses)
            {
                var isUserAvaliable = context.ManageLicenses.Where(x=>x.EmailAddress == entity.EmailAddress && x.SubscriptionId == subscriptionId).FirstOrDefault();
                if (isUserAvaliable == null)
                {
                    usersToAdd.Add(entity);
                }

            }
            this.context.ManageLicenses.AddRange(usersToAdd);
            return this.context.SaveChanges();
        }

        public int RemoveAllManageLicenses(IEnumerable<ManageLicenses> manageLicenses, string subscriptionId)
        {
            var usersToRemove = new List<ManageLicenses>();
            foreach (var entity in context.ManageLicenses.ToList())
            {
                var isUserAvaliable = manageLicenses.Where(x => x.EmailAddress == entity.EmailAddress && x.SubscriptionId == subscriptionId).FirstOrDefault();
                if (isUserAvaliable == null)
                {
                    usersToRemove.Add(entity);
                }

            }
            this.context.ManageLicenses.RemoveRange(usersToRemove);
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
}
