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


namespace SL.FG.PFL.WebParts.IRDForm
{
    public partial class IRDFormUserControl : UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {


                    String FRID = Page.Request.QueryString["FRID"];

                    FillDropdowns();
                    disableControls();
                    LoadPageOnUserBases(FRID);

                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->Page_Load)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

            }

        }

        private void LoadPageOnUserBases(String FRID)
        {
            String IR5ID = null;
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oSPWeb = oSPsite.OpenWeb())
                    {
                        if (!String.IsNullOrEmpty(FRID))
                        {
                            IR5ID = Check1StFromDraft(oSPWeb, FRID);
                            String Status = CheckStatus(oSPWeb, IR5ID);

                            this.hdnIR05ID.Value = IR5ID;

                            if (IR5ID != null)
                            {
                                if (CheckCurrentUserIsAdmin(oSPWeb))
                                {
                                    LoadPageFromDraft(IR5ID);
                                    disableFieldsForAdmin();
                                }
                                else if (CheckAssignee(oSPWeb, IR5ID))
                                {
                                    if (CheckCurrentUserIsFRTeamLead(oSPWeb, FRID) && Status != null && Status.Equals("Inprogress", StringComparison.OrdinalIgnoreCase))
                                    {
                                        LoadPageFromDraft(IR5ID);
                                        EnableFieldsForTeamLead();

                                    }
                                    else if (CheckCurrentUserIsFRTeamMembers(oSPWeb, FRID) && Status != null && Status.Equals("Inprogress", StringComparison.OrdinalIgnoreCase))
                                    {
                                        LoadPageFromDraft(IR5ID);

                                    }
                                    else if (CheckCurrentUserIsFRApprovingAuthority(oSPWeb, FRID) && Status != null && Status.Equals("Submitted", StringComparison.OrdinalIgnoreCase))
                                    {
                                        LoadPageFromDraft(IR5ID);
                                        disableFieldsForApprovingAuthority();
                                    }
                                    else if (CheckCurrentUserIsHSEMember(oSPWeb) && Status != null && Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                                    {
                                        LoadPageFromDraft(IR5ID);
                                        disableFieldsForHSE();
                                    }
                                    else if (Status != null && Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                                    {

                                        this.btnSave.Visible = false;
                                        this.btnSaveAsDraft.Visible = false;
                                        this.btnApprovingAuthoritySave.Visible = false;
                                        this.btnApprovingAuthorityApprove.Visible = false;
                                        this.btnApprovingAuthorityDisApprove.Visible = false;
                                        this.btnHSEApprove.Visible = false;

                                    }
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
                                //   LoadValuesFromIR1ANDFlashReport(FRID);
                                this.btnSave.Visible = false;
                                this.btnSaveAsDraft.Visible = false;
                                this.btnApprovingAuthoritySave.Visible = false;
                                this.btnApprovingAuthorityApprove.Visible = false;
                                this.btnApprovingAuthorityDisApprove.Visible = false;
                                this.btnHSEApprove.Visible = false;

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->LoadPageOnUserBases)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

            }
        }

        private void disableControls()
        {
            this.DateOfIncident_dtc.Enabled = false;
            this.TimeOfIncident_dtc.Enabled = false;

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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->CheckCurrentUserIsAdmin)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return IsSupervisor;
        }

        private void disableFieldsForHSE()
        {
            this.btnSaveAsDraft.Visible = false;
            this.btnSave.Visible = false;
            this.btnHSEApprove.Visible = true;

            this.approvalDate_dtc.Enabled = true;
            this.HSEDepartment_div.Visible = true;

        }

        private void EnableFieldsForTeamLead()
        {
            this.btnSave.Visible = true;

        }

        private void disableFieldsForAdmin()
        {
            this.btnSaveAsDraft.Visible = false;
            this.btnApprovingAuthoritySave.Visible = true;

        }

        private void disableFieldsForApprovingAuthority()
        {
            this.btnSaveAsDraft.Visible = false;
            this.btnApprovingAuthoritySave.Visible = true;
            this.btnApprovingAuthorityApprove.Visible = true;
            this.btnApprovingAuthorityDisApprove.Visible = true;
            this.approvalDate_div.Attributes.Add("style", "display: normal");

            this.approvalDate_dtc.Enabled = true;
        }

        private void LoadTargetDateValuesFromFlashReportInToHiddnField(SPWeb oSPWeb, String FRID)
        {
            try
            {

                if (!String.IsNullOrEmpty(FRID))
                {

                    SPListItemCollection FRInfoList = oSPWeb.Lists["FlashReportOff"].Items;

                    if (FRInfoList != null)
                    {
                        SPListItem FRItem = FRInfoList.GetItemById(Convert.ToInt32(FRID));
                        if (FRItem != null)
                        {

                            if (FRItem["TargetDate"] != null && !String.IsNullOrEmpty(Convert.ToString(FRItem["TargetDate"])))
                            {
                                DateTime Date;
                                bool bValid = DateTime.TryParse(Convert.ToString(FRItem["TargetDate"]), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out Date);

                                if (!bValid)
                                {
                                    Date = Convert.ToDateTime(FRItem["TargetDate"]);
                                }
                                this.FRTargetDate_dtc.SelectedDate = Date;

                            }


                        }
                    }

                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->LoadTargetDateValuesFromFlashReportInToHiddnField)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }


        private void LoadValuesFromIR1ANDFlashReport(String FRID)
        {
            try
            {

                if (!String.IsNullOrEmpty(FRID))
                {

                    using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                    {
                        using (SPWeb oSPWeb = oSPsite.OpenWeb())
                        {

                            SPListItemCollection FRInfoList = oSPWeb.Lists["FlashReportOff"].Items;
                            SPListItemCollection IRInfoList = oSPWeb.Lists["IR-1-Off"].Items;
                            String IRID = null;
                            Boolean IsInjury = false;
                            if (FRInfoList != null)
                            {
                                SPListItem FRItem = FRInfoList.GetItemById(Convert.ToInt32(FRID));
                                if (FRItem != null)
                                {
                                    if (!String.IsNullOrEmpty(Convert.ToString(FRItem["IRID"])))
                                    {
                                        IRID = Convert.ToString(FRItem["IRID"]);
                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(FRItem["DateOfIncident"])))

                                        this.DateOfIncident_dtc.SelectedDate = Convert.ToDateTime(FRItem["DateOfIncident"]);

                                    if (!String.IsNullOrEmpty(Convert.ToString(FRItem["TimeOfIncident"])))

                                        this.TimeOfIncident_dtc.SelectedDate = Convert.ToDateTime(FRItem["TimeOfIncident"]);


                                    if (!String.IsNullOrEmpty(Convert.ToString(FRItem["Unit_x002f_Section"])))
                                    {

                                        this.Unit_Area_ddl.Items.FindByValue(Convert.ToString(FRItem["Unit_x002f_Section"])).Selected = true;

                                        this.Unit_Area_hdn.Value = Convert.ToString(FRItem["Unit_x002f_Section"]);
                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(FRItem["DescriptionOfIncident"])))

                                        this.Description_ta.Value = Convert.ToString(FRItem["DescriptionOfIncident"]);

                                    if (!String.IsNullOrEmpty(Convert.ToString(FRItem["ActionTaken"])))

                                        this.ActionTaken_ta.Value = Convert.ToString(FRItem["ActionTaken"]);

                                    if (!String.IsNullOrEmpty(Convert.ToString(FRItem["ApprovingAuthority"])))

                                        this.approvedBy_tf.Value = Convert.ToString(FRItem["ApprovingAuthority"]);

                                }

                            }
                            if (IRInfoList != null)
                            {
                                SPListItem IRItem = IRInfoList.GetItemById(Convert.ToInt32(IRID));
                                if (IRItem != null)
                                {
                                    if (!String.IsNullOrEmpty(Convert.ToString(IRItem["IncidentCategory"])))
                                    {
                                        String s = Convert.ToString(IRItem["IncidentCategory"]);

                                        string[] IncidentCategoryItem = s.Split(',');

                                        this.IncidentCategory_hdn.Value = s;

                                        this.IncidentCategory_ta.Value = s;

                                        foreach (string Item in IncidentCategoryItem)
                                        {
                                            this.IncidentCategory_ddl.Items.FindByValue(Item).Selected = true;

                                            if (Item == "Injury")
                                                IsInjury = true;

                                        }

                                    }

                                    if (IsInjury)
                                        this.Injury_div.Attributes.Add("Style", "display:normal");
                                    else
                                        this.Violation_div.Attributes.Add("Style", "display:normal");



                                    if (!String.IsNullOrEmpty(Convert.ToString(IRItem["InjuryCategory"])))
                                    {
                                        String s = Convert.ToString(IRItem["InjuryCategory"]);

                                        string[] InjuryCategoryItem = s.Split(',');

                                        this.InjuryCategory_hdn.Value = s;

                                        this.InjuryCategory_ta.Value = s;
                                        foreach (string Item in InjuryCategoryItem)
                                        {
                                            this.InjuryCategory_ddl.Items.FindByValue(Item).Selected = true;

                                        }

                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(IRItem["EmployeeType"])))

                                        this.EmployeeType_ddl.Value = Convert.ToString(IRItem["EmployeeType"]);

                                    if (!String.IsNullOrEmpty(Convert.ToString(IRItem["TitleOfIncident"])))

                                        this.Title_tf.Value = Convert.ToString(IRItem["TitleOfIncident"]);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->LoadValuesFromIR1ANDFlashReport)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        private void FillDropdowns()
        {
            using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
            {
                using (SPWeb oSPWeb = oSPsite.OpenWeb())
                {
                    FillDepartment(oSPWeb);
                    FillSection(oSPWeb);
                    FillArea(oSPWeb);
                    FillIncidentCategory(oSPWeb);
                    FillInjuryCategory(oSPWeb);
                    String User = oSPWeb.CurrentUser.LoginName;
                    String[] Name = User.Split('|');
                    if (Name.Length > 1)
                        this.EmployeeName_tf.Value = Name[1];
                }
            }
        }

        private void FillDepartment(SPWeb oSPWeb)
        {


            try
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

                this.Department_Violation_ddl.DataSource = spListItems;
                this.Department_Violation_ddl.DataTextField = "Title";
                this.Department_Violation_ddl.DataValueField = "Title";
                this.Department_Violation_ddl.DataBind();

                this.Department_Violation_ddl.Items.Insert(0, new ListItem("Please Select", "0"));

                this.Department_Injury_ddl.DataSource = spListItems;
                this.Department_Injury_ddl.DataTextField = "Title";
                this.Department_Injury_ddl.DataValueField = "Title";
                this.Department_Injury_ddl.DataBind();

                this.Department_Injury_ddl.Items.Insert(0, new ListItem("Please Select", "0"));

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->FillDepartment)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        private void FillSection(SPWeb oSPWeb)
        {


            try
            {

                string listName = "Section";

                // Fetch the List
                SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));

                SPQuery query = new SPQuery();
                SPListItemCollection spListItems;
                // Include only the fields you will use.
                query.ViewFields = "<FieldRef Name='ID'/><FieldRef Name='Title'/>";
                query.ViewFieldsOnly = true;
                //query.RowLimit = 200; // Only select the top 200.
                StringBuilder sb = new StringBuilder();
                sb.Append("<OrderBy Override='True';><FieldRef Name='Title'/></OrderBy>");
                query.Query = sb.ToString();
                spListItems = spList.GetItems(query);

                this.Section_Violation_ddl.DataSource = spListItems;
                this.Section_Violation_ddl.DataTextField = "Title";
                this.Section_Violation_ddl.DataValueField = "Title";
                this.Section_Violation_ddl.DataBind();
                this.Section_Violation_ddl.Items.Insert(0, new ListItem("Please Select", "0"));


                this.Section_Injury_ddl.DataSource = spListItems;
                this.Section_Injury_ddl.DataTextField = "Title";
                this.Section_Injury_ddl.DataValueField = "Title";
                this.Section_Injury_ddl.DataBind();
                this.Section_Injury_ddl.Items.Insert(0, new ListItem("Please Select", "0"));

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->FillSection)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
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
                sb.Append("<OrderBy Override='True';><FieldRef Name='Title'/></OrderBy>");
                query.Query = sb.ToString();
                spListItems = spList.GetItems(query);

                this.Unit_Area_ddl.DataSource = spListItems;
                this.Unit_Area_ddl.DataTextField = "Title";
                this.Unit_Area_ddl.DataValueField = "Title";
                this.Unit_Area_ddl.DataBind();

                this.Unit_Area_ddl.Items.Insert(0, new ListItem("Please Select", "0"));

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->FillArea)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        private void FillIncidentCategory(SPWeb oSPWeb)
        {


            try
            {
                string listName = "IncidentCategory";

                // Fetch the List
                SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));

                SPQuery query = new SPQuery();
                SPListItemCollection spListItems;
                // Include only the fields you will use.
                query.ViewFields = "<FieldRef Name='ID'/><FieldRef Name='Title'/>";
                query.ViewFieldsOnly = true;
                //query.RowLimit = 200; // Only select the top 200.
                StringBuilder sb = new StringBuilder();
                sb.Append("<OrderBy Override='True';><FieldRef Name='Title'/></OrderBy>");
                query.Query = sb.ToString();
                spListItems = spList.GetItems(query);

                this.IncidentCategory_ddl.DataSource = spListItems;
                this.IncidentCategory_ddl.DataTextField = "Title";
                this.IncidentCategory_ddl.DataValueField = "Title";
                this.IncidentCategory_ddl.DataBind();

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->FillIncidentCategory)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }

        private void FillInjuryCategory(SPWeb oSPWeb)
        {
            try
            {
                string listName = "InjuryCategory";

                // Fetch the List
                SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));

                SPQuery query = new SPQuery();
                SPListItemCollection spListItems;
                // Include only the fields you will use.
                query.ViewFields = "<FieldRef Name='ID'/><FieldRef Name='Title'/>";
                query.ViewFieldsOnly = true;
                //query.RowLimit = 200; // Only select the top 200.
                StringBuilder sb = new StringBuilder();
                sb.Append("<OrderBy Override='True';><FieldRef Name='Title'/></OrderBy>");
                query.Query = sb.ToString();
                spListItems = spList.GetItems(query);

                this.InjuryCategory_ddl.DataSource = spListItems;
                this.InjuryCategory_ddl.DataTextField = "Title";
                this.InjuryCategory_ddl.DataValueField = "Title";
                this.InjuryCategory_ddl.DataBind();
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->FillInjuryCategory)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                String redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");


                if (!String.IsNullOrEmpty(redirectUrl))
                {
                    Page.Response.Redirect(redirectUrl, false);
                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->btnCancel_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string redirectUrl = null;
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {

                        if (oWebSite != null)
                        {
                            //SPListItemCollection IR_1infoList = oWebSite.Lists["IR-1"].Items;

                            string listName = "IR-5-Off";

                            // Fetch the List
                            SPList list = oWebSite.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oWebSite.Url, listName));

                            //Optimize
                            //String IR5ID = Page.Request.QueryString["IR5ID"];
                            //int IR5ItemID = Convert.ToInt32(IR5ID);

                            String FRID = Page.Request.QueryString["FRID"];
                            int FRItemID = Convert.ToInt32(FRID);



                            if (FRItemID != 0 && list != null)
                            {

                                SPListItem spListItem = null;

                                String IR5_ID = Check1StFromDraft(oWebSite, FRID);
                                int IR5_ItemID = Convert.ToInt32(IR5_ID);

                                if (IR5_ID != null)
                                    spListItem = list.Items.GetItemById(IR5_ItemID);
                                else
                                    spListItem = list.Items.Add();


                                if (spListItem != null)
                                {

                                    var pattern1 = new[] { "~|~" };
                                    var pattern2 = new[] { "*|*" };

                                    bool isSaved = false;

                                    string recommendationListStr = this.hdnRecommendationList.Value;

                                    var recommendationList = this.GetFormattedRecommendations(recommendationListStr, pattern1, pattern2);


                                    isSaved = UpdateIR_5Values(recommendationList, spListItem, false, oWebSite, false, false, false, pattern1, pattern2);


                                    if (isSaved)
                                    {
                                        redirectUrl = Utility.GetRedirectUrl("IRR01DI_SaveAsDraft_Redirect");

                                        if (String.IsNullOrEmpty(redirectUrl))
                                        {
                                            redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");
                                        }

                                        //     DisableControls(true);

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

                                            //  DisableControls(true);
                                        }

                                    }


                                }
                            }
                            //Optimize
                            //else if (IR5ItemID != 0 && list != null)
                            //{
                            //    SPListItem spListItem = list.Items.GetItemById(IR5ItemID);

                            //    if (spListItem != null)
                            //    {

                            //        UpdateIR_5Values(spListItem, false, oWebSite);

                            //    }

                            //}
                        }

                    }

                    redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");

                    DisableControls(true);
                    if (!String.IsNullOrEmpty(redirectUrl))
                    {
                        Page.Response.Redirect(redirectUrl, false);
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->btnSave_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }

        protected void btnSaveAsDraft_Click(object sender, EventArgs e)
        {
            string redirectUrl = null;
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {
                        if (oWebSite != null)
                        {

                            string listName = "IR-5-Off";


                            SPList list = oWebSite.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oWebSite.Url, listName));

                            String FRID = Page.Request.QueryString["FRID"];
                            int FRItemID = Convert.ToInt32(FRID);


                            String IR5_ID = Check1StFromDraft(oWebSite, FRID);
                            int IR5_ItemID = Convert.ToInt32(IR5_ID);

                            if (FRItemID != 0 && list != null)
                            {
                                SPListItem spListItem = null;

                                if (IR5_ID != null)
                                    spListItem = list.Items.GetItemById(IR5_ItemID);
                                else
                                    spListItem = list.Items.Add();


                                if (spListItem != null)
                                {

                                    var pattern1 = new[] { "~|~" };
                                    var pattern2 = new[] { "*|*" };

                                    bool isSaved = false;

                                    string recommendationListStr = this.hdnRecommendationList.Value;

                                    var recommendationList = this.GetFormattedRecommendations(recommendationListStr, pattern1, pattern2);


                                    isSaved = UpdateIR_5Values(recommendationList, spListItem, true, oWebSite, false, false, false, pattern1, pattern2);


                                    if (isSaved)
                                    {
                                        redirectUrl = Utility.GetRedirectUrl("IRR01DI_SaveAsDraft_Redirect");

                                        if (String.IsNullOrEmpty(redirectUrl))
                                        {
                                            redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");
                                        }

                                        //     DisableControls(true);

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

                                            //  DisableControls(true);
                                        }

                                    }

                                }

                                redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");

                                DisableControls(true);
                                if (!String.IsNullOrEmpty(redirectUrl))
                                {
                                    Page.Response.Redirect(redirectUrl, false);
                                }
                            }

                        }


                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->btnSaveAsDraft_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }

        protected void btnApprovingAuthoritySave_Click(object sender, EventArgs e)
        {
            string redirectUrl = null;

            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {

                        if (oWebSite != null)
                        {
                            //SPListItemCollection IR_1infoList = oWebSite.Lists["IR-1"].Items;

                            string listName = "IR-5-Off";

                            // Fetch the List
                            SPList list = oWebSite.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oWebSite.Url, listName));

                            //Optimize
                            //String IR5ID = Page.Request.QueryString["IR5ID"];
                            //int IR5ItemID = Convert.ToInt32(IR5ID);

                            String FRID = Page.Request.QueryString["FRID"];
                            int FRItemID = Convert.ToInt32(FRID);



                            if (FRItemID != 0 && list != null)
                            {

                                SPListItem spListItem = null;

                                String IR5_ID = Check1StFromDraft(oWebSite, FRID);
                                int IR5_ItemID = Convert.ToInt32(IR5_ID);

                                if (IR5_ID != null)
                                    spListItem = list.Items.GetItemById(IR5_ItemID);
                                else
                                    spListItem = list.Items.Add();


                                if (spListItem != null)
                                {

                                    var pattern1 = new[] { "~|~" };
                                    var pattern2 = new[] { "*|*" };

                                    bool isSaved = false;

                                    string recommendationListStr = this.hdnRecommendationList.Value;

                                    var recommendationList = this.GetFormattedRecommendations(recommendationListStr, pattern1, pattern2);


                                    isSaved = UpdateIR_5Values(recommendationList, spListItem, false, oWebSite, true, false, false, pattern1, pattern2);


                                    if (isSaved)
                                    {
                                        redirectUrl = Utility.GetRedirectUrl("IRR01DI_SaveAsDraft_Redirect");

                                        if (String.IsNullOrEmpty(redirectUrl))
                                        {
                                            redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");
                                        }

                                        //     DisableControls(true);

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

                                            //  DisableControls(true);
                                        }

                                    }

                                }
                            }

                        }
                        redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");

                        DisableControls(true);
                        if (!String.IsNullOrEmpty(redirectUrl))
                        {
                            Page.Response.Redirect(redirectUrl, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->btnApprovingAuthoritySave_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }

        protected void btnHSEApprove_Click(object sender, EventArgs e)
        {
            String redirectUrl = null;
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {
                        if (oWebSite != null)
                        {


                            string listName = "IR-5-Off";


                            SPList list = oWebSite.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oWebSite.Url, listName));

                            String FRID = Page.Request.QueryString["FRID"];
                            int FRItemID = Convert.ToInt32(FRID);


                            if (FRItemID != 0 && list != null)
                            {
                                SPListItem spListItem = null;
                                String IR5_ID = Check1StFromDraft(oWebSite, FRID);
                                int IR5_ItemID = Convert.ToInt32(IR5_ID);



                                if (IR5_ID != null)
                                    spListItem = list.Items.GetItemById(IR5_ItemID);




                                if (spListItem != null)
                                {

                                    var pattern1 = new[] { "~|~" };
                                    var pattern2 = new[] { "*|*" };

                                    bool isSaved = false;

                                    string recommendationListStr = this.hdnRecommendationList.Value;

                                    var recommendationList = this.GetFormattedRecommendations(recommendationListStr, pattern1, pattern2);


                                    isSaved = UpdateIR_5Values(recommendationList, spListItem, false, oWebSite, false, false, true, pattern1, pattern2);


                                    if (isSaved)
                                    {
                                        redirectUrl = Utility.GetRedirectUrl("IRR01DI_SaveAsDraft_Redirect");

                                        if (String.IsNullOrEmpty(redirectUrl))
                                        {
                                            redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");
                                        }

                                        //     DisableControls(true);

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

                                            //  DisableControls(true);
                                        }

                                    }

                                }

                                redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");

                                DisableControls(true);
                                if (!String.IsNullOrEmpty(redirectUrl))
                                {
                                    Page.Response.Redirect(redirectUrl, false);
                                }


                                //spListItem["Assignee"] = Utility.GetValueByKey("MasterGroup");

                                //String User = oWebSite.CurrentUser.LoginName;
                                //String[] Name = User.Split('|');
                                //if (Name.Length > 1)
                                //    spListItem["SubmittedBy"] = Name[1];

                                //spListItem["Status"] = "Complete";

                                //spListItem.Update();



                                SendHSEEmailToTeamLeadAndMembers(spListItem);

                                SendHSEEmailToApprovingAuthority(spListItem);

                                SendIR05AcceptanceEmailToHSE(spListItem);

                            }

                        }
                        redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");

                        DisableControls(true);
                        if (!String.IsNullOrEmpty(redirectUrl))
                        {
                            Page.Response.Redirect(redirectUrl, false);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->btnHSEApprove_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }


        //        protected void btnHSEDisapprove_Click(object sender, EventArgs e)
        //        {
        //            try
        //            {
        //                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
        //                {
        //                    using (SPWeb oWebSite = oSPsite.OpenWeb())
        //                    {
        //                        if (oWebSite != null)
        //                        {
        //                            //SPListItemCollection IR_1infoList = oWebSite.Lists["IR-1"].Items;

        //                            string listName = "IR-5-Off";

        //                            // Fetch the List
        //                            SPList list = oWebSite.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oWebSite.Url, listName));

        ////Optimized
        //                            //String IR5ID = Page.Request.QueryString["IR5ID"];
        //                            //int IR5ItemID = Convert.ToInt32(IR5ID);

        //                            String FRID = Page.Request.QueryString["FRID"];
        //                            int FRItemID = Convert.ToInt32(FRID);



        //                            if (FRItemID != 0 && list != null)
        //                            {
        //                                SPListItem spListItem = null;

        //                                String IR5_ID = Check1StFromDraft(oWebSite, FRID);
        //                                int IR5_ItemID = Convert.ToInt32(IR5_ID);

        //                                if (IR5_ID != null)
        //                                    spListItem = list.Items.GetItemById(IR5_ItemID);
        //                                //else
        //                                //    spListItem = list.Items.GetItemById(FRItemID);

        //                                String TeamMembers = GetAllFRTeamMembers(oWebSite, FRID);

        //                                spListItem["Assignee"] = TeamMembers;

        //                                String User = oWebSite.CurrentUser.LoginName;
        //                                String[] Name = User.Split('|');
        //                                spListItem["SubmittedBy"] = Name[1];



        //                                spListItem.Update();

        //                                SendRejectionEmailToTeamLeadAndMembers(spListItem);
        //                            }
        ////Optimized
        //                            //else if (IR5ItemID != 0 && list != null)
        //                            //{
        //                            //    SPListItem spListItem = list.Items.GetItemById(IR5ItemID);

        //                            //    String FR_ID = GetFRID(oWebSite, IR5_ID);

        //                            //    String TeamMembers = GetAllFRTeamMembers(oWebSite, FR_ID);

        //                            //    spListItem["Assignee"] = TeamMembers;
        //                            //    spListItem.Update();

        //                            //}
        //                        }

        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(->SaveMSA)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
        //            }
        //        }

        protected void btnApprovingAuthorityApprove_Click(object sender, EventArgs e)
        {
            String redirectUrl = null;
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {

                        if (oWebSite != null)
                        {
                            //SPListItemCollection IR_1infoList = oWebSite.Lists["IR-1"].Items;

                            string listName = "IR-5-Off";

                            // Fetch the List
                            SPList list = oWebSite.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oWebSite.Url, listName));

                            //Optimize
                            //String IR5ID = Page.Request.QueryString["IR5ID"];
                            //int IR5ItemID = Convert.ToInt32(IR5ID);

                            String FRID = Page.Request.QueryString["FRID"];
                            int FRItemID = Convert.ToInt32(FRID);



                            if (FRItemID != 0 && list != null)
                            {

                                SPListItem spListItem = null;

                                String IR5_ID = Check1StFromDraft(oWebSite, FRID);
                                int IR5_ItemID = Convert.ToInt32(IR5_ID);

                                if (IR5_ID != null)
                                    spListItem = list.Items.GetItemById(IR5_ItemID);
                                else
                                    spListItem = list.Items.Add();


                                if (spListItem != null)
                                {

                                    var pattern1 = new[] { "~|~" };
                                    var pattern2 = new[] { "*|*" };

                                    bool isSaved = false;

                                    string recommendationListStr = this.hdnRecommendationList.Value;

                                    var recommendationList = this.GetFormattedRecommendations(recommendationListStr, pattern1, pattern2);


                                    isSaved = UpdateIR_5Values(recommendationList, spListItem, false, oWebSite, false, true, false, pattern1, pattern2);


                                    if (isSaved)
                                    {
                                        redirectUrl = Utility.GetRedirectUrl("IRR01DI_SaveAsDraft_Redirect");

                                        if (String.IsNullOrEmpty(redirectUrl))
                                        {
                                            redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");
                                        }

                                        //     DisableControls(true);

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

                                            //  DisableControls(true);
                                        }

                                    }

                                }
                            }

                        }
                        redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");

                        DisableControls(true);
                        if (!String.IsNullOrEmpty(redirectUrl))
                        {
                            Page.Response.Redirect(redirectUrl, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(FlashReportOffJobForm->btnApprovingAuthorityApprove_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }

        protected void btnApprovingAuthorityDisapprove_Click(object sender, EventArgs e)
        {
            String redirectUrl = null;

            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {
                        if (oWebSite != null)
                        {
                            //SPListItemCollection IR_1infoList = oWebSite.Lists["IR-1"].Items;

                            string listName = "IR-5-Off";

                            // Fetch the List
                            SPList list = oWebSite.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oWebSite.Url, listName));

                            //Optimize
                            //String IR5ID = Page.Request.QueryString["IR5ID"];
                            //int IR5ItemID = Convert.ToInt32(IR5ID);

                            String FRID = Page.Request.QueryString["FRID"];
                            int FRItemID = Convert.ToInt32(FRID);



                            if (FRItemID != 0 && list != null)
                            {
                                SPListItem spListItem = null;

                                String IR5_ID = Check1StFromDraft(oWebSite, FRID);
                                int IR5_ItemID = Convert.ToInt32(IR5_ID);

                                if (IR5_ID != null)
                                    spListItem = list.Items.GetItemById(IR5_ItemID);

                                String TeamMembers = GetAllFRTeamMembers(oWebSite, FRID);

                                spListItem["Assignee"] = TeamMembers;

                                spListItem["Status"] = "Inprogress";

                                String User = oWebSite.CurrentUser.LoginName;
                                String[] Name = User.Split('|');
                                if (Name.Length > 1)
                                    spListItem["SubmittedBy"] = Name[1];

                                spListItem.Update();

                                SendRejectionEmailToTeamLeadTeamMembers(spListItem);
                            }
                            //Optimize
                            //else if (IR5ItemID != 0 && list != null)
                            //{
                            //    SPListItem spListItem = list.Items.GetItemById(IR5ItemID);

                            //    String FR_ID = GetFRID(oWebSite, IR5_ID);

                            //    String TeamMembers = GetAllFRTeamMembers(oWebSite, FR_ID);

                            //    spListItem["Assignee"] = TeamMembers;

                            //}
                        }
                        redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");

                        DisableControls(true);
                        if (!String.IsNullOrEmpty(redirectUrl))
                        {
                            Page.Response.Redirect(redirectUrl, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->btnApprovingAuthorityDisapprove_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }

        protected bool UpdateIR_5Values(List<IRRecommendationOnJob> recommendations, SPListItem IR5Item, Boolean IsSaveAsDraft, SPWeb oWebSite, Boolean ApprovingAuthority, Boolean ApprovingAuthorityApprove, Boolean HSEApprove, String[] pattern1, String[] pattern2)
        {
            bool isSaved = true;
            try
            {
                List<Message> lstMessage = null;


                SPUser currentUser = oWebSite.CurrentUser;

                string keyFindings = this.hdnKeyFindingsList.Value;
                string peopleInterviewed = this.hdnPeopleInterviewedList.Value;
                string rootCauses = this.hdnRootCausesList.Value;

                string p1 = "~|~";
                if (pattern1.Length > 0)
                {
                    p1 = pattern1[0];
                }

                if (IsValid_IRDI_Data(oWebSite, recommendations))
                {


                    if (IR5Item != null)
                    {



                        if (!String.IsNullOrEmpty(Convert.ToString(this.rvf_reportViewed_ta.Value)))

                            IR5Item["Reportviewed"] = this.rvf_reportViewed_ta.Value;

                        if (!String.IsNullOrEmpty(Convert.ToString(this.UM_HSE_Comments_ta.Value)))

                            IR5Item["UMHSEComments"] = this.UM_HSE_Comments_ta.Value;


                        if (!String.IsNullOrEmpty(Convert.ToString(this.approvedBy_tf.Value)))

                            IR5Item["ApprovalAuthority"] = this.approvedBy_tf.Value;



                        if (!String.IsNullOrEmpty(Convert.ToString(this.approvalDate_dtc.SelectedDate)))
                        {
                            DateTime date;
                            bool bValid = DateTime.TryParse(this.approvalDate_dtc.SelectedDate.ToShortDateString(), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);


                            if (bValid)
                                IR5Item["ApprovalDate"] = date;
                            else
                                IR5Item["ApprovalDate"] = Convert.ToDateTime(this.approvalDate_dtc.SelectedDate);
                        }





                        if (!String.IsNullOrEmpty(Convert.ToString(this.IncidentCategory_hdn.Value)))
                        {
                            IR5Item["IncidentCategory"] = Convert.ToString(this.IncidentCategory_hdn.Value);
                        }


                        if (!String.IsNullOrEmpty(Convert.ToString(this.InjuryCategory_hdn.Value)))
                        {
                            IR5Item["InjuryCategory"] = Convert.ToString(this.InjuryCategory_hdn.Value);

                        }




                        if (!String.IsNullOrEmpty(Convert.ToString(this.DateOfIncident_dtc.SelectedDate)))
                        {
                            DateTime date;
                            bool bValid = DateTime.TryParse(this.DateOfIncident_dtc.SelectedDate.ToShortDateString(), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);


                            if (bValid)
                                IR5Item["DateOfIncident"] = date;
                            else
                                IR5Item["DateOfIncident"] = Convert.ToDateTime(this.DateOfIncident_dtc.SelectedDate);
                        }


                        if (!String.IsNullOrEmpty(Convert.ToString(this.TimeOfIncident_dtc.SelectedDate)))
                            IR5Item["TimeOfIncident"] = this.TimeOfIncident_dtc.SelectedDate.ToShortTimeString();
                        else
                            IR5Item["TimeOfIncident"] = null;


                        if (!String.IsNullOrEmpty(Convert.ToString(this.Title_tf.Value)))
                            IR5Item["TitleOfIncident"] = Convert.ToString(this.Title_tf.Value);


                        if (!String.IsNullOrEmpty(Convert.ToString(this.Unit_Area_hdn.Value)))
                            IR5Item["Unit/Area"] = (Convert.ToString(this.Unit_Area_hdn.Value));


                        if (!String.IsNullOrEmpty(Convert.ToString(this.EmployeeType_ddl.SelectedIndex)) && this.EmployeeType_ddl.SelectedIndex > 0)
                            IR5Item["EmployeeType"] = Convert.ToString(this.EmployeeType_ddl.Items[this.EmployeeType_ddl.SelectedIndex]);

                        String User = oWebSite.CurrentUser.LoginName;
                        String[] Name = User.Split('|');
                        if (Name.Length > 1)
                            IR5Item["EmployeeName"] = Name[1];


                        if (this.ViolationBy_PeopleEditor.ResolvedEntities != null && this.ViolationBy_PeopleEditor.ResolvedEntities.Count > 0)
                        {
                            PickerEntity MOentity = (PickerEntity)this.ViolationBy_PeopleEditor.ResolvedEntities[0];

                            IR5Item["ViolationBy"] = MOentity.Claim.Value;
                        }

                        if (!String.IsNullOrEmpty(Convert.ToString(this.VehicleNo_tf.Value)))
                            IR5Item["VehicleNo"] = Convert.ToString(this.VehicleNo_tf.Value);

                        if (!String.IsNullOrEmpty(Convert.ToString(this.VehicleCategory_tf.Value)))
                            IR5Item["VehicleCategory"] = Convert.ToString(this.VehicleCategory_tf.Value);

                        if (!String.IsNullOrEmpty(Convert.ToString(this.TypeOfViolation_ddl.SelectedIndex)) && this.TypeOfViolation_ddl.SelectedIndex > 0)
                            IR5Item["TypeOfViolation"] = Convert.ToString(this.TypeOfViolation_ddl.Items[this.TypeOfViolation_ddl.SelectedIndex]);


                        if (!String.IsNullOrEmpty(Convert.ToString(this.Violation_Section_hdn.Value)))
                            IR5Item["EmpSection"] = (Convert.ToString(this.Violation_Section_hdn.Value));

                        if (!String.IsNullOrEmpty(Convert.ToString(this.Violation_Departmentt_hdn.Value)))
                            IR5Item["EmpDepartment"] = (Convert.ToString(this.Violation_Departmentt_hdn.Value));


                        if (this.NameOfInjured_PeopleEditor.ResolvedEntities != null && this.NameOfInjured_PeopleEditor.ResolvedEntities.Count > 0)
                        {
                            PickerEntity Injurdentity = (PickerEntity)this.NameOfInjured_PeopleEditor.ResolvedEntities[0];

                            IR5Item["NameOfInjured"] = Injurdentity.Claim.Value;
                        }

                        if (!String.IsNullOrEmpty(Convert.ToString(this.PNO_tf.Value)))
                            IR5Item["PNo"] = Convert.ToString(this.PNO_tf.Value);

                        if (!String.IsNullOrEmpty(Convert.ToString(this.OccupationTrade_tf.Value)))
                            IR5Item["OccupationTrade"] = Convert.ToString(this.OccupationTrade_tf.Value);

                        if (!String.IsNullOrEmpty(Convert.ToString(this.Description_ta.Value)))
                            IR5Item["IncidentDescription"] = Convert.ToString(this.Description_ta.Value);


                        if (!String.IsNullOrEmpty(Convert.ToString(this.ActionTaken_ta.Value)))
                            IR5Item["ActionTaken"] = Convert.ToString(this.ActionTaken_ta.Value);

                        if (!String.IsNullOrEmpty(Convert.ToString(this.Injury_Section_hdn.Value)))
                            IR5Item["EmpSection"] = (Convert.ToString(this.Injury_Section_hdn.Value));

                        if (!String.IsNullOrEmpty(Convert.ToString(this.Injury_Department_hdn.Value)))
                            IR5Item["EmpDepartment"] = (Convert.ToString(this.Injury_Department_hdn.Value));




                        User = oWebSite.CurrentUser.LoginName;
                        Name = User.Split('|');


                        String FRID = Page.Request.QueryString["FRID"];


                        IR5Item["FRID"] = Page.Request.QueryString["FRID"];

                        if (IsSaveAsDraft)
                        {
                            if (FRID != null)
                            {
                                if (CheckCurrentUserIsFRTeamLead(oWebSite, FRID) || CheckCurrentUserIsFRTeamMembers(oWebSite, FRID))
                                {
                                    IR5Item["Assignee"] = GetAllFRTeamMembers(oWebSite, FRID);
                                    if (Name.Length > 1)
                                        IR5Item["SubmittedBy"] = Name[1];
                                }

                            }




                            IR5Item["Status"] = "Inprogress";

                            IR5Item["AuditedBy"] = Name[1];

                            // IR5Item["SubmittedBy"] = Name[1];

                            IR5Item["IsSaveAsDraft"] = true;



                            IR5Item.Update();



                        }
                        else if (ApprovingAuthority)
                        {
                            IR5Item["AuditedBy"] = Name[1];

                            IR5Item.Update();
                        }
                        else if (ApprovingAuthorityApprove)
                        {
                            IR5Item["AuditedBy"] = Name[1];

                            IR5Item["Status"] = "Approved";

                            IR5Item.Update();

                            SendAcceptanceEmailToTeamLeadAndMembers(IR5Item);
                            SendEmailToHSE(IR5Item);
                        }
                        else if (HSEApprove)
                        {

                            IR5Item["Assignee"] = Utility.GetValueByKey("MasterGroup");

                            User = oWebSite.CurrentUser.LoginName;
                            Name = User.Split('|');
                            if (Name.Length > 1)
                                IR5Item["AuditedBy"] = Name[1];

                            IR5Item["Status"] = "Completed";

                            IR5Item.Update();

                        }
                        else
                        {
                            if (FRID != null)
                            {

                                if (CheckCurrentUserIsFRTeamLead(oWebSite, FRID))
                                {
                                    IR5Item["Assignee"] = GetFRApprovingAuthority(oWebSite, FRID);

                                    IR5Item["SubmittedBy"] = Name[1];

                                    IR5Item["Status"] = "Submitted";

                                    IR5Item.Update();

                                    SendSubmissionEmailToTeamMembers(IR5Item);

                                    SendEmailToApprovingAuthority(IR5Item);
                                    // SendEmailToApprovingAuthority(IR5Item);
                                }

                            }

                        }

                        if (currentUser != null)
                        {
                            if (!String.IsNullOrEmpty(currentUser.Email))
                            {
                                this.hdnSentFrom.Value = currentUser.Email;
                            }
                        }

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


                        int IR5ID = Convert.ToInt32(IR5Item["ID"]);



                        if (isSaved)
                        {
                            if (recommendationIds != null)
                            {
                                //In case of approved, no need to update recommendations(isApproved)
                                lstMessage = SaveRecommendations(oWebSite, recommendations, IR5ID, sentFrom, recommendationIds);
                            }
                            else
                            {
                                lstMessage = SaveRecommendations(oWebSite, recommendations, IR5ID, sentFrom);
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

                    }

                }

                if (!isSaved)
                {
                    bool statusRecommendations = FillRecommendationGrid(recommendations);

                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->UpdateIR_5Values)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return isSaved;
        }

        private String GetAllFRTeamMembers(SPWeb oWebSite, String FRID)
        {
            String TeamMembers = null;

            try
            {
                SPListItemCollection FRInfoList = oWebSite.Lists["FlashReportOff"].Items;
                if (FRInfoList != null)
                {
                    SPListItem FRItem = FRInfoList.GetItemById(Convert.ToInt32(FRID));
                    if (FRItem != null)
                    {
                        if (!String.IsNullOrEmpty(Convert.ToString(FRItem["TeamMembers"])) && !String.IsNullOrEmpty(Convert.ToString(FRItem["TeamLead"])))

                            TeamMembers = Convert.ToString(FRItem["TeamMembers"]) + "," + Convert.ToString(FRItem["TeamLead"]);

                    }
                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->GetAllFRTeamMembers)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return TeamMembers;

        }

        private String GetFRTeamLead(SPWeb oWebSite, String FRID)
        {
            String TeamLead = null;

            try
            {
                SPListItemCollection FRInfoList = oWebSite.Lists["FlashReportOff"].Items;
                if (FRInfoList != null)
                {
                    SPListItem FRItem = FRInfoList.GetItemById(Convert.ToInt32(FRID));
                    if (FRItem != null)
                    {
                        if (!String.IsNullOrEmpty(Convert.ToString(FRItem["TeamLead"])))

                            TeamLead = Convert.ToString(FRItem["TeamLead"]);

                    }
                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->GetFRTeamLead)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return TeamLead;

        }

        private String GetFRApprovingAuthority(SPWeb oWebSite, String FRID)
        {
            String ApprovingAuthority = null;

            try
            {
                SPListItemCollection FRInfoList = oWebSite.Lists["FlashReportOff"].Items;
                if (FRInfoList != null)
                {
                    SPListItem FRItem = FRInfoList.GetItemById(Convert.ToInt32(FRID));
                    if (FRItem != null)
                    {
                        if (!String.IsNullOrEmpty(Convert.ToString(FRItem["ApprovingAuthority"])))

                            ApprovingAuthority = Convert.ToString(FRItem["ApprovingAuthority"]);

                    }
                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->GetFRApprovingAuthority)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return ApprovingAuthority;

        }

        private String GetFRTeamMembers(SPWeb oWebSite, String FRID)
        {
            String TeamMembers = null;

            try
            {
                SPListItemCollection FRInfoList = oWebSite.Lists["FlashReportOff"].Items;
                if (FRInfoList != null)
                {
                    SPListItem FRItem = FRInfoList.GetItemById(Convert.ToInt32(FRID));
                    if (FRItem != null)
                    {
                        if (!String.IsNullOrEmpty(Convert.ToString(FRItem["TeamMembers"])))

                            TeamMembers = Convert.ToString(FRItem["TeamMembers"]);

                    }
                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->GetFRTeamMembers)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return TeamMembers;

        }

        private String GetFRID(SPWeb oWebSite, String IR5_ID)
        {
            String FRID = null;

            try
            {


                if (oWebSite != null)
                {
                    string listName = "IR-5-Off";


                    SPList spList = oWebSite.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oWebSite.Url, listName));

                    SPQuery query = new SPQuery();
                    SPListItemCollection spListItems;

                    query.ViewFields = "<FieldRef Name='FRID' /><FieldRef Name='ID' />";
                    query.ViewFieldsOnly = true;

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<Where><Eq><FieldRef Name='ID' /><Value Type='Text'>" + IR5_ID + "</Value></Eq></Where>");
                    query.Query = sb.ToString();
                    spListItems = spList.GetItems(query);



                    if (spListItems != null)
                    {
                        foreach (SPListItem IR_1item in spListItems)
                        {
                            FRID = IR_1item["FRID"].ToString();

                        }
                    }
                }


            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->GetFRID)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return FRID;
        }

        private Boolean CheckCurrentUserIsFRTeamMembers(SPWeb oSPWeb, String FRID)
        {

            Boolean IsTeaMembers = false;
            try
            {

                SPListItemCollection FlashReportInfoList = oSPWeb.Lists["FlashReportOff"].Items;
                if (FlashReportInfoList != null)
                {
                    SPListItem ListItem = FlashReportInfoList.GetItemById(Convert.ToInt32(FRID));

                    if (ListItem != null)
                    {


                        String User = oSPWeb.CurrentUser.LoginName;
                        String[] Name = User.Split('|');
                        String currentUser = Name[1];
                        if (Name.Length > 1)
                        {
                            String s = Convert.ToString(ListItem["TeamMembers"]);
                            if (s != null)
                            {
                                String[] AssigneeList = s.Split(',');

                                foreach (String person in AssigneeList)
                                {
                                    if (person == currentUser)
                                        IsTeaMembers = true;
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->CheckCurrentUserIsFRTeamMembers)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return IsTeaMembers;

        }

        private Boolean CheckCurrentUserIsFRTeamLead(SPWeb oSPWeb, String FRID)
        {

            Boolean IsTeamLead = false;
            try
            {

                SPListItemCollection FlashReportInfoList = oSPWeb.Lists["FlashReportOff"].Items;

                if (FlashReportInfoList != null)
                {
                    SPListItem ListItem = FlashReportInfoList.GetItemById(Convert.ToInt32(FRID));

                    if (ListItem != null)
                    {
                        String User = oSPWeb.CurrentUser.LoginName;
                        String[] Name = User.Split('|');
                        String currentUser = Name[1];
                        if (Name.Length > 1)
                        {
                            String TeamLead = Convert.ToString(ListItem["TeamLead"]);
                            if (TeamLead != null)
                            {
                                if (currentUser == TeamLead)
                                    IsTeamLead = true;


                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->CheckCurrentUserIsFRTeamLead)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return IsTeamLead;

        }

        private Boolean CheckCurrentUserIsFRApprovingAuthority(SPWeb oSPWeb, String FRID)
        {

            Boolean ApprovingAuthority = false;
            try
            {

                SPListItemCollection FlashReportInfoList = oSPWeb.Lists["FlashReportOff"].Items;

                if (FlashReportInfoList != null)
                {
                    SPListItem ListItem = FlashReportInfoList.GetItemById(Convert.ToInt32(FRID));

                    if (ListItem != null)
                    {
                        String User = oSPWeb.CurrentUser.LoginName;
                        String[] Name = User.Split('|');
                        String currentUser = Name[1];
                        if (Name.Length > 1)
                        {
                            String TeamLead = Convert.ToString(ListItem["ApprovingAuthority"]);
                            if (TeamLead != null)
                            {
                                if (currentUser == TeamLead)
                                    ApprovingAuthority = true;


                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->CheckCurrentUserIsFRApprovingAuthority)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return ApprovingAuthority;

        }

        private Boolean CheckCurrentUserIsHSEMember(SPWeb oSPWeb)
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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->CheckCurrentUserIsHSEMember)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return IsHSEMember;
        }

        private Boolean CheckAssignee(SPWeb oSPWeb, String IR5ID)
        {
            Boolean assignee = false;

            SPListItemCollection IR_1InfoList = oSPWeb.Lists["IR-5-Off"].Items;
            if (IR_1InfoList != null)
            {
                SPListItem IR5Item = IR_1InfoList.GetItemById(Convert.ToInt32(IR5ID));

                if (IR5Item != null)
                {
                    String User = oSPWeb.CurrentUser.LoginName;
                    String[] Name = User.Split('|');
                    String currentUser = Name[1];
                    if (Name.Length > 1)
                    {
                        String s = Convert.ToString(IR5Item["Assignee"]);
                        if (s != null)
                        {
                            String[] AssigneeList = s.Split(',');

                            foreach (String person in AssigneeList)
                            {
                                if (person == currentUser)
                                    assignee = true;
                            }
                        }
                    }
                }

            }
            return assignee;

        }

        private Boolean CheckHSEAssignee(SPWeb oSPWeb, String IR5ID)
        {
            Boolean assignee = false;

            try
            {

                SPListItemCollection FR_ListItems = oSPWeb.Lists["IR-5-Off"].Items;
                if (FR_ListItems != null)
                {

                    SPListItem ListItem = FR_ListItems.GetItemById(Convert.ToInt32(IR5ID));

                    String Assignees = Convert.ToString(ListItem["Assignee"]).ToLower();

                    String User = oSPWeb.CurrentUser.LoginName;
                    String[] Name = User.Split('|');
                    String currentUser = Name[1];

                    if (Assignees.Contains(currentUser.ToLower()))
                    {
                        assignee = true;

                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->CheckAssignee)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return assignee;

        }

        private Boolean CheckSubmittedBy(SPWeb oSPWeb, String IR5ID)
        {
            Boolean assignee = false;

            SPListItemCollection IR_1InfoList = oSPWeb.Lists["IR-5-Off"].Items;
            if (IR_1InfoList != null)
            {
                SPListItem IR5Item = IR_1InfoList.GetItemById(Convert.ToInt32(IR5ID));

                if (IR5Item != null)
                {
                    String User = oSPWeb.CurrentUser.LoginName;
                    String[] Name = User.Split('|');
                    String currentUser = Name[1];
                    if (Name.Length > 1)
                    {
                        String s = Convert.ToString(IR5Item["SubmittedBy"]);
                        if (s != null)
                        {
                            String[] AssigneeList = s.Split(',');

                            foreach (String person in AssigneeList)
                            {
                                if (person == currentUser)
                                    assignee = true;
                            }
                        }
                    }
                }

            }
            return assignee;

        }



        private String CheckStatus(SPWeb oWebSite, String IR5ID)
        {
            String Status = null;

            try
            {
                SPListItemCollection IR_1InfoList = oWebSite.Lists["IR-5-Off"].Items;
                if (IR_1InfoList != null)
                {
                    SPListItem IR5Item = IR_1InfoList.GetItemById(Convert.ToInt32(IR5ID));

                    if (IR5Item != null)
                    {

                        if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["Status"])))
                        {
                            Status = Convert.ToString(IR5Item["Status"]);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->CheckSubmittedBy)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return Status;
        }

        private String Check1StFromDraft(SPWeb oWebSite, String FRID)
        {
            String IR5ID = null;

            try
            {

                if (oWebSite != null)
                {
                    string listName = "IR-5-Off";

                    SPList spList = oWebSite.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oWebSite.Url, listName));

                    SPQuery query = new SPQuery();
                    SPListItemCollection spListItems;

                    query.ViewFields = "<FieldRef Name='FRID' /><FieldRef Name='ID' />";
                    query.ViewFieldsOnly = true;

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<Where><Eq><FieldRef Name='FRID' /><Value Type='Text'>" + FRID + "</Value></Eq></Where>");
                    query.Query = sb.ToString();
                    spListItems = spList.GetItems(query);



                    if (spListItems != null)
                    {
                        foreach (SPListItem IR_1item in spListItems)
                        {
                            IR5ID = IR_1item["ID"].ToString();

                        }
                    }
                }


            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->Check1StFromDraft)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return IR5ID;
        }

        private void LoadPageFromDraft(String IR5ID)
        {

            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oSPWeb = oSPsite.OpenWeb())
                    {
                        SPListItemCollection IR_1InfoList = oSPWeb.Lists["IR-5-Off"].Items;
                        if (IR_1InfoList != null)
                        {
                            SPListItem IR5Item = IR_1InfoList.GetItemById(Convert.ToInt32(IR5ID));

                            if (IR5Item != null)
                            {

                                Boolean IsInjury = false;


                                if (IR5Item.Attachments.Count > 0)
                                {
                                    foreach (String attachmentname in IR5Item.Attachments)
                                    {
                                        String attachmentAbsoluteURL =
                                        IR5Item.Attachments.UrlPrefix // gets the containing directory URL
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

                                String FRID = Page.Request.QueryString["FRID"];

                                LoadTargetDateValuesFromFlashReportInToHiddnField(oSPWeb, FRID);


                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["IncidentCategory"])))
                                {
                                    String s = Convert.ToString(IR5Item["IncidentCategory"]);

                                    string[] IncidentCategoryItem = s.Split(',');

                                    this.IncidentCategory_hdn.Value = s;

                                    this.IncidentCategory_ta.Value = s;


                                    foreach (string Item in IncidentCategoryItem)
                                    {
                                        this.IncidentCategory_ddl.Items.FindByValue(Item).Selected = true;

                                        if (Item == "Injury")
                                            IsInjury = true;

                                    }

                                }

                                if (IsInjury)
                                    this.Injury_div.Attributes.Add("Style", "display:normal");
                                else
                                    this.Violation_div.Attributes.Add("Style", "display:normal");


                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["InjuryCategory"])))
                                {
                                    String s = Convert.ToString(IR5Item["InjuryCategory"]);

                                    string[] InjuryCategoryItem = s.Split(',');

                                    this.InjuryCategory_hdn.Value = s;

                                    this.InjuryCategory_ta.Value = s;

                                    foreach (string Item in InjuryCategoryItem)
                                    {
                                        this.InjuryCategory_ddl.Items.FindByValue(Item).Selected = true;

                                    }

                                }

                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["EmployeeType"])))

                                    this.EmployeeType_ddl.Value = Convert.ToString(IR5Item["EmployeeType"]);




                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["DateOfIncident"])))
                                {
                                    DateTime Date;
                                    bool bValid = DateTime.TryParse(Convert.ToString(IR5Item["DateOfIncident"]), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out Date);

                                    if (!bValid)
                                    {
                                        Date = Convert.ToDateTime(IR5Item["DateOfIncident"]);
                                    }

                                    this.DateOfIncident_dtc.SelectedDate = Date;
                                }

                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["Status"])) && Convert.ToString(IR5Item["Status"]).Equals("Approved", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (IR5Item["ApprovalDate"] != null && !String.IsNullOrEmpty(Convert.ToString(IR5Item["ApprovalDate"])))
                                    {
                                        DateTime Date;
                                        bool bValid = DateTime.TryParse(Convert.ToString(IR5Item["ApprovalDate"]), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out Date);

                                        if (!bValid)
                                        {
                                            Date = Convert.ToDateTime(IR5Item["ApprovalDate"]);
                                        }

                                        this.approvalDate_dtc.SelectedDate = Date;
                                    }
                                }
                                else
                                {
                                    DateTime Date;
                                    bool bValid = DateTime.TryParse(Convert.ToString(DateTime.Now.Date), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out Date);

                                    if (!bValid)
                                    {
                                        Date = DateTime.Now.Date;
                                    }

                                    this.approvalDate_dtc.SelectedDate = Date;
                                }


                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["TimeOfIncident"])))

                                    this.TimeOfIncident_dtc.SelectedDate = Convert.ToDateTime(IR5Item["TimeOfIncident"]);


                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["Unit_x002f_Area"])))
                                {
                                    this.Unit_Area_ddl.Items.FindByValue(Convert.ToString(IR5Item["Unit_x002f_Area"])).Selected = true;
                                    this.Unit_Area_hdn.Value = Convert.ToString(IR5Item["Unit_x002f_Area"]);
                                }




                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["TitleOfIncident"])))

                                    this.Title_tf.Value = Convert.ToString(IR5Item["TitleOfIncident"]);


                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["IncidentDescription"])))

                                    this.Description_ta.Value = Convert.ToString(IR5Item["IncidentDescription"]);

                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["ActionTaken"])))

                                    this.ActionTaken_ta.Value = Convert.ToString(IR5Item["ActionTaken"]);


                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["EmployeeName"])))

                                    this.EmployeeName_tf.Value = Convert.ToString(IR5Item["EmployeeName"]);


                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["ViolationBy"])))
                                {
                                    PeopleEditor pe = new PeopleEditor();
                                    PickerEntity UserEntity = new PickerEntity();
                                    String username = Convert.ToString(IR5Item["ViolationBy"]);
                                    //get Spuser
                                    SPUser SPuser = Utility.GetUser(oSPWeb, username, null, 0);
                                    if (SPuser != null)
                                    {
                                        // CurrentUser is SPUser object
                                        UserEntity.DisplayText = SPuser.Name;
                                        UserEntity.Key = SPuser.LoginName;

                                        UserEntity = pe.ValidateEntity(UserEntity);

                                        // Add PickerEntity to People Picker control
                                        this.ViolationBy_PeopleEditor.AddEntities(new List<PickerEntity> { UserEntity });

                                    }

                                }

                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["VehicleNo"])))

                                    this.VehicleNo_tf.Value = Convert.ToString(IR5Item["VehicleNo"]);


                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["VehicleCategory"])))

                                    this.VehicleCategory_tf.Value = Convert.ToString(IR5Item["VehicleCategory"]);


                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["TypeOfViolation"])))

                                    this.TypeOfViolation_ddl.Value = Convert.ToString(IR5Item["TypeOfViolation"]);


                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["EmpSection"])))
                                {
                                    this.Section_Injury_ddl.Items.FindByValue(Convert.ToString(IR5Item["EmpSection"])).Selected = true;
                                    this.Injury_Section_hdn.Value = Convert.ToString(IR5Item["EmpSection"]);
                                }

                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["EmpSection"])))
                                {
                                    this.Section_Violation_ddl.Items.FindByValue(Convert.ToString(IR5Item["EmpSection"])).Selected = true;
                                    this.Violation_Section_hdn.Value = Convert.ToString(IR5Item["EmpSection"]);
                                }

                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["EmpDepartment"])))
                                {
                                    this.Department_Injury_ddl.Items.FindByValue(Convert.ToString(IR5Item["EmpDepartment"])).Selected = true;
                                    this.Injury_Department_hdn.Value = Convert.ToString(IR5Item["EmpDepartment"]);
                                }


                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["EmpDepartment"])))
                                {
                                    this.Department_Violation_ddl.Items.FindByValue(Convert.ToString(IR5Item["EmpDepartment"])).Selected = true;
                                    this.Violation_Departmentt_hdn.Value = Convert.ToString(IR5Item["EmpDepartment"]);
                                }


                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["NameOfInjured"])))
                                {
                                    PeopleEditor pe = new PeopleEditor();
                                    PickerEntity UserEntity = new PickerEntity();
                                    String username = Convert.ToString(IR5Item["NameOfInjured"]);
                                    //get Spuser
                                    SPUser SPuser = Utility.GetUser(oSPWeb, username, null, 0);
                                    if (SPuser != null)
                                    {
                                        // CurrentUser is SPUser object
                                        UserEntity.DisplayText = SPuser.Name;
                                        UserEntity.Key = SPuser.LoginName;

                                        UserEntity = pe.ValidateEntity(UserEntity);

                                        // Add PickerEntity to People Picker control
                                        this.NameOfInjured_PeopleEditor.AddEntities(new List<PickerEntity> { UserEntity });

                                        //   this.NameOfInjured_tf.Value = SPuser.Name;
                                    }

                                }

                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["PNo"])))
                                    this.PNO_tf.Value = Convert.ToString(IR5Item["PNo"]);


                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["OccupationTrade"])))
                                    this.OccupationTrade_tf.Value = Convert.ToString(IR5Item["OccupationTrade"]);





                                String UserName = oSPWeb.CurrentUser.LoginName;


                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["Reportviewed"])))

                                    this.rvf_reportViewed_ta.Value = Convert.ToString(IR5Item["Reportviewed"]);
                                else
                                    this.rvf_reportViewed_ta.Value = UserName;

                                this.rvf_reportViewed_ta.Disabled = true;

                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["UMHSEComments"])))

                                    this.UM_HSE_Comments_ta.Value = Convert.ToString(IR5Item["UMHSEComments"]);


                                if (!String.IsNullOrEmpty(Convert.ToString(IR5Item["ApprovalDate"])))
                                {
                                    DateTime Date;
                                    bool bValid = DateTime.TryParse(Convert.ToString(IR5Item["ApprovalDate"]), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out Date);

                                    if (!bValid)
                                    {
                                        Date = Convert.ToDateTime(IR5Item["ApprovalDate"]);
                                    }

                                    this.approvalDate_dtc.SelectedDate = Date;
                                }


                                SPListItemCollection FRInfoList = oSPWeb.Lists["FlashReportOff"].Items;



                                if (FRInfoList != null)
                                {
                                    SPListItem FRItem = FRInfoList.GetItemById(Convert.ToInt32(FRID));
                                    if (FRItem != null)
                                    {
                                        if (!String.IsNullOrEmpty(Convert.ToString(FRItem["ApprovingAuthority"])))
                                            this.approvedBy_tf.Value = Convert.ToString(FRItem["ApprovingAuthority"]);
                                    }


                                }

                                this.approvedBy_tf.Disabled = true;

                                //Recommendation Starts
                                string p1 = "~|~"; //separate records
                                string p2 = "*|*"; //separate content with in a record


                                List<IRRecommendationOnJob> lstRecommendation = GetFormattedRecommendationsByIRDI_Id(oSPWeb, Convert.ToInt32(IR5ID));

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
                                        HtmlTableCell type = new HtmlTableCell();
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

                                        type.InnerHtml = "<span class='type'>" + recommendation.Type + "</span>";
                                        status.InnerHtml = "<span class='status'>" + Convert.ToString(recommendation.Status) + "</span>";

                                        tRow.Cells.Add(recommendationId);
                                        tRow.Cells.Add(description);
                                        tRow.Cells.Add(responsiblePersonUsername);
                                        tRow.Cells.Add(responsibleSection);
                                        tRow.Cells.Add(responsibleSectionId);
                                        tRow.Cells.Add(responsibleDepartment);
                                        tRow.Cells.Add(responsibleDepartmentId);
                                        tRow.Cells.Add(targetDate);
                                        tRow.Cells.Add(type);
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
                }
            }

            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->LoadPageFromDraft)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->GetGroupMembers)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return Users;
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
                            String FRID = Page.Request.QueryString["FRID"];
                            string IR_1Link = Utility.GetRedirectUrl("IR_5FormLink");
                            string subject = Utility.GetValueByKey("TeamLeadEmailSubject");
                            string body = Utility.GetValueByKey("TeamLeadEmailTemplate");

                            StringBuilder linkSB = new StringBuilder();
                            linkSB.Append(IR_1Link)
                                        .Append("?FRID=")
                                        .Append(FRID);

                            //body = body.Replace("~|~", linkSB.ToString());
                            body = linkSB.ToString();

                            SPUser spSender = Utility.GetUser(oWebSite, Convert.ToString(imiItem["SubmittedBy"]));
                            Message message = new Message();
                            message.Subject = subject;
                            message.Body = body;
                            message.From = spSender.Email;

                            String TeamLead = Convert.ToString(GetFRTeamLead(oWebSite, FRID));

                            SPUser user = Utility.GetUser(oWebSite, TeamLead);
                            message.To = user.Email;
                            Email.SendEmail(message);


                            subject = Utility.GetValueByKey("TeamMemberEmailSubject");
                            body = Utility.GetValueByKey("TeamMemberEmailTemplate");

                            message.Subject = subject;
                            message.Body = Utility.GetValueByKey("TeamMemberEmailBody") + " " + TeamLead;

                            String s = Convert.ToString(GetFRTeamMembers(oWebSite, FRID));

                            string[] TeamMembers = s.Split(',');

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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->SendEmailToTeamLeadAndMembers)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        protected void SendSubmissionEmailToTeamMembers(SPListItem imiItem)
        {
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {

                        if (oWebSite != null)
                        {
                            String FRID = Page.Request.QueryString["FRID"];
                            // string IR_1Link = Utility.GetRedirectUrl("IR_5FormLink");
                            string subject = Utility.GetValueByKey("TeamLeadApprovelEmailSubject");
                            string body = Utility.GetValueByKey("TeamLeadApprovelEmailTemplate") + FRID;

                            //StringBuilder linkSB = new StringBuilder();
                            //linkSB.Append(IR_1Link)
                            //            .Append("?FRID=")
                            //            .Append(imiItem.ID);

                            //body = body.Replace("~|~", linkSB.ToString());
                            //body = "IR-05 Form Submission for FRID = " + FRID + " has Submitted to HSE";

                            SPUser spSender = Utility.GetUser(oWebSite, Convert.ToString(imiItem["SubmittedBy"]));
                            Message message = new Message();
                            message.Subject = subject;
                            message.Body = body;
                            message.From = spSender.Email;


                            SPUser user = null;

                            String s = GetFRTeamMembers(oWebSite, FRID);

                            string[] TeamMembers = s.Split(',');

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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->SendSubmissionEmailToTeamMembers)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        protected void SendRejectionEmailToTeamLeadTeamMembers(SPListItem imiItem)
        {
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {

                        if (oWebSite != null)
                        {
                            String FRID = Page.Request.QueryString["FRID"];
                            string IR_1Link = Utility.GetRedirectUrl("IR_5FormLink");
                            string subject = Utility.GetValueByKey("ApprovingAuthorityDisapprovelEmailSubject");
                            string body = Utility.GetValueByKey("ApprovingAuthorityDisapprovelEmailTemplate");

                            StringBuilder linkSB = new StringBuilder();
                            linkSB.Append(IR_1Link)
                                        .Append("?FRID=")
                                        .Append(FRID);

                            //body = body.Replace("~|~", linkSB.ToString());
                            body = linkSB.ToString();

                            SPUser spSender = Utility.GetUser(oWebSite, Convert.ToString(imiItem["SubmittedBy"]));
                            Message message = new Message();
                            message.Subject = subject;
                            message.Body = body;
                            message.From = spSender.Email;

                            String TeamLead = Convert.ToString(GetFRTeamLead(oWebSite, FRID));

                            SPUser user = Utility.GetUser(oWebSite, TeamLead);
                            message.To = user.Email;
                            Email.SendEmail(message);

                            String s = Convert.ToString(GetFRTeamMembers(oWebSite, FRID));

                            string[] TeamMembers = s.Split(',');

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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->SendRejectionEmailToTeamLeadTeamMembers)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        protected void SendEmailToHSE(SPListItem imiItem)
        {
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {

                        if (oWebSite != null)
                        {
                            String FRID = Page.Request.QueryString["FRID"];
                            string IR_1Link = Utility.GetRedirectUrl("IR_5FormLink");
                            string subject = Utility.GetValueByKey("HSEIR05EmailSubject");
                            string body = Utility.GetValueByKey("HSEIR05EmailTemplate");

                            StringBuilder linkSB = new StringBuilder();
                            linkSB.Append(IR_1Link)
                                        .Append("?FRID=")
                                        .Append(FRID);

                            //body = body.Replace("~|~", linkSB.ToString());
                            body = linkSB.ToString();

                            SPUser spSender = Utility.GetUser(oWebSite, Convert.ToString(imiItem["SubmittedBy"]));
                            Message message = new Message();
                            message.Subject = subject;
                            message.Body = body;
                            message.From = spSender.Email;

                            List<SPUser> Users = new List<SPUser>();

                            Users = GetGroupMembers("MasterGroup");
                            //Optimized
                            StringBuilder AssigneeUsers = new StringBuilder();

                            //if (!String.IsNullOrEmpty(Convert.ToString(imiItem["Assignee"])))
                            //{
                            //    AssigneeUsers.Append(Convert.ToString(imiItem["Assignee"])).Append(",");
                            //}




                            foreach (SPUser user in Users)
                            {
                                string username = Utility.GetUsername(user.LoginName, true);
                                if (username != null)
                                    AssigneeUsers.Append(username).Append(",");
                            }

                            AssigneeUsers.Length = AssigneeUsers.Length - 1;

                            imiItem["Assignee"] = AssigneeUsers.ToString();

                            imiItem.Update();

                            foreach (SPUser user in Users)
                            {
                                // User = user.LoginName;
                                // Name = User.Split('|');


                                //if (Name.Length > 1)
                                //{
                                //SPUser HSEUser = Utility.GetUser(oWebSite, Name[1]);
                                message.To = user.Email;
                                Email.SendEmail(message);
                                //}
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->SendEmailToHSE)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        protected void SendIR05AcceptanceEmailToHSE(SPListItem imiItem)
        {
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {

                        if (oWebSite != null)
                        {
                            String FRID = Page.Request.QueryString["FRID"];
                            string IR_1Link = Utility.GetRedirectUrl("IR_5FormLink");
                            string subject = Utility.GetValueByKey("HSEIR05GroupEmailSubject");
                            string body = Utility.GetValueByKey("HSEIR05GroupEmailTemplate");

                            StringBuilder linkSB = new StringBuilder();
                            linkSB.Append(IR_1Link)
                                        .Append("?FRID=")
                                        .Append(FRID);

                            //body = body.Replace("~|~", linkSB.ToString());
                            body = linkSB.ToString();

                            SPUser spSender = Utility.GetUser(oWebSite, Convert.ToString(imiItem["SubmittedBy"]));
                            Message message = new Message();
                            message.Subject = subject;
                            message.Body = body;
                            message.From = spSender.Email;

                            List<SPUser> Users = new List<SPUser>();

                            Users = GetGroupMembers("MasterGroup");
                            //Optimized
                            //StringBuilder AssigneeUsers = new StringBuilder();

                            //if (!String.IsNullOrEmpty(Convert.ToString(imiItem["Assignee"])))
                            //{
                            //    AssigneeUsers.Append(Convert.ToString(imiItem["Assignee"])).Append(",");
                            //}

                            //foreach (SPUser user in Users)
                            //{
                            //    AssigneeUsers.Append(user.LoginName).Append(",");
                            //}

                            //AssigneeUsers.Length = AssigneeUsers.Length - 1;

                            //imiItem["Assignee"] = AssigneeUsers.ToString();

                            //imiItem.Update();




                            foreach (SPUser user in Users)
                            {


                                message.To = user.Email;
                                Email.SendEmail(message);

                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->SendIR05AcceptanceEmailToHSE)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        protected void SendAcceptanceEmailToTeamLeadAndMembers(SPListItem imiItem)
        {
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {

                        if (oWebSite != null)
                        {
                            String FRID = Page.Request.QueryString["FRID"];
                            // string IR_1Link = Utility.GetRedirectUrl("IR_5FormLink");
                            string subject = Utility.GetValueByKey("TeamEmailSubject");
                            string body = Utility.GetValueByKey("TeamEmailTemplate");

                            //StringBuilder linkSB = new StringBuilder();
                            //linkSB.Append(IR_1Link)
                            //            .Append("?FRID=")
                            //            .Append(FRID);

                            //body = body.Replace("~|~", linkSB.ToString());
                            body = "IR-05 Form Submission For FRID = " + FRID + " has Approved";

                            SPUser spSender = Utility.GetUser(oWebSite, Convert.ToString(imiItem["SubmittedBy"]));
                            Message message = new Message();
                            message.Subject = subject;
                            message.Body = body;
                            message.From = spSender.Email;

                            String TeamLead = GetFRTeamLead(oWebSite, FRID);

                            SPUser user = Utility.GetUser(oWebSite, TeamLead);
                            message.To = user.Email;
                            Email.SendEmail(message);


                            String s = GetFRTeamMembers(oWebSite, FRID);

                            string[] TeamMembers = s.Split(',');

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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->SendAcceptanceEmailToTeamLeadAndMembers)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        protected void SendHSEEmailToTeamLeadAndMembers(SPListItem imiItem)
        {
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {

                        if (oWebSite != null)
                        {
                            String FRID = Page.Request.QueryString["FRID"];
                            // string IR_1Link = Utility.GetRedirectUrl("IR_5FormLink");
                            string subject = Utility.GetValueByKey("HSEToAllTeamMembersEmailSubject");
                            string body = Utility.GetValueByKey("HSEToAllTeamMembersEmailTemplate");

                            //StringBuilder linkSB = new StringBuilder();
                            //linkSB.Append(IR_1Link)
                            //            .Append("?FRID=")
                            //            .Append(FRID);

                            //body = body.Replace("~|~", linkSB.ToString());
                            //body = "IR-05 Form Submission For FRID = " + FRID + " has Approved";

                            SPUser spSender = Utility.GetUser(oWebSite, Convert.ToString(imiItem["SubmittedBy"]));
                            Message message = new Message();
                            message.Subject = subject;
                            message.Body = body;
                            message.From = spSender.Email;

                            String TeamLead = GetFRTeamLead(oWebSite, FRID);

                            SPUser user = Utility.GetUser(oWebSite, TeamLead);
                            message.To = user.Email;
                            Email.SendEmail(message);


                            String s = GetFRTeamMembers(oWebSite, FRID);

                            string[] TeamMembers = s.Split(',');

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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->SendHSEEmailToTeamLeadAndMembers)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        protected void SendRejectionEmailToTeamLeadAndMembers(SPListItem imiItem)
        {
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {

                        if (oWebSite != null)
                        {
                            String FRID = Page.Request.QueryString["FRID"];
                            string IR_1Link = Utility.GetRedirectUrl("IR_5FormLink");
                            string subject = Utility.GetValueByKey("HSEDisapprovelEmailSubject");
                            string body = Utility.GetValueByKey("HSEDisapprovelEmailTemplate");

                            StringBuilder linkSB = new StringBuilder();
                            linkSB.Append(IR_1Link)
                                        .Append("?FRID=")
                                        .Append(FRID);

                            //body = body.Replace("~|~", linkSB.ToString());
                            body = linkSB.ToString();

                            SPUser spSender = Utility.GetUser(oWebSite, Convert.ToString(imiItem["SubmittedBy"]));
                            Message message = new Message();
                            message.Subject = subject;
                            message.Body = body;
                            message.From = spSender.Email;

                            String TeamLead = GetFRTeamLead(oWebSite, FRID);

                            SPUser user = Utility.GetUser(oWebSite, TeamLead);
                            message.To = user.Email;
                            Email.SendEmail(message);


                            String s = GetFRTeamMembers(oWebSite, FRID);

                            string[] TeamMembers = s.Split(',');

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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->SendRejectionEmailToTeamLeadAndMembers)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        protected void SendEmailToApprovingAuthority(SPListItem imiItem)
        {
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {

                        if (oWebSite != null)
                        {
                            String FRID = Page.Request.QueryString["FRID"];
                            string IR_1Link = Utility.GetRedirectUrl("IR_5FormLink");
                            string subject = Utility.GetValueByKey("TeamLeadToApprovingAuthorityEmailSubject");
                            string body = Utility.GetValueByKey("TeamLeadToApprovingAuthorityEmailTemplate");

                            StringBuilder linkSB = new StringBuilder();
                            linkSB.Append(IR_1Link)
                                        .Append("?FRID=")
                                        .Append(FRID);

                            //body = body.Replace("~|~", linkSB.ToString());
                            body = linkSB.ToString();

                            SPUser spSender = Utility.GetUser(oWebSite, Convert.ToString(imiItem["SubmittedBy"]));
                            Message message = new Message();
                            message.Subject = subject;
                            message.Body = body;
                            message.From = spSender.Email;

                            String ApprovingAuthority = Convert.ToString(GetFRApprovingAuthority(oWebSite, FRID));

                            SPUser user = Utility.GetUser(oWebSite, ApprovingAuthority);
                            message.To = user.Email;
                            Email.SendEmail(message);


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->SendEmailToApprovingAuthority)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        protected void SendHSEEmailToApprovingAuthority(SPListItem imiItem)
        {
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {

                        if (oWebSite != null)
                        {
                            String FRID = Page.Request.QueryString["FRID"];
                            string IR_1Link = Utility.GetRedirectUrl("IR_5FormLink");
                            string subject = Utility.GetValueByKey("HSEToApprovingAuthorityEmailSubject");
                            string body = Utility.GetValueByKey("HSEToApprovingAuthorityEmailTemplate");

                            StringBuilder linkSB = new StringBuilder();
                            linkSB.Append(IR_1Link)
                                        .Append("?FRID=")
                                        .Append(FRID);

                            //body = body.Replace("~|~", linkSB.ToString());
                            body = linkSB.ToString();

                            SPUser spSender = Utility.GetUser(oWebSite, Convert.ToString(imiItem["SubmittedBy"]));
                            Message message = new Message();
                            message.Subject = subject;
                            message.Body = body;
                            message.From = spSender.Email;

                            String ApprovingAuthority = Convert.ToString(GetFRApprovingAuthority(oWebSite, FRID));

                            SPUser user = Utility.GetUser(oWebSite, ApprovingAuthority);
                            message.To = user.Email;
                            Email.SendEmail(message);


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->SendHSEEmailToApprovingAuthority)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        ///////////////////

        private List<IRRecommendationOnJob> GetFormattedRecommendations(string recommendatons, String[] pattern1, String[] pattern2)
        {
            try
            {
                List<IRRecommendationOnJob> lstIRRecommendationOnJob = new List<IRRecommendationOnJob>();

                var lstRecommendation = recommendatons.Split(pattern1, StringSplitOptions.None);

                foreach (var item in lstRecommendation)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        var recommendationStr = item.Split(pattern2, StringSplitOptions.None);
                        if (recommendationStr.Length > 0)
                        {
                            IRRecommendationOnJob recommendation = new IRRecommendationOnJob();

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
                            recommendation.Type = recommendationStr[9];
                            recommendation.Status = recommendationStr[10];
                            recommendation.RecommendationNo = recommendationStr[11];
                            recommendation.IsSavedAsDraft = recommendationStr[12].Equals("true", StringComparison.OrdinalIgnoreCase) ? true : false;
                            recommendation.ValidationStatus = 0;

                            lstIRRecommendationOnJob.Add(recommendation);
                        }
                    }
                }
                return lstIRRecommendationOnJob;
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->GetFormattedRecommendations)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("IRR01DI_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator at email address - FFL.HSE@fatima-group.com.";
                }

                message_div.InnerHtml = message;

                // DisableControls(true);
            }
            return null;
        }

        private bool IsValid_IRDI_Data(SPWeb oSPWeb, List<IRRecommendationOnJob> recommendationList)
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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->IsValid_IRDI_Data)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return isValid;
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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->GetFormattedIds)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("IRR01DI_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator at email address - FFL.HSE@fatima-group.com.";
                }

                message_div.InnerHtml = message;
                //   DisableControls(true);
            }
            return null;
        }

        public List<Message> SaveRecommendations(SPWeb oSPWeb, List<IRRecommendationOnJob> recommendations, int IR01DI_ID, string sentFrom, List<int> recommendationIds = null)
        {
            try
            {
                List<Message> lstMessage = new List<Message>();

                if (oSPWeb != null)
                {
                    string listName = "IIRRecommendation_OffJob";

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
                                string infoMessage = Utility.GetValueByKey("IRR01DI_RP_Info_Incomplete");

                                if (String.IsNullOrEmpty(infoMessage))
                                {
                                    message_div.InnerHtml = "Information of Responsible Person is incomplete or needs more permission. Please Contact the Administrator!";
                                }

                                message_div.InnerHtml = infoMessage;
                                //    DisableControls(true);


                                return null;
                            }

                            itemToAdd["IRID"] = IR01DI_ID;


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

                            itemToAdd["IRDescription"] = item.Description;
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

                            itemToAdd["Type"] = item.Type;
                            itemToAdd["Status"] = item.Status;
                            itemToAdd["IsSavedAsDraft"] = item.IsSavedAsDraft;

                            //Is From IR01DI
                            itemToAdd["IsFromIR01DI"] = true;
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
                                    .Append("?IR05DI_ID=")
                                    .Append(itemToAdd.ID);

                                string body = Utility.GetValueByKey("IR01DI_ON_From_FB_To_RP_B");

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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->SaveRecommendations)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("IRR01DI_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator at email address - FFL.HSE@fatima-group.com.";
                }

                message_div.InnerHtml = message;
                //   DisableControls(true);
            }
            return null;
        }

        private bool FillRecommendationGrid(List<IRRecommendationOnJob> lstRecommendation)
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
                        HtmlTableCell type = new HtmlTableCell();
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

                        type.InnerHtml = "<span class='type'>" + Convert.ToString(recommendation.Type) + "</span>";
                        status.InnerHtml = "<span class='status'>" + Convert.ToString(recommendation.Status) + "</span>";

                        tRow.Cells.Add(recommendationId);
                        tRow.Cells.Add(description);
                        tRow.Cells.Add(responsiblePersonUsername);
                        tRow.Cells.Add(responsibleSection);
                        tRow.Cells.Add(responsibleSectionId);
                        tRow.Cells.Add(responsibleDepartment);
                        tRow.Cells.Add(responsibleDepartmentId);
                        tRow.Cells.Add(targetDate);
                        tRow.Cells.Add(type);
                        tRow.Cells.Add(status);

                        tRow.Cells.Add(new HtmlTableCell() { InnerHtml = actions });

                        this.recommendationDetails_table.Rows.Add(tRow);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->FillRecommendationGrid)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return false;
        }


        //////////////////

        private List<IRRecommendationOnJob> GetFormattedRecommendationsByIRDI_Id(SPWeb oSPWeb, int IRID)
        {
            try
            {
                string spListName = "IRRecommendation_OffJob";
                // Fetch the List
                SPList spListIIRRecommedation = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListName));

                List<IRRecommendationOnJob> lstIIRRecommedation_OnJob = new List<IRRecommendationOnJob>();

                if (spListIIRRecommedation != null)
                {
                    SPQuery query = new SPQuery();
                    SPListItemCollection spListItems;
                    // Include only the fields you will use.
                    StringBuilder vf = new StringBuilder();
                    vf.Append("<FieldRef Name='ID'/>")
                        .Append("<FieldRef Name='RecommendationNo'/>")
                        .Append("<FieldRef Name='TargetDate'/>")
                        .Append("<FieldRef Name='IRDescription'/>")
                        .Append("<FieldRef Name='TypeOfVoilation'/>")
                        .Append("<FieldRef Name='ResponsiblePerson'/>")
                        .Append("<FieldRef Name='AssigneeEmail'/>")
                        .Append("<FieldRef Name='Assignee'/>")
                        .Append("<FieldRef Name='ResponsibleSection'/>")
                        .Append("<FieldRef Name='ResponsibleDepartment'/>")
                        .Append("<FieldRef Name='Type'/>")
                        .Append("<FieldRef Name='Status'/>");

                    query.ViewFields = vf.ToString();
                    query.ViewFieldsOnly = true;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<Where>")
                         .Append("  <Eq>")
                         .Append("    <FieldRef Name='IRID' />")
                         .Append("    <Value Type='Text'>" + Convert.ToString(IRID) + "</Value>")
                         .Append("  </Eq>")
                         .Append("</Where>");

                    query.Query = sb.ToString();
                    spListItems = spListIIRRecommedation.GetItems(query);

                    for (int i = 0; i < spListItems.Count; i++)
                    {
                        SPListItem listItem = spListItems[i];
                        IRRecommendationOnJob recommendation = new IRRecommendationOnJob();
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

                        recommendation.Description = Convert.ToString(listItem["IRDescription"]);
                        recommendation.RPUsername = Convert.ToString(listItem["ResponsiblePerson"]);
                        recommendation.RPEmail = Convert.ToString(listItem["AssigneeEmail"]);
                        recommendation.AssigneeUsername = Convert.ToString(listItem["Assignee"]);
                        recommendation.AssigneeEmail = Convert.ToString(listItem["AssigneeEmail"]);
                        recommendation.Type = Convert.ToString(listItem["Type"]);
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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-5OffJobForm->GetFormattedRecommendationsByIRDI_Id)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                string message = Utility.GetValueByKey("IRR01DI_ContactAdmin");

                if (String.IsNullOrEmpty(message))
                {
                    message = "Something went wrong!!!  Please contact the administrator at email address - FFL.HSE@fatima-group.com.";
                }

                message_div.InnerHtml = message;
                DisableControls(true);
            }
            return null;
        }

        private void DisableControls(bool disableAll)
        {
            this.btnSave.Visible = false;
            this.btnSaveAsDraft.Visible = false;
        }
    }
}
