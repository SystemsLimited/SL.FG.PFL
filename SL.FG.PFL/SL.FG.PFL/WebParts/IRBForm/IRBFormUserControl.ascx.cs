
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using SL.FG.PFL.Layouts.SL.FG.PFL.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
namespace SL.FG.PFL.WebParts.IRBForm
{
    public partial class IRBFormUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    FillDropdowns();//Fill Dropdowns values

                    if (!String.IsNullOrEmpty(Page.Request.QueryString["FRID"]))
                    {
                        this.hdnFRID.Value = Page.Request.QueryString["FRID"];
                        int FRId;

                        Int32.TryParse(this.hdnFRID.Value, out FRId);

                        bool isSuccess = InitializeIRDIDetailedIncidenceControls(FRId);
                        if (isSuccess == false)
                        {
                            DisableControls(true);
                        }
                    }
                    else if (!String.IsNullOrEmpty(Page.Request.QueryString["IRB_Id"]))
                    {
                        this.hdnIRB_Id.Value = Page.Request.QueryString["IRB_Id"];
                        int IRB_Id;

                        Int32.TryParse(this.hdnIRB_Id.Value, out IRB_Id);

                        bool isSuccess = InitializeIRDIDetailedIncidenceControls(null, IRB_Id);
                        if (isSuccess == false)
                        {
                            DisableControls(true);
                        }
                    }
                    else
                    {
                        DisableControls(true);
                    }



                    DateTime date;
                    bool bValid = DateTime.TryParse(Convert.ToString(DateTime.Now.Date), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);

                    if (bValid)
                    {
                        this.approvalDate_dtc.SelectedDate = date;
                        this.targetDate_dtc.SelectedDate = date;
                    }
                    else
                    {
                        this.approvalDate_dtc.SelectedDate = DateTime.Now.Date;
                        this.targetDate_dtc.SelectedDate = DateTime.Now.Date;
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRB->Page_Load)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                message_div.InnerHtml = "Something went wrong!!!  Please contact the administrator.";
                DisableControls(false);
            }
        }

        private bool IRSavedAsDraft(SPWeb oSPWeb, int irid, int frid)
        {
            try
            {
                if (oSPWeb != null)
                {
                    string spListNameIR = "IRB";

                    SPList spListIR = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListNameIR));

                    string spListNameFR = "FlashReport";

                    SPList spListFR = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListNameFR));

                    if (spListFR != null && spListNameIR != null)
                    {
                        SPListItem spListItemFR = spListFR.GetItemById(frid);
                        SPListItem spListItemIR = spListIR.GetItemById(irid);

                        if (spListItemFR != null && spListItemIR != null)
                        {
                            string teamMembers = null;
                            string teamLead = null;

                            if (!String.IsNullOrEmpty(Convert.ToString(spListItemFR["TeamLead"])))
                            {
                                teamLead = Convert.ToString(spListItemFR["TeamLead"]);
                            }

                            if (!String.IsNullOrEmpty(Convert.ToString(spListItemFR["TeamMembers"])))
                            {
                                teamMembers = Convert.ToString(spListItemFR["TeamMembers"]);
                            }

                            StringBuilder sbAssignee = new StringBuilder();
                            StringBuilder sbAssigneeEmail = new StringBuilder();

                            SPUser spUser = Utility.GetUser(oSPWeb, teamLead);

                            if (spUser != null)
                            {
                                if (!String.IsNullOrEmpty(spUser.LoginName))
                                {
                                    sbAssignee.Append(Utility.GetUsername(spUser.LoginName))
                                              .Append(",");
                                }

                                if (!String.IsNullOrEmpty(spUser.Email))
                                {
                                    sbAssigneeEmail.Append(spUser.Email)
                                              .Append(",");
                                }
                            }

                            string[] TeamMembers = teamMembers.Split(',');

                            foreach (String member in TeamMembers)
                            {
                                spUser = Utility.GetUser(oSPWeb, member);

                                if (spUser != null)
                                {
                                    if (!String.IsNullOrEmpty(spUser.LoginName))
                                    {
                                        sbAssignee.Append(Utility.GetUsername(spUser.LoginName))
                                                  .Append(",");
                                    }

                                    if (!String.IsNullOrEmpty(spUser.Email))
                                    {
                                        sbAssigneeEmail.Append(spUser.Email)
                                                  .Append(",");
                                    }
                                }
                            }

                            spListItemIR["Assignee"] = Convert.ToString(sbAssignee);
                            spListItemIR["AssigneeEmail"] = Convert.ToString(sbAssigneeEmail);

                            //Update record
                            oSPWeb.AllowUnsafeUpdates = true;
                            spListItemIR.Update();
                            oSPWeb.AllowUnsafeUpdates = false;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRDI->IRSavedAsDraft)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                message_div.InnerHtml = "Something went wrong!!!  Please contact the administrator.";
                DisableControls(false);
            }
            return false;
        }
        private bool IRSubmittedByTeamLead(SPWeb oSPWeb, int irid, int frid)
        {
            try
            {
                if (oSPWeb != null)
                {
                    string spListNameIR = "IRB";

                    SPList spListIR = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListNameIR));

                    string spListNameFR = "FlashReport";

                    SPList spListFR = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListNameFR));

                    if (spListFR != null && spListNameIR != null)
                    {
                        SPListItem spListItemFR = spListFR.GetItemById(frid);
                        SPListItem spListItemIR = spListIR.GetItemById(irid);

                        if (spListItemFR != null && spListItemIR != null)
                        {
                            SPUser currentUser = oSPWeb.CurrentUser;

                            List<Message> lstMessage = new List<Message>();

                            string teamMembers = null;
                            string teamLead = null;

                            if (!String.IsNullOrEmpty(Convert.ToString(spListItemFR["TeamLead"])))
                            {
                                teamLead = Convert.ToString(spListItemFR["TeamLead"]);
                            }

                            if (!String.IsNullOrEmpty(Convert.ToString(spListItemFR["TeamMembers"])))
                            {
                                teamMembers = Convert.ToString(spListItemFR["TeamMembers"]);
                            }

                            string currentUsername = Utility.GetUsername(currentUser.LoginName, true);


                            if (!String.IsNullOrEmpty(currentUsername))
                            {
                                string formLink = Utility.GetRedirectUrl("IRBFormLink");
                                string subject = Utility.GetValueByKey("IRBFormLink_TE_Submitted_S");
                                string body = Utility.GetValueByKey("IRBFormLink_TE_Submitted_B");



                                StringBuilder linkSB = new StringBuilder();
                                linkSB.Append(formLink)
                                            .Append("?FRID=")
                                            .Append(spListItemFR.ID);

                                body = body.Replace("~|~", linkSB.ToString());

                                if (String.IsNullOrEmpty(subject))
                                {
                                    subject = "IRBFormLink_TE_Submitted_S";
                                }

                                if (String.IsNullOrEmpty(body))
                                {
                                    body = linkSB.ToString();
                                }


                                SPUser spUser = Utility.GetUser(oSPWeb, teamLead);

                                if (spUser != null)
                                {
                                    Message message = new Message();
                                    message.Subject = subject;
                                    message.Body = body;
                                    message.From = currentUser.Email;
                                    message.To = spUser.Email;

                                    lstMessage.Add(message);

                                    if (!String.IsNullOrEmpty(this.hdnApprovalAuthority.Value))
                                    {
                                        spListItemIR["Assignee"] = this.hdnApprovalAuthority.Value;

                                        SPUser assigneeUser = Utility.GetUser(oSPWeb, this.hdnApprovalAuthority.Value);

                                        if (assigneeUser != null && !String.IsNullOrEmpty(assigneeUser.Email))
                                        {
                                            spListItemIR["AssigneeEmail"] = assigneeUser.Email;

                                            message = new Message();
                                            message.Subject = subject;
                                            message.Body = body;
                                            message.From = spUser.Email;
                                            message.To = assigneeUser.Email;

                                            lstMessage.Add(message);
                                        }
                                    }
                                }


                                string[] TeamMembers = teamMembers.Split(',');

                                foreach (String member in TeamMembers)
                                {
                                    spUser = Utility.GetUser(oSPWeb, member);

                                    if (spUser != null)
                                    {
                                        Message message = new Message();
                                        message.Subject = subject;
                                        message.Body = body;
                                        message.From = currentUser.Email;
                                        message.To = spUser.Email;

                                        lstMessage.Add(message);
                                    }
                                }
                                //Update record
                                oSPWeb.AllowUnsafeUpdates = true;
                                spListItemIR.Update();
                                oSPWeb.AllowUnsafeUpdates = false;

                                return Email.SendEmail(lstMessage);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRDI->IRSubmittedByTeamLead)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                message_div.InnerHtml = "Something went wrong!!!  Please contact the administrator.";
                DisableControls(false);
            }
            return false;
        }
        private bool IRRejected(SPWeb oSPWeb, int irid, int frid)
        {
            try
            {
                if (oSPWeb != null)
                {
                    string spListNameIR = "IRB";

                    SPList spListIR = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListNameIR));

                    string spListNameFR = "FlashReport";

                    SPList spListFR = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListNameFR));

                    if (spListFR != null && spListNameIR != null)
                    {
                        SPListItem spListItemFR = spListFR.GetItemById(frid);
                        SPListItem spListItemIR = spListIR.GetItemById(irid);

                        if (spListItemFR != null && spListItemIR != null)
                        {
                            SPUser currentUser = oSPWeb.CurrentUser;

                            List<Message> lstMessage = new List<Message>();

                            string teamMembers = null;
                            string teamLead = null;

                            if (!String.IsNullOrEmpty(Convert.ToString(spListItemFR["TeamLead"])))
                            {
                                teamLead = Convert.ToString(spListItemFR["TeamLead"]);
                            }

                            if (!String.IsNullOrEmpty(Convert.ToString(spListItemFR["TeamMembers"])))
                            {
                                teamMembers = Convert.ToString(spListItemFR["TeamMembers"]);
                            }


                            string currentUsername = Utility.GetUsername(currentUser.LoginName, true);


                            if (!String.IsNullOrEmpty(currentUsername))
                            {
                                string formLink = Utility.GetRedirectUrl("IRBFormLink");
                                string subject = Utility.GetValueByKey("IRBFormLink_TE_Reject_S");
                                string body = Utility.GetValueByKey("IRBFormLink_TE_Reject_B");

                                StringBuilder linkSB = new StringBuilder();
                                linkSB.Append(formLink)
                                            .Append("?FRID=")
                                            .Append(spListItemFR.ID);

                                body = body.Replace("~|~", linkSB.ToString());

                                if (String.IsNullOrEmpty(subject))
                                {
                                    subject = "IRBFormLink_TE_Reject_S";
                                }

                                if (String.IsNullOrEmpty(body))
                                {
                                    body = linkSB.ToString();
                                }

                                StringBuilder sbAssignee = new StringBuilder();
                                StringBuilder sbAssigneeEmail = new StringBuilder();

                                SPUser spUser = Utility.GetUser(oSPWeb, teamLead);

                                if (spUser != null)
                                {
                                    Message message = new Message();
                                    message.Subject = subject;
                                    message.Body = body;
                                    message.From = currentUser.Email;
                                    message.To = spUser.Email;

                                    lstMessage.Add(message);

                                    if (!String.IsNullOrEmpty(spUser.LoginName))
                                    {
                                        sbAssignee.Append(Utility.GetUsername(spUser.LoginName))
                                                  .Append(",");
                                    }

                                    if (!String.IsNullOrEmpty(spUser.Email))
                                    {
                                        sbAssigneeEmail.Append(spUser.Email)
                                                  .Append(",");
                                    }
                                }

                                string[] TeamMembers = teamMembers.Split(',');

                                foreach (String member in TeamMembers)
                                {
                                    spUser = Utility.GetUser(oSPWeb, member);

                                    if (spUser != null)
                                    {
                                        Message message = new Message();
                                        message.Subject = subject;
                                        message.Body = body;
                                        message.From = currentUser.Email;
                                        message.To = spUser.Email;

                                        lstMessage.Add(message);

                                        if (!String.IsNullOrEmpty(spUser.LoginName))
                                        {
                                            sbAssignee.Append(Utility.GetUsername(spUser.LoginName))
                                                      .Append(",");
                                        }

                                        if (!String.IsNullOrEmpty(spUser.Email))
                                        {
                                            sbAssigneeEmail.Append(spUser.Email)
                                                      .Append(",");
                                        }
                                    }
                                }

                                spListItemIR["Assignee"] = Convert.ToString(sbAssignee);
                                spListItemIR["AssigneeEmail"] = Convert.ToString(sbAssigneeEmail);

                                //Update record
                                oSPWeb.AllowUnsafeUpdates = true;
                                spListItemIR.Update();
                                oSPWeb.AllowUnsafeUpdates = false;

                                string approversGroup = Utility.GetValueByKey("HSE_Approvers");

                                List<SPUser> lstSPUsers = Utility.GetGroupMembers(approversGroup);

                                foreach (SPUser itemUser in lstSPUsers)
                                {
                                    if (itemUser != null)
                                    {
                                        Message message = new Message();
                                        message.Subject = subject;
                                        message.Body = body;
                                        message.From = currentUser.Email;
                                        message.To = itemUser.Email;

                                        lstMessage.Add(message);
                                    }
                                }

                                return Email.SendEmail(lstMessage);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRDI->IRRejected)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                message_div.InnerHtml = "Something went wrong!!!  Please contact the administrator.";
                DisableControls(false);
            }
            return false;
        }
        private bool IRApproved(SPWeb oSPWeb, int irid, int frid)
        {
            try
            {
                if (oSPWeb != null)
                {
                    string spListNameIR = "IRB";

                    SPList spListIR = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListNameIR));

                    string spListNameFR = "FlashReport";

                    SPList spListFR = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListNameFR));

                    if (spListFR != null && spListNameIR != null)
                    {
                        SPListItem spListItemFR = spListFR.GetItemById(frid);
                        SPListItem spListItemIR = spListIR.GetItemById(irid);

                        if (spListItemFR != null && spListItemIR != null)
                        {
                            SPUser currentUser = oSPWeb.CurrentUser;

                            List<Message> lstMessage = new List<Message>();

                            string teamMembers = null;
                            string teamLead = null;

                            if (!String.IsNullOrEmpty(Convert.ToString(spListItemFR["TeamLead"])))
                            {
                                teamLead = Convert.ToString(spListItemFR["TeamLead"]);
                            }

                            if (!String.IsNullOrEmpty(Convert.ToString(spListItemFR["TeamMembers"])))
                            {
                                teamMembers = Convert.ToString(spListItemFR["TeamMembers"]);
                            }

                            string currentUsername = Utility.GetUsername(currentUser.LoginName, true);


                            if (!String.IsNullOrEmpty(currentUsername))
                            {
                                string formLink = Utility.GetRedirectUrl("IRBFormLink");
                                string subject = Utility.GetValueByKey("IRBFormLink_TE_Approve_S");
                                string body = Utility.GetValueByKey("IRBFormLink_TE_Approve_B");

                                StringBuilder linkSB = new StringBuilder();
                                linkSB.Append(formLink)
                                            .Append("?FRID=")
                                            .Append(spListItemFR.ID);

                                body = body.Replace("~|~", linkSB.ToString());

                                if (String.IsNullOrEmpty(subject))
                                {
                                    subject = "IRBFormLink_TE_Approve_S";
                                }

                                if (String.IsNullOrEmpty(body))
                                {
                                    body = linkSB.ToString();
                                }

                                SPUser spUser = Utility.GetUser(oSPWeb, teamLead);

                                if (spUser != null)
                                {
                                    Message message = new Message();
                                    message.Subject = subject;
                                    message.Body = body;
                                    message.From = currentUser.Email;
                                    message.To = spUser.Email;

                                    lstMessage.Add(message);
                                }

                                string[] TeamMembers = teamMembers.Split(',');

                                foreach (String member in TeamMembers)
                                {
                                    spUser = Utility.GetUser(oSPWeb, member);

                                    if (spUser != null)
                                    {
                                        Message message = new Message();
                                        message.Subject = subject;
                                        message.Body = body;
                                        message.From = currentUser.Email;
                                        message.To = spUser.Email;

                                        lstMessage.Add(message);
                                    }
                                }

                                string approversGroup = Utility.GetValueByKey("HSE_Approvers");

                                List<SPUser> lstSPUsers = Utility.GetGroupMembers(approversGroup);


                                StringBuilder sbAssignee = new StringBuilder();
                                StringBuilder sbAssigneeEmail = new StringBuilder();


                                foreach (SPUser itemUser in lstSPUsers)
                                {
                                    if (itemUser != null)
                                    {
                                        Message message = new Message();
                                        message.Subject = subject;
                                        message.Body = body;
                                        message.From = currentUser.Email;
                                        message.To = itemUser.Email;

                                        lstMessage.Add(message);
                                    }

                                    if (itemUser != null)
                                    {
                                        if (!String.IsNullOrEmpty(itemUser.LoginName))
                                        {
                                            sbAssignee.Append(Utility.GetUsername(itemUser.LoginName))
                                                      .Append(",");
                                        }

                                        if (!String.IsNullOrEmpty(itemUser.Email))
                                        {
                                            sbAssigneeEmail.Append(itemUser.Email)
                                                      .Append(",");
                                        }
                                    }
                                }


                                spListItemIR["Assignee"] = Convert.ToString(sbAssignee);
                                spListItemIR["AssigneeEmail"] = Convert.ToString(sbAssigneeEmail);

                                //Update record
                                oSPWeb.AllowUnsafeUpdates = true;
                                spListItemIR.Update();
                                oSPWeb.AllowUnsafeUpdates = false;

                                return Email.SendEmail(lstMessage);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRDI->IRApproved)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                message_div.InnerHtml = "Something went wrong!!!  Please contact the administrator.";
                DisableControls(false);
            }
            return false;
        }
        private bool IRForwarded(SPWeb oSPWeb, int irid, int frid)
        {
            try
            {
                if (oSPWeb != null)
                {
                    string spListNameIR = "IRB";

                    SPList spListIR = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListNameIR));

                    string spListNameFR = "FlashReport";

                    SPList spListFR = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListNameFR));

                    if (spListFR != null && spListNameIR != null)
                    {
                        SPListItem spListItemFR = spListFR.GetItemById(frid);
                        SPListItem spListItemIR = spListIR.GetItemById(irid);

                        if (spListItemFR != null && spListItemIR != null)
                        {
                            SPUser currentUser = oSPWeb.CurrentUser;
                            List<Message> lstMessage = new List<Message>();

                            string formLink = Utility.GetRedirectUrl("IRBFormLink");
                            string subject = Utility.GetValueByKey("IRBFormLink_TE_Forward_S");
                            string body = Utility.GetValueByKey("IRBFormLink_TE_Forward_B");

                            StringBuilder linkSB = new StringBuilder();
                            linkSB.Append(formLink)
                                        .Append("?IRB_Id=")
                                        .Append(irid);

                            body = body.Replace("~|~", linkSB.ToString());

                            if (String.IsNullOrEmpty(subject))
                            {
                                subject = "IRBFormLink_TE_Forward_S";
                            }

                            if (String.IsNullOrEmpty(body))
                            {
                                body = linkSB.ToString();
                            }



                            if (currentUser != null)
                            {
                                string approversGroup = Utility.GetValueByKey("HSE_Approvers");

                                List<SPUser> lstSPUsers1 = Utility.GetGroupMembers(approversGroup);

                                foreach (SPUser itemUser in lstSPUsers1)
                                {
                                    if (itemUser != null)
                                    {
                                        Message message = new Message();
                                        message.Subject = subject;
                                        message.Body = body;
                                        message.From = currentUser.Email;
                                        message.To = itemUser.Email;

                                        lstMessage.Add(message);
                                    }
                                }

                                string masterGroup = Utility.GetValueByKey("MasterGroup");

                                List<SPUser> lstSPUsers2 = Utility.GetGroupMembers(approversGroup);

                                StringBuilder sbAssignee = new StringBuilder();
                                StringBuilder sbAssigneeEmail = new StringBuilder();


                                foreach (SPUser itemUser in lstSPUsers2)
                                {
                                    if (itemUser != null && !lstSPUsers1.Contains(itemUser))
                                    {
                                        Message message = new Message();
                                        message.Subject = subject;
                                        message.Body = body;
                                        message.From = currentUser.Email;
                                        message.To = itemUser.Email;

                                        lstMessage.Add(message);
                                    }

                                    if (itemUser != null)
                                    {
                                        if (!String.IsNullOrEmpty(itemUser.LoginName))
                                        {
                                            sbAssignee.Append(Utility.GetUsername(itemUser.LoginName))
                                                      .Append(",");
                                        }

                                        if (!String.IsNullOrEmpty(itemUser.Email))
                                        {
                                            sbAssigneeEmail.Append(itemUser.Email)
                                                      .Append(",");
                                        }
                                    }
                                }

                                spListItemIR["Assignee"] = Convert.ToString(sbAssignee);
                                spListItemIR["AssigneeEmail"] = Convert.ToString(sbAssigneeEmail);

                                //Update record
                                oSPWeb.AllowUnsafeUpdates = true;
                                spListItemIR.Update();
                                oSPWeb.AllowUnsafeUpdates = false;


                                return Email.SendEmail(lstMessage);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRForwarded)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                message_div.InnerHtml = "Something went wrong!!!  Please contact the administrator.";
                DisableControls(false);
            }
            return false;
        }
        private bool IRLastSaved(SPWeb oSPWeb, int irid, int frid)
        {
            try
            {
                if (oSPWeb != null)
                {
                    string spListNameIR = "IRB";

                    SPList spListIR = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListNameIR));

                    string spListNameFR = "FlashReport";

                    SPList spListFR = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListNameFR));

                    if (spListFR != null && spListNameIR != null)
                    {
                        SPListItem spListItemFR = spListFR.GetItemById(frid);
                        SPListItem spListItemIR = spListIR.GetItemById(irid);

                        if (spListItemFR != null && spListItemIR != null)
                        {
                            SPUser currentUser = oSPWeb.CurrentUser;
                            List<Message> lstMessage = new List<Message>();

                            string formLink = Utility.GetRedirectUrl("IRBFormLink");
                            string subject = Utility.GetValueByKey("IRBFormLink_TE_LastSaved_S");
                            string body = Utility.GetValueByKey("IRBFormLink_TE_LastSaved_B");

                            StringBuilder linkSB = new StringBuilder();
                            linkSB.Append(formLink)
                                        .Append("?IRB_Id=")
                                        .Append(irid);

                            body = body.Replace("~|~", linkSB.ToString());

                            if (String.IsNullOrEmpty(subject))
                            {
                                subject = "IRBFormLink_TE_LastSaved_S";
                            }

                            if (String.IsNullOrEmpty(body))
                            {
                                body = linkSB.ToString();
                            }

                            if (currentUser != null)
                            {
                                string approversGroup = Utility.GetValueByKey("MasterGroup");

                                List<SPUser> lstSPUsers = Utility.GetGroupMembers(approversGroup);

                                foreach (SPUser itemUser in lstSPUsers)
                                {
                                    if (itemUser != null)
                                    {
                                        Message message = new Message();
                                        message.Subject = subject;
                                        message.Body = body;
                                        message.From = currentUser.Email;
                                        message.To = itemUser.Email;

                                        lstMessage.Add(message);
                                    }
                                }

                                return Email.SendEmail(lstMessage);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRLastSaved)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                message_div.InnerHtml = "Something went wrong!!!  Please contact the administrator.";
                DisableControls(false);
            }
            return false;
        }

        private void DisableControls(bool disableAll)
        {
            this.btnSave.Visible = false;
            this.btnSaveAsDraft.Visible = false;
            this.btnForward.Visible = false;
            this.btnReject.Visible = false;
            this.btnApprove.Visible = false;
            this.btnLastSave.Visible = false;

            if (disableAll)
            {
                this.rvf_reportViewed_ta.Disabled = true;
                this.UM_HSE_Comments_ta.Disabled = true;

                this.rootCauses_tf.Disabled = true;
                this.peopleInterviewed_tf.Disabled = true;
                this.keyFindings_tf.Disabled = true;

                this.responsiblePerson_PeopleEditor.Enabled = false;
                this.responsibleSection_ddl.Disabled = true;
                this.targetDate_dtc.Enabled = false;

                this.conclusion_ta.Disabled = true;

                this.hdnIsChangesAllowed.Value = "0";

                DisableDropdown(this.PSMsViolated_ddl);
                DisableDropdown(this.procedureRelatedCause_Proc_R_ddl);
                DisableDropdown(this.procedureRelatedCause_Per_R_ddl);
                DisableDropdown(this.causeOfIncident_PR_ddl);
                DisableDropdown(this.causeOfIncident_ER_ddl);
                DisableDropdown(this.basicActivityInProgress_ddl);
            }
        }

        private void FillDropdowns()
        {
            using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
            {
                using (SPWeb oSPWeb = oSPsite.OpenWeb())
                {
                    FillArea(oSPWeb);
                }
            }
        }
        private void FillArea(SPWeb oSPWeb)
        {
            try
            {
                string listName = "Area";

                // Fetch the List
                SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));

                SPQuery query = new SPQuery();
                SPListItemCollection spListItems;
                // Include only the fields you will use.
                query.ViewFields = "<FieldRef Name='ID'/><FieldRef Name='Title'/>";
                query.ViewFieldsOnly = true;
                //query.RowLimit = 200; // Only select the top 200.
                StringBuilder sb = new StringBuilder();
                sb.Append("<OrderBy Override='TRUE;><FieldRef Name='Title'/></OrderBy>");
                query.Query = sb.ToString();
                spListItems = spList.GetItems(query);


                this.incidentUnitArea_ddl.DataSource = spListItems;
                this.incidentUnitArea_ddl.DataTextField = "Title";
                this.incidentUnitArea_ddl.DataValueField = "Title"; //As we dont save Area Id, therefore no need to use here
                this.incidentUnitArea_ddl.DataBind();

                this.incidentUnitArea_ddl.Items.Insert(0, new ListItem("Please Select", "0"));
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRB->FillArea)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("Msg_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator.";
                }

                message_div.InnerHtml = message;
                DisableControls(true);
            }
        }

        private void FillSelectedValues(HtmlSelect control_ddl, string data, char delimeter)
        {
            if (!String.IsNullOrEmpty(data) && control_ddl != null)
            {
                string[] items = data.Split(delimeter);

                foreach (string item in items)
                {
                    control_ddl.Items.FindByValue(item).Selected = true;
                }
            }
        }

        private void DisableDropdown(HtmlSelect control_ddl)
        {
            if (control_ddl != null)
            {
                for (int i = 0; i < control_ddl.Items.Count; i++)
                {
                    control_ddl.Items[i].Enabled = false;
                    control_ddl.Items[i].Attributes.Add("disabled", "disabled");
                    control_ddl.Items[i].Attributes.Add("style", "background-color: white");
                }
            }
        }
        private bool CheckPermission()
        {
            bool isMember = false;
            using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
            {
                using (SPWeb oSPWeb = oSPsite.OpenWeb())
                {
                    string groupName = Utility.GetValueByKey("MasterGroup");
                    var spGroup = oSPWeb.Groups[groupName];
                    if (spGroup != null)
                    {
                        isMember = oSPWeb.IsCurrentUserMemberOfGroup(spGroup.ID);
                    }
                }
            }
            return isMember;
        }

        private bool CheckPermissionHSE_Approvers()
        {
            bool isMember = false;
            using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
            {
                using (SPWeb oSPWeb = oSPsite.OpenWeb())
                {
                    string groupName = Utility.GetValueByKey("HSE_Approvers");
                    var spGroup = oSPWeb.Groups[groupName];
                    if (spGroup != null)
                    {
                        isMember = oSPWeb.IsCurrentUserMemberOfGroup(spGroup.ID);
                    }
                }
            }
            return isMember;
        }


        private void UpdateHSEMembersControl(bool isApproved, bool isSubmitted)
        {
            if (isApproved)
            {
                this.incidentDescription_ta.Disabled = true;
                //this.incidentActionsTaken_ta.Disabled = true;
                this.approvalDate_div.Visible = true;
            }

            if (isApproved && isSubmitted)
            {
                this.IRRCQuality_div.Visible = true;
            }

            this.HSEDepartment_div.Visible = true;
        }
        private int GetIRIdByFlashReportId(SPWeb oSPWeb, int flashReportId)
        {
            int IRID = 0;

            try
            {
                string listName = "IRB";

                // Fetch the List
                SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));

                SPQuery query = new SPQuery();
                SPListItemCollection spListItems;
                // Include only the fields you will use.
                query.ViewFields = "<FieldRef Name='ID'/>";
                query.ViewFieldsOnly = true;
                query.RowLimit = 1;
                StringBuilder sb = new StringBuilder();
                sb.Append("<Where><Eq><FieldRef Name='FlashReportID' /><Value Type='Text'>" + Convert.ToString(flashReportId) + "</Value></Eq></Where>");
                query.Query = sb.ToString();
                spListItems = spList.GetItems(query);

                if (spListItems.Count > 0)
                {
                    IRID = Convert.ToInt32(spListItems[0]["ID"]);
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRB->IsFlashReportSubmitted)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("Msg_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator.";
                }

                message_div.InnerHtml = message;
                DisableControls(true);
            }
            return IRID;
        }

        private int CheckUserIsTeamLeadOrMember(SPUser currentUser, SPListItem spListItemFR)
        {
            string teamMembers = null;
            string teamLead = null;

            if (!String.IsNullOrEmpty(Convert.ToString(spListItemFR["TeamLead"])))
            {
                teamLead = Convert.ToString(spListItemFR["TeamLead"]);
            }

            if (!String.IsNullOrEmpty(Convert.ToString(spListItemFR["TeamMembers"])))
            {
                teamMembers = Convert.ToString(spListItemFR["TeamMembers"]);
            }

            string currentUsername = Utility.GetUsername(currentUser.LoginName, true);


            if (!String.IsNullOrEmpty(currentUsername))
            {
                currentUsername = currentUsername.ToLower();

                if (!String.IsNullOrEmpty(teamLead) && teamLead.ToLower().Contains(currentUsername))
                {
                    return 1;
                }
                else if (!String.IsNullOrEmpty(teamMembers) && teamMembers.ToLower().Contains(currentUsername))
                {
                    return 2;
                }
            }
            return 0;
        }

        private bool InitializeIRDIDetailedIncidenceControls(int? FRID_ = null, int? IRB_ID = null)
        {
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oSPWeb = oSPsite.OpenWeb())
                    {
                        SPUser currentUser = oSPWeb.CurrentUser;

                        SPListItem spListItemIRB = null;

                        if (FRID_ != null)
                        {
                            this.hdnFRID.Value = Convert.ToString(FRID_);

                            int IRID_temp = GetIRIdByFlashReportId(oSPWeb, (int)FRID_);

                            if (IRID_temp > 0)
                            {
                                IRB_ID = IRID_temp;
                            }

                            string spListNameFR = "FlashReport";

                            SPList spListFR = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListNameFR));

                            if (spListFR != null)
                            {
                                SPListItem spListItemFR = spListFR.GetItemById((int)FRID_);

                                if (spListItemFR != null)
                                {
                                    int result = CheckUserIsTeamLeadOrMember(currentUser, spListItemFR);

                                    if (result == 1) //Team Lead
                                    {
                                        DisableControls(false);
                                        this.btnSave.Visible = true;
                                        this.btnSaveAsDraft.Visible = true;
                                    }
                                    else if (result == 2) //Team Member
                                    {
                                        DisableControls(false);
                                        this.btnSaveAsDraft.Visible = true;
                                    }
                                    else //Check Master Group Membership
                                    {
                                        DisableControls(true);
                                        if (!CheckPermission())
                                        {
                                            string accessDeniedUrl = Utility.GetRedirectUrl("Access_Denied");

                                            if (!String.IsNullOrEmpty(accessDeniedUrl))
                                            {
                                                DisableControls(true);
                                                Page.Response.Redirect(accessDeniedUrl, false);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        bool isSubmitted = false;
                        bool isApproved = false;


                        if (IRB_ID != null)
                        {
                            string spListNameIRB = "IRB";

                            SPList spListIRB = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListNameIRB));

                            if (spListIRB != null)
                            {
                                this.hdnIRB_Id.Value = Convert.ToString(IRB_ID);

                                int IRBId = Convert.ToInt32(this.hdnIRB_Id.Value);

                                spListItemIRB = spListIRB.GetItemById(IRBId);

                                if (spListIRB != null)
                                {
                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRB["Conclusion"])))
                                    {
                                        this.conclusion_ta.Value = Convert.ToString(spListItemIRB["Conclusion"]);
                                    }

                                    //if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRB["ActualOrPotentialInquiry"])))
                                    //{
                                    //    this.hdnActualOrPotentialInquiry.Value = Convert.ToString(spListItemIRB["ActualOrPotentialInquiry"]);

                                    //    FillSelectedValues(this.actualOrPotentialInquiry_ddl, this.hdnActualOrPotentialInquiry.Value, delimeter);
                                    //}
                                    //if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRB["ProcessSafetyIncidents"])))
                                    //{
                                    //    this.hdnProcessSafetyIncidents.Value = Convert.ToString(spListItemIRB["ProcessSafetyIncidents"]);

                                    //    FillSelectedValues(this.processSafetyIncidents_ddl, this.hdnProcessSafetyIncidents.Value, delimeter);
                                    //}
                                    //if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRB["MiscellaneousIncidents"])))
                                    //{
                                    //    this.hdnMiscellaneousIncidents.Value = Convert.ToString(spListItemIRB["MiscellaneousIncidents"]);

                                    //    FillSelectedValues(this.miscellaneousIncidents_ddl, this.hdnMiscellaneousIncidents.Value, delimeter);
                                    //}
                                    //if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRB["BasicActivityInProgress"])))
                                    //{
                                    //    this.hdnBasicActivityInProgress.Value = Convert.ToString(spListItemIRB["BasicActivityInProgress"]);

                                    //    FillSelectedValues(this.basicActivityInProgress_ddl, this.hdnBasicActivityInProgress.Value, delimeter);
                                    //}
                                    //if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRB["RiskBasedElements"])))
                                    //{
                                    //    this.hdnRiskBasedElements.Value = Convert.ToString(spListItemIRB["RiskBasedElements"]);

                                    //    FillSelectedValues(this.riskBasedElements_ddl, this.hdnRiskBasedElements.Value, delimeter);
                                    //}
                                    //if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRB["CulturalBasedElements"])))
                                    //{
                                    //    this.hdnCulturalBasedElements.Value = Convert.ToString(spListItemIRB["CulturalBasedElements"]);

                                    //    FillSelectedValues(this.culturalBasedElements_ddl, this.hdnCulturalBasedElements.Value, delimeter);
                                    //}

                                    if (spListItemIRB["IsSubmitted"] != null && !String.IsNullOrEmpty(Convert.ToString(spListItemIRB["IsSubmitted"])))
                                    {
                                        isSubmitted = Convert.ToBoolean(spListItemIRB["IsSubmitted"]);
                                    }

                                    if (spListItemIRB["IsApproved"] != null && !String.IsNullOrEmpty(Convert.ToString(spListItemIRB["IsApproved"])))
                                    {
                                        isApproved = Convert.ToBoolean(spListItemIRB["IsApproved"]);
                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRB["FlashReportID"])))
                                    {
                                        this.hdnFRID.Value = Convert.ToString(spListItemIRB["FlashReportID"]);
                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRB["KeyFindings"])))
                                    {
                                        this.hdnKeyFindingsList.Value = Convert.ToString(spListItemIRB["KeyFindings"]);
                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRB["PeopleInterviewed"])))
                                    {
                                        this.hdnPeopleInterviewedList.Value = Convert.ToString(spListItemIRB["PeopleInterviewed"]);
                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRB["RootCauses"])))
                                    {
                                        this.hdnRootCausesList.Value = Convert.ToString(spListItemIRB["RootCauses"]);
                                    }

                                    if (spListItemIRB["ApprovalDate1"] != null && !String.IsNullOrEmpty(Convert.ToString(spListItemIRB["ApprovalDate1"])))
                                    {
                                        DateTime date;
                                        bool bValid = DateTime.TryParse(Convert.ToString(spListItemIRB["ApprovalDate1"]), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);

                                        if (!bValid)
                                        {
                                            date = Convert.ToDateTime(spListItemIRB["ApprovalDate1"]);
                                        }

                                        this.approvalDate_dtc.SelectedDate = date;
                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRB["HSEApprover"])))
                                    {
                                        this.rvf_reportViewed_ta.Value = Convert.ToString(spListItemIRB["HSEApprover"]);
                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRB["HSEComments"])))
                                    {
                                        this.UM_HSE_Comments_ta.Value = Convert.ToString(spListItemIRB["HSEComments"]);
                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRB["IRRCQualityScore"])))
                                    {
                                        this.IRRCQualityScore_ta.Value = Convert.ToString(spListItemIRB["IRRCQualityScore"]);
                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRB["QualityAssessedBy"])))
                                    {
                                        this.IRRCQualityAccessedBy_ta.Value = Convert.ToString(spListItemIRB["QualityAssessedBy"]);
                                    }

                                    string p1 = "~|~"; //separate records
                                    string p2 = "*|*"; //separate content with in a record

                                    //Key Findings List
                                    List<string> lstKeyFindings = Utility.GetFormattedDataList(this.hdnKeyFindingsList.Value, p1, true);

                                    //People Interviewed List
                                    List<string> lstPeopleInterviewed = Utility.GetFormattedDataList(this.hdnPeopleInterviewedList.Value, p1, true);

                                    //Root Causes List
                                    List<string> lstRootCauses = Utility.GetFormattedDataList(this.hdnRootCausesList.Value, p1, true);


                                    if (lstKeyFindings != null)
                                    {
                                        FillKeyFindingsGrid(lstKeyFindings);
                                    }

                                    if (lstPeopleInterviewed != null)
                                    {
                                        FillPeopleInterviewedGrid(lstPeopleInterviewed);
                                    }

                                    if (lstRootCauses != null)
                                    {
                                        FillRootCausesGrid(lstRootCauses);
                                    }

                                    //Recommendations
                                    List<IRRecommendation_OnJob> lstRecommendation = GetFormattedRecommendationsByIRDI_Id(oSPWeb, IRBId);

                                    StringBuilder ids = new StringBuilder();

                                    if (lstRecommendation != null)
                                    {
                                        ids.Append(p1);

                                        //Add recommendations in grid
                                        foreach (var recommendation in lstRecommendation)
                                        {
                                            HtmlTableRow tRow = new HtmlTableRow();

                                            tRow.Attributes.Add("class", "recommendationItem");

                                            tRow.Cells.Add(new HtmlTableCell() { InnerHtml = Convert.ToString(this.recommendationDetails_table.Rows.Count) });

                                            HtmlTableCell recommendationId = new HtmlTableCell();
                                            HtmlTableCell recommendationNo = new HtmlTableCell();
                                            HtmlTableCell description = new HtmlTableCell();
                                            HtmlTableCell responsiblePersonUsername = new HtmlTableCell();
                                            HtmlTableCell responsiblePersonEmail = new HtmlTableCell();
                                            HtmlTableCell responsibleSection = new HtmlTableCell();
                                            HtmlTableCell responsibleSectionId = new HtmlTableCell();
                                            HtmlTableCell responsibleDepartment = new HtmlTableCell();
                                            HtmlTableCell responsibleDepartmentId = new HtmlTableCell();
                                            HtmlTableCell targetDate = new HtmlTableCell();
                                            HtmlTableCell concurrenceOfRP = new HtmlTableCell();
                                            HtmlTableCell status = new HtmlTableCell();

                                            string actions = "<span class='btn btn-default editRecommendation' ><i class='glyphicon glyphicon-pencil'></i></span><span class='btn btn-danger removeRecommendation'><i class='glyphicon glyphicon-remove'></i></span>";

                                            recommendationId.InnerHtml = "<span class='recommendationId'>" + Convert.ToString(recommendation.RecommendationId) + "</span>";
                                            recommendationId.Attributes.Add("style", "display:none");

                                            recommendationNo.InnerHtml = "<span class='recommendationNo'>" + Convert.ToString(recommendation.RecommendationNo) + "</span>";
                                            recommendationNo.Attributes.Add("style", "display:none");

                                            description.Attributes.Add("class", "td-description");
                                            description.InnerHtml = "<span class='description'>" + Convert.ToString(recommendation.Description) + "</span>";
                                            responsiblePersonUsername.InnerHtml = "<span class='username'>" + Convert.ToString(recommendation.RPUsername) + "</span>";

                                            responsiblePersonEmail.InnerHtml = "<span class='email'>" + Convert.ToString(recommendation.RPEmail) + "</span>";
                                            responsiblePersonEmail.Attributes.Add("style", "display:none");

                                            responsibleSection.InnerHtml = "<span class='sectionName'>" + Convert.ToString(recommendation.SectionName) + "</span>";

                                            responsibleSectionId.InnerHtml = "<span class='sectionId'>" + Convert.ToString(recommendation.SectionId) + "</span>";
                                            responsibleSectionId.Attributes.Add("style", "display:none");

                                            responsibleDepartment.InnerHtml = "<span class='departmentName'>" + Convert.ToString(recommendation.DepartmentName) + "</span>";

                                            responsibleDepartmentId.InnerHtml = "<span class='departmentId'>" + Convert.ToString(recommendation.DepartmentId) + "</span>";
                                            responsibleDepartmentId.Attributes.Add("style", "display:none");

                                            targetDate.InnerHtml = "<span class='targetDate'>" + Convert.ToString(recommendation.TargetDate) + "</span>";

                                            concurrenceOfRP.InnerHtml = "<span class='concurrenceOfRP'>" + ((recommendation.ConcurrenceOfRP == true) ? "Yes" : "No") + "</span>";
                                            status.InnerHtml = "<span class='status'>" + Convert.ToString(recommendation.Status) + "</span>";

                                            tRow.Cells.Add(recommendationId);
                                            tRow.Cells.Add(description);
                                            tRow.Cells.Add(responsiblePersonUsername);
                                            tRow.Cells.Add(responsibleSection);
                                            tRow.Cells.Add(responsibleSectionId);
                                            tRow.Cells.Add(responsibleDepartment);
                                            tRow.Cells.Add(responsibleDepartmentId);
                                            tRow.Cells.Add(targetDate);
                                            tRow.Cells.Add(concurrenceOfRP);
                                            tRow.Cells.Add(status);

                                            tRow.Cells.Add(new HtmlTableCell() { InnerHtml = actions });

                                            this.recommendationDetails_table.Rows.Add(tRow);

                                            ids.Append(Convert.ToString(recommendation.RecommendationId));
                                            ids.Append(p2);
                                        }

                                        this.hdnIdList.Value = ids.ToString();
                                    }
                                }
                            }
                        }

                        if (!String.IsNullOrEmpty(this.hdnFRID.Value))
                        {
                            int FRID = Convert.ToInt32(this.hdnFRID.Value);

                            string spListNameFR = "FlashReport";

                            SPList spListFR = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListNameFR));

                            if (spListFR != null)
                            {
                                SPListItem spListItemFR = spListFR.GetItemById(FRID);

                                if (spListItemFR != null)
                                {
                                    if (spListItemFR["DateOfIncident"] != null && !String.IsNullOrEmpty(Convert.ToString(spListItemFR["DateOfIncident"])))
                                    {
                                        DateTime date;
                                        string dateStr = Convert.ToString(spListItemFR["DateOfIncident"]);

                                        bool bValid = DateTime.TryParse(dateStr, new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);

                                        if (bValid)
                                        {
                                            this.incidentDate_dtc.SelectedDate = date;
                                            this.incidentDateOfOccurence.SelectedDate = date;
                                        }
                                        else
                                        {
                                            var dateTemp = Convert.ToDateTime(dateStr);

                                            this.incidentDate_dtc.SelectedDate = dateTemp;
                                            this.incidentDateOfOccurence.SelectedDate = dateTemp;
                                        }
                                    }

                                    if (spListItemFR["TargetDate"] != null && !String.IsNullOrEmpty(Convert.ToString(spListItemFR["TargetDate"])))
                                    {
                                        DateTime date;
                                        string dateStr = Convert.ToString(spListItemFR["TargetDate"]);

                                        bool bValid = DateTime.TryParse(dateStr, new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);

                                        if (bValid)
                                        {
                                            this.reportDueOnDate.SelectedDate = date;

                                            //Validation 
                                            this.FRTargetDate_dtc.SelectedDate = date;
                                        }
                                        else
                                        {
                                            var dateTemp = Convert.ToDateTime(dateStr);

                                            this.reportDueOnDate.SelectedDate = dateTemp;

                                            //Validation 
                                            this.FRTargetDate_dtc.SelectedDate = dateTemp;
                                        }
                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemFR["TimeOfIncident"])))
                                    {
                                        DateTime timeOfIncident = Convert.ToDateTime(spListItemFR["TimeOfIncident"]);
                                        this.incidentTime_dtc.SelectedDate = timeOfIncident;
                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemFR["Unit_x002f_Section"])))
                                    {
                                        if (this.incidentUnitArea_ddl != null && this.incidentUnitArea_ddl.Items != null)
                                        {
                                            this.incidentUnitArea_ddl.SelectedValue = Convert.ToString(spListItemFR["Unit_x002f_Section"]);
                                        }
                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemFR["IncidentScore"])))
                                    {
                                        this.incidentScore_tf.Value = Convert.ToString(spListItemFR["IncidentScore"]);
                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemFR["DescriptionOfIncident"])))
                                    {
                                        this.incidentDescription_ta.Value = Convert.ToString(spListItemFR["DescriptionOfIncident"]);
                                    }

                                    SPUser approvalAuthority = null;

                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemFR["ApprovingAuthority"])))
                                    {
                                        string approvedBy = Convert.ToString(spListItemFR["ApprovingAuthority"]);

                                        this.hdnApprovalAuthority.Value = approvedBy;

                                        if (!String.IsNullOrEmpty(approvedBy))
                                        {
                                            approvalAuthority = Utility.GetUser(oSPWeb, approvedBy);

                                            if (approvalAuthority != null)
                                            {
                                                this.approvedBy_tf.Value = approvalAuthority.Name;
                                            }
                                        }
                                    }


                                    if (FRID_ == null && isSubmitted == true && isApproved == false) //Submitted
                                    {
                                        string assignee = null;

                                        if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRB["Assignee"])))
                                        {
                                            assignee = Convert.ToString(spListItemIRB["Assignee"]);

                                            if (!Utility.CompareUsername(assignee, currentUser.LoginName))
                                            {
                                                DisableControls(true);
                                                this.hdnIsChangesAllowed.Value = "0";
                                                if (!CheckPermission())
                                                {
                                                    string accessDeniedUrl = Utility.GetRedirectUrl("Access_Denied");

                                                    if (!String.IsNullOrEmpty(accessDeniedUrl))
                                                    {
                                                        DisableControls(true);
                                                        Page.Response.Redirect(accessDeniedUrl, false);
                                                    }
                                                }
                                                else
                                                {
                                                    UpdateHSEMembersControl(isApproved, isSubmitted);
                                                }
                                            }
                                            else
                                            {
                                                DisableControls(false);
                                                this.btnReject.Visible = true;
                                                this.btnApprove.Visible = true;
                                            }
                                        }
                                    }
                                    else if (FRID_ == null && isApproved == true && isSubmitted == false)
                                    {
                                        DisableControls(true);
                                        this.hdnIsChangesAllowed.Value = "0";
                                        if (!CheckPermissionHSE_Approvers())
                                        {
                                            if (!CheckPermission())
                                            {
                                                string accessDeniedUrl = Utility.GetRedirectUrl("Access_Denied");

                                                if (!String.IsNullOrEmpty(accessDeniedUrl))
                                                {
                                                    DisableControls(true);
                                                    Page.Response.Redirect(accessDeniedUrl, false);
                                                }
                                            }
                                            else
                                            {
                                                UpdateHSEMembersControl(isApproved, isSubmitted);
                                            }
                                        }
                                        else
                                        {
                                            UpdateHSEMembersControl(isApproved, isSubmitted);
                                            this.btnForward.Visible = true;
                                            this.UM_HSE_Comments_ta.Disabled = false;
                                            this.rvf_reportViewed_ta.Disabled = false;
                                        }
                                    }
                                    else if (FRID_ == null && isApproved == true && isSubmitted == true)
                                    {
                                        DisableControls(true);
                                        this.hdnIsChangesAllowed.Value = "0";
                                        if (!CheckPermission())
                                        {
                                            string accessDeniedUrl = Utility.GetRedirectUrl("Access_Denied");

                                            if (!String.IsNullOrEmpty(accessDeniedUrl))
                                            {
                                                DisableControls(true);
                                                Page.Response.Redirect(accessDeniedUrl, false);
                                            }
                                        }
                                        else
                                        {
                                            UpdateHSEMembersControl(isApproved, isSubmitted);
                                            this.btnLastSave.Visible = true;
                                        }
                                    }
                                    else if (FRID_ != null && isApproved == true)
                                    {
                                        DisableControls(true);
                                        if (!CheckPermission())
                                        {
                                            string accessDeniedUrl = Utility.GetRedirectUrl("Access_Denied");

                                            if (!String.IsNullOrEmpty(accessDeniedUrl))
                                            {
                                                DisableControls(true);
                                                this.hdnIsChangesAllowed.Value = "0";
                                                Page.Response.Redirect(accessDeniedUrl, false);
                                            }
                                        }
                                    }

                                    if (isSubmitted == true && isApproved == false)
                                    {
                                        this.btnSave.Visible = false;
                                        this.btnSaveAsDraft.Visible = false;
                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemFR["ActionRequired"])))
                                    {
                                        string actionRequired = Convert.ToString(spListItemFR["ActionRequired"]);

                                        if (!actionRequired.Contains("IR-3"))
                                        {
                                            DisableControls(true);
                                            return false;
                                        }
                                    }


                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemFR["IR1ID"])))
                                    {
                                        this.hdnIR1ID.Value = Convert.ToString(spListItemFR["IR1ID"]);

                                        if (!String.IsNullOrEmpty(this.hdnIR1ID.Value))
                                        {
                                            int IRDIID = Convert.ToInt32(this.hdnIR1ID.Value);

                                            string spListNameIR1 = "IR-1";

                                            SPList spListIR1 = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListNameIR1));

                                            if (spListIR1 != null)
                                            {
                                                SPListItem spListItemIRDI = spListIR1.GetItemById(IRDIID);

                                                if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRDI["TitleOfIncident"])))
                                                {
                                                    this.incidentTitle_tf.Value = Convert.ToString(spListItemIRDI["TitleOfIncident"]);
                                                }

                                                if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRDI["SubmittedBy"])))
                                                {
                                                    string submittedBy = Convert.ToString(spListItemIRDI["SubmittedBy"]);

                                                    if (!String.IsNullOrEmpty(submittedBy))
                                                    {
                                                        var submittedByUser = Utility.GetUser(oSPWeb, submittedBy);

                                                        if (submittedByUser != null)
                                                        {
                                                            this.investigatedBy_tf.Value = submittedByUser.Name;
                                                        }
                                                    }
                                                }

                                                if (spListItemIRDI["DateOFSubmission"] != null && !String.IsNullOrEmpty(Convert.ToString(spListItemIRDI["DateOFSubmission"])))
                                                {
                                                    DateTime date;
                                                    bool bValid = DateTime.TryParse(Convert.ToString(spListItemIRDI["DateOFSubmission"]), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);

                                                    if (!bValid)
                                                    {
                                                        date = Convert.ToDateTime(spListItemIRDI["DateOFSubmission"]);
                                                    }

                                                    this.investigationDate_dtc.SelectedDate = date;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (isApproved)
                        {
                            approvalDate_div.Visible = true;
                        }

                        //Update Values
                        if (spListItemIRB != null)
                        {
                            if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRB["IncidentDescription"])))
                            {
                                this.incidentDescription_ta.Value = Convert.ToString(spListItemIRB["IncidentDescription"]);
                            }
                            //if (!String.IsNullOrEmpty(Convert.ToString(spListItemIRB["IncidentActionTaken"])))
                            //{
                            //    this.incidentActionsTaken_ta.Value = Convert.ToString(spListItemIRB["IncidentActionTaken"]);
                            //}
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRB->InitializeIRDIDetailedIncidenceControls)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("Msg_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator.";
                }

                message_div.InnerHtml = message;
                DisableControls(true);
            }
            return false;
        }
        private List<IRRecommendation_OnJob> GetFormattedRecommendationsByIRDI_Id(SPWeb oSPWeb, int IRDI_Id)
        {
            try
            {
                string spListName = "IIRRecommendationOnJob";
                // Fetch the List
                SPList spListIIRRecommedation_OnJob = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListName));

                List<IRRecommendation_OnJob> lstIIRRecommedation_OnJob = new List<IRRecommendation_OnJob>();

                if (spListIIRRecommedation_OnJob != null)
                {
                    SPQuery query = new SPQuery();
                    SPListItemCollection spListItems;
                    // Include only the fields you will use.
                    StringBuilder vf = new StringBuilder();
                    vf.Append("<FieldRef Name='ID'/>")
                        .Append("<FieldRef Name='RecommendationNo'/>")
                        .Append("<FieldRef Name='TargetDate'/>")
                        .Append("<FieldRef Name='IIRDescription'/>")
                        .Append("<FieldRef Name='TypeOfVoilation'/>")
                        .Append("<FieldRef Name='ResponsiblePerson'/>")
                        .Append("<FieldRef Name='AssigneeEmail'/>")
                        .Append("<FieldRef Name='Assignee'/>")
                        .Append("<FieldRef Name='ResponsibleSection'/>")
                        .Append("<FieldRef Name='ResponsibleDepartment'/>")
                        .Append("<FieldRef Name='ConcurrenceOfRP'/>")
                        .Append("<FieldRef Name='Status'/>");

                    query.ViewFields = vf.ToString();
                    query.ViewFieldsOnly = true;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<Where>")
                         .Append("  <Eq>")
                         .Append("    <FieldRef Name='IRID' />")
                         .Append("    <Value Type='Text'>" + Convert.ToString(IRDI_Id) + "</Value>")
                         .Append("  </Eq>")
                         .Append("</Where>");

                    query.Query = sb.ToString();
                    spListItems = spListIIRRecommedation_OnJob.GetItems(query);

                    for (int i = 0; i < spListItems.Count; i++)
                    {
                        SPListItem listItem = spListItems[i];
                        IRRecommendation_OnJob recommendation = new IRRecommendation_OnJob();
                        recommendation.RecommendationId = Convert.ToInt32(listItem["ID"]);
                        recommendation.RecommendationNo = Convert.ToString(listItem["RecommendationNo"]);

                        string targetDateStr = Convert.ToString(listItem["TargetDate"]);

                        if (!String.IsNullOrEmpty(targetDateStr))
                        {
                            DateTime date;
                            bool bValid = DateTime.TryParse(targetDateStr, new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);

                            if (bValid)
                            {
                                recommendation.TargetDate = date.ToShortDateString();
                            }
                            else
                            {
                                recommendation.TargetDate = Convert.ToDateTime(targetDateStr).ToShortDateString();
                            }
                        }

                        recommendation.Description = Convert.ToString(listItem["IIRDescription"]);
                        recommendation.RPUsername = Convert.ToString(listItem["ResponsiblePerson"]);
                        recommendation.RPEmail = Convert.ToString(listItem["AssigneeEmail"]);
                        recommendation.AssigneeUsername = Convert.ToString(listItem["Assignee"]);
                        recommendation.AssigneeEmail = Convert.ToString(listItem["AssigneeEmail"]);
                        recommendation.ConcurrenceOfRP = Convert.ToBoolean(listItem["ConcurrenceOfRP"]);
                        recommendation.Status = Convert.ToString(listItem["Status"]);

                        if (listItem["ResponsibleSection"] != null)
                        {
                            recommendation.SectionId = Convert.ToInt32(listItem["ResponsibleSection"]);

                            //Section
                            spListName = "Section";
                            // Fetch the List
                            SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListName));

                            if (spList != null && recommendation.SectionId > 0)
                            {
                                SPListItem spListItem = spList.GetItemById(recommendation.SectionId);

                                if (spListItem != null)
                                {
                                    recommendation.SectionName = Convert.ToString(spListItem["Title"]);
                                }
                            }
                        }

                        if (listItem["ResponsibleDepartment"] != null)
                        {
                            recommendation.DepartmentId = Convert.ToInt32(listItem["ResponsibleDepartment"]);

                            //Department
                            spListName = "Department";
                            // Fetch the List
                            SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListName));

                            if (spList != null && recommendation.DepartmentId > 0)
                            {
                                SPListItem spListItem = spList.GetItemById(recommendation.DepartmentId);

                                if (spListItem != null)
                                {
                                    recommendation.DepartmentName = Convert.ToString(spListItem["Title"]);
                                }
                            }
                        }
                        lstIIRRecommedation_OnJob.Add(recommendation);
                    }
                }

                return lstIIRRecommedation_OnJob;
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRD1->GetFormattedRecommendationsByIRDI_Id)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("Msg_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator.";
                }

                message_div.InnerHtml = message;
                DisableControls(true);
            }
            return null;
        }
        private List<IRRecommendation_OnJob> GetFormattedRecommendations(string recommendatons, String[] pattern1, String[] pattern2)
        {
            try
            {
                List<IRRecommendation_OnJob> lstIRRRecommendation_OnJob = new List<IRRecommendation_OnJob>();

                var lstRecommendation = recommendatons.Split(pattern1, StringSplitOptions.None);

                foreach (var item in lstRecommendation)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        var recommendationStr = item.Split(pattern2, StringSplitOptions.None);
                        if (recommendationStr.Length > 0)
                        {
                            IRRecommendation_OnJob recommendation = new IRRecommendation_OnJob();

                            recommendation.RecommendationId = String.IsNullOrEmpty(recommendationStr[0]) ? 0 : Int32.Parse(recommendationStr[0]);
                            recommendation.Description = recommendationStr[1];
                            recommendation.RPUsername = recommendationStr[2];
                            recommendation.RPEmail = recommendationStr[3];
                            recommendation.AssigneeUsername = recommendationStr[2];
                            recommendation.AssigneeEmail = recommendationStr[3];
                            recommendation.SectionId = String.IsNullOrEmpty(recommendationStr[4]) ? 0 : Int32.Parse(recommendationStr[4]);
                            recommendation.SectionName = recommendationStr[5];
                            recommendation.DepartmentId = String.IsNullOrEmpty(recommendationStr[6]) ? 0 : Int32.Parse(recommendationStr[6]);
                            recommendation.DepartmentName = recommendationStr[7];
                            recommendation.TargetDate = recommendationStr[8];
                            recommendation.ConcurrenceOfRP = recommendationStr[9].Equals("yes", StringComparison.OrdinalIgnoreCase) ? true : false;
                            recommendation.Status = recommendationStr[10];
                            recommendation.RecommendationNo = recommendationStr[11];
                            recommendation.IsSavedAsDraft = recommendationStr[12].Equals("true", StringComparison.OrdinalIgnoreCase) ? true : false;
                            recommendation.ValidationStatus = 0;

                            lstIRRRecommendation_OnJob.Add(recommendation);
                        }
                    }
                }
                return lstIRRRecommendation_OnJob;
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRB->GetFormattedRecommendations)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("Msg_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator.";
                }

                message_div.InnerHtml = message;
                DisableControls(true);
            }
            return null;
        }
        private bool FillKeyFindingsGrid(List<string> lstKeyFindings)
        {
            try
            {
                if (lstKeyFindings != null)
                {
                    //Add Key Findings in grid
                    foreach (var item in lstKeyFindings)
                    {
                        HtmlTableRow tRow = new HtmlTableRow();

                        tRow.Attributes.Add("class", "keyFindingsItem");

                        tRow.Cells.Add(new HtmlTableCell() { InnerHtml = Convert.ToString(this.keyFindings_table.Rows.Count) });

                        HtmlTableCell description = new HtmlTableCell();

                        string actions = "<span class='btn btn-default editKeyFindings'><i class='glyphicon glyphicon-pencil'></i></span><span class='btn btn-danger removeKeyFindings'><i class='glyphicon glyphicon-remove'></i></span>";

                        description.InnerHtml = "<span class='keyFindingsDescription'>" + Convert.ToString(item) + "</span>";

                        tRow.Cells.Add(description);

                        tRow.Cells.Add(new HtmlTableCell() { InnerHtml = actions });

                        this.keyFindings_table.Rows.Add(tRow);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRB->FillKeyFindingsGrid)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return false;
        }
        private bool FillPeopleInterviewedGrid(List<string> lstPeopleViewed)
        {
            try
            {
                if (lstPeopleViewed != null)
                {
                    //Add People Viewed in grid
                    foreach (var item in lstPeopleViewed)
                    {
                        HtmlTableRow tRow = new HtmlTableRow();

                        tRow.Attributes.Add("class", "peopleInterviewedItem");

                        tRow.Cells.Add(new HtmlTableCell() { InnerHtml = Convert.ToString(this.peopleInterviewed_table.Rows.Count) });

                        HtmlTableCell description = new HtmlTableCell();

                        string actions = "<span class='btn btn-default editPeopleInterviewed'><i class='glyphicon glyphicon-pencil'></i></span><span class='btn btn-danger removePeopleInterviewed'><i class='glyphicon glyphicon-remove'></i></span>";

                        description.InnerHtml = "<span class='peopleInterviewedDescription'>" + Convert.ToString(item) + "</span>";

                        tRow.Cells.Add(description);

                        tRow.Cells.Add(new HtmlTableCell() { InnerHtml = actions });

                        this.peopleInterviewed_table.Rows.Add(tRow);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRB->FillPeopleViewedGrid)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return false;
        }
        private bool FillRootCausesGrid(List<string> lstRootCauses)
        {
            try
            {
                if (lstRootCauses != null)
                {
                    //Add Root Causes in grid
                    foreach (var item in lstRootCauses)
                    {
                        HtmlTableRow tRow = new HtmlTableRow();

                        tRow.Attributes.Add("class", "rootCausesItem");

                        tRow.Cells.Add(new HtmlTableCell() { InnerHtml = Convert.ToString(this.rootCauses_table.Rows.Count) });

                        HtmlTableCell description = new HtmlTableCell();

                        string actions = "<span class='btn btn-default editRootCauses'><i class='glyphicon glyphicon-pencil'></i></span><span class='btn btn-danger removeRootCauses'><i class='glyphicon glyphicon-remove'></i></span>";

                        description.InnerHtml = "<span class='rootCausesDescription'>" + Convert.ToString(item) + "</span>";

                        tRow.Cells.Add(description);

                        tRow.Cells.Add(new HtmlTableCell() { InnerHtml = actions });

                        this.rootCauses_table.Rows.Add(tRow);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRB->FillRootCausesGrid)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return false;
        }
        private bool FillRecommendationGrid(List<IRRecommendation_OnJob> lstRecommendation)
        {
            try
            {
                if (lstRecommendation != null)
                {
                    //Add recommendations in grid
                    foreach (var recommendation in lstRecommendation)
                    {
                        HtmlTableRow tRow = new HtmlTableRow();

                        tRow.Attributes.Add("class", "recommendationItem");

                        tRow.Cells.Add(new HtmlTableCell() { InnerHtml = Convert.ToString(this.recommendationDetails_table.Rows.Count) });

                        HtmlTableCell recommendationId = new HtmlTableCell();
                        HtmlTableCell recommendationNo = new HtmlTableCell();
                        HtmlTableCell description = new HtmlTableCell();
                        HtmlTableCell responsiblePersonUsername = new HtmlTableCell();
                        HtmlTableCell responsiblePersonEmail = new HtmlTableCell();
                        HtmlTableCell responsibleSection = new HtmlTableCell();
                        HtmlTableCell responsibleSectionId = new HtmlTableCell();
                        HtmlTableCell responsibleDepartment = new HtmlTableCell();
                        HtmlTableCell responsibleDepartmentId = new HtmlTableCell();
                        HtmlTableCell targetDate = new HtmlTableCell();
                        HtmlTableCell concurrenceOfRP = new HtmlTableCell();
                        HtmlTableCell status = new HtmlTableCell();

                        string actions = "<span class='btn btn-default editRecommendation' ><i class='glyphicon glyphicon-pencil'></i></span><span class='btn btn-danger removeRecommendation'><i class='glyphicon glyphicon-remove'></i></span>";

                        recommendationId.InnerHtml = "<span class='recommendationId'>" + Convert.ToString(recommendation.RecommendationId) + "</span>";
                        recommendationId.Attributes.Add("style", "display:none");

                        recommendationNo.InnerHtml = "<span class='recommendationNo'>" + Convert.ToString(recommendation.RecommendationNo) + "</span>";
                        recommendationNo.Attributes.Add("style", "display:none");

                        description.Attributes.Add("class", "td-description");
                        description.InnerHtml = "<span class='description'>" + Convert.ToString(recommendation.Description) + "</span>";
                        responsiblePersonUsername.InnerHtml = "<span class='username'>" + Convert.ToString(recommendation.RPUsername) + "</span>";

                        responsiblePersonEmail.InnerHtml = "<span class='email'>" + Convert.ToString(recommendation.RPEmail) + "</span>";
                        responsiblePersonEmail.Attributes.Add("style", "display:none");

                        responsibleSection.InnerHtml = "<span class='sectionName'>" + Convert.ToString(recommendation.SectionName) + "</span>";

                        responsibleSectionId.InnerHtml = "<span class='sectionId'>" + Convert.ToString(recommendation.SectionId) + "</span>";
                        responsibleSectionId.Attributes.Add("style", "display:none");

                        responsibleDepartment.InnerHtml = "<span class='departmentName'>" + Convert.ToString(recommendation.DepartmentName) + "</span>";

                        responsibleDepartmentId.InnerHtml = "<span class='departmentId'>" + Convert.ToString(recommendation.DepartmentId) + "</span>";
                        responsibleDepartmentId.Attributes.Add("style", "display:none");

                        targetDate.InnerHtml = "<span class='targetDate'>" + Convert.ToString(recommendation.TargetDate) + "</span>";

                        concurrenceOfRP.InnerHtml = "<span class='concurrenceOfRP'>" + ((recommendation.ConcurrenceOfRP == true) ? "Yes" : "No") + "</span>";
                        status.InnerHtml = "<span class='status'>" + Convert.ToString(recommendation.Status) + "</span>";

                        tRow.Cells.Add(recommendationId);
                        tRow.Cells.Add(description);
                        tRow.Cells.Add(responsiblePersonUsername);
                        tRow.Cells.Add(responsibleSection);
                        tRow.Cells.Add(responsibleSectionId);
                        tRow.Cells.Add(responsibleDepartment);
                        tRow.Cells.Add(responsibleDepartmentId);
                        tRow.Cells.Add(targetDate);
                        tRow.Cells.Add(concurrenceOfRP);
                        tRow.Cells.Add(status);

                        tRow.Cells.Add(new HtmlTableCell() { InnerHtml = actions });

                        switch (recommendation.ValidationStatus)
                        {
                            case 0:
                                {
                                    break;
                                }
                            case 1:
                                {
                                    tRow.Attributes.Add("style", "background-color: rgba(238, 118, 173, 0.88)");
                                    message_div.InnerHtml = "Responsible Persons in Highlighted Recommendations needs more permission. Please Contact the Administrator!";
                                    break;
                                }
                            case 2:
                                {
                                    tRow.Attributes.Add("style", "background-color: rgba(238, 118, 173, 0.88)");
                                    message_div.InnerHtml = "Target Date in Highlighted Recommendations are not valid. Please Contact the Administrator!";
                                    break;
                                }
                            default:
                                {
                                    break;
                                }

                        }

                        this.recommendationDetails_table.Rows.Add(tRow);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRBForm->FillRecommendationGrid)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return false;
        }
        private bool IsValid_IRDI_Data(SPWeb oSPWeb, List<IRRecommendation_OnJob> recommendationList)
        {
            bool isValid = true;

            try
            {
                foreach (var recommendation in recommendationList)
                {
                    SPUser responsiblePerson = null;

                    if (!String.IsNullOrEmpty(recommendation.RPUsername))
                    {
                        responsiblePerson = Utility.GetUser(null, recommendation.RPUsername);

                        if (responsiblePerson == null && !String.IsNullOrEmpty(recommendation.RPEmail))
                        {
                            responsiblePerson = Utility.GetUser(oSPWeb, null, recommendation.RPEmail);
                        }
                    }

                    if (responsiblePerson == null)
                    {
                        recommendation.ValidationStatus = 1;
                        isValid = false;
                    }
                    else
                    {
                        recommendation.ResponsiblePerson = responsiblePerson;
                    }


                    if (!String.IsNullOrEmpty(recommendation.TargetDate))
                    {
                        DateTime date;
                        bool bValid = DateTime.TryParse(recommendation.TargetDate, new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);

                        if (!bValid)
                        {
                            bValid = DateTime.TryParse(recommendation.TargetDate, new CultureInfo("en-US"), DateTimeStyles.AssumeLocal, out date);

                            if (!bValid)
                            {
                                recommendation.ValidationStatus = 2;
                                isValid = false;
                            }
                        }
                    }
                    else
                    {
                        recommendation.ValidationStatus = 2;
                        isValid = false;
                    }
                }
                return isValid;
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IIRB->IsValid_IRDI_Data)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return isValid;
        }
        private bool SaveIRDetailedIncidenceDetails(List<IRRecommendation_OnJob> recommendations, bool isSavedAsDraft, bool isSubmitCase, bool isApproveCase, bool isForwardCase, bool isLastSave, String[] pattern1, String[] pattern2, int? IRB_ID = null)
        {
            bool isSaved = false;
            try
            {
                List<Message> lstMessage = null;

                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oSPWeb = oSPsite.OpenWeb())
                    {
                        SPUser currentUser = oSPWeb.CurrentUser;

                        string keyFindings = this.hdnKeyFindingsList.Value;
                        string peopleInterviewed = this.hdnPeopleInterviewedList.Value;
                        string rootCauses = this.hdnRootCausesList.Value;

                        string p1 = "~|~";
                        if (pattern1.Length > 0)
                        {
                            p1 = pattern1[0];
                        }

                        //Key Findings List
                        List<string> lstKeyFindings = Utility.GetFormattedDataList(this.hdnKeyFindingsList.Value, p1, true);

                        //People Viewed List
                        List<string> lstPeopleViewed = Utility.GetFormattedDataList(this.hdnPeopleInterviewedList.Value, p1, true);

                        //Root Causes List
                        List<string> lstRootCauses = Utility.GetFormattedDataList(this.hdnRootCausesList.Value, p1, true);


                        //Validate IR Details
                        //Success
                        if (IsValid_IRDI_Data(oSPWeb, recommendations))
                        {
                            string listName = "IRB";

                            // Fetch the List
                            SPList list = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));

                            if (list != null)
                            {
                                SPListItem spListItem = null;

                                //Add a new item 
                                if (IRB_ID == null)
                                {
                                    spListItem = list.Items.Add();
                                }
                                //Update existing item
                                else
                                {
                                    spListItem = list.Items.GetItemById((int)IRB_ID);
                                }

                                if (spListItem != null)
                                {
                                    if (!isForwardCase)
                                    {
                                        spListItem["KeyFindings"] = keyFindings;
                                        spListItem["PeopleInterviewed"] = peopleInterviewed;
                                        spListItem["RootCauses"] = rootCauses;


                                        if (!String.IsNullOrEmpty(this.incidentDescription_ta.Value))
                                        {
                                            spListItem["IncidentDescription"] = this.incidentDescription_ta.Value;
                                        }

                                        if (!String.IsNullOrEmpty(this.incidentTitle_tf.Value))
                                        {
                                            spListItem["IncidentTitle"] = this.incidentTitle_tf.Value;
                                        }

                                        if (!String.IsNullOrEmpty(this.conclusion_ta.Value))
                                        {
                                            spListItem["Conclusion"] = this.conclusion_ta.Value;
                                        }
                                        //if (!String.IsNullOrEmpty(this.hdnCulturalBasedElements.Value))
                                        //{
                                        //    spListItem["CulturalBasedElements"] = this.hdnCulturalBasedElements.Value;
                                        //}
                                        //if (!String.IsNullOrEmpty(this.hdnRiskBasedElements.Value))
                                        //{
                                        //    spListItem["RiskBasedElements"] = this.hdnRiskBasedElements.Value;
                                        //}
                                        //if (!String.IsNullOrEmpty(this.hdnBasicActivityInProgress.Value))
                                        //{
                                        //    spListItem["BasicActivityInProgress"] = this.hdnBasicActivityInProgress.Value;
                                        //}
                                        //if (!String.IsNullOrEmpty(this.hdnMiscellaneousIncidents.Value))
                                        //{
                                        //    spListItem["MiscellaneousIncidents"] = this.hdnMiscellaneousIncidents.Value;
                                        //}
                                        //if (!String.IsNullOrEmpty(this.hdnProcessSafetyIncidents.Value))
                                        //{
                                        //    spListItem["ProcessSafetyIncidents"] = this.hdnProcessSafetyIncidents.Value;
                                        //}
                                        //if (!String.IsNullOrEmpty(this.hdnActualOrPotentialInquiry.Value))
                                        //{
                                        //    spListItem["ActualOrPotentialInquiry"] = this.hdnActualOrPotentialInquiry.Value;
                                        //}


                                        if (!String.IsNullOrEmpty(this.hdnFRID.Value))
                                        {
                                            spListItem["FlashReportID"] = this.hdnFRID.Value;
                                        }

                                        if (isApproveCase)
                                        {
                                            string approvalDate1 = this.approvalDate_dtc.SelectedDate != null ? this.approvalDate_dtc.SelectedDate.ToShortDateString() : null;
                                            DateTime date;
                                            bool bValid = DateTime.TryParse(approvalDate1, new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);

                                            if (bValid)
                                            {
                                                spListItem["ApprovalDate1"] = date;
                                            }
                                            else
                                            {
                                                spListItem["ApprovalDate1"] = Convert.ToDateTime(approvalDate1);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        if (currentUser != null)
                                        {
                                            string username = Utility.GetUsername(currentUser.LoginName, true);
                                            if (!String.IsNullOrEmpty(username))
                                            {
                                                spListItem["Assignee"] = username;
                                            }

                                            if (!String.IsNullOrEmpty(currentUser.Email))
                                            {
                                                spListItem["AssigneeEmail"] = currentUser.Email; ;
                                                this.hdnSentFrom.Value = currentUser.Email;
                                            }
                                        }

                                        if (!String.IsNullOrEmpty(this.rvf_reportViewed_ta.Value))
                                        {
                                            spListItem["HSEApprover"] = this.rvf_reportViewed_ta.Value;
                                        }

                                        if (!String.IsNullOrEmpty(this.UM_HSE_Comments_ta.Value))
                                        {
                                            spListItem["HSEComments"] = this.UM_HSE_Comments_ta.Value;
                                        }
                                    }

                                    //new 
                                    bool isAlreadySaved = false;

                                    if (Page.Request.QueryString["IRB_Id"] == null && Page.Request.QueryString["FRID"] != null && spListItem["IsSavedAsDraft"] != null && spListItem["IsSubmitted"] != null && isSavedAsDraft)
                                    {
                                        bool isSavedAsDraft_t = Convert.ToBoolean(spListItem["IsSavedAsDraft"]);
                                        bool isSubmitted_t = Convert.ToBoolean(spListItem["IsSubmitted"]);


                                        if (isSavedAsDraft_t == false && isSubmitted_t == true)
                                        {
                                            isAlreadySaved = true;
                                        }

                                    }

                                    if (isAlreadySaved == false && Page.Request.QueryString["IRB_Id"] != null && Page.Request.QueryString["FRID"] == null && isSavedAsDraft == false && isSubmitCase == true && isApproveCase == true && isForwardCase == true && isLastSave == false && spListItem["IsApproved"] != null && spListItem["IsSubmitted"] != null)
                                    {
                                        bool isApproved_t = Convert.ToBoolean(spListItem["IsApproved"]);
                                        bool isSubmitted_t = Convert.ToBoolean(spListItem["IsSubmitted"]);

                                        if (isApproved_t == true && isSubmitted_t == true)
                                        {
                                            isAlreadySaved = true;
                                        }
                                    }
                                    //new 

                                    spListItem["IsSavedAsDraft"] = isSavedAsDraft;
                                    spListItem["IsSubmitted"] = isSubmitCase;
                                    spListItem["IsApproved"] = isApproveCase;
                                    spListItem["IsClosed"] = isLastSave;

                                    if (!String.IsNullOrEmpty(this.IRRCQualityScore_ta.Value))
                                    {
                                        spListItem["IRRCQualityScore"] = this.IRRCQualityScore_ta.Value;
                                    }

                                    if (!String.IsNullOrEmpty(this.IRRCQualityAccessedBy_ta.Value))
                                    {
                                        spListItem["QualityAssessedBy"] = this.IRRCQualityAccessedBy_ta.Value;
                                    }

                                    //new 
                                    if (isAlreadySaved == false)
                                    {
                                        //Update added record
                                        oSPWeb.AllowUnsafeUpdates = true;
                                        spListItem.Update();
                                        oSPWeb.AllowUnsafeUpdates = false;
                                    }
                                    //new 

                                    if (IRB_ID == null)
                                    {
                                        IRB_ID = Convert.ToInt32(spListItem["ID"]);
                                    }

                                    this.hdnIRB_Id.Value = Convert.ToString(IRB_ID);

                                    isSaved = true;

                                    List<int> recommendationIds = null;

                                    if (!String.IsNullOrEmpty(this.hdnIdList.Value))
                                    {
                                        recommendationIds = GetFormattedIds(this.hdnIdList.Value, pattern1, pattern2);
                                    }

                                    string sentFrom = null;

                                    if (!String.IsNullOrEmpty(this.hdnSentFrom.Value))
                                    {
                                        sentFrom = this.hdnSentFrom.Value;
                                    }


                                    if (isSaved && isAlreadySaved == false)
                                    {
                                        if (recommendationIds != null)
                                        {
                                            //In case of approved, no need to update recommendations(isApproved)
                                            lstMessage = SaveRecommendations(oSPWeb, recommendations, (int)IRB_ID, sentFrom, recommendationIds);
                                        }
                                        else
                                        {
                                            lstMessage = SaveRecommendations(oSPWeb, recommendations, (int)IRB_ID, sentFrom);
                                        }

                                        if (lstMessage == null)
                                        {
                                            isSaved = false;
                                        }
                                    }

                                    //Roll Back in case of error
                                    if (isSaved == false)
                                    {
                                        //Write some code here
                                    }
                                    else
                                    {
                                        if (isAlreadySaved == false && Page.Request.QueryString["IRB_Id"] == null && Page.Request.QueryString["FRID"] != null && isSavedAsDraft == true && isSubmitCase == false && isApproveCase == false && isForwardCase == false && isLastSave == false)
                                        {
                                            IRSavedAsDraft(oSPWeb, Convert.ToInt32(this.hdnIRB_Id.Value), Convert.ToInt32(this.hdnFRID.Value));
                                        }
                                        else if (Page.Request.QueryString["IRB_Id"] == null && Page.Request.QueryString["FRID"] != null && isSavedAsDraft == false && isSubmitCase == true)
                                        {
                                            IRSubmittedByTeamLead(oSPWeb, Convert.ToInt32(this.hdnIRB_Id.Value), Convert.ToInt32(this.hdnFRID.Value));
                                        }
                                        else if (Page.Request.QueryString["IRB_Id"] != null && Page.Request.QueryString["FRID"] == null && isSavedAsDraft == false && isSubmitCase == false && isApproveCase == true && isForwardCase == false && isLastSave == false)
                                        {
                                            IRApproved(oSPWeb, Convert.ToInt32(this.hdnIRB_Id.Value), Convert.ToInt32(this.hdnFRID.Value));
                                        }
                                        else if (Page.Request.QueryString["IRB_Id"] != null && Page.Request.QueryString["FRID"] == null && isSavedAsDraft == true && isSubmitCase == false && isApproveCase == false && isForwardCase == false && isLastSave == false)
                                        {
                                            IRRejected(oSPWeb, Convert.ToInt32(this.hdnIRB_Id.Value), Convert.ToInt32(this.hdnFRID.Value));
                                        }
                                        else if (isAlreadySaved == false && Page.Request.QueryString["IRB_Id"] != null && Page.Request.QueryString["FRID"] == null && isSavedAsDraft == false && isSubmitCase == true && isApproveCase == true && isForwardCase == true && isLastSave == false && !String.IsNullOrEmpty(this.hdnSentFrom.Value))
                                        {
                                            if (lstMessage != null && lstMessage.Count > 0)
                                            {
                                                isSaved = Email.SendEmail(lstMessage);

                                                IRForwarded(oSPWeb, Convert.ToInt32(this.hdnIRB_Id.Value), Convert.ToInt32(this.hdnFRID.Value));
                                            }
                                        }
                                        else if (Page.Request.QueryString["IRB_Id"] != null && Page.Request.QueryString["FRID"] == null && isSavedAsDraft == false && isSubmitCase == true && isApproveCase == true && isForwardCase == true && isLastSave == true)
                                        {
                                            IRLastSaved(oSPWeb, Convert.ToInt32(this.hdnIRB_Id.Value), Convert.ToInt32(this.hdnFRID.Value));
                                        }
                                    }
                                }
                            }
                        }
                        //Failure
                        if (!isSaved)
                        {
                            bool statusRecommendations = FillRecommendationGrid(recommendations);
                            bool statusKeyFindings = FillKeyFindingsGrid(lstKeyFindings);
                            bool statusPeopleViewed = FillPeopleInterviewedGrid(lstPeopleViewed);
                            bool statusRootCause = FillRootCausesGrid(lstRootCauses);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRB->SaveIRDetailedIncidenceDetails)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("Msg_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator.";
                }

                message_div.InnerHtml = message;
                DisableControls(true);
            }
            return isSaved;
        }
        public List<Message> SaveRecommendations(SPWeb oSPWeb, List<IRRecommendation_OnJob> recommendations, int IRB_ID, string sentFrom, List<int> recommendationIds = null)
        {
            try
            {
                List<Message> lstMessage = new List<Message>();

                if (oSPWeb != null)
                {
                    string listName = "IIRRecommendationOnJob";

                    // Fetch the List
                    SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));
                    int itemCount = spList.ItemCount + 1;


                    foreach (var item in recommendations)
                    {
                        Message message = new Message();

                        SPListItem itemToAdd = null;

                        if (item.RecommendationId > 0)
                        {
                            itemToAdd = spList.GetItemById(item.RecommendationId);
                            if (recommendationIds != null && recommendationIds.Count > 0)
                            {
                                recommendationIds.Remove(item.RecommendationId);
                            }
                        }
                        else
                        {
                            //Add a new item in the List
                            itemToAdd = spList.Items.Add();
                        }

                        if (itemToAdd != null)
                        {
                            SPUser responsiblePerson = null;

                            if (!String.IsNullOrEmpty(item.RPUsername))
                            {
                                responsiblePerson = Utility.GetUser(oSPWeb, item.RPUsername);

                                if (responsiblePerson == null && !String.IsNullOrEmpty(item.RPEmail))
                                {
                                    responsiblePerson = Utility.GetUser(oSPWeb, null, item.RPEmail);
                                    if (responsiblePerson != null)
                                    {
                                        item.RPUsername = Utility.GetUsername(responsiblePerson.LoginName, true);
                                    }
                                    else
                                    {
                                        return null;
                                    }
                                }
                            }
                            else
                            {
                                return null;
                            }

                            if (responsiblePerson == null)
                            {
                                string infoMessage = Utility.GetValueByKey("IRB_RP_Info_Incomplete");

                                if (String.IsNullOrEmpty(infoMessage))
                                {
                                    message_div.InnerHtml = "Information of Responsible Person is incomplete or needs more permission. Please Contact the Administrator!";
                                }

                                message_div.InnerHtml = infoMessage;
                                DisableControls(true);


                                return null;
                            }

                            itemToAdd["IRID"] = IRB_ID;


                            string tempRecommendationNo = "";

                            if (item.SectionId > 0)
                            {
                                //Section
                                listName = "Section";
                                // Fetch the List
                                SPList spSectionList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));

                                if (spList != null && item.SectionId > 0)
                                {
                                    SPListItem spSectionListItem = spSectionList.GetItemById(item.SectionId);

                                    if (spSectionListItem != null)
                                    {
                                        tempRecommendationNo += Convert.ToString(spSectionListItem["SectionCode"]);
                                    }
                                }
                            }

                            tempRecommendationNo += "-";

                            if (item.DepartmentId > 0)
                            {
                                //Department
                                listName = "Department";
                                // Fetch the List
                                SPList spDepartmentList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));

                                if (spList != null && item.DepartmentId > 0)
                                {
                                    SPListItem spDepartmentListItem = spDepartmentList.GetItemById(item.DepartmentId);

                                    if (spDepartmentListItem != null)
                                    {
                                        tempRecommendationNo += Convert.ToString(spDepartmentListItem["DepartmentCode"]);
                                    }
                                }
                            }

                            itemToAdd["ResponsiblePerson"] = item.RPUsername;
                            itemToAdd["Assignee"] = item.RPUsername;
                            if (responsiblePerson != null && !String.IsNullOrEmpty(responsiblePerson.Email))
                            {
                                itemToAdd["AssigneeEmail"] = responsiblePerson.Email;
                            }
                            else if (!String.IsNullOrEmpty(item.RPEmail) && !item.RPEmail.Equals("undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                itemToAdd["AssigneeEmail"] = item.RPEmail;
                            }
                            else
                            {
                                message_div.InnerHtml = "Responsible Person Email Address not available";
                                return null;
                            }

                            itemToAdd["IIRDescription"] = item.Description;
                            itemToAdd["ResponsibleSection"] = item.SectionId;
                            itemToAdd["ResponsibleDepartment"] = item.DepartmentId;


                            if (!String.IsNullOrEmpty(item.TargetDate))
                            {
                                DateTime date;
                                bool bValid = DateTime.TryParse(item.TargetDate, new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);

                                if (bValid)
                                {
                                    itemToAdd["TargetDate"] = date;
                                }
                                else
                                {
                                    itemToAdd["TargetDate"] = Convert.ToDateTime(item.TargetDate);
                                }
                            }

                            itemToAdd["ConcurrenceOfRP"] = item.ConcurrenceOfRP;
                            itemToAdd["Status"] = item.Status;
                            itemToAdd["IsSavedAsDraft"] = item.IsSavedAsDraft;

                            //Is From IR01DI
                            itemToAdd["IsFromIR01DI"] = false;
                            //End

                            oSPWeb.AllowUnsafeUpdates = true;
                            itemToAdd.Update();
                            oSPWeb.AllowUnsafeUpdates = false;


                            string itemID = Convert.ToString(itemToAdd.ID);

                            int length = itemID.Length;

                            string recommendationNo = "";

                            for (int i = 0; i < 6 - length; i++)
                            {
                                recommendationNo += "0";
                            }

                            recommendationNo += itemID + "-" + tempRecommendationNo;

                            itemToAdd["RecommendationNo"] = recommendationNo;


                            oSPWeb.AllowUnsafeUpdates = true;
                            itemToAdd.Update();
                            oSPWeb.AllowUnsafeUpdates = false;


                            if (!Convert.ToString(itemToAdd["Status"]).Equals("Completed", StringComparison.OrdinalIgnoreCase))
                            {
                                StringBuilder linkSB = new StringBuilder();

                                string recommendationLink = Utility.GetRedirectUrl("IRRecommendationFormLink");

                                linkSB.Append(recommendationLink)
                                    .Append("?IRB_ID=")
                                    .Append(itemToAdd.ID);

                                string body = Utility.GetValueByKey("IRB_ON_From_FB_To_RP_B");

                                body = body.Replace("~|~", linkSB.ToString());

                                if (String.IsNullOrEmpty(body))
                                {
                                    body = linkSB.ToString();
                                }

                                SPUser toUser = null;

                                if (!String.IsNullOrEmpty(item.RPUsername) && !item.RPUsername.Equals("undefined", StringComparison.OrdinalIgnoreCase))
                                {
                                    toUser = Utility.GetUser(oSPWeb, item.RPUsername);
                                }

                                message.From = sentFrom;

                                if (toUser != null)
                                {
                                    message.To = toUser.Email;
                                }
                                else if (!String.IsNullOrEmpty(item.RPEmail) && !item.RPEmail.Equals("undefined", StringComparison.OrdinalIgnoreCase))
                                {
                                    message.To = item.RPEmail;
                                }

                                message.Subject = "IR01DI_ON_From_FB_To_RP_S";
                                var subject = Utility.GetValueByKey(message.Subject);

                                if (!String.IsNullOrEmpty(subject))
                                {
                                    message.Subject = subject;
                                }
                                message.Body = body;

                                lstMessage.Add(message);
                            }
                        }
                    }
                    if (recommendationIds != null && recommendationIds.Count > 0)
                    {
                        foreach (var id in recommendationIds)
                        {
                            var spListItem = spList.GetItemById(id);

                            if (spListItem != null)
                            {
                                oSPWeb.AllowUnsafeUpdates = true;
                                spListItem.Delete();
                                oSPWeb.AllowUnsafeUpdates = false;
                            }
                        }
                    }
                    return lstMessage;
                }
            }

            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRB->SaveRecommendations)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("Msg_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator.";
                }

                message_div.InnerHtml = message;
                DisableControls(true);
            }
            return null;
        }
        private List<int> GetFormattedIds(string ids, String[] pattern1, String[] pattern2)
        {
            try
            {
                List<int> recommendationIds = new List<int>();

                var pairOfIdsStr = ids.Split(pattern1, StringSplitOptions.None);

                if (pairOfIdsStr.Length > 1)
                {
                    var recommendadtionIds = pairOfIdsStr[1].Split(pattern2, StringSplitOptions.None);

                    foreach (var item in recommendadtionIds)
                    {
                        if (!String.IsNullOrEmpty(item))
                        {
                            recommendationIds.Add(Int32.Parse(item));
                        }
                    }
                }
                return recommendationIds;
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRB->GetFormattedIds)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("Msg_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator.";
                }

                message_div.InnerHtml = message;
                DisableControls(true);
            }
            return null;
        }
        //Events
        protected void btnSaveAsDraft_Click(object sender, EventArgs e)
        {
            try
            {
                var pattern1 = new[] { "~|~" };
                var pattern2 = new[] { "*|*" };

                string recommendationListStr = this.hdnRecommendationList.Value;

                var recommendationList = this.GetFormattedRecommendations(recommendationListStr, pattern1, pattern2);

                bool isSaved = false;
                if (recommendationList != null)
                {

                    if (!String.IsNullOrEmpty(this.hdnIRB_Id.Value))
                    {
                        isSaved = SaveIRDetailedIncidenceDetails(recommendationList, true, false, false, false, false, pattern1, pattern2, Convert.ToInt32(this.hdnIRB_Id.Value));
                    }
                    else
                    {
                        isSaved = SaveIRDetailedIncidenceDetails(recommendationList, true, false, false, false, false, pattern1, pattern2);
                    }
                }

                if (isSaved)
                {
                    string redirectUrl = Utility.GetRedirectUrl("IRB_SaveAsDraft_Redirect");

                    if (String.IsNullOrEmpty(redirectUrl))
                    {
                        redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");
                    }

                    DisableControls(true);

                    if (!String.IsNullOrEmpty(redirectUrl))
                    {
                        Page.Response.Redirect(redirectUrl, false);
                    }

                }
                else
                {
                    if (String.IsNullOrEmpty(message_div.InnerHtml.Replace("\r", " ").Replace("\n", " ").Trim()))
                    {
                        message_div.InnerHtml = "Operation Save Failed. Kindly verify that you provide valid information.";
                        DisableControls(true);
                    }

                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRB->btnSaveAsDraft_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("Msg_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator.";
                }

                message_div.InnerHtml = message;
                DisableControls(true);
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var pattern1 = new[] { "~|~" };
                var pattern2 = new[] { "*|*" };

                string recommendationListStr = this.hdnRecommendationList.Value;

                var recommendationList = this.GetFormattedRecommendations(recommendationListStr, pattern1, pattern2);

                bool isSaved = false;
                if (recommendationList != null)
                {
                    if (!String.IsNullOrEmpty(this.hdnIRB_Id.Value))
                    {
                        isSaved = SaveIRDetailedIncidenceDetails(recommendationList, false, true, false, false, false, pattern1, pattern2, Convert.ToInt32(this.hdnIRB_Id.Value));
                    }
                    else
                    {
                        isSaved = SaveIRDetailedIncidenceDetails(recommendationList, false, true, false, false, false, pattern1, pattern2);
                    }
                }

                if (isSaved)
                {
                    string redirectUrl = Utility.GetRedirectUrl("IRB_Save_Redirect");

                    if (String.IsNullOrEmpty(redirectUrl))
                    {
                        redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");
                    }

                    DisableControls(true);

                    if (!String.IsNullOrEmpty(redirectUrl))
                    {
                        Page.Response.Redirect(redirectUrl, false);
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(message_div.InnerHtml.Replace("\r", " ").Replace("\n", " ").Trim()))
                    {
                        message_div.InnerHtml = "Operation Save Failed. Kindly verify that you provide valid information.";
                        DisableControls(true);
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRB->btnSave_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("Msg_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator.";
                }

                message_div.InnerHtml = message;
                DisableControls(true);
            }
        }
        protected void btnReject_Click(object sender, EventArgs e)
        {
            try
            {
                var pattern1 = new[] { "~|~" };
                var pattern2 = new[] { "*|*" };

                string recommendationListStr = this.hdnRecommendationList.Value;

                var recommendationList = this.GetFormattedRecommendations(recommendationListStr, pattern1, pattern2);

                bool isSaved = false;
                if (recommendationList != null)
                {
                    if (!String.IsNullOrEmpty(this.hdnIRB_Id.Value))
                    {
                        isSaved = SaveIRDetailedIncidenceDetails(recommendationList, true, false, false, false, false, pattern1, pattern2, Convert.ToInt32(this.hdnIRB_Id.Value));
                    }
                    else
                    {
                        isSaved = SaveIRDetailedIncidenceDetails(recommendationList, true, false, false, false, false, pattern1, pattern2);
                    }
                }

                if (isSaved)
                {
                    string redirectUrl = Utility.GetRedirectUrl("IRB_Reject_Redirect");

                    if (String.IsNullOrEmpty(redirectUrl))
                    {
                        redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");
                    }

                    DisableControls(true);

                    if (!String.IsNullOrEmpty(redirectUrl))
                    {
                        Page.Response.Redirect(redirectUrl, false);
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(message_div.InnerHtml.Replace("\r", " ").Replace("\n", " ").Trim()))
                    {
                        message_div.InnerHtml = "Operation Save Failed. Kindly verify that you provide valid information.";
                        DisableControls(true);
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRB->btnReject_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("Msg_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator.";
                }

                message_div.InnerHtml = message;
                DisableControls(true);
            }
        }
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                var pattern1 = new[] { "~|~" };
                var pattern2 = new[] { "*|*" };

                string recommendationListStr = this.hdnRecommendationList.Value;

                var recommendationList = this.GetFormattedRecommendations(recommendationListStr, pattern1, pattern2);

                bool isSaved = false;
                if (recommendationList != null)
                {
                    if (!String.IsNullOrEmpty(this.hdnIRB_Id.Value))
                    {
                        isSaved = SaveIRDetailedIncidenceDetails(recommendationList, false, false, true, false, false, pattern1, pattern2, Convert.ToInt32(this.hdnIRB_Id.Value));
                    }
                    else
                    {
                        isSaved = SaveIRDetailedIncidenceDetails(recommendationList, false, false, true, false, false, pattern1, pattern2);
                    }
                }

                if (isSaved)
                {
                    string redirectUrl = Utility.GetRedirectUrl("IRB_Approve_Redirect");

                    if (String.IsNullOrEmpty(redirectUrl))
                    {
                        redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");
                    }

                    DisableControls(true);

                    if (!String.IsNullOrEmpty(redirectUrl))
                    {
                        Page.Response.Redirect(redirectUrl, false);
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(message_div.InnerHtml.Replace("\r", " ").Replace("\n", " ").Trim()))
                    {
                        message_div.InnerHtml = "Operation Save Failed. Kindly verify that you provide valid information.";
                        DisableControls(true);
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRB->btnApprove_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("Msg_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator.";
                }

                message_div.InnerHtml = message;
                DisableControls(true);
            }
        }
        protected void btnForward_Click(object sender, EventArgs e)
        {
            try
            {
                var pattern1 = new[] { "~|~" };
                var pattern2 = new[] { "*|*" };

                string recommendationListStr = this.hdnRecommendationList.Value;

                var recommendationList = this.GetFormattedRecommendations(recommendationListStr, pattern1, pattern2);

                bool isSaved = false;
                if (recommendationList != null)
                {
                    if (!String.IsNullOrEmpty(this.hdnIRB_Id.Value))
                    {
                        isSaved = SaveIRDetailedIncidenceDetails(recommendationList, false, true, true, true, false, pattern1, pattern2, Convert.ToInt32(this.hdnIRB_Id.Value));
                    }
                    else
                    {
                        isSaved = SaveIRDetailedIncidenceDetails(recommendationList, false, true, true, true, false, pattern1, pattern2);
                    }
                }

                if (isSaved)
                {
                    string redirectUrl = Utility.GetRedirectUrl("IRB_Forward_Redirect");

                    if (String.IsNullOrEmpty(redirectUrl))
                    {
                        redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");
                    }

                    DisableControls(true);

                    if (!String.IsNullOrEmpty(redirectUrl))
                    {
                        Page.Response.Redirect(redirectUrl, false);
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(message_div.InnerHtml.Replace("\r", " ").Replace("\n", " ").Trim()))
                    {
                        message_div.InnerHtml = "Operation Save Failed. Kindly verify that you provide valid information.";
                        DisableControls(true);
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRB->btnForward_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("Msg_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator.";
                }

                message_div.InnerHtml = message;
                DisableControls(true);
            }
        }
        protected void btnLastSave_Click(object sender, EventArgs e)
        {
            try
            {
                var pattern1 = new[] { "~|~" };
                var pattern2 = new[] { "*|*" };

                string recommendationListStr = this.hdnRecommendationList.Value;

                var recommendationList = this.GetFormattedRecommendations(recommendationListStr, pattern1, pattern2);

                bool isSaved = false;
                if (recommendationList != null)
                {
                    if (!String.IsNullOrEmpty(this.hdnIRB_Id.Value))
                    {
                        isSaved = SaveIRDetailedIncidenceDetails(recommendationList, false, true, true, true, true, pattern1, pattern2, Convert.ToInt32(this.hdnIRB_Id.Value));
                    }
                    else
                    {
                        isSaved = SaveIRDetailedIncidenceDetails(recommendationList, false, true, true, true, true, pattern1, pattern2);
                    }
                }

                if (isSaved)
                {
                    string redirectUrl = Utility.GetRedirectUrl("IRB_LastSave_Redirect");

                    if (String.IsNullOrEmpty(redirectUrl))
                    {
                        redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");
                    }

                    DisableControls(true);

                    if (!String.IsNullOrEmpty(redirectUrl))
                    {
                        Page.Response.Redirect(redirectUrl, false);
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(message_div.InnerHtml.Replace("\r", " ").Replace("\n", " ").Trim()))
                    {
                        message_div.InnerHtml = "Operation Save Failed. Kindly verify that you provide valid information.";
                        DisableControls(true);
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRB->btnLastSave_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("Msg_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator.";
                }

                message_div.InnerHtml = message;
                DisableControls(true);
            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                string redirectUrl = Utility.GetRedirectUrl("IRB_Cancel_Redirect");

                if (String.IsNullOrEmpty(redirectUrl))
                {
                    redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");
                }

                DisableControls(true);
                if (!String.IsNullOrEmpty(redirectUrl))
                {
                    Page.Response.Redirect(redirectUrl, false);
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRB->GetFormattedIds)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("Msg_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator.";
                }

                message_div.InnerHtml = message;
                DisableControls(true);
            }
        }
    }
}