using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivocCommon
{
    /// <summary>
    /// Helper class for Graph API REST endpoints
    /// Non-parameterized endpoints as properties, parameterized as methods
    /// </summary>
    public static class EndPoints
    {
        #region Properties

        /// <summary>
        /// URL for the endpoint for the signed-in user's profile 
        /// </summary>
        public static string UserProfile { get { return "https://graph.microsoft.com/v1.0/me/"; } }
        /// <summary>
        /// URL for the endpoint to get the default site of the tenant
        /// </summary>
        public static string DefaultSite { get { return "https://graph.microsoft.com/v1.0/sites/root"; } }
        /// <summary>
        /// URL for the endpoint to get the default drive of the default site of the tenant
        /// </summary>
        public static string DefaultSiteDefaultDrive { get { return "https://graph.microsoft.com/v1.0/sites/root/drive"; } }
        /// <summary>
        /// URL for the endpoint to get all of the drives for the default side of the tenant
        /// </summary>
        public static string DefaultSiteDrives { get { return "https://graph.microsoft.com/v1.0/sites/root/drives"; } }
        /// <summary>
        /// URL for the endpoint to get the root item of the default drive of the default site of the tenant
        /// </summary>
        public static string DefaultSiteDefaultDriveRootItem { get { return "https://graph.microsoft.com/v1.0/sites/root/drive/root"; } }

        #endregion

        #region Methods

        /// <summary>
        /// Get the root item of a given drive
        /// </summary>
        /// <param name="driveId">Id of the drive</param>
        /// <returns>Formatted string for the endpoint URL</returns>
        public static string DriveRootItem(string driveId)
        {
            return string.Format("https://graph.microsoft.com/v1.0/drives/{0}/root", driveId);
        }

        /// <summary>
        /// Get the children of a given item in a given drive
        /// </summary>
        /// <param name="driveId">Id of the drive</param>
        /// <param name="parentId">Id of the parent item to query for children</param>
        /// <returns>Formatted string for the endpoint URL</returns>
        public static string ChildrenForItem(string driveId, string parentId)
        {
            return string.Format("https://graph.microsoft.com/v1.0/drives/{0}/items/{1}/children", driveId, parentId);
        }

        /// <summary>
        /// Create a new item under the specified parent in the specified drive
        /// </summary>
        /// <param name="driveId">Id of the drive</param>
        /// <param name="parentId">Id of the parent item to create the new item under</param>
        /// <param name="name">Name of the new item</param>
        /// <returns>Formatted string for the endpoint URL</returns>
        public static string NewItem(string driveId, string parentId, string name)
        {
            return string.Format("https://graph.microsoft.com/v1.0/drives/{0}/items/{1}:/{2}:/content", driveId, parentId, name);
        }

        #endregion
    }
}
