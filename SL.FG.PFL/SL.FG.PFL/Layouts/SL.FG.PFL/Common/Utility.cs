﻿using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.FG.PFL.Layouts.SL.FG.PFL.Common
{
    public class Utility
    {
        public static SPUser GetUser(SPWeb oSPWeb_, string username = null, string email = null, int userId = 0)
        {
            SPUser spUser = null;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                    {
                        using (SPWeb oSPWeb_EP = oSPsite.OpenWeb())
                        {

                            if (!String.IsNullOrEmpty(username))
                            {
                                if (username.Contains("|"))
                                {
                                    var temp = username.Split('|');
                                    if (temp.Length > 1)
                                    {
                                        spUser = oSPWeb_EP.AllUsers[temp[1]];
                                    }
                                }
                                else
                                {
                                    string temp = "i:0#.w|" + username;
                                    spUser = oSPWeb_EP.AllUsers[temp];
                                }
                            }
                            if (spUser == null && !String.IsNullOrEmpty(email))
                            {
                                spUser = oSPWeb_EP.AllUsers.GetByEmail(email);
                            }
                            if (spUser == null && userId > 0)
                            {
                                spUser = oSPWeb_EP.AllUsers.GetByID(userId);
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(Utility->GetUser)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return spUser;
        }


        public static string GetRedirectUrl(string key)
        {
            string redirectUrl = null;
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oSPWeb = oSPsite.OpenWeb())
                    {
                        string listName = "CommonDictionary";

                        // Fetch the List
                        SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));
                        SPQuery query = new SPQuery();
                        SPListItemCollection spListItems;
                        // Include only the fields you will use.
                        query.ViewFields = "<FieldRef Name='Title'/><FieldRef Name='Value'/>";
                        query.ViewFieldsOnly = true;
                        query.RowLimit = 1; // Only select the top 1.
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<Where>")
                                  .Append("<Eq>")
                                     .Append("<FieldRef Name='Title' />")
                                     .Append("<Value Type='Text'>" + key + "</Value>")
                                  .Append("</Eq>")
                               .Append("</Where>");
                        query.Query = sb.ToString();
                        spListItems = spList.GetItems(query);

                        for (int i = 0; i < spListItems.Count; i++)
                        {
                            SPListItem listItem = spListItems[i];
                            redirectUrl = string.Format("{0}/{1}", oSPWeb.Url, Convert.ToString(listItem["Value"]));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(Utility->GetRedirectUrl)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return redirectUrl;
        }

        public static List<CommonDictionary> GetValuesByKey(string key)
        {
            List<CommonDictionary> lstCommonDictionary = new List<CommonDictionary>();
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oSPWeb = oSPsite.OpenWeb())
                    {
                        string listName = "CommonDictionary";

                        // Fetch the List
                        SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));
                        SPQuery query = new SPQuery();
                        SPListItemCollection spListItems;
                        // Include only the fields you will use.
                        query.ViewFields = "<FieldRef Name='Title'/><FieldRef Name='Value'/><FieldRef Name='SortOrder'/>";
                        query.ViewFieldsOnly = true;

                        StringBuilder sb = new StringBuilder();
                        sb.Append("<Where>")
                                  .Append("<Eq>")
                                     .Append("<FieldRef Name='Title' />")
                                     .Append("<Value Type='Text'>" + key + "</Value>")
                                  .Append("</Eq>")
                               .Append("</Where>");
                        query.Query = sb.ToString();
                        spListItems = spList.GetItems(query);

                        for (int i = 0; i < spListItems.Count; i++)
                        {
                            SPListItem listItem = spListItems[i];
                            CommonDictionary commonDictionary = new CommonDictionary();
                            commonDictionary.Key = Convert.ToString(listItem["Title"]);
                            commonDictionary.Value = Convert.ToString(listItem["Value"]);
                            commonDictionary.SortOrder = Convert.ToInt32(listItem["SortOrder"]);


                            lstCommonDictionary.Add(commonDictionary);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(Utility->GetValuesByKey)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return lstCommonDictionary;
        }

        public static string GetValueByKey(string key)
        {
            SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("71", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, "1", "1");
            string value = "";
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oSPWeb = oSPsite.OpenWeb())
                    {
                        string listName = "CommonDictionary";
                        SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("72", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, oSPWeb.Url + "1333", "1333");
                        // Fetch the List
                        SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));
                        SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("73", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, "1000", "10000");
                        SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, spList.Title + "Title", "1");
                        SPQuery query = new SPQuery();
                        SPListItemCollection spListItems;
                        // Include only the fields you will use.
                        query.ViewFields = "<FieldRef Name='Value'/>";
                        query.ViewFieldsOnly = true;
                        query.RowLimit = 1; // Only select the top 1.
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<Where>")
                                  .Append("<Eq>")
                                     .Append("<FieldRef Name='Title' />")
                                     .Append("<Value Type='Text'>" + key + "</Value>")
                                  .Append("</Eq>")
                               .Append("</Where>");
                        query.Query = sb.ToString();
                        spListItems = spList.GetItems(query);
                        SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, spListItems.Count + "Count", "1");
                        if (spListItems.Count > 0)
                        {
                            SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, Convert.ToString(spListItems[0]["Value"]) + "111", "1");
                            value = Convert.ToString(spListItems[0]["Value"]);
                            SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("111SL.FG.PFL", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, "111", "1");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(Utility->GetValueByKey)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return value;
        }


        public static bool CompareUsername(string username1, string username2)
        {
            string u1 = username1;
            string u2 = username2;

            if (username1.Equals(username2, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (username1.Contains("\\"))
            {
                var temp = username1.Split('\\');
                if (temp.Length > 1)
                {
                    u1 = temp[1];
                }
            }
            if (username2.Contains("\\"))
            {
                var temp = username2.Split('\\');
                if (temp.Length > 1)
                {
                    u2 = temp[1];
                }
            }
            if (u1.Equals(u2, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public static string GetUsername(string data, bool withDomain = true)
        {
            string username = null;
            if (!String.IsNullOrEmpty(data))
            {
                if (data.Contains("|"))
                {
                    var temp = data.Split('|');
                    if (temp.Length > 1)
                    {
                        username = temp[1];
                    }
                }
                else
                {
                    username = data;
                }

                if (withDomain == false && username.Contains("\\"))
                {
                    var temp = data.Split('\\');
                    if (temp.Length > 1)
                    {
                        username = temp[1];
                    }
                }
            }
            return username;
        }


        public static string GetFormattedData(string data, string pattern, bool isSorted)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                var tempPattern = new[] { pattern };

                if (!String.IsNullOrEmpty(data) && !String.IsNullOrEmpty(pattern))
                {
                    var tempData = data.Split(tempPattern, StringSplitOptions.None);

                    if (tempData.Length > 0 && isSorted == false)
                    {
                        var tempDataReverse = tempData.Reverse();

                        foreach (var item in tempDataReverse)
                        {
                            if (!String.IsNullOrEmpty(item))
                            {
                                sb.Append(item);
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in tempData)
                        {
                            if (!String.IsNullOrEmpty(item))
                            {
                                sb.Append(item);
                            }
                        }
                    }
                }
                else
                {
                    sb.Append(data);
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(Utility->GetFormattedData)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return sb.ToString();
        }

        public static List<string> GetFormattedDataList(string data, string pattern, bool isSorted)
        {
            List<string> lstData = null;

            try
            {
                var tempPattern = new[] { pattern };

                if (!String.IsNullOrEmpty(data) && !String.IsNullOrEmpty(pattern))
                {
                    lstData = new List<string>();

                    var tempData = data.Split(tempPattern, StringSplitOptions.None);

                    if (tempData.Length > 0 && isSorted == false)
                    {
                        var tempDataReverse = tempData.Reverse();

                        foreach (var item in tempDataReverse)
                        {
                            if (!String.IsNullOrEmpty(item))
                            {
                                lstData.Add(item);
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in tempData)
                        {
                            if (!String.IsNullOrEmpty(item))
                            {
                                lstData.Add(item);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(Utility->GetFormattedDataList)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return lstData;
        }

        public static bool CompareUsers(SPUser user1, SPUser user2)
        {
            SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("90-Page_Load)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, "1", "1");
            if (user1 != null && user2 != null)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("91-Page_Load)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, "1", "1");
                if (user1.Email.Equals(user2.Email, StringComparison.OrdinalIgnoreCase))
                {
                    SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("92-Page_Load)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, "1", "1");
                    return true;
                }
                else {
                    SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("93-Page_Load)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, "1", "1");
                    return CompareUsername(user1.LoginName, user2.LoginName);
                }
                    
            }
            SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("94-Page_Load)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, "1", "1");
            return false;
        }
        public static List<SPUser> GetGroupMembers(string groupName)
        {

            List<SPUser> Users = new List<SPUser>();
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                    {
                        using (SPWeb oSPWeb_EP = oSPsite.OpenWeb())
                        {
                            if (!String.IsNullOrEmpty(groupName))
                            {
                                SPGroup Group = oSPWeb_EP.Groups[groupName];

                                foreach (SPUser user in Group.Users)
                                {
                                    Users.Add(user);
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(Utility->GetGroupMembers)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return Users;
        }

    }

}
