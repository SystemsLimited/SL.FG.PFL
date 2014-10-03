using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;
using SL.FG.PFL.Layouts.SL.FG.PFL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace SL.FG.PFL.WebParts.FlashReportOnJobForm
{
    public partial class FlashReportOnJobFormUserControl : UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {

                    String IRID = Page.Request.QueryString["IRID"];

                    FillArea();
                    FillDepartment();
                    OnLoadDisableControls();
                    LoadPageOnUserBases(IRID);

                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->Page_Load)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

            }
        }

        private void OnLoadDisableControls()
        {
            this.IR_IReceivingDate_dtc.Enabled = false;
            this.FlashIssueDate_dtc.Enabled = false;
            this.TimeOfIncident_dtc.Enabled = false;
            this.Unit_Section_ddl.Disabled = true;
            this.DateOfIncident_dtc.Enabled = false;
        }


        private void DisableControls(bool disableAll)
        {
            btnSaveAsDraft.Visible = false;
            btnSave.Visible = false;
        }

        //private void LoadPageOnUserBases(String FRID, String IRID)
        //{
        //    try
        //    {
        //        using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
        //        {
        //            using (SPWeb oSPWeb = oSPsite.OpenWeb())
        //            {
        //                if (!String.IsNullOrEmpty(IRID))
        //                {
        //                    if (CheckCurrentUserIsHSEMember(oSPWeb))
        //                    {

        //                        SetIR_1Link(IRID);
        //                        GetValuesFromIR_1(IRID);

        //                    }
        //                    else 
        //                    {
        //                        string accessDeniedUrl = Utility.GetRedirectUrl("Access_Denied");

        //                        if (!String.IsNullOrEmpty(accessDeniedUrl))
        //                        {
        //                            Page.Response.Redirect(accessDeniedUrl, false);
        //                        }


        //                    }
        //                }
        //                else if (!String.IsNullOrEmpty(FRID))
        //                {

        //                    SPListItemCollection IR_1InfoList = oSPWeb.Lists["FlashReport"].Items;

        //                    if (IR_1InfoList != null)
        //                    {
        //                        SPListItem imiItem = IR_1InfoList.GetItemById(Convert.ToInt32(FRID));
        //                        if (imiItem != null)
        //                        {


        //                            if (CheckAssignee(imiItem, oSPWeb) && CheckCurrentUserIsHSEMember(oSPWeb))
        //                            {
        //                                LoadPageFromDraft(oSPWeb, FRID);
        //                            }
        //                            else if (CheckAssignee(imiItem, oSPWeb)) 
        //                            {

        //                                SetPageReadOnly();
        //                            }
        //                            else
        //                            {
        //                                string accessDeniedUrl = Utility.GetRedirectUrl("Access_Denied");

        //                                if (!String.IsNullOrEmpty(accessDeniedUrl))
        //                                {
        //                                    Page.Response.Redirect(accessDeniedUrl, false);
        //                                }

        //                            }

        //                        }
        //                    }

        //                }
        //                else
        //                {

        //                    disablefieldsfornewUser(); 
        //                }
        //            }
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-1OffJobForm->PageLoadOnUserBases)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
        //    }
        //}


        private void LoadPageOnUserBases(String IRID)
        {

            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oSPWeb = oSPsite.OpenWeb())
                    {
                        if (!String.IsNullOrEmpty(IRID))
                        {
                            String FRID = null;

                            FRID = Check1StFromDraft(oSPWeb, IRID);

                            if (FRID != null)
                            {
                                if (CheckCurrentUserIsAdmin(oSPWeb) || CheckCurrentUserIsHSEMember(oSPWeb) && CheckAssignee(FRID, oSPWeb))
                                {
                                    LoadPageFromDraft(oSPWeb, FRID);
                                }
                                else if (CheckCurrentUserIsHSEMember(oSPWeb))
                                {
                                    LoadPageFromDraft(oSPWeb, FRID);
                                    SetPageReadOnly();
                                }
                                else
                                {
                                    string accessDeniedUrl = Utility.GetRedirectUrl("Access_Denied");

                                    if (!String.IsNullOrEmpty(accessDeniedUrl))
                                    {
                                        Page.Response.Redirect(accessDeniedUrl, false);
                                    }
                                }
                            }
                            else
                            {
                                if (CheckCurrentUserIsAdmin(oSPWeb) || CheckCurrentUserIsHSEMember(oSPWeb))
                                {
                                    SetIR_1Link(IRID);
                                    GetValuesFromIR_1(IRID);
                                }
                                else
                                {
                                    string accessDeniedUrl = Utility.GetRedirectUrl("Access_Denied");

                                    if (!String.IsNullOrEmpty(accessDeniedUrl))
                                    {
                                        Page.Response.Redirect(accessDeniedUrl, false);
                                    }


                                }
                            }
                        }
                        else
                        {

                            disablefieldsfornewUser();
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->LoadPageOnUserBases)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }

        private String Check1StFromDraft(SPWeb oWebSite, String IRID)
        {
            String FRID = null;

            try
            {


                if (oWebSite != null)
                {
                    string listName = "FlashReport";


                    SPList spList = oWebSite.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oWebSite.Url, listName));

                    SPQuery query = new SPQuery();
                    SPListItemCollection spListItems;

                    query.ViewFields = "<FieldRef Name='IR1ID' /><FieldRef Name='ID' />";
                    query.ViewFieldsOnly = true;

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<Where><Eq><FieldRef Name='IR1ID' /><Value Type='Text'>" + IRID + "</Value></Eq></Where>");
                    query.Query = sb.ToString();
                    spListItems = spList.GetItems(query);



                    if (spListItems != null)
                    {
                        foreach (SPListItem IR_1item in spListItems)
                        {
                            FRID = IR_1item["ID"].ToString();

                        }
                    }
                }


            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->Check1StFromDraft)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return FRID;
        }

        private void disablefieldsfornewUser()
        {

            this.btnSaveAsDraft.Attributes.Add("style", "display:none");
            this.btnSave.Attributes.Add("style", "display:none");

        }

        private void SetPageReadOnly()
        {
            this.IR_IReceivingDate_dtc.Enabled = false;
            this.FlashIssueDate_dtc.Enabled = false;
            this.Description1_ta.Disabled = true;
            this.Unit_Section_ddl.Disabled = true;
            this.DateOfIncident_dtc.Enabled = false;
            this.TimeOfIncident_dtc.Enabled = false;
            this.ActionTaken_ta.Disabled = true;
            this.ActionRequired_Unit_ddl.Enabled = false;
            this.ResponsibleSection_Unit_ddl.Disabled = true;
            this.ResponsibleDepartmentt_ddl.Disabled = true;
            this.TargetDate_dtc.Enabled = false;
            this.ApprovingAuthority_PeopleEditor.Enabled = false;
            this.TeamLead_PeopleEditor.Enabled = false;
            this.TeamMembers_PeopleEditor.Enabled = false;
            this.Description2_ta.Disabled = true;

            this.btnSaveAsDraft.Attributes.Add("style", "display:none");
            this.btnSave.Attributes.Add("style", "display:none");
        }

        private bool CheckCurrentUserIsAdmin(SPWeb oSPWeb)
        {
            Boolean IsSupervisor = false;
            try
            {
                //var spGroup = oSPWeb.Groups.GetByName("Supervisor");
                //if (spGroup != null)
                //{
                //    IsSupervisor = oSPWeb.IsCurrentUserMemberOfGroup(spGroup.ID);
                //}

                string groupName = Utility.GetValueByKey("AdminGroup");
                var spGroup = oSPWeb.Groups[groupName];

                if (spGroup != null)
                {
                    IsSupervisor = oSPWeb.IsCurrentUserMemberOfGroup(spGroup.ID);
                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->CheckCurrentUserIsAdmin)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return IsSupervisor;
        }

        private bool CheckCurrentUserIsHSEMember(SPWeb oSPWeb)
        {
            Boolean IsHSEMember = false;
            try
            {
                string groupName = Utility.GetValueByKey("MasterGroup");
                var spGroup = oSPWeb.Groups[groupName];

                if (spGroup != null)
                {
                    IsHSEMember = oSPWeb.IsCurrentUserMemberOfGroup(spGroup.ID);
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->CheckCurrentUserIsHSEMember)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return IsHSEMember;
        }

        private bool CheckCurrentUserIsAllFGMembersAndAssignee(SPWeb oSPWeb, String FRID)
        {
            Boolean IsHSEMember = false;
            try
            {

                SPListItemCollection IR_1InfoList = oSPWeb.Lists["FlashReport"].Items;
                if (IR_1InfoList != null)
                {
                    SPListItem ListItem = IR_1InfoList.GetItemById(Convert.ToInt32(FRID));

                    if (ListItem != null)
                    {
                        string groupName = Utility.GetValueByKey("AllFGMembersGroup");
                        var spGroup = oSPWeb.Groups[groupName];

                        if (spGroup != null)
                        {
                            if (!String.IsNullOrEmpty(Convert.ToString(ListItem["Assignee"])) && Convert.ToString(ListItem["Assignee"]) == groupName)
                            {
                                IsHSEMember = oSPWeb.IsCurrentUserMemberOfGroup(spGroup.ID);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->CheckCurrentUserIsAllFGMembersAndAssignee)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return IsHSEMember;
        }

        //private Boolean CheckAssignee(String FRID, SPWeb oSPWeb)
        //{
        //    Boolean assignee = false;

        //    try
        //    {

        //        SPListItemCollection FR_ListItems = oSPWeb.Lists["FlashReport"].Items;
        //        if (FR_ListItems != null)
        //        {
        //            SPListItem ListItem = FR_ListItems.GetItemById(Convert.ToInt32(FRID));


        //            String Assignees = Convert.ToString(ListItem["Assignee"]);

        //            String GroupName = Utility.GetValueByKey("MasterGroup");

        //            if (Assignees == GroupName)
        //            {
        //                assignee = true;

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->CheckCurrentUserIsHSEMember)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
        //    }
        //    return assignee;

        //}

        private Boolean CheckAssignee(String FRID, SPWeb oSPWeb)
        {
            Boolean assignee = false;

            SPListItemCollection IR_1InfoList = oSPWeb.Lists["FlashReport"].Items;
            if (IR_1InfoList != null)
            {
                SPListItem ListItem = IR_1InfoList.GetItemById(Convert.ToInt32(FRID));

                if (ListItem != null)
                {

                    String User = oSPWeb.CurrentUser.LoginName;
                    String[] Name = User.Split('|');
                    String currentUser = Name[1]; ;

                    if (currentUser != null)
                    {
                        String s = Convert.ToString(ListItem["Assignee"]);

                        string[] AssigneeList = s.Split(',');


                        foreach (string person in AssigneeList)
                        {
                            if (person == currentUser)
                                assignee = true;
                        }
                    }
                }
            }
            return assignee;

        }

        private void SetIR_1Link(String ID)
        {
            String Link = Utility.GetValueByKey("IR1FormLink");

            if (!String.IsNullOrEmpty(ID))
            {
                this.IR_link.Attributes.Add("href", "/" + Link + "?IRID=" + ID);
            }
        }

        private void FillDepartment()
        {
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oSPWeb = oSPsite.OpenWeb())
                    {
                        string listName = "Department";

                        // Fetch the List
                        SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));

                        SPQuery query = new SPQuery();
                        SPListItemCollection spListItems;
                        // Include only the fields you will use.
                        query.ViewFields = "<FieldRef Name='ID'/><FieldRef Name='Title'/>";
                        query.ViewFieldsOnly = true;
                        //query.RowLimit = 200; // Only select the top 200.
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<Where><Eq><FieldRef Name='DepartmentDescription' /><Value Type='Note'>HOD</Value></Eq></Where><OrderBy Override='TRUE';><FieldRef Name='Title'/></OrderBy>");
                        query.Query = sb.ToString();
                        spListItems = spList.GetItems(query);

                        this.ResponsibleDepartmentt_ddl.DataSource = spListItems;
                        this.ResponsibleDepartmentt_ddl.DataTextField = "Title";
                        this.ResponsibleDepartmentt_ddl.DataValueField = "Title";
                        this.ResponsibleDepartmentt_ddl.DataBind();

                        this.ResponsibleDepartmentt_ddl.Items.Insert(0, new ListItem("Please Select", "0"));

                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->CheckAssignee)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }


        }

        private void FillArea()
        {


            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oSPWeb = oSPsite.OpenWeb())
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
                        sb.Append("<OrderBy Override='TRUE';><FieldRef Name='Title'/></OrderBy>");
                        query.Query = sb.ToString();
                        spListItems = spList.GetItems(query);

                        this.Unit_Section_ddl.DataSource = spListItems;
                        this.Unit_Section_ddl.DataTextField = "Title";
                        this.Unit_Section_ddl.DataValueField = "Title";
                        this.Unit_Section_ddl.DataBind();

                        this.Unit_Section_ddl.Items.Insert(0, new ListItem("Please Select", "0"));

                        this.ResponsibleSection_Unit_ddl.DataSource = spListItems;
                        this.ResponsibleSection_Unit_ddl.DataTextField = "Title";
                        this.ResponsibleSection_Unit_ddl.DataValueField = "Title";
                        this.ResponsibleSection_Unit_ddl.DataBind();

                        this.ResponsibleSection_Unit_ddl.Items.Insert(0, new ListItem("Please Select", "0"));
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->FillArea)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        private void GetValuesFromIR_1(String IRID)
        {

            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oSPWeb = oSPsite.OpenWeb())
                    {
                        SPListItemCollection IR_1InfoList = oSPWeb.Lists["IR-1"].Items;
                        if (IR_1InfoList != null)
                        {
                            SPListItem imiItem = IR_1InfoList.GetItemById(Convert.ToInt32(IRID));

                            if (imiItem != null)
                            {

                                if (!String.IsNullOrEmpty(Convert.ToString(imiItem["DateOfIncident"])))

                                    this.DateOfIncident_dtc.SelectedDate = Convert.ToDateTime(imiItem["DateOfIncident"]);

                                if (!String.IsNullOrEmpty(Convert.ToString(imiItem["TimeOfIncident"])))

                                    this.TimeOfIncident_dtc.SelectedDate = Convert.ToDateTime(imiItem["TimeOfIncident"]);


                                if (!String.IsNullOrEmpty(Convert.ToString(imiItem["Unit_x002f_Area"])))
                                {

                                    this.Unit_Section_ddl.Items.FindByValue(Convert.ToString(imiItem["Unit_x002f_Area"])).Selected = true;

                                    this.Unit_Section_hdn.Value = Convert.ToString(imiItem["Unit_x002f_Area"]);
                                }

                                if (!String.IsNullOrEmpty(Convert.ToString(imiItem["Unit_x002f_Area"])))
                                {

                                    this.ResponsibleSection_Unit_ddl.Items.FindByValue(Convert.ToString(imiItem["Unit_x002f_Area"])).Selected = true;

                                    this.ResponsibleSection_Unit_hdn.Value = Convert.ToString(imiItem["Unit_x002f_Area"]);
                                }

                                this.ResponsibleDepartmentt_hdn.Value = this.ResponsibleDepartmentt_ddl.Items[0].Value;



                                if (!String.IsNullOrEmpty(Convert.ToString(imiItem["DateOFSubmission"])))
                                {
                                    this.IR_IReceivingDate_dtc.SelectedDate = Convert.ToDateTime(imiItem["DateOFSubmission"]);
                                }
                                this.FlashIssueDate_dtc.SelectedDate = System.DateTime.Now;



                                if (!String.IsNullOrEmpty(Convert.ToString(imiItem["Description"])))

                                    this.Description1_ta.Value = Convert.ToString(imiItem["Description"]);

                                if (!String.IsNullOrEmpty(Convert.ToString(imiItem["ActionTaken"])))

                                    this.ActionTaken_ta.Value = Convert.ToString(imiItem["ActionTaken"]);

                                if (!String.IsNullOrEmpty(Convert.ToString(imiItem["IncidentScore"])))

                                    this.IncidentScore_tf.Value = Convert.ToString(imiItem["IncidentScore"]);


                                this.TargetDate_dtc.SelectedDate = System.DateTime.Now;

                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->GetValuesFromIR_1)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }


        }

        public static SPUser GetUser(SPWeb oSPWeb, string username = null, string email = null, int userId = 0)
        {
            SPUser spUser = null;
            try
            {
                if (oSPWeb != null)
                {
                    if (!String.IsNullOrEmpty(username))
                    {
                        if (username.Contains("|"))
                        {
                            var temp = username.Split('|');
                            if (temp.Length > 1)
                            {
                                spUser = oSPWeb.AllUsers[temp[1]];
                            }
                        }
                        else
                        {
                            string temp = "i:0#.w|" + username;
                            spUser = oSPWeb.AllUsers[temp];
                        }
                    }
                    if (spUser == null && !String.IsNullOrEmpty(email))
                    {
                        spUser = oSPWeb.AllUsers.GetByEmail(email);
                    }
                    if (spUser == null && userId > 0)
                    {
                        spUser = oSPWeb.AllUsers.GetByID(userId);
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->GetUser)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return spUser;
        }


        //private SPUser GetUser(SPWeb oSPWeb, string username = null, string email = null, int userId = 0)
        //{
        //    SPUser spUser = null;
        //    try
        //    {
        //        if (oSPWeb != null)
        //        {
        //            if (!String.IsNullOrEmpty(username))
        //            {
        //                string temp = "i:0#.w|" + username;
        //                spUser = oSPWeb.AllUsers[temp];
        //            }
        //            else if (!String.IsNullOrEmpty(email))
        //            {
        //                spUser = oSPWeb.AllUsers.GetByEmail(email);
        //            }
        //            else if (userId > 0)
        //            {
        //                spUser = oSPWeb.AllUsers.GetByID(userId);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->Page_Load)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
        //    }

        //    return spUser;
        //}

        private void LoadPageFromDraft(SPWeb oSPWeb, String FRID)
        {

            try
            {

                SPListItemCollection IR_1InfoList = oSPWeb.Lists["FlashReport"].Items;
                if (IR_1InfoList != null)
                {
                    SPListItem imiItem = IR_1InfoList.GetItemById(Convert.ToInt32(FRID));

                    if (imiItem != null)
                    {

                        if (imiItem.Attachments.Count > 0)
                        {
                            foreach (String attachmentname in imiItem.Attachments)
                            {
                                String attachmentAbsoluteURL =
                                imiItem.Attachments.UrlPrefix // gets the containing directory URL
                                + attachmentname;
                                // To get the SPSile reference to the attachment just use this code
                                SPFile attachmentFile = oSPWeb.GetFile(attachmentAbsoluteURL);

                                StringBuilder sb = new StringBuilder();

                                HtmlTableRow tRow = new HtmlTableRow();

                                HtmlTableCell removeLink = new HtmlTableCell();
                                HtmlTableCell fileLink = new HtmlTableCell();

                                sb.Append(String.Format("<a href='{0}/{1}' target='_blank'>{2}</a>", oSPWeb.Url, attachmentFile.Url, attachmentname));
                                removeLink.InnerHtml = "<span class='btn-danger removeLink' style='padding:3px; margin-right:3px; border-radius:2px;'><i class='glyphicon glyphicon-remove'></i></span><span class='fileName' style='display:none;'>" + attachmentFile.Name + "</span>";

                                fileLink.InnerHtml = sb.ToString();

                                tRow.Cells.Add(removeLink);
                                tRow.Cells.Add(fileLink);

                                this.grdAttachments.Rows.Add(tRow);

                            }

                        }




                        if (!String.IsNullOrEmpty(Convert.ToString(imiItem["IRID"])))
                            SetIR_1Link(Convert.ToString(imiItem["IRID"]));



                        if (!String.IsNullOrEmpty(Convert.ToString(imiItem["IR_x002d_1ReceivingDate"])))
                        {
                            DateTime Date;
                            bool bValid = DateTime.TryParse(Convert.ToString(imiItem["IR_x002d_1ReceivingDate"]), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out Date);

                            if (!bValid)
                            {
                                Date = Convert.ToDateTime(imiItem["IR_x002d_1ReceivingDate"]);
                            }

                            this.IR_IReceivingDate_dtc.SelectedDate = Date;
                        }

                        if (!String.IsNullOrEmpty(Convert.ToString(DateTime.Now)))
                        {
                            DateTime Date;
                            bool bValid = DateTime.TryParse(Convert.ToString(DateTime.Now), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out Date);

                            if (!bValid)
                            {
                                Date = Convert.ToDateTime(DateTime.Now);
                            }

                            this.FlashIssueDate_dtc.SelectedDate = Date;

                            this.FlashIssueDate_dtc.Enabled = false;
                        }



                        if (!String.IsNullOrEmpty(Convert.ToString(imiItem["Unit_x002f_Section"])))
                        {

                            this.Unit_Section_ddl.Items.FindByValue(Convert.ToString(imiItem["Unit_x002f_Section"])).Selected = true;

                            this.Unit_Section_hdn.Value = Convert.ToString(imiItem["Unit_x002f_Section"]);
                        }



                        if (!String.IsNullOrEmpty(Convert.ToString(imiItem["DateOfIncident"])))
                        {
                            DateTime Date;
                            bool bValid = DateTime.TryParse(Convert.ToString(imiItem["DateOfIncident"]), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out Date);

                            if (!bValid)
                            {
                                Date = Convert.ToDateTime(imiItem["DateOfIncident"]);
                            }

                            this.DateOfIncident_dtc.SelectedDate = Date;
                        }




                        if (!String.IsNullOrEmpty(Convert.ToString(imiItem["TimeOfIncident"])))

                            this.TimeOfIncident_dtc.SelectedDate = Convert.ToDateTime(imiItem["TimeOfIncident"]);

                        if (!String.IsNullOrEmpty(Convert.ToString(imiItem["DescriptionOfIncident"])))

                            this.Description1_ta.Value = Convert.ToString(imiItem["DescriptionOfIncident"]);

                        if (!String.IsNullOrEmpty(Convert.ToString(imiItem["ActionTaken"])))

                            this.ActionTaken_ta.Value = Convert.ToString(imiItem["ActionTaken"]);

                        if (!String.IsNullOrEmpty(Convert.ToString(imiItem["IncidentScore"])))

                            this.IncidentScore_tf.Value = Convert.ToString(imiItem["IncidentScore"]);

                        this.IncidentScore_tf.Disabled = true;

                        if (!String.IsNullOrEmpty(Convert.ToString(imiItem["ActionRequired"])))

                            this.ActionRequired_Unit_ddl.SelectedValue = Convert.ToString(imiItem["ActionRequired"]);



                        if (!String.IsNullOrEmpty(Convert.ToString(imiItem["ResponcibleSection/Unit"])))
                        {

                            this.ResponsibleSection_Unit_ddl.Items.FindByValue(Convert.ToString(imiItem["ResponcibleSection/Unit"])).Selected = true;

                            this.ResponsibleSection_Unit_hdn.Value = Convert.ToString(imiItem["ResponcibleSection/Unit"]);
                        }

                        //FillDepartment();

                        if (!String.IsNullOrEmpty(Convert.ToString(imiItem["ResponcibleDepartment"])))
                        {
                            this.ResponsibleDepartmentt_ddl.Items.FindByValue(Convert.ToString(imiItem["ResponcibleDepartment"])).Selected = true;

                            this.ResponsibleDepartmentt_hdn.Value = Convert.ToString(imiItem["ResponcibleDepartment"]);
                        }



                        if (!String.IsNullOrEmpty(Convert.ToString(imiItem["TargetDate"])))
                        {
                            DateTime Date;
                            bool bValid = DateTime.TryParse(Convert.ToString(imiItem["TargetDate"]), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out Date);

                            if (!bValid)
                            {
                                Date = Convert.ToDateTime(imiItem["TargetDate"]);
                            }

                            this.TargetDate_dtc.SelectedDate = Date;
                        }



                        if (!String.IsNullOrEmpty(Convert.ToString(imiItem["ApprovingAuthority"])))
                        {


                            PeopleEditor pe = new PeopleEditor();
                            PickerEntity UserEntity = new PickerEntity();
                            String username = Convert.ToString(imiItem["ApprovingAuthority"]);
                            //get Spuser
                            SPUser SPuser = GetUser(oSPWeb, username, null, 0);
                            if (SPuser != null)
                            {
                                // CurrentUser is SPUser object
                                UserEntity.DisplayText = SPuser.Name;
                                UserEntity.Key = SPuser.LoginName;

                                UserEntity = pe.ValidateEntity(UserEntity);

                                // Add PickerEntity to People Picker control
                                this.ApprovingAuthority_PeopleEditor.AddEntities(new List<PickerEntity> { UserEntity });


                            }



                        }

                        if (!String.IsNullOrEmpty(Convert.ToString(imiItem["TeamLead"])))
                        {

                            // Clear existing users from control
                            // this.TeamLead_PeopleEditor.ResolvedEntities.Clear();

                            // PickerEntity object is used by People Picker Control
                            PeopleEditor pe = new PeopleEditor();
                            PickerEntity UserEntity = new PickerEntity();
                            String username = Convert.ToString(imiItem["TeamLead"]);
                            //get Spuser
                            SPUser SPuser = GetUser(oSPWeb, username, null, 0);
                            if (SPuser != null)
                            {
                                // CurrentUser is SPUser object
                                UserEntity.DisplayText = SPuser.Name;
                                UserEntity.Key = SPuser.LoginName;

                                UserEntity = pe.ValidateEntity(UserEntity);

                                // Add PickerEntity to People Picker control
                                this.TeamLead_PeopleEditor.AddEntities(new List<PickerEntity> { UserEntity });


                            }

                        }


                        if (!String.IsNullOrEmpty(Convert.ToString(imiItem["TeamMembers"])))
                        {

                            // Clear existing users from control
                            this.TeamMembers_PeopleEditor.AllEntities.Clear();

                            // PickerEntity object is used by People Picker Control


                            List<PickerEntity> UserList = new List<PickerEntity>();

                            String[] AllUsers = Convert.ToString(imiItem["TeamMembers"].ToString()).Split(',');

                            foreach (string username in AllUsers)
                            {
                                SPUser SPuser = GetUser(oSPWeb, username, null, 0);

                                if (SPuser != null)
                                {
                                    // CurrentUser is SPUser object
                                    PickerEntity UserEntity = new PickerEntity();
                                    PeopleEditor pe = new PeopleEditor();
                                    UserEntity.DisplayText = SPuser.Name;
                                    UserEntity.Key = SPuser.LoginName;
                                    UserEntity = pe.ValidateEntity(UserEntity);

                                    UserList.Add(UserEntity);



                                }

                            }
                            // Add PickerEntity to People Picker control
                            this.TeamMembers_PeopleEditor.AddEntities(UserList);
                        }


                        if (!String.IsNullOrEmpty(Convert.ToString(imiItem["Description"])))

                            this.Description2_ta.Value = Convert.ToString(imiItem["Description"]);
                    }
                }
            }

            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->LoadPageFromDraft)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {

            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {

                        if (oWebSite != null)
                        {
                            //SPListItemCollection IR_1infoList = oWebSite.Lists["IR-1"].Items;

                            string listName = "FlashReport";

                            // Fetch the List
                            SPList list = oWebSite.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oWebSite.Url, listName));

                            String IRID = Page.Request.QueryString["IRID"];
                            int IRItemID = Convert.ToInt32(IRID);



                            //String FRID = Page.Request.QueryString["FRID"];

                            //int FRItemID = Convert.ToInt32(FRID);

                            if (IRItemID != 0 && list != null)
                            {

                                String FR_ID = Check1StFromDraft(oWebSite, IRID);

                                int FR_ItemID = Convert.ToInt32(FR_ID);

                                SPListItem spListItem = null;

                                if (FR_ID != null)
                                    spListItem = list.Items.GetItemById(FR_ItemID);
                                else
                                    spListItem = list.Items.Add();

                                if (spListItem != null)
                                {

                                    UpdateFlashReportValues(spListItem, false, oWebSite);

                                }

                            }
                            //else if (FRItemID != 0 && list != null)
                            //{
                            //    SPListItem spListItem = list.Items.GetItemById(FRItemID);

                            //    if (spListItem != null)
                            //    {

                            //        UpdateFlashReportValues(spListItem, false, oWebSite);

                            //    }

                            //}

                            string redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");

                            DisableControls(true);
                            if (!String.IsNullOrEmpty(redirectUrl))
                            {
                                Page.Response.Redirect(redirectUrl, false);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->btnSave_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }

        protected void btnSaveAsDraft_Click(object sender, EventArgs e)
        {
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {
                        if (oWebSite != null)
                        {
                            //SPListItemCollection IR_1infoList = oWebSite.Lists["IR-1"].Items;

                            string listName = "FlashReport";

                            // Fetch the List
                            SPList list = oWebSite.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oWebSite.Url, listName));



                            String IRID = Page.Request.QueryString["IRID"];
                            int IRItemID = Convert.ToInt32(IRID);



                            //String FRID = Page.Request.QueryString["FRID"];

                            //int FRItemID = Convert.ToInt32(FRID);

                            if (IRItemID != 0 && list != null)
                            {

                                String FR_ID = Check1StFromDraft(oWebSite, IRID);

                                int FR_ItemID = Convert.ToInt32(FR_ID);

                                SPListItem spListItem = null;

                                if (FR_ID != null)
                                    spListItem = list.Items.GetItemById(FR_ItemID);
                                else
                                    spListItem = list.Items.Add();

                                if (spListItem != null)
                                {

                                    UpdateFlashReportValues(spListItem, true, oWebSite);

                                }

                            }
                            //    else if (IRItemID != 0 && list != null)
                            //    {
                            //        SPListItem spListItem = null;

                            //        if (FR_ID != null)
                            //            spListItem = list.Items.GetItemById(FR_ItemID);
                            //        else
                            //            spListItem = list.Items.Add();


                            //        if (spListItem != null)
                            //        {

                            //            UpdateFlashReportValues(spListItem, true, oWebSite);

                            //        }
                            //    }

                            string redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");

                            DisableControls(true);
                            if (!String.IsNullOrEmpty(redirectUrl))
                            {
                                Page.Response.Redirect(redirectUrl, false);
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->btnSaveAsDraft_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }

        protected void UpdateFlashReportValues(SPListItem ListItem, Boolean IsSaveAsDraft, SPWeb oWebSite)
        {
            try
            {
                if (ListItem != null)
                {



                    if (!String.IsNullOrEmpty(Convert.ToString(this.IR_IReceivingDate_dtc.SelectedDate)))
                    {
                        DateTime date;
                        bool bValid = DateTime.TryParse(this.IR_IReceivingDate_dtc.SelectedDate.ToShortDateString(), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);


                        if (bValid)
                            ListItem["IR-1ReceivingDate"] = date;
                        else
                            ListItem["IR-1ReceivingDate"] = Convert.ToDateTime(this.IR_IReceivingDate_dtc.SelectedDate);
                    }



                    if (!String.IsNullOrEmpty(Convert.ToString(this.IR_IReceivingDate_dtc.SelectedDate)))
                    {
                        DateTime date;
                        bool bValid = DateTime.TryParse(this.IR_IReceivingDate_dtc.SelectedDate.ToShortDateString(), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);


                        if (bValid)
                            ListItem["FlashIssueDate"] = date;
                        else
                            ListItem["FlashIssueDate"] = Convert.ToDateTime(this.IR_IReceivingDate_dtc.SelectedDate);
                    }

                    if (!String.IsNullOrEmpty(Convert.ToString(this.Unit_Section_hdn.Value)))
                        ListItem["Unit/Section"] = (Convert.ToString(this.Unit_Section_hdn.Value));




                    if (!String.IsNullOrEmpty(Convert.ToString(this.DateOfIncident_dtc.SelectedDate)))
                        ListItem["DateOfIncident"] = Convert.ToDateTime(this.DateOfIncident_dtc.SelectedDate.ToShortDateString());
                    else
                        ListItem["DateOfIncident"] = null;


                    if (!String.IsNullOrEmpty(Convert.ToString(this.TimeOfIncident_dtc.SelectedDate)))
                        ListItem["TimeOfIncident"] = this.TimeOfIncident_dtc.SelectedDate.ToShortTimeString();
                    else
                        ListItem["TimeOfIncident"] = null;


                    if (!String.IsNullOrEmpty(Convert.ToString(this.Description1_ta.Value)))
                        ListItem["DescriptionOfIncident"] = Convert.ToString(this.Description1_ta.Value);


                    if (!String.IsNullOrEmpty(Convert.ToString(this.ActionTaken_ta.Value)))
                        ListItem["ActionTaken"] = Convert.ToString(this.ActionTaken_ta.Value);


                    if (!String.IsNullOrEmpty(Convert.ToString(this.IncidentScore_tf.Value)))
                        ListItem["IncidentScore"] = Convert.ToString(this.IncidentScore_tf.Value);


                    if (!String.IsNullOrEmpty(Convert.ToString(this.ActionRequired_Unit_ddl.SelectedItem.Text)))
                        ListItem["ActionRequired"] = Convert.ToString(this.ActionRequired_Unit_ddl.SelectedItem.Text);

                    if (!String.IsNullOrEmpty(Convert.ToString(this.ResponsibleSection_Unit_hdn.Value)))
                        ListItem["ResponcibleSection/Unit"] = (Convert.ToString(this.ResponsibleSection_Unit_hdn.Value));



                    if (!String.IsNullOrEmpty(Convert.ToString(this.ResponsibleDepartmentt_hdn.Value)))
                        ListItem["ResponcibleDepartment"] = (Convert.ToString(this.ResponsibleDepartmentt_hdn.Value));


                    if (!String.IsNullOrEmpty(Convert.ToString(this.TargetDate_dtc.SelectedDate)))
                    {
                        DateTime date;
                        bool bValid = DateTime.TryParse(this.TargetDate_dtc.SelectedDate.ToShortDateString(), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);


                        if (bValid)
                            ListItem["TargetDate"] = date;
                        else
                            ListItem["TargetDate"] = Convert.ToDateTime(this.TargetDate_dtc.SelectedDate);
                    }


                    if (this.ApprovingAuthority_PeopleEditor.ResolvedEntities != null && this.ApprovingAuthority_PeopleEditor.ResolvedEntities.Count > 0)
                    {
                        PickerEntity entity = (PickerEntity)this.ApprovingAuthority_PeopleEditor.ResolvedEntities[0];

                        ListItem["ApprovingAuthority"] = entity.Claim.Value;
                    }


                    if (this.TeamLead_PeopleEditor.ResolvedEntities != null && this.TeamLead_PeopleEditor.ResolvedEntities.Count > 0)
                    {
                        PickerEntity entity = (PickerEntity)this.TeamLead_PeopleEditor.ResolvedEntities[0];

                        ListItem["TeamLead"] = entity.Claim.Value;
                    }

                    if (this.TeamMembers_PeopleEditor.ResolvedEntities != null && this.TeamMembers_PeopleEditor.ResolvedEntities.Count > 0)
                    {
                        PickerEntity entity;

                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        int i = 0;
                        for (; i < this.TeamMembers_PeopleEditor.ResolvedEntities.Count - 1; i++)
                        {
                            entity = (PickerEntity)this.TeamMembers_PeopleEditor.ResolvedEntities[i];
                            sb.Append(entity.Claim.Value.ToString() + ",");

                        }
                        entity = (PickerEntity)this.TeamMembers_PeopleEditor.ResolvedEntities[i];
                        sb.Append(entity.Claim.Value.ToString());
                        ListItem["TeamMembers"] = sb.ToString();
                    }


                    if (!String.IsNullOrEmpty(Convert.ToString(this.Description2_ta.Value)))
                        ListItem["Description"] = Convert.ToString(this.Description2_ta.Value);

                    if (IsSaveAsDraft)
                    {
                        ListItem["IsSaveAsDraft"] = true;

                        ListItem["Status"] = "Inprogress";

                        if (Page.Request.QueryString["IRID"] != null)
                            ListItem["IRID"] = Page.Request.QueryString["IRID"];

                        String currentUserName = Utility.GetUsername(oWebSite.CurrentUser.LoginName);



                        StringBuilder AssigneeUsers = new StringBuilder();

                        var Users = GetGroupMembers("MasterGroup");


                        foreach (SPUser user in Users)
                        {
                            String User = user.LoginName;
                            String[] Name = User.Split('|');


                            if (Name.Length > 1)
                                AssigneeUsers.Append(Name[1]).Append(",");
                        }

                        AssigneeUsers.Length = AssigneeUsers.Length - 1;

                        ListItem["Assignee"] = AssigneeUsers.ToString();




                        ListItem["SubmittedBy"] = currentUserName;

                        ListItem.Update();


                    }
                    else
                    {
                        //StringBuilder AssigneeUsers = new StringBuilder();

                        //string groupName = Utility.GetValueByKey("MasterGroup");
                        //AssigneeUsers.Append(groupName).Append(",");
                        //  string groupName = Utility.GetValueByKey("AllFGMembersGroup");
                        //AssigneeUsers.Append(groupName);
                        //ListItem["Assignee"] = AssigneeUsers.ToString();
                        ListItem["Assignee"] = "";

                        String User = oWebSite.CurrentUser.LoginName;
                        String[] Name = User.Split('|');
                        if (Name.Length > 1)
                            ListItem["SubmittedBy"] = Name[1];
                        //ListItem["IsSaveAsDraft"] = false;

                        //string groupName = Utility.GetValueByKey("MasterGroup");
                        //AssignToGroup(oWebSite, ListItem, groupName);

                        //groupName = Utility.GetValueByKey("AllFGMembersGroup");
                        //AssignToGroup(oWebSite, ListItem, groupName);

                        ListItem["Status"] = "Comlete";


                        ListItem["IsSaveAsDraft"] = false;

                        ListItem.Update();

                        SendEmailToTeamLeadAndMembers(ListItem);
                        //  SendEmailToHSEGroupAndFGAllMembers(ListItem);
                        //Send Email To All HSE And FG members
                        //send Email to Teamlead and team members for tast IR-05 Form

                    }


                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->UpdateFlashReportValues)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }

        protected List<SPUser> GetGroupMembers(String GroupName)
        {
            List<SPUser> Users = new List<SPUser>();
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                    {
                        using (SPWeb oWebSite = oSPsite.OpenWeb())
                        {

                            String groupName = Utility.GetValueByKey(GroupName);
                            SPGroup Group = oWebSite.Groups[groupName];

                            foreach (SPUser user in Group.Users)
                            {
                                // add all the group users to the list
                                Users.Add(user);
                            }

                        }
                    }
                });
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->GetGroupMembers)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return Users;
        }

        private void AssignToGroup(SPWeb oWebSite, SPListItem ListItem, String GroupName)
        {

            try
            {
                List<SPUser> Users = new List<SPUser>();

                Users = GetGroupMembers(oWebSite, GroupName);

                StringBuilder AssigneeUsers = new StringBuilder();

                if (!String.IsNullOrEmpty(Convert.ToString(ListItem["Assignee"])))
                {
                    foreach (SPUser user in Users)
                    {
                        String User = user.LoginName;
                        String[] Name = User.Split('|');
                        if (Name.Length > 1)
                            AssigneeUsers.Append(Name[1]).Append(",");
                    }

                    AssigneeUsers.Append(Convert.ToString(ListItem["Assignee"]));

                    ListItem["Assignee"] = AssigneeUsers.ToString();
                }
                else
                {
                    foreach (SPUser user in Users)
                    {
                        String User = user.LoginName;
                        String[] Name = User.Split('|');
                        if (Name.Length > 1)

                            AssigneeUsers.Append(Name[1]).Append(",");
                    }

                    AssigneeUsers.Length = AssigneeUsers.Length - 1;

                    ListItem["Assignee"] = AssigneeUsers.ToString();
                }


            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->AssignToGroup)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        protected List<SPUser> GetGroupMembers(SPWeb oWebSite, String GroupName)
        {
            List<SPUser> Users = new List<SPUser>();
            try
            {

                String groupName = Utility.GetValueByKey(GroupName);



                SPGroup Group = oWebSite.Groups[groupName];

                foreach (SPUser user in Group.Users)
                {
                    // add all the group users to the list
                    Users.Add(user);
                }


            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->GetGroupMembers)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return Users;
        }

        protected void SendEmailToHSEGroupAndFGAllMembers(SPListItem imiItem)
        {
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {

                        if (oWebSite != null)
                        {
                            string IR_1Link = Utility.GetRedirectUrl("FlashReportFormLink");
                            string subject = Utility.GetValueByKey("HSEGroupEmailSubject");
                            string body = Utility.GetValueByKey("HSEGroupEmailTemplate");

                            StringBuilder linkSB = new StringBuilder();
                            linkSB.Append(IR_1Link)
                                        .Append("?FRID=")
                                        .Append(imiItem.ID);

                            //body = body.Replace("~|~", linkSB.ToString());
                            body = linkSB.ToString();

                            SPUser spSender = Utility.GetUser(oWebSite, Convert.ToString(imiItem["SubmittedBy"]));
                            Message message = new Message();
                            message.Subject = subject;
                            message.Body = body;
                            message.From = spSender.Email;

                            List<SPUser> Users = new List<SPUser>();

                            string GroupName = Utility.GetValueByKey("MasterGroup");

                            Users = GetGroupMembers(oWebSite, GroupName);

                            foreach (SPUser user in Users)
                            {
                                String User = user.LoginName;
                                String[] Name = User.Split('|');
                                if (Name.Length > 1)
                                {

                                    SPUser HSEmember = Utility.GetUser(oWebSite, Name[1]);
                                    message.To = HSEmember.Email;
                                    Email.SendEmail(message);
                                }

                            }

                            GroupName = Utility.GetValueByKey("AllFGMembersGroup");

                            Users = GetGroupMembers(oWebSite, GroupName);

                            foreach (SPUser user in Users)
                            {
                                String User = user.LoginName;
                                String[] Name = User.Split('|');
                                if (Name.Length > 1)
                                {
                                    SPUser FGMember = Utility.GetUser(oWebSite, Name[1]);
                                    message.To = FGMember.Email;
                                    Email.SendEmail(message);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->SendEmailToHSEGroupAndFGAllMembers)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        protected void SendEmailToTeamLeadAndMembers(SPListItem imiItem)
        {
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {

                        if (oWebSite != null)
                        {
                            string IR_1Link = Utility.GetRedirectUrl("IR_5FormLink");
                            string subject = Utility.GetValueByKey("TeamEmailSubject");
                            string body = Utility.GetValueByKey("TeamEmailTemplate");

                            StringBuilder linkSB = new StringBuilder();
                            linkSB.Append(IR_1Link)
                                        .Append("?FRID=")
                                        .Append(imiItem.ID);

                            //body = body.Replace("~|~", linkSB.ToString());
                            body = linkSB.ToString();

                            SPUser spSender = Utility.GetUser(oWebSite, Convert.ToString(imiItem["SubmittedBy"]));
                            Message message = new Message();
                            message.Subject = subject;
                            message.Body = body;
                            message.From = spSender.Email;

                            string listName = null;

                            //if (!String.IsNullOrEmpty(Convert.ToString(imiItem["IncidentScore"])))
                            //{
                            //    int incidentScore = 0;

                            //    Int32.TryParse(Convert.ToString(imiItem["IncidentScore"]), out incidentScore);




                            //    if (incidentScore > 0 && incidentScore < 75)
                            //    {
                            //        listName = "IR01DI";
                            //    }
                            //    else
                            //    {
                            //        listName = "IR03DI";
                            //    }
                            //}

                            int ActionRequiredIndex = 0;

                            if (!String.IsNullOrEmpty(Convert.ToString(this.ActionRequired_Unit_ddl.SelectedItem.Text)))
                                ActionRequiredIndex = this.ActionRequired_Unit_ddl.SelectedIndex;

                            if (ActionRequiredIndex == 0)
                            {
                                listName = "IR01DI";
                            }
                            else
                            {
                                listName = "IR03DI";
                            }

                            AddDummyEntryInIRDIListToAssignTaskToTeam(oWebSite, imiItem, listName);


                            String TeamLead = Convert.ToString(imiItem["TeamLead"]);

                            SPUser user = Utility.GetUser(oWebSite, TeamLead);
                            message.To = user.Email;
                            Email.SendEmail(message);



                            String s = Convert.ToString(imiItem["TeamMembers"]);

                            String[] TeamMembers = s.Split(',');

                            foreach (String member in TeamMembers)
                            {
                                user = Utility.GetUser(oWebSite, member);
                                message.To = user.Email;
                                Email.SendEmail(message);

                            }


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->SendEmailToTeamLeadAndMembers)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }
        protected void AddDummyEntryInIRDIListToAssignTaskToTeam(SPWeb oWebSite, SPListItem FRItem, string listName)
        {

            try
            {
                if (oWebSite != null && !String.IsNullOrEmpty(listName))
                {
                    SPList list = oWebSite.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oWebSite.Url, listName));

                    if (list != null)
                    {
                        SPListItem spListItemIR = list.Items.Add();

                        if (spListItemIR != null)
                        {

                            string teamMembers = null;
                            string teamLead = null;

                            if (!String.IsNullOrEmpty(Convert.ToString(FRItem["TeamLead"])))
                            {
                                teamLead = Convert.ToString(FRItem["TeamLead"]);
                            }

                            if (!String.IsNullOrEmpty(Convert.ToString(FRItem["TeamMembers"])))
                            {
                                teamMembers = Convert.ToString(FRItem["TeamMembers"]);
                            }

                            StringBuilder sbAssignee = new StringBuilder();
                            StringBuilder sbAssigneeEmail = new StringBuilder();

                            SPUser spUser = Utility.GetUser(oWebSite, teamLead);

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
                                spUser = Utility.GetUser(oWebSite, member);

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

                            spListItemIR["IsSavedAsDraft"] = true;
                            spListItemIR["IsSubmitted"] = false;
                            spListItemIR["IsApproved"] = false;
                            spListItemIR["IsClosed"] = false;

                            if (!String.IsNullOrEmpty(Convert.ToString(FRItem["IR1ID"])))
                            {
                                String IR1IDStr = Convert.ToString(FRItem["IR1ID"]);

                                if (!String.IsNullOrEmpty(IR1IDStr))
                                {
                                    int IR01ID = Convert.ToInt32(IR1IDStr);

                                    string spListNameIR1 = "IR-1";

                                    SPList spListIR1 = oWebSite.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oWebSite.Url, spListNameIR1));

                                    if (spListIR1 != null)
                                    {
                                        SPListItem spListItemIR01 = spListIR1.GetItemById(IR01ID);

                                        if (!String.IsNullOrEmpty(Convert.ToString(spListItemIR01["TitleOfIncident"])))
                                        {
                                            spListItemIR["IncidentTitle"] = Convert.ToString(spListItemIR01["TitleOfIncident"]);
                                        }
                                    }
                                }
                            }

                            if (!String.IsNullOrEmpty(Convert.ToString(FRItem["DescriptionOfIncident"])))
                            {
                                spListItemIR["IncidentDescription"] = Convert.ToString(FRItem["DescriptionOfIncident"]);
                            }

                            spListItemIR["FlashReportID"] = FRItem["ID"];

                            oWebSite.AllowUnsafeUpdates = true;
                            spListItemIR.Update();
                            oWebSite.AllowUnsafeUpdates = false;
                        }
                    }


                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->AddDummyEntryInIRDIListToAssignTaskToTeam)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }

        protected void btnIRLink_Click(object sender, EventArgs e)
        {

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                string redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");

                DisableControls(true);
                if (!String.IsNullOrEmpty(redirectUrl))
                {
                    Page.Response.Redirect(redirectUrl, false);
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOnJobForm->btnIRLink_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

            }
        }

    }
}
