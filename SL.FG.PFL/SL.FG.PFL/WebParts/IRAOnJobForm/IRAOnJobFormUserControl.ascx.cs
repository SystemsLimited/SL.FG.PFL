using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;
using SL.FG.PFL.Layouts.SL.FG.PFL.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace SL.FG.PFL.WebParts.IRAOnJobForm
{
    public partial class IRAOnJobFormUserControl : UserControl
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {



                    String IRAID = Page.Request.QueryString["IRID"];

                    FillDropdowns();

                    if (!String.IsNullOrEmpty(IRAID))
                    {
                         PageLoadOnUserBases();
                    }
                    else
                    {
                        this.btnSave.Visible = false;
                        this.btnSaveAsDraft.Visible = false;
                        this.btnMOSave.Visible = false;
                        //this.btnHSESave.Visible = false;

                    }
                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAOnJobForm->Page_Load)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

            }
        }


        private void PageLoadOnUserBases()
        {
            try
            {

                String IRID = Page.Request.QueryString["IRID"];
                if (!String.IsNullOrEmpty(IRID))
                {

                    using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                    {
                        using (SPWeb oSPWeb = oSPsite.OpenWeb())
                        {

                            Boolean checkMo = CheckCurrentUserIsMo(oSPWeb);
                            Boolean checkHSEMember = CheckCurrentUserIsHSEMember(oSPWeb);
                            Boolean checkSupervisor = CheckCurrentUserIsSupervisor(oSPWeb);
                            Boolean checkAdmin = CheckCurrentUserIsAdmin(oSPWeb);

                            SPListItemCollection IR_1InfoList = oSPWeb.Lists["IR-1-Off"].Items;
                            if (IR_1InfoList != null)
                            {
                                SPListItem imiItem = IR_1InfoList.GetItemById(Convert.ToInt32(IRID));
                                if (imiItem != null)
                                {
                                    Boolean checkSubmittedBy = CheckSubmitByCurrentUser(imiItem, oSPWeb);
                                    Boolean checkAudittedBy = CheckAuditByCurrentUser(imiItem, oSPWeb);
                                    Boolean SaveAsDraft = CheckSaveAsDraft(imiItem, oSPWeb);

                                    if (checkAdmin)
                                    {
                                      //  LoadPageFromDraft(imiItem, oSPWeb, IRID);
                                        DisableUnableFieldsForAssigneeHSEMembers();
                                    }
                                    else if (CheckAssignee(imiItem, oSPWeb))
                                    {

                                        if (checkMo && checkAudittedBy && SaveAsDraft)
                                        {
                                         //   LoadPageFromDraft(imiItem, oSPWeb, IRID);
                                            DisableFieldsForNewPerson();
                                        }

                                        else if (checkMo && checkSubmittedBy && checkAudittedBy)
                                        {
                                         //   LoadPageFromDraft(imiItem, oSPWeb, IRID);
                                            DisableUnableFieldsForMO();
                                        }
                                        else if (checkMo && checkAudittedBy)
                                        {
                                         //   LoadPageFromDraft(imiItem, oSPWeb, IRID);
                                            DisableFieldsForNewPerson();
                                        }
                                        else if (checkMo)
                                        {
                                         //   LoadPageFromDraft(imiItem, oSPWeb, IRID);
                                            DisableUnableFieldsForMO();
                                        }
                                        else if (checkHSEMember && checkAudittedBy)
                                        {

                                       //     LoadPageFromDraft(imiItem, oSPWeb, IRID);
                                            DisableFieldsForNewPerson();


                                        }
                                        //else if (checkHSEMember)
                                        //{

                                        //    LoadPageFromDraft(imiItem, oSPWeb, IRID);
                                        //    DisableUnableFieldsForAssigneeHSEMembers();

                                        //}
                                        else
                                        {

                                        //    LoadPageFromDraft(imiItem, oSPWeb, IRID);
                                            DisableFieldsForNewPerson();

                                        }


                                    }
                                    else if (checkHSEMember)
                                    {

                                     //   LoadPageFromDraft(imiItem, oSPWeb, IRID);
                                        DisableUnableFieldsForHSEMembers();
                                    }
                                    else
                                    {

                                        string accessDeniedUrl = Utility.GetRedirectUrl("Access_Denied");

                                        if (!String.IsNullOrEmpty(accessDeniedUrl))
                                        {
                                            Page.Response.Redirect(accessDeniedUrl, false);
                                        }

                                    }


                                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["Status"])))
                                    {
                                        string status = Convert.ToString(imiItem["Status"]);

                                        if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                                        {
                                            this.btnSave.Visible = false;
                                            this.btnSaveAsDraft.Visible = false;
                                            this.btnMOSave.Visible = false;
                                          //  this.btnHSESave.Visible = false;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
                else
                {
                    DisableFieldsForNewPerson();
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IRAOnJobForm->PageLoadOnUserBases)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }
        private Boolean CheckSaveAsDraft(SPListItem ListItem, SPWeb oSPWeb)
        {
            Boolean IsSaveAsDraft = false;

            String User = oSPWeb.CurrentUser.LoginName;
            String[] Name = User.Split('|');
            String currentUser = Name[1];

            if (currentUser != null)
            {
                String s = Convert.ToString(ListItem["IsSaveAsDraft"]);

                if (s.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    IsSaveAsDraft = true;
                }
            }
            return IsSaveAsDraft;

        }
        private bool CheckCurrentUserIsMo(SPWeb oSPWeb)
        {
            Boolean IsMO = false;
            try
            {
                //string groupName = Utility.GetValueByKey("MasterGroup");
                //var spGroup = oSPWeb.Groups[groupName];
                //if (spGroup != null)
                //{
                //    isMember = oSPWeb.IsCurrentUserMemberOfGroup(spGroup.ID);
                //}

                string groupName = Utility.GetValueByKey("MOGroup");
                var spGroup = oSPWeb.Groups[groupName];

                //var spGroup = oSPWeb.Groups.GetByName("MO Group");

                if (spGroup != null)
                {
                    IsMO = oSPWeb.IsCurrentUserMemberOfGroup(spGroup.ID);
                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-1OffJobForm->CheckCurrentUserIsMo)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return IsMO;
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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-1OffJobForm->CheckCurrentUserIsHSEMember)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return IsHSEMember;
        }

        private bool CheckCurrentUserIsSupervisor(SPWeb oSPWeb)
        {
            Boolean IsSupervisor = false;
            try
            {
                //var spGroup = oSPWeb.Groups.GetByName("Supervisor");
                //if (spGroup != null)
                //{
                //    IsSupervisor = oSPWeb.IsCurrentUserMemberOfGroup(spGroup.ID);
                //}

                string groupName = Utility.GetValueByKey("SupervisorGroup");
                var spGroup = oSPWeb.Groups[groupName];

                if (spGroup != null)
                {
                    IsSupervisor = oSPWeb.IsCurrentUserMemberOfGroup(spGroup.ID);
                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-1OffJobForm->CheckCurrentUserIsSupervisor)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return IsSupervisor;
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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-1OffJobForm->CheckCurrentUserIsAdmin)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

            return IsSupervisor;
        }

        private void DisableFieldsForNewPerson()
        {

            this.MORemarks_ta.Disabled = true;
            this.MORemarks_ta.Visible = false;
            this.MORemarks_ldl.Visible = false;
            this.MORemarks_str.Visible = false;
            this.btnMOSave.Visible = false;
            //  this.btnSupervisorSave.Visible = false;
        }

        private void DisableUnableFieldsForMO()
        {

            this.IncidentCategory_ddl.Visible = false;

            this.IncidentCategory_ta.Visible = true;

            this.IncidentCategory_ta.Disabled = true;

            this.InjuryCategory_ddl.Visible = false;

            this.InjuryCategory_ta.Visible = true;

            this.InjuryCategory_ta.Disabled = true;

            this.EmployeeType_ddl.Disabled = true;          

            this.MOName_div.Attributes.Add("style", "display:none");

            ////this.MOFields_div.Attributes.Add("style", "display:normal");

            this.MOName_tf.Visible = true;

            this.MOName_tf.Disabled = true;

            this.MOName_PeopleEditor.Enabled = false;

            this.DateOfIncident_dtc.Enabled = false;

            this.TimeOfIncident_dtc.Enabled = false;

            this.Unit_Area_ddl.Disabled = true;        

            this.SubmissionDate_dtc.Enabled = false;

            this.Title_tf.Disabled = true;

            this.Description_ta.Disabled = true;        

            this.SubmittedBy_div.Attributes.Add("style", "display:none");

            this.SubmittedBy_tf.Visible = true;

            this.SubmittedBy_tf.Disabled = true;

            this.outside_cb.Disabled = true;
        
            this.btnSaveAsDraft.Visible = false;

            this.btnSave.Visible = false;

            this.btnMOSave.Visible = true;

        }

        private void DisableUnableFieldsForAssigneeHSEMembers()
        {

            //this.IncidentCategory_ddl.Visible = false;

            //this.IncidentCategory_ta.Visible = true;

            //this.IncidentCategory_ta.Disabled = true;

            //this.InjuryCategory_ddl.Visible = false;

            //this.InjuryCategory_ta.Visible = true;

            //this.InjuryCategory_ta.Disabled = true;

            //this.EmployeeType_ddl.Disabled = true;

            //this.ConsentTaken_ddl.Disabled = true;

            //this.MOName_div.Attributes.Add("style", "display:none");

            //this.MOName_tf.Visible = true;

            //this.MOName_tf.Disabled = true;

            //this.Date_dtc.Enabled = false;

            //this.MOName_PeopleEditor.Enabled = false;

            //this.DateOfIncident_dtc.Enabled = false;

            //this.TimeOfIncident_dtc.Enabled = false;

            //this.Unit_Area_ddl.Disabled = true;

            //this.IncidentScore_tf.Disabled = true;



            //this.SubmissionDate_dtc.Enabled = false;

            //this.Title_tf.Disabled = true;

            //this.Description_ta.Disabled = true;

            //this.ActionTaken_ta.Disabled = true;

            //this.SubmittedBy_div.Attributes.Add("style", "display:none");

            //this.SubmittedBy_tf.Visible = true;

            //this.SubmittedBy_tf.Disabled = true;

            //this.ReportRequired_cb.Disabled = true;

            //this.TeamRequired_cb.Disabled = true;

            this.btnSaveAsDraft.Visible = false;

            this.btnSave.Visible = false;

            //this.btnHSESave.Visible = true;

            this.MOName_div.Attributes.Add("style", "display:normal");

            this.MORemarks_ldl.Visible = true;

            this.MORemarks_ta.Visible = true;

            this.MORemarks_ta.Disabled = true;

        }

        private void DisableUnableFieldsForHSEMembers()
        {

            this.IncidentCategory_ddl.Visible = false;

            this.IncidentCategory_ta.Visible = true;

            this.IncidentCategory_ta.Disabled = true;

            this.InjuryCategory_ddl.Visible = false;

            this.InjuryCategory_ta.Visible = true;

            this.InjuryCategory_ta.Disabled = true;

            this.EmployeeType_ddl.Disabled = true;

            this.MOName_div.Attributes.Add("style", "display:none");

            this.MOName_tf.Visible = true;

            this.MOName_tf.Disabled = true;
    
            this.MOName_PeopleEditor.Enabled = false;

            this.DateOfIncident_dtc.Enabled = false;

            this.TimeOfIncident_dtc.Enabled = false;

            this.Unit_Area_ddl.Disabled = true;

            this.SubmissionDate_dtc.Enabled = false;

            this.Title_tf.Disabled = true;

            this.Description_ta.Disabled = true;

            this.SubmittedBy_div.Attributes.Add("style", "display:none");

            this.SubmittedBy_tf.Visible = true;

            this.SubmittedBy_tf.Disabled = true;

            this.outside_cb.Disabled = true;

            this.btnSaveAsDraft.Visible = false;

            this.btnSave.Visible = false;

            //  this.btnHSESave.Visible = true;

            this.MORemarks_ldl.Visible = true;

            this.MORemarks_ta.Visible = true;

            this.MORemarks_ta.Disabled = true;



        }


        //private void DisableUnableFieldsForSupervisor()
        //{

        //    this.IncidentCategory_ddl.Visible = false;

        //    this.IncidentCategory_ta.Visible = true;

        //    this.IncidentCategory_ta.Disabled = true;

        //    this.InjuryCategory_ddl.Visible = false;

        //    this.InjuryCategory_ta.Visible = true;

        //    this.InjuryCategory_ta.Disabled = true;

        //    this.EmployeeType_ddl.Disabled = true;

        //    this.ConsentTaken_ddl.Disabled = true;

        //    this.MOName_div.Attributes.Add("style", "display:none");

        //    this.MOName_tf.Visible = true;

        //    this.MOName_tf.Disabled = true;

        //    this.Date_dtc.Enabled = false;

        //    this.MOName_PeopleEditor.Enabled = false;

        //    this.DateOfIncident_dtc.Enabled = false;

        //    this.TimeOfIncident_dtc.Enabled = false;

        //    this.Unit_Area_ddl.Disabled = true;

        //    this.IncidentScore_tf.Disabled = true;

        //    this.btnHSESave.Attributes.Add("style", "display:none");

        //    this.SubmissionDate_dtc.Enabled = false;

        //    this.Title_tf.Disabled = true;

        //    this.Description_ta.Disabled = true;

        //    this.ActionTaken_ta.Disabled = true;

        //    this.SubmittedBy_div.Attributes.Add("style", "display:none");

        //    this.SubmittedBy_tf.Visible = true;

        //    this.SubmittedBy_tf.Disabled = true;

        //    this.ReportRequired_cb.Disabled = true;

        //    this.TeamRequired_cb.Disabled = true;

        //    this.btnSaveAsDraft.Visible = false;

        //    this.btnSave.Visible = false;

        //    this.btnMOSave.Visible = false;

        //    this.MORemarks_ldl.Visible = true;

        //    this.MORemarks_ta.Visible = true;

        //    this.MORemarks_ta.Disabled = true;

        //    this.HSEManagerName_div.Attributes.Add("style", "display:none");

        //    this.btnHSESave.Visible = true;


        //}


        private Boolean CheckAssignee(SPListItem ListItem, SPWeb oSPWeb)
        {
            Boolean assignee = false;

            String User = oSPWeb.CurrentUser.LoginName;
            String[] Name = User.Split('|');
            String currentUser = Name[1];

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
            return assignee;

        }
        private Boolean CheckAuditByCurrentUser(SPListItem ListItem, SPWeb oSPWeb)
        {
            Boolean SubmittedBy = false;

            String User = oSPWeb.CurrentUser.LoginName;
            String[] Name = User.Split('|');
            String currentUser = Name[1];

            if (currentUser != null)
            {
                String s = Convert.ToString(ListItem["AuditedBy"]);

                string[] AssigneeList = s.Split(',');


                foreach (string person in AssigneeList)
                {
                    if (person == currentUser)
                        SubmittedBy = true;
                }
            }
            return SubmittedBy;

        }

        private Boolean CheckSubmitByCurrentUser(SPListItem ListItem, SPWeb oSPWeb)
        {
            Boolean SubmittedBy = false;

            String User = oSPWeb.CurrentUser.LoginName;
            String[] Name = User.Split('|');
            String currentUser = Name[1];

            if (currentUser != null)
            {
                String s = Convert.ToString(ListItem["SubmittedBy"]);

                string[] AssigneeList = s.Split(',');


                foreach (string person in AssigneeList)
                {
                    if (person == currentUser)
                        SubmittedBy = true;
                }
            }
            return SubmittedBy;

        }

        private void FillDropdowns()
        {
            using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
            {
                using (SPWeb oSPWeb = oSPsite.OpenWeb())
                {
                    FillArea(oSPWeb);
                    FillSection(oSPWeb);
                    FillIncidentCategory(oSPWeb);
                    FillInjuryCategory(oSPWeb);
                    FillCauseOfInjury(oSPWeb);
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

                this.Unit_Area_ddl.DataSource = spListItems;
                this.Unit_Area_ddl.DataTextField = "Title";
                this.Unit_Area_ddl.DataValueField = "Title";
                this.Unit_Area_ddl.DataBind();
                this.Unit_Area_ddl.Items.Insert(0, new ListItem("Please Select", "0"));

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAOnJobForm->FillArea)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
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
                sb.Append("<OrderBy Override='TRUE;><FieldRef Name='Title'/></OrderBy>");
                query.Query = sb.ToString();
                spListItems = spList.GetItems(query);

                this.IncidentCategory_ddl.DataSource = spListItems;
                this.IncidentCategory_ddl.DataTextField = "Title";
                this.IncidentCategory_ddl.DataValueField = "Title";
                this.IncidentCategory_ddl.DataBind();

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAOnJobForm->FillIncidentCategory)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
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
                sb.Append("<OrderBy Override='TRUE;><FieldRef Name='Title'/></OrderBy>");
                query.Query = sb.ToString();
                spListItems = spList.GetItems(query);

                this.InjuryCategory_ddl.DataSource = spListItems;
                this.InjuryCategory_ddl.DataTextField = "Title";
                this.InjuryCategory_ddl.DataValueField = "Title";
                this.InjuryCategory_ddl.DataBind();
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAOnJobForm->FillInjuryCategory)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
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
                sb.Append("<OrderBy Override='TRUE;><FieldRef Name='Title'/></OrderBy>");
                query.Query = sb.ToString();
                spListItems = spList.GetItems(query);

                //this.Section_Violation_ddl.DataSource = spListItems;
                //this.Section_Violation_ddl.DataTextField = "Title";
                //this.Section_Violation_ddl.DataValueField = "Title";
                //this.Section_Violation_ddl.DataBind();
                //this.Section_Violation_ddl.Items.Insert(0, new ListItem("Please Select", "0"));


                this.Section_Injury_ddl.DataSource = spListItems;
                this.Section_Injury_ddl.DataTextField = "Title";
                this.Section_Injury_ddl.DataValueField = "Title";
                this.Section_Injury_ddl.DataBind();
                this.Section_Injury_ddl.Items.Insert(0, new ListItem("Please Select", "0"));

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAOnJobForm->FillSection)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        private void FillCauseOfInjury(SPWeb oSPWeb)
        {


            try
            {

                //string listName = "CauseOfInjury";

                //// Fetch the List
                //SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));

                //SPQuery query = new SPQuery();
                //SPListItemCollection spListItems;
                //// Include only the fields you will use.
                //query.ViewFields = "<FieldRef Name='ID'/><FieldRef Name='Title'/>";
                //query.ViewFieldsOnly = true;
                ////query.RowLimit = 200; // Only select the top 200.
                //StringBuilder sb = new StringBuilder();
                //sb.Append("<OrderBy Override='TRUE;><FieldRef Name='Title'/></OrderBy>");
                //query.Query = sb.ToString();
                //spListItems = spList.GetItems(query);

                ////this.Section_Violation_ddl.DataSource = spListItems;
                ////this.Section_Violation_ddl.DataTextField = "Title";
                ////this.Section_Violation_ddl.DataValueField = "Title";
                ////this.Section_Violation_ddl.DataBind();
                ////this.Section_Violation_ddl.Items.Insert(0, new ListItem("Please Select", "0"));


                //this.Section_Injury_ddl.DataSource = spListItems;
                //this.Section_Injury_ddl.DataTextField = "Title";
                //this.Section_Injury_ddl.DataValueField = "Title";
                //this.Section_Injury_ddl.DataBind();
                //this.Section_Injury_ddl.Items.Insert(0, new ListItem("Please Select", "0"));

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAOnJobForm->FillSection)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        private void FillTypeOfInjury(SPWeb oSPWeb)
        {


            try
            {

                //string listName = "CauseOfInjury";

                //// Fetch the List
                //SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));

                //SPQuery query = new SPQuery();
                //SPListItemCollection spListItems;
                //// Include only the fields you will use.
                //query.ViewFields = "<FieldRef Name='ID'/><FieldRef Name='Title'/>";
                //query.ViewFieldsOnly = true;
                ////query.RowLimit = 200; // Only select the top 200.
                //StringBuilder sb = new StringBuilder();
                //sb.Append("<OrderBy Override='TRUE;><FieldRef Name='Title'/></OrderBy>");
                //query.Query = sb.ToString();
                //spListItems = spList.GetItems(query);

                ////this.Section_Violation_ddl.DataSource = spListItems;
                ////this.Section_Violation_ddl.DataTextField = "Title";
                ////this.Section_Violation_ddl.DataValueField = "Title";
                ////this.Section_Violation_ddl.DataBind();
                ////this.Section_Violation_ddl.Items.Insert(0, new ListItem("Please Select", "0"));


                //this.Section_Injury_ddl.DataSource = spListItems;
                //this.Section_Injury_ddl.DataTextField = "Title";
                //this.Section_Injury_ddl.DataValueField = "Title";
                //this.Section_Injury_ddl.DataBind();
                //this.Section_Injury_ddl.Items.Insert(0, new ListItem("Please Select", "0"));

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAOnJobForm->FillSection)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
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
                            

                            string listName = "IRAOnJob";

                            // Fetch the List
                            SPList list = oWebSite.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oWebSite.Url, listName));
                            SPListItem spListItem = null;

                            String IRID = Page.Request.QueryString["IRID"];
                            int ItemID = Convert.ToInt32(IRID);

                            if (ItemID != 0 && list != null)
                            {

                                spListItem = list.Items.GetItemById(ItemID);

                                if (spListItem != null)
                                {

                                          UpdateIR_AValues(spListItem, false, oWebSite);

                                }
                            }

                            else if (list != null)
                            {
                                spListItem = list.Items.Add();


                                if (spListItem != null)
                                {

                                          UpdateIR_AValues(spListItem, false, oWebSite);

                                }
                            }
                        }
                        string redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");

                        DisableControls();
                        if (!String.IsNullOrEmpty(redirectUrl))
                        {
                            Page.Response.Redirect(redirectUrl, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAOnJobForm->btnSave_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                string redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");

                DisableControls();
                if (!String.IsNullOrEmpty(redirectUrl))
                {
                    Page.Response.Redirect(redirectUrl, false);
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAOnJobForm->btnCancel_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

            }
        }

        protected void btnSaveAsDraft_Click()
        {

            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {

                        if (oWebSite != null)
                        {
                         

                            string listName = "IRAONJob";

                            // Fetch the List
                            SPList list = oWebSite.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oWebSite.Url, listName));
                            SPListItem spListItem = null;
                            String IRID = Page.Request.QueryString["IRID"];
                            int ItemID = Convert.ToInt32(IRID);

                            if (ItemID != 0 && list != null)
                            {

                                spListItem = list.Items.GetItemById(ItemID);

                                if (spListItem != null)
                                {

                                    UpdateIR_AValues(spListItem, true, oWebSite);

                                }
                            }


                            else if (list != null)
                            {
                                spListItem = list.Items.Add();


                                if (spListItem != null)
                                {

                                    UpdateIR_AValues(spListItem, true, oWebSite);

                                }
                            }
                        }
                        string redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");

                        DisableControls();
                        if (!String.IsNullOrEmpty(redirectUrl))
                        {
                            Page.Response.Redirect(redirectUrl, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAOnJobForm->btnSaveAsDraft_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }

        protected void btnMOSave_Click(object sender, EventArgs e)
        {

            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWebSite = oSPsite.OpenWeb())
                    {

                        if (oWebSite != null)
                        {
                            //SPListItemCollection IR_1infoList = oWebSite.Lists["IR-1-Off"].Items;

                            string listName = "IRAOnJob";

                            // Fetch the List
                            SPList list = oWebSite.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oWebSite.Url, listName));
                            SPListItem spListItem = null;
                            String IRID = Page.Request.QueryString["IRID"];
                            int ItemID = Convert.ToInt32(IRID);

                            if (ItemID != 0 && list != null)
                            {

                                spListItem = list.Items.GetItemById(ItemID);

                                if (spListItem != null)
                                {
                                    if (!String.IsNullOrEmpty(Convert.ToString(this.MORemarks_ta.Value)))
                                        spListItem["MORemarks"] = Convert.ToString(this.MORemarks_ta.Value);


                                    String User = oWebSite.CurrentUser.LoginName;
                                    String[] Name = User.Split('|');

                                    if (Name.Length > 1)
                                    {
                                        spListItem["SubmittedBy"] = Name[1];
                                        spListItem["AuditedBy"] = Name[1];
                                    }
                                    spListItem["Status"] = "Completed";


                                    spListItem.Update();

                                  
                                   // SendEmailToHSE(spListItem);

                                   

                                }
                            }


                        }
                        string redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");

                        DisableControls();
                        if (!String.IsNullOrEmpty(redirectUrl))
                        {
                            Page.Response.Redirect(redirectUrl, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.FFL(IR-1OffJobForm->btnMOSave_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }

        protected void DisableControls()
        {
            this.btnSave.Visible = false;
            this.btnSaveAsDraft.Visible = false;
        }

        private void LoadPageFromDraft(SPListItem imiItem, SPWeb oSPWeb, String IRID)
        {

            try
            {

                if (imiItem != null)
                {

                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["IncidentType"])))

                        this.IncidentType_ddl.Value = Convert.ToString(imiItem["IncidentType"]);


                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["EmployeeType"])))

                        this.EmployeeType_ddl.Value = Convert.ToString(imiItem["EmployeeType"]);


                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["IncidentCategory"])))
                    {
                        String s = Convert.ToString(imiItem["IncidentCategory"]);

                        string[] IncidentCategoryItem = s.Split(',');

                        this.IncidentCategory_hdn.Value = s;

                        this.IncidentCategory_ta.Value = s;

                        foreach (string Item in IncidentCategoryItem)
                        {
                            this.IncidentCategory_ddl.Items.FindByValue(Item).Selected = true;

                        }

                    }

                   


                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["Unit_x002f_Area"])))
                    {
                        this.Unit_Area_ddl.Items.FindByValue(Convert.ToString(imiItem["Unit_x002f_Area"])).Selected = true;
                        this.Unit_Area_hdn.Value = Convert.ToString(imiItem["Unit_x002f_Area"]);
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


                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["TitleOfIncident"])))

                        this.Title_tf.Value = Convert.ToString(imiItem["TitleOfIncident"]);



                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["InjuryTo"])))

                        this.InjuryTo_ddl.Value = Convert.ToString(imiItem["InjuryTo"]);



                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["ContractorName"])))
                    {
                        PeopleEditor pe = new PeopleEditor();
                        PickerEntity UserEntity = new PickerEntity();
                        String username = Convert.ToString(imiItem["ContractorName"]);
                        //get Spuser
                        SPUser SPuser = Utility.GetUser(oSPWeb, username, null, 0);
                        if (SPuser != null)
                        {
                            // CurrentUser is SPUser object
                            UserEntity.DisplayText = SPuser.Name;
                            UserEntity.Key = SPuser.LoginName;

                            UserEntity = pe.ValidateEntity(UserEntity);

                            // Add PickerEntity to People Picker control
                            this.NameOfContractor_PeopleEditor.AddEntities(new List<PickerEntity> { UserEntity });
                            
                        }



                    }


                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["NameOfInjured"])))
                    {
                        PeopleEditor pe = new PeopleEditor();
                        PickerEntity UserEntity = new PickerEntity();
                        String username = Convert.ToString(imiItem["NameOfInjured"]);
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

                        }



                    }

                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["PNo"])))

                        this.PNO_tf.Value = Convert.ToString(imiItem["PNo"]);


                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["OccupationTrade"])))

                        this.OccupationTrade_tf.Value = Convert.ToString(imiItem["OccupationTrade"]);



                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["EmployeeSection"])))
                    {
                        this.Section_Injury_ddl.Items.FindByValue(Convert.ToString(imiItem["EmployeeSection"])).Selected = true;
                        this.Injury_Section_hdn.Value = Convert.ToString(imiItem["EmployeeSection"]);
                    }


                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["EmployeeDepartment"])))
                    {
                        this.Department_Injury_ddl.Items.FindByValue(Convert.ToString(imiItem["EmployeeDepartment"])).Selected = true;
                        this.Injury_Department_hdn.Value = Convert.ToString(imiItem["EmployeeDepartment"]);
                    }


                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["InjuryCategory"])))
                    {
                        String s = Convert.ToString(imiItem["InjuryCategory"]);



                        string[] InjuryCategoryItem = s.Split(',');

                        this.InjuryCategory_hdn.Value = s;

                        this.InjuryCategory_ta.Value = s;
                        foreach (string Item in InjuryCategoryItem)
                        {
                            this.InjuryCategory_ddl.Items.FindByValue(Item).Selected = true;



                        }

                    }


//------------------------------------------------------------------------------------








                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["MOName"])))
                    {


                        PeopleEditor pe = new PeopleEditor();
                        PickerEntity UserEntity = new PickerEntity();
                        String username = Convert.ToString(imiItem["MOName"]);
                        //get Spuser
                        SPUser SPuser = Utility.GetUser(oSPWeb, username, null, 0);
                        if (SPuser != null)
                        {
                            // CurrentUser is SPUser object
                            UserEntity.DisplayText = SPuser.Name;
                            UserEntity.Key = SPuser.LoginName;

                            UserEntity = pe.ValidateEntity(UserEntity);

                            // Add PickerEntity to People Picker control
                            this.MOName_PeopleEditor.AddEntities(new List<PickerEntity> { UserEntity });

                            this.MOName_tf.Value = SPuser.Name;
                        }



                    }


                   

                  

                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["SubmittedBy"])))
                    {
                        PeopleEditor pe = new PeopleEditor();
                        PickerEntity UserEntity = new PickerEntity();
                        String username = Convert.ToString(imiItem["SubmittedBy"]);
                        //get Spuser
                        SPUser SPuser = Utility.GetUser(oSPWeb, username, null, 0);
                        if (SPuser != null)
                        {
                            // CurrentUser is SPUser object
                            UserEntity.DisplayText = SPuser.Name;
                            UserEntity.Key = SPuser.LoginName;

                            UserEntity = pe.ValidateEntity(UserEntity);

                            // Add PickerEntity to People Picker control
                            this.SubmittedBy_PeopleEditor.AddEntities(new List<PickerEntity> { UserEntity });

                            this.SubmittedBy_tf.Value = SPuser.Name;
                        }

                    }

                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["DateOFSubmission"])))
                    {
                        DateTime Date;
                        bool bValid = DateTime.TryParse(Convert.ToString(imiItem["DateOFSubmission"]), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out Date);

                        if (!bValid)
                        {
                            Date = Convert.ToDateTime(imiItem["DateOFSubmission"]);
                        }

                        this.SubmissionDate_dtc.SelectedDate = Date;
                    }



                 

                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["IncidentDescription"])))

                        this.Description_ta.Value = Convert.ToString(imiItem["IncidentDescription"]);

                  
                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["IsInvestigationTeamRequired?"])))

                        if (Convert.ToString(imiItem["IsInvestigationTeamRequired?"]) == "Yes")
                            this.outside_cb.Checked = true;
                        else
                            this.outside_cb.Checked = false;


                    if (!String.IsNullOrEmpty(Convert.ToString(imiItem["MORemarks"])))

                        this.MORemarks_ta.Value = Convert.ToString(imiItem["MORemarks"]);

                }

            }

            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAONJobJobForm->LoadPageFromDraft)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }

        }

        protected void UpdateIR_AValues(SPListItem imiItem, Boolean IsSaveAsDraft, SPWeb oWebSite)
        {
            try
            {
                bool IsInjury = false;

                if (imiItem != null)
                {

                    if (!String.IsNullOrEmpty(Convert.ToString(this.IncidentType_ddl.SelectedIndex)) && this.IncidentType_ddl.SelectedIndex > 0)
                        imiItem["TypeOfInjury"] = Convert.ToString(this.IncidentType_ddl.Items[this.IncidentType_ddl.SelectedIndex]);


                    if (!String.IsNullOrEmpty(Convert.ToString(this.EmployeeType_ddl.SelectedIndex)) && this.EmployeeType_ddl.SelectedIndex > 0)
                        imiItem["EmployeeType"] = Convert.ToString(this.EmployeeType_ddl.Items[this.EmployeeType_ddl.SelectedIndex]);


                    if (!String.IsNullOrEmpty(Convert.ToString(this.IncidentCategory_hdn.Value)))
                    {
                        imiItem["IncidentCategory"] = Convert.ToString(this.IncidentCategory_hdn.Value);

                    }


                    if (!String.IsNullOrEmpty(Convert.ToString(this.Unit_Area_hdn.Value)))
                        imiItem["Unit/Area"] = (Convert.ToString(this.Unit_Area_hdn.Value));



                    if (!String.IsNullOrEmpty(Convert.ToString(this.DateOfIncident_dtc.SelectedDate)))
                    {
                        DateTime date;
                        bool bValid = DateTime.TryParse(this.DateOfIncident_dtc.SelectedDate.ToShortDateString(), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);


                        if (bValid)
                            imiItem["DateOfIncident"] = date;
                        else
                            imiItem["DateOfIncident"] = Convert.ToDateTime(this.DateOfIncident_dtc.SelectedDate);
                    }



                    if (!String.IsNullOrEmpty(Convert.ToString(this.TimeOfIncident_dtc.SelectedDate)))
                        imiItem["TimeOfIncident"] = this.TimeOfIncident_dtc.SelectedDate.ToShortTimeString();
                    else
                        imiItem["TimeOfIncident"] = null;


                    if (!String.IsNullOrEmpty(Convert.ToString(this.Title_tf.Value)))
                        imiItem["TitleOfIncident"] = Convert.ToString(this.Title_tf.Value);


                    if (!String.IsNullOrEmpty(Convert.ToString(this.InjuryTo_ddl.SelectedIndex)) && this.InjuryTo_ddl.SelectedIndex > 0)
                        imiItem["InjuryTo"] = Convert.ToString(this.InjuryTo_ddl.Items[this.InjuryTo_ddl.SelectedIndex]);

                    if (this.NameOfContractor_PeopleEditor.ResolvedEntities != null && this.NameOfContractor_PeopleEditor.ResolvedEntities.Count > 0)
                    {
                        PickerEntity ContractorEntity = (PickerEntity)this.NameOfContractor_PeopleEditor.ResolvedEntities[0];

                        imiItem["ContractorName"] = ContractorEntity.Claim.Value;
                    }


                    if (this.NameOfInjured_PeopleEditor.ResolvedEntities != null && this.NameOfInjured_PeopleEditor.ResolvedEntities.Count > 0)
                    {
                        PickerEntity InjurdEntity = (PickerEntity)this.NameOfInjured_PeopleEditor.ResolvedEntities[0];

                        imiItem["NameOfInjured"] = InjurdEntity.Claim.Value;
                    }

                    if (!String.IsNullOrEmpty(Convert.ToString(this.PNO_tf.Value)))
                        imiItem["PNo"] = Convert.ToString(this.PNO_tf.Value);

                    if (!String.IsNullOrEmpty(Convert.ToString(this.OccupationTrade_tf.Value)))
                        imiItem["OccupationTrade"] = Convert.ToString(this.OccupationTrade_tf.Value);


                    if (!String.IsNullOrEmpty(Convert.ToString(this.Injury_Section_hdn.Value)))
                    {
                        imiItem["EmployeeSection"] = Convert.ToString(this.Injury_Section_hdn.Value);

                    }

                    if (!String.IsNullOrEmpty(Convert.ToString(this.Injury_Department_hdn.Value)))
                    {
                        imiItem["EmployeeDepartment"] = Convert.ToString(this.Injury_Department_hdn.Value);

                    }

                    if (!String.IsNullOrEmpty(Convert.ToString(this.InjuryCategory_hdn.Value)))
                    {
                        imiItem["InjuryCategory"] = Convert.ToString(this.InjuryCategory_hdn.Value);

                    }


                    if (!String.IsNullOrEmpty(Convert.ToString(this.CauseOfInjury_hdn.Value)))
                    {
                        imiItem["CauseOfInjury"] = Convert.ToString(this.CauseOfInjury_hdn.Value);

                    }


                    if (!String.IsNullOrEmpty(Convert.ToString(this.TypeOfInjury_hdn.Value)))
                    {
                        imiItem["TypeOfInjury"] = Convert.ToString(this.TypeOfInjury_hdn.Value);

                    }


                    if (!String.IsNullOrEmpty(Convert.ToString(this.outside_cb.Checked)))
                        if (Convert.ToInt32(this.outside_cb.Checked) == 1)
                            imiItem["OutsideForMedical"] = "Yes";
                        else
                            imiItem["OutsideForMedical"] = "No";

                    if (this.MOName_PeopleEditor.ResolvedEntities != null && this.MOName_PeopleEditor.ResolvedEntities.Count > 0)
                    {
                        PickerEntity MOentity = (PickerEntity)this.MOName_PeopleEditor.ResolvedEntities[0];

                        imiItem["MOName"] = MOentity.Claim.Value;
                    }


                    if (!String.IsNullOrEmpty(Convert.ToString(this.MORemarks_ta.Value )))

                     imiItem["MORemarks"] = this.MORemarks_ta.Value;


                    if (!String.IsNullOrEmpty(Convert.ToString(this.Description_ta.Value)))

                        imiItem["DescriptionOfIncident"] = this.Description_ta.Value;


                    if (!String.IsNullOrEmpty(Convert.ToString(this.ReasoneSendingReportlate_ta.Value)))

                        imiItem["ReasonsForLate"] = this.ReasoneSendingReportlate_ta.Value;



                    if (this.SubmittedBy_PeopleEditor.ResolvedEntities != null && this.SubmittedBy_PeopleEditor.ResolvedEntities.Count > 0)
                    {
                        PickerEntity entity = (PickerEntity)this.SubmittedBy_PeopleEditor.ResolvedEntities[0];

                        imiItem["SubmittedBy"] = entity.Claim.Value;
                    }


                    if (!String.IsNullOrEmpty(Convert.ToString(this.SubmissionDate_dtc.SelectedDate)))
                    {
                        DateTime date;
                        bool bValid = DateTime.TryParse(this.SubmissionDate_dtc.SelectedDate.ToShortDateString(), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);


                        if (bValid)
                            imiItem["DateOFSubmission"] = date;
                        else
                            imiItem["DateOFSubmission"] = Convert.ToDateTime(this.SubmissionDate_dtc.SelectedDate);
                    }


                    String User = oWebSite.CurrentUser.LoginName;
                    String[] Name = User.Split('|');

                    if (Name.Length > 1)
                        imiItem["AuditedBy"] = Name[1];

                    if (IsSaveAsDraft)
                    {
                        if (Name.Length > 1)
                            imiItem["Assignee"] = Name[1];

                        imiItem["IsSaveAsDraft"] = true;

                        imiItem["Status"] = "Inprogress";

                        imiItem.Update();
                    }                   
                    else
                    {


                        imiItem["IsSaveAsDraft"] = false;



                        if (!String.IsNullOrEmpty(Convert.ToString(this.IncidentCategory_hdn.Value)))
                        {
                            String s = Convert.ToString(this.IncidentCategory_hdn.Value);

                            string[] IncidentCategoryItem = s.Split(',');

                            foreach (String Item in IncidentCategoryItem)
                            {
                                if (Item == "Injury")
                                    IsInjury = true;
                            }

                        }






                        if (IsInjury)
                        {
                            SPUser spMO = Utility.GetUser(oWebSite, Convert.ToString(imiItem["MOName"]));

                            User = spMO.LoginName;
                            Name = User.Split('|');
                            if (Name.Length > 1)
                                imiItem["Assignee"] = Name[1];

                            imiItem.Update();

                         //   SendEmailToMO(imiItem);
                        }
                        else
                        {

                            imiItem["Status"] = "Completed";

                            imiItem.Update();
                         // SendEmailToSupervisor(imiItem);
                         //   SendEmailToHSE(imiItem);



                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAOnJobForm->UpdateIR_AValues)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }

     
    }
}
