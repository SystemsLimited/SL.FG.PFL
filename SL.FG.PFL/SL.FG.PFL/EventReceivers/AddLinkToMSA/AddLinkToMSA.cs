using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.Administration;

namespace SL.FG.PFL.EventReceivers.AddLinkToMSA
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class AddLinkToMSA : SPItemEventReceiver
    {
        /// <summary>
        /// An item was added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            try
            {
                SPList spList = properties.List;

                if (spList.Title.Equals("MSA Schedule"))
                {
                    SPFieldUrlValue spFieldURL = new SPFieldUrlValue();
                    spFieldURL.Url = "/sites/pfl/Pages/MSA.aspx?SID=" + properties.ListItemId;
                    spFieldURL.Description = "Please click here";

                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (SPSite spSite = new SPSite(properties.Web.Url))
                        {
                            using (SPWeb spWeb = spSite.OpenWeb())
                            {
                                SPList spList1 = spWeb.Lists["MSA Schedule"];
                                SPListItem spListItem = spList1.GetItemById(properties.ListItemId);

                                spListItem["MSAFormLink"] = spFieldURL;

                                SPGroup spGroup = properties.Web.SiteGroups["HSE-PFL"];
                                SPRoleDefinition spRole = properties.Web.RoleDefinitions["Contribute"];

                                SPRoleAssignment roleAssignment = new SPRoleAssignment(spGroup);
                                roleAssignment.RoleDefinitionBindings.Add(spRole);

                                if (Convert.ToString(spListItem["PFLEmailAddress"]) != null)
                                {
                                    SPUser spUSer = properties.Web.SiteUsers.GetByEmail(Convert.ToString(spListItem["PFLEmailAddress"]));
                                    SPRoleDefinition spRole1 = properties.Web.RoleDefinitions["Read"];
                                    SPRoleAssignment roleAssignment1 = new SPRoleAssignment(spUSer);
                                    roleAssignment1.RoleDefinitionBindings.Add(spRole1);
                                    spListItem.BreakRoleInheritance(false);
                                    spListItem.RoleAssignments.Add(roleAssignment1);
                                }
                                else
                                {
                                    spListItem.BreakRoleInheritance(false);
                                }
                                spListItem.RoleAssignments.Add(roleAssignment);
                                spListItem.Update();
                            }
                        }
                    });
                }
                else
                {
                    properties.Status = SPEventReceiverStatus.CancelNoError;
                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("MSAEventReceiver", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            finally
            {
                base.ItemAdded(properties);
            }
        }


    }
}