using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;
using SL.FG.PFL.Layouts.SL.FG.PFL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;


namespace SL.FG.PFL.WebParts.IRRecommendationForm
{
    public partial class IRRecommendationFormUserControl : UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!String.IsNullOrEmpty(Page.Request.QueryString["IRB_ID"]))
                    {
                        this.hdnRecommendationId.Value = Page.Request.QueryString["IRB_ID"];
                        this.hdnParamName.Value = "IRB_ID";
                        this.hdnParentListName.Value = "IRB";
                        this.hdnRecommendationListName.Value = "IRRecommendationOnJob";
                        this.hdnFRIDName.Value = "FlashReportID";
                        this.hdnFlashReportName.Value = "FlashReport";
                    }
                    else if (!String.IsNullOrEmpty(Page.Request.QueryString["IRD_ID"]))
                    {
                        this.hdnRecommendationId.Value = Page.Request.QueryString["IRD_ID"];
                        this.hdnParamName.Value = "IRD_ID";
                        this.hdnParentListName.Value = "IRD";
                        this.hdnRecommendationListName.Value = "IRRecommendationOffJob";
                        this.hdnFRIDName.Value = "FRID";
                        this.hdnFlashReportName.Value = "FlashReportOff";
                    }

                    if (!String.IsNullOrEmpty(this.hdnRecommendationId.Value) && !String.IsNullOrEmpty(this.hdnRecommendationListName.Value) && !String.IsNullOrEmpty(this.hdnParentListName.Value))
                    {
                        int recommendationId;
                        string parentList = this.hdnParentListName.Value;
                        string recommendationList = this.hdnRecommendationListName.Value;

                        Int32.TryParse(this.hdnRecommendationId.Value, out recommendationId);

                        bool isSuccess = InitializeRecommendationControls(recommendationId, recommendationList, parentList);

                        DateTime date;
                        bool bValid = DateTime.TryParse(Convert.ToString(DateTime.Now), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);

                        if (bValid)
                        {
                            this.closureDate_dtc.SelectedDate = date;
                        }
                        else
                        {
                            this.closureDate_dtc.SelectedDate = DateTime.Now;
                        }

                        this.closureDate_dtc.Enabled = false;

                        if (isSuccess == false)
                        {
                            DisableControls();
                        }
                    }
                    else
                    {
                        DisableControls();//Set default values and restrict controls on the basis of situation
                    }
                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(RecommendationForm->Page_Load)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                message_div.InnerHtml = " { " + ex.Message + " } " + "  Please contact the administrator.";
                DisableControls();
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
        private void DisableControls()
        {
            //this.btnSave.Visible = false;
            this.btnSend.Visible = false;
            this.btnApprove.Visible = false;
            this.btnReject.Visible = false;
            this.btnWaiver.Enabled = false;

            this.closureJustification_ta.Disabled = true;

            this.hdnIsChangesAllowed.Value = "0";

            this.fileUploadControl.Enabled = false;

            this.approvalAuthority_ddl.Enabled = false;
            this.approvalAuthority_ddl.Attributes.Add("class", "formcontrol disableControl");
        }

        private ListItem FillApprovalAuthority(SPWeb oSPWeb, string departmentName)
        {
            ListItem hodLI = null;
            try
            {
                var currentUser = oSPWeb.CurrentUser;

                string currentUserEmail = null;
                string currentUserRole = null;

                if (currentUser != null)
                {
                    currentUserEmail = currentUser.Email;
                }
                string listName = "Department";

                // Fetch the List
                SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));

                SPQuery query = new SPQuery();
                SPListItemCollection spListItems;
                // Include only the fields you will use.
                query.ViewFields = "<FieldRef Name='HOD'/><FieldRef Name='HODEmail'/><FieldRef Name='DepartmentDescription'/>";
                query.ViewFieldsOnly = true;
                StringBuilder sb = new StringBuilder();
                sb.Append("<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + departmentName + "</Value></Eq></Where>");
                query.Query = sb.ToString();
                spListItems = spList.GetItems(query);

                List<ListItem> lstItems = new List<ListItem>();

                foreach (SPListItem spListItem in spListItems)
                {
                    string email = Convert.ToString(spListItem["HODEmail"]);
                    string name = Convert.ToString(spListItem["HOD"]);
                    string description = Convert.ToString(spListItem["DepartmentDescription"]);

                    if (currentUserEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
                    {
                        currentUserRole = description;
                    }


                    //string title = name + "  (" + description + ")  ";
                    string title = name;

                    if (!String.IsNullOrEmpty(title) && !String.IsNullOrEmpty(email))
                    {
                        lstItems.Add(new ListItem(title, email));

                        if (description.Equals("HOD", StringComparison.OrdinalIgnoreCase))
                        {
                            hodLI = new ListItem();
                            hodLI.Text = title;
                            hodLI.Value = email;
                        }
                    }
                }


                if (currentUserRole != null && (currentUserRole.Equals("Unit Manager", StringComparison.OrdinalIgnoreCase) || currentUserRole.Equals("HOD", StringComparison.OrdinalIgnoreCase)))
                {
                    this.approvalAuthority_ddl.Items.Add(hodLI);
                }
                else
                {
                    foreach (var item in lstItems)
                    {
                        this.approvalAuthority_ddl.Items.Add(new ListItem(item.Text, item.Value));
                    }
                }
                this.approvalAuthority_ddl.Items.Insert(0, new ListItem("Please Select", "0"));

                return hodLI;
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(RecommendationForm->FillApprovalAuthority)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
            return hodLI;
        }
        private bool InitializeRecommendationControls(int recommendationId, string recommendationList, string parentList)
        {
            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oSPWeb = oSPsite.OpenWeb())
                    {
                        if (!String.IsNullOrEmpty(recommendationList) && !String.IsNullOrEmpty(parentList))
                        {
                            string listName = recommendationList;

                            // Fetch the List
                            SPList spListR = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));

                            if (spListR != null)
                            {
                                SPListItem spListItemR = spListR.GetItemById(recommendationId);

                                if (spListItemR != null)
                                {
                                    bool isAllowed = true;
                                    bool isCompleted = false;
                                    SPUser responsiblePerson = null;

                                    //Check Permissions
                                    if (spListItemR["Assignee"] != null)
                                    {
                                        string assignee = Convert.ToString(spListItemR["Assignee"]);
                                        SPUser currentUser = oSPWeb.CurrentUser;

                                        if (currentUser != null && !Utility.CompareUsername(currentUser.LoginName, assignee))
                                        {
                                            isAllowed = false;
                                        }

                                        if (isAllowed == false)
                                        {
                                            if (CheckPermission())
                                            {
                                                DisableControls();
                                            }
                                            else
                                            {
                                                string accessDeniedUrl = Utility.GetRedirectUrl("Access_Denied");

                                                if (!String.IsNullOrEmpty(accessDeniedUrl))
                                                {
                                                    DisableControls();
                                                    Page.Response.Redirect(accessDeniedUrl, false);
                                                }
                                                return false;
                                            }
                                        }
                                    }

                                    if (spListItemR["Status"] != null)
                                    {
                                        string status = Convert.ToString(spListItemR["Status"]);

                                        if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                                        {
                                            isCompleted = true;
                                            this.approvedBy_div.Visible = true;
                                            DisableControls();
                                        }
                                    }

                                    if (spListItemR["Type"] != null)
                                    {
                                        string type = Convert.ToString(spListItemR["Type"]);

                                        if (type.Equals("Suggestion", StringComparison.OrdinalIgnoreCase))
                                        {
                                            this.type2_rb.Checked = true;
                                            this.recommendationNo_label.InnerText = "Suggestion No";
                                            this.hdnTypeName.Value = "Suggestion";
                                        }
                                    }

                                    if (spListItemR["TargetDate"] != null)
                                    {
                                        DateTime targetDate;
                                        bool bValid = DateTime.TryParse(Convert.ToString(spListItemR["TargetDate"]), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out targetDate);

                                        if (!bValid)
                                        {
                                            targetDate = Convert.ToDateTime(spListItemR["TargetDate"]);
                                        }

                                        this.targetDate_tf.Value = targetDate.ToShortDateString();
                                    }
                                    if (spListItemR["WaivedTargetDate1"] != null)
                                    {
                                        DateTime targetDate;
                                        bool bValid = DateTime.TryParse(Convert.ToString(spListItemR["WaivedTargetDate1"]), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out targetDate);

                                        if (!bValid)
                                        {
                                            targetDate = Convert.ToDateTime(spListItemR["WaivedTargetDate1"]);
                                        }

                                        this.waivedTargetDate1_tf.Value = targetDate.ToShortDateString();
                                        this.waivedTargetDate1_div.Visible = true;
                                    }
                                    if (spListItemR["WaivedTargetDate2"] != null)
                                    {
                                        DateTime targetDate;
                                        bool bValid = DateTime.TryParse(Convert.ToString(spListItemR["WaivedTargetDate2"]), new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out targetDate);

                                        if (!bValid)
                                        {
                                            targetDate = Convert.ToDateTime(spListItemR["WaivedTargetDate2"]);
                                        }

                                        this.waivedTargetDate2_tf.Value = targetDate.ToShortDateString();
                                        this.waivedTargetDate2_div.Visible = true;
                                        this.btnWaiver.Enabled = false;
                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemR["RecommendationNo"])))
                                    {
                                        this.recommendationNo_tf.Value = Convert.ToString(spListItemR["RecommendationNo"]);
                                    }
                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemR["Status"])))
                                    {
                                        this.status_ddl.Value = Convert.ToString(spListItemR["Status"]);
                                    }
                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemR["IRDescription"])))
                                    {
                                        this.description_ta.Value = Convert.ToString(spListItemR["IRDescription"]);
                                    }
                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemR["ResponsibleDepartment"])))
                                    {
                                        this.responsibleDepartment_tf.Value = Convert.ToString(spListItemR["ResponsibleDepartment"]);
                                    }
                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemR["ResponsibleSection"])))
                                    {
                                        this.responsibleSection_tf.Value = Convert.ToString(spListItemR["ResponsibleSection"]);
                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemR["ResponsibleSection"])))
                                    {
                                        this.responsibleSection_tf.Value = Convert.ToString(spListItemR["ResponsibleSection"]);
                                    }
                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemR["LastStatement"])))
                                    {
                                        this.lastStatement_ta.Value = Convert.ToString(spListItemR["LastStatement"]);
                                    }
                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemR["ClosureJustification"])))
                                    {
                                        string guessMePattern = "*|~^|^~|*";

                                        string justifications = Convert.ToString(spListItemR["ClosureJustification"]);
                                        this.history_div.InnerHtml = Utility.GetFormattedData(justifications, guessMePattern, false);
                                    }
                                    else
                                    {
                                        this.history_div.InnerHtml = "<p class='dataItem'>There is no history available.</p>";
                                    }

                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemR["ApprovedBy"])))
                                    {
                                        this.approvedBy_tf.Value = Convert.ToString(spListItemR["ApprovedBy"]);
                                    }

                                    foreach (String attachmentname in spListItemR.Attachments)
                                    {
                                        String attachmentAbsoluteURL =
                                        spListItemR.Attachments.UrlPrefix // gets the containing directory URL
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

                                    //Responsible Person
                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemR["ResponsiblePerson"])))
                                    {
                                        string username = Convert.ToString(spListItemR["ResponsiblePerson"]);

                                        responsiblePerson = Utility.GetUser(oSPWeb, username);

                                        if (responsiblePerson == null)
                                        {
                                            if (!String.IsNullOrEmpty(Convert.ToString(spListItemR["Assignee"])))
                                            {
                                                string tempUsername = Convert.ToString(spListItemR["Assignee"]);
                                                responsiblePerson = Utility.GetUser(oSPWeb, tempUsername);
                                            }
                                        }
                                        if (responsiblePerson != null)
                                        {
                                            // Clear existing users from control
                                            this.responsiblePerson_PeopleEditor.Entities.Clear();

                                            // PickerEntity object is used by People Picker Control
                                            PickerEntity UserEntity = new PickerEntity();

                                            // CurrentUser is SPUser object
                                            UserEntity.DisplayText = responsiblePerson.Name;
                                            UserEntity.Key = responsiblePerson.LoginName;

                                            // Add PickerEntity to People Picker control
                                            this.responsiblePerson_PeopleEditor.Entities.Add(this.responsiblePerson_PeopleEditor.ValidateEntity(UserEntity));
                                        }
                                    }


                                    //Status
                                    if (!String.IsNullOrEmpty(Convert.ToString(spListItemR["Status"])))
                                    {
                                        //Write some code here
                                    }

                                    //ResponsibleSection
                                    if (spListItemR["ResponsibleSection"] != null)
                                    {
                                        int sectionId = Convert.ToInt32(spListItemR["ResponsibleSection"]);

                                        listName = "Section";
                                        // Fetch the List
                                        SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));

                                        if (spList != null && sectionId > 0)
                                        {
                                            SPListItem spListItem = spList.GetItemById(sectionId);

                                            if (spListItem != null)
                                            {
                                                this.responsibleSection_tf.Value = Convert.ToString(spListItem["Title"]);
                                            }
                                        }
                                    }

                                    //ResponsibleDepartment
                                    if (spListItemR["ResponsibleDepartment"] != null)
                                    {
                                        int id = Convert.ToInt32(spListItemR["ResponsibleDepartment"]);

                                        listName = "Department";
                                        // Fetch the List
                                        SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));

                                        if (spList != null && id > 0)
                                        {
                                            SPListItem spListItem = spList.GetItemById(id);

                                            if (spListItem != null)
                                            {
                                                string departmentName = Convert.ToString(spListItem["Title"]);
                                                this.responsibleDepartment_tf.Value = departmentName;

                                                if (String.IsNullOrEmpty(this.approvedBy_tf.Value))
                                                {
                                                    var hodLI = FillApprovalAuthority(oSPWeb, departmentName);

                                                    var currentUser = oSPWeb.CurrentUser;

                                                    if (currentUser != null && hodLI != null && currentUser.Email.Equals(hodLI.Value, StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        this.approvalAuthority_ddl.SelectedValue = hodLI.Value;
                                                        this.approvalAuthority_ddl.Enabled = false;
                                                        this.approvalAuthority_ddl.Attributes.Add("class", "formcontrol disableControl");
                                                    }
                                                }
                                                else
                                                {
                                                    var user = Utility.GetUser(oSPWeb, null, this.approvedBy_tf.Value);
                                                    if (user != null)
                                                    {
                                                        this.approvalAuthority_ddl.Items.Add(new ListItem(user.Name, user.Email));

                                                        this.approvalAuthority_ddl.Enabled = false;
                                                        this.approvalAuthority_ddl.Attributes.Add("class", "formcontrol disableControl");

                                                        this.approvedBy_tf.Value = user.Name;

                                                        this.approvalAuthority_ddl.Items.Insert(0, new ListItem("Please Select", "0"));

                                                        this.approvalAuthority_ddl.DataBind();

                                                        this.approvalAuthority_ddl.SelectedValue = user.Email;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    //Parent List
                                    //Initiated By
                                    if (spListItemR["IRID"] != null)
                                    {
                                        int id = Convert.ToInt32(spListItemR["IRID"]);

                                        //Parent List
                                        listName = parentList;

                                        // Fetch the List
                                        SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));

                                        if (spList != null)
                                        {
                                            SPListItem spListItem = spList.GetItemById(id);

                                            if (spListItem != null)
                                            {
                                                if (!String.IsNullOrEmpty(this.hdnFRIDName.Value) && !String.IsNullOrEmpty(Convert.ToString(spListItem[this.hdnFRIDName.Value])))
                                                {
                                                    int flashReportId = 0;
                                                    string spListNameFR = null;
                                                    flashReportId = Convert.ToInt32(Convert.ToString(spListItem[this.hdnFRIDName.Value]));
                                                    spListNameFR = this.hdnFlashReportName.Value;

                                                    if (!String.IsNullOrEmpty(spListNameFR))
                                                    {
                                                        SPList spListFR = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, spListNameFR));

                                                        if (spListFR != null)
                                                        {
                                                            SPListItem spListItemFR = spListFR.GetItemById(flashReportId);

                                                            if (spListItemFR != null)
                                                            {
                                                                string username = Convert.ToString(spListItemFR["TeamLead"]);

                                                                var spUser = Utility.GetUser(oSPWeb, username);
                                                                if (spUser != null)
                                                                {
                                                                    this.initiatedBy_tf.Value = spUser.Name;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }


                                    //Update Control on the basis of current operation

                                    SPUser spCurrentUser = oSPWeb.CurrentUser;
                                    string approvingAuthorityEmail = null;

                                    if (this.approvalAuthority_ddl != null && this.approvalAuthority_ddl.Items.Count > 0)
                                    {
                                        approvingAuthorityEmail = this.approvalAuthority_ddl.SelectedValue;
                                    }

                                    //Case: Responsible Person is also Approving Authority
                                    if (isAllowed == true && isCompleted == false && responsiblePerson != null && !String.IsNullOrEmpty(responsiblePerson.Email) && responsiblePerson.Email.Equals(approvingAuthorityEmail, StringComparison.OrdinalIgnoreCase))
                                    {
                                        this.btnReject.Visible = false;
                                        this.btnApprove.Visible = true;
                                        this.btnSend.Visible = false;
                                        this.approvedBy_div.Visible = true;
                                        this.approvedBy_tf.Value = responsiblePerson.Name;
                                    }
                                    else if (isAllowed == true && isCompleted == false && spCurrentUser != null && !String.IsNullOrEmpty(approvingAuthorityEmail))
                                    {
                                        if (spCurrentUser.Email.Equals(approvingAuthorityEmail, StringComparison.OrdinalIgnoreCase))
                                        {
                                            this.btnApprove.Visible = true;
                                            this.btnReject.Visible = true;
                                            this.btnSend.Visible = false;
                                            //this.btnSave.Visible = true;
                                            this.approvedBy_div.Visible = true;
                                        }
                                        else
                                        {
                                            this.btnApprove.Visible = false;
                                            this.btnReject.Visible = false;
                                            this.btnSend.Visible = true;
                                            //this.btnSave.Visible = true;
                                            this.approvedBy_div.Visible = false;
                                        }
                                    }
                                    else
                                    {
                                        DisableControls();
                                    }

                                    bool isSavedAsDraft = Convert.ToBoolean(spListItemR["IsSavedAsDraft"]);

                                    if (isSavedAsDraft == true)
                                    {
                                        DisableControls();
                                    }
                                }
                            }
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(Recommendation->InitializeRecommendationControls)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                message_div.InnerHtml = "Something went wrong!!!  Please contact the administrator.";
                DisableControls();
            }

            return false;
        }
        private bool SaveRecommendation(string currentOperation, string listName, string paramName)
        {
            bool isSaved = false;

            try
            {
                using (SPSite oSPsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oSPWeb = oSPsite.OpenWeb())
                    {
                        if (!String.IsNullOrEmpty(this.hdnRecommendationId.Value))
                        {
                            int recommendationId = Convert.ToInt32(this.hdnRecommendationId.Value);

                            // Fetch the List
                            SPList spList = oSPWeb.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oSPWeb.Url, listName));

                            SPListItem spListItem = spList.GetItemById(recommendationId);

                            if (spListItem != null)
                            {
                                string closureJustification = this.closureJustification_ta.Value;

                                string closureDateStr = this.closureDate_dtc.SelectedDate != null ? Convert.ToString(this.closureDate_dtc.SelectedDate) : null;

                                string approvedBy = null;

                                if (approvalAuthority_ddl != null && approvalAuthority_ddl.SelectedIndex > 0)
                                {
                                    approvedBy = approvalAuthority_ddl.SelectedValue;
                                }

                                if (!String.IsNullOrEmpty(closureJustification))
                                {
                                    spListItem["LastStatement"] = closureJustification;

                                    StringBuilder sb = new StringBuilder();

                                    if (!String.IsNullOrEmpty(closureDateStr))
                                    {
                                        DateTime date;
                                        bool bValid = DateTime.TryParse(closureDateStr, new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);

                                        if (bValid)
                                        {
                                            closureDateStr = Convert.ToString(date);
                                        }
                                    }

                                    string previousCJ = Convert.ToString(spListItem["ClosureJustification"]);
                                    SPUser spUser = oSPWeb.CurrentUser;
                                    if (spUser != null)
                                    {
                                        string responsiblePerson = spUser.Name;

                                        string guessMePattern = "*|~^|^~|*";

                                        sb.Append("<p class='dataItem_by'>")
                                         .Append(responsiblePerson)
                                         .Append("<span class='dataItem_by_date'>")
                                         .Append(" (")
                                         .Append(closureDateStr)
                                         .Append(") ")
                                         .Append("</span>")
                                         .Append("</p>")
                                         .Append("<p class='dataItem'>")
                                         .Append(Convert.ToString(closureJustification))
                                         .Append("</p>")
                                         .Append(guessMePattern)
                                         .Append(previousCJ);

                                        spListItem["ClosureJustification"] = sb.ToString();
                                    }
                                }

                                if (!String.IsNullOrEmpty(closureDateStr))
                                {
                                    DateTime date;
                                    bool bValid = DateTime.TryParse(closureDateStr, new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out date);

                                    if (bValid)
                                    {
                                        spListItem["ClosureDate"] = date;
                                    }
                                    else
                                    {
                                        spListItem["ClosureDate"] = Convert.ToDateTime(closureDateStr);
                                    }
                                }

                                if (!String.IsNullOrEmpty(approvedBy))
                                {
                                    spListItem["ApprovedBy"] = approvedBy;
                                }
                                else
                                {
                                    message_div.InnerHtml = "Approving Authority not available!!!  Please contact the administrator.";
                                    return false;
                                }

                                if (!String.IsNullOrEmpty(this.hdnFilesNames.Value))
                                {
                                    var fileNames = hdnFilesNames.Value.Split('~');

                                    foreach (var item in fileNames)
                                    {
                                        if (!String.IsNullOrEmpty(item))
                                        {
                                            spListItem.Attachments.Delete(item);
                                        }
                                    }
                                }

                                if (this.fileUploadControl.HasFiles)
                                {
                                    foreach (var uploadedFile in fileUploadControl.PostedFiles)
                                    {
                                        int maxFileLimit = 20971520;

                                        if (uploadedFile.ContentLength > maxFileLimit)
                                        {
                                            message_div.InnerHtml = "Attachment file size limit is 20MB. Please reattach files.";
                                            isSaved = false;
                                            return isSaved;
                                        }
                                    }

                                    foreach (var uploadedFile in fileUploadControl.PostedFiles)
                                    {
                                        Stream fs = uploadedFile.InputStream;
                                        byte[] _bytes = new byte[fs.Length];
                                        fs.Position = 0;
                                        fs.Read(_bytes, 0, (int)fs.Length);
                                        fs.Close();
                                        fs.Dispose();

                                        spListItem.Attachments.Add(uploadedFile.FileName, _bytes);
                                    }
                                }

                                //Decide values on the basis of operation
                                string recommendationLink = Utility.GetRedirectUrl("IRRecommendationFormLink");

                                StringBuilder linkSB = new StringBuilder();
                                linkSB.Append(recommendationLink)
                                            .Append("?" + paramName + "=")
                                            .Append(spListItem.ID);

                                string subject = "";
                                string body = "";

                                if (currentOperation.Equals("Send", StringComparison.OrdinalIgnoreCase))
                                {
                                    subject = Utility.GetValueByKey("From_ResponsiblePerson_To_HOD_RE_Subject");

                                    if (String.IsNullOrEmpty(subject))
                                    {
                                        subject = "From_ResponsiblePerson_To_HOD_RE_Subject";
                                    }

                                    body = Utility.GetValueByKey("From_ResponsiblePerson_To_HOD_RE");
                                    body = body.Replace("~|~", linkSB.ToString());



                                    spListItem["Status"] = "In Progress";
                                }
                                else if (currentOperation.Equals("Approve", StringComparison.OrdinalIgnoreCase))
                                {
                                    subject = Utility.GetValueByKey("From_HOD_To_ResponsiblePerson_Approve_RE_Subject");
                                    body = Utility.GetValueByKey("From_HOD_To_ResponsiblePerson_Approve_RE");
                                    body = body.Replace("~|~", linkSB.ToString());

                                    if (String.IsNullOrEmpty(subject))
                                    {
                                        subject = "From_HOD_To_ResponsiblePerson_Approve_RE_Subject";
                                    }
                                    spListItem["Status"] = "Completed";
                                }
                                else if (currentOperation.Equals("Reject", StringComparison.OrdinalIgnoreCase))
                                {
                                    subject = Utility.GetValueByKey("From_HOD_To_ResponsiblePerson_Reject_RE_Subject");
                                    body = Utility.GetValueByKey("From_HOD_To_ResponsiblePerson_Reject_RE");
                                    body = body.Replace("~|~", linkSB.ToString());


                                    if (String.IsNullOrEmpty(subject))
                                    {
                                        subject = "From_HOD_To_ResponsiblePerson_Reject_RE_Subject";
                                    }
                                }


                                if (!currentOperation.Equals("Save", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (String.IsNullOrEmpty(body))
                                    {
                                        body = linkSB.ToString();
                                    }

                                    Message message = new Message();

                                    SPUser spCurrentUser = oSPWeb.CurrentUser;
                                    string approvingAuthorityEmail = Convert.ToString(spListItem["ApprovedBy"]);

                                    SPUser approvingAuthority = null;

                                    if (!String.IsNullOrEmpty(approvingAuthorityEmail))
                                    {
                                        approvingAuthority = Utility.GetUser(oSPWeb, null, approvingAuthorityEmail);

                                        if (approvingAuthority != null)
                                        {
                                            if (spCurrentUser.Email.Equals(approvingAuthorityEmail, StringComparison.OrdinalIgnoreCase))
                                            {
                                                SPUser responsiblePerson = Utility.GetUser(oSPWeb, Convert.ToString(spListItem["ResponsiblePerson"]));

                                                if (responsiblePerson != null)
                                                {
                                                    spListItem["Assignee"] = Convert.ToString(spListItem["ResponsiblePerson"]);
                                                    spListItem["AssigneeEmail"] = responsiblePerson.Email;

                                                    message.To = responsiblePerson.Email;
                                                    message.From = approvingAuthorityEmail;
                                                    message.Subject = subject;
                                                    message.Body = body;
                                                }
                                            }
                                            else
                                            {
                                                spListItem["Assignee"] = approvingAuthority.LoginName;
                                                spListItem["AssigneeEmail"] = approvingAuthority.Email;

                                                message.To = approvingAuthority.Email;
                                                message.From = spCurrentUser.Email;
                                                message.Subject = subject;
                                                message.Body = body;
                                            }

                                            oSPWeb.AllowUnsafeUpdates = true;
                                            spListItem.Update();
                                            oSPWeb.AllowUnsafeUpdates = false;

                                            isSaved = Email.SendEmail(message);

                                            if (!isSaved)
                                            {
                                                message_div.InnerHtml = "Recommendation Saved Successfully but Email Sending Failed, Please Contact your Administrator.";
                                            }
                                        }
                                    }

                                    if (approvingAuthority == null)
                                    {
                                        message_div.InnerHtml = "Information of Approving Authority is incomplete or needs more permission. Please Contact the Administrator!";
                                        isSaved = false;
                                    }
                                }
                                else
                                {
                                    oSPWeb.AllowUnsafeUpdates = true;
                                    spListItem.Update();
                                    oSPWeb.AllowUnsafeUpdates = false;

                                    isSaved = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(Recommendation->SaveRecommendation)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                message_div.InnerHtml = "Something went wrong!!!  Please contact the administrator.";
                DisableControls();
            }
            return isSaved;
        }
        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                bool isSaved = SaveRecommendation("Send", this.hdnRecommendationListName.Value, this.hdnParamName.Value);

                if (isSaved)
                {
                    string redirectUrl = Utility.GetRedirectUrl("RecommendationForm_Save_Redirect_" + this.hdnParentListName.Value);

                    if (String.IsNullOrEmpty(redirectUrl))
                    {
                        redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");
                    }

                    if (!String.IsNullOrEmpty(redirectUrl))
                    {
                        DisableControls();
                        Page.Response.Redirect(redirectUrl, false);
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(message_div.InnerHtml.Replace("\r", " ").Replace("\n", " ").Trim()))
                    {
                        message_div.InnerHtml = "Operation Save Failed. Kindly verify that you provide valid information.";
                    }
                    DisableControls();
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(RecommendationForm->btnSend_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                message_div.InnerHtml = "Something went wrong!!!  Please contact the administrator.";
                DisableControls();
            }
        }
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                bool isSaved = SaveRecommendation("Approve", this.hdnRecommendationListName.Value, this.hdnParamName.Value);

                if (isSaved)
                {
                    string redirectUrl = Utility.GetRedirectUrl("RecommendationForm_Approve_Redirect_" + this.hdnParentListName.Value);

                    if (String.IsNullOrEmpty(redirectUrl))
                    {
                        redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");
                    }

                    if (!String.IsNullOrEmpty(redirectUrl))
                    {
                        DisableControls();
                        Page.Response.Redirect(redirectUrl, false);
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(message_div.InnerHtml.Replace("\r", " ").Replace("\n", " ").Trim()))
                    {
                        message_div.InnerHtml = "Operation Save Failed. Kindly verify that you provide valid information.";
                    }
                    DisableControls();
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(RecommendationForm->btnApprove_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                message_div.InnerHtml = "Something went wrong!!!  Please contact the administrator.";
                DisableControls();
            }
        }
        protected void btnReject_Click(object sender, EventArgs e)
        {
            try
            {
                bool isSaved = SaveRecommendation("Reject", this.hdnRecommendationListName.Value, this.hdnParamName.Value);

                if (isSaved)
                {
                    string redirectUrl = Utility.GetRedirectUrl("RecommendationForm_Reject_Redirect_" + this.hdnParentListName.Value);

                    if (String.IsNullOrEmpty(redirectUrl))
                    {
                        redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");
                    }

                    if (!String.IsNullOrEmpty(redirectUrl))
                    {
                        DisableControls();
                        Page.Response.Redirect(redirectUrl, false);
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(message_div.InnerHtml.Replace("\r", " ").Replace("\n", " ").Trim()))
                    {
                        message_div.InnerHtml = "Operation Save Failed. Kindly verify that you provide valid information.";
                    }
                    DisableControls();
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(RecommendationForm->btnReject_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                message_div.InnerHtml = "Something went wrong!!!  Please contact the administrator.";
                DisableControls();
            }
        }
        protected void btnWaiver_Click(object sender, EventArgs e)
        {
            try
            {
                string waiverLink = null;

                if (this.hdnRecommendationListName.Value.Equals("IRRecommendationOnJob"))
                {
                    waiverLink = Utility.GetRedirectUrl("WaiverFormLink");
                }
                else
                {
                    waiverLink = Utility.GetRedirectUrl("WaiverFormLink");
                }

                string paramName = this.hdnParamName.Value;
                string id = this.hdnRecommendationId.Value;

                string redirectUrl = string.Format("{0}?{1}={2}", waiverLink, paramName, id);

                if (!String.IsNullOrEmpty(redirectUrl))
                {
                    DisableControls();
                    Page.Response.Redirect(redirectUrl, false);
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(RecommendationForm->btnWaiver_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                message_div.InnerHtml = "Something went wrong!!!  Please contact the administrator.";
                DisableControls();
            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                string redirectUrl = Utility.GetRedirectUrl("RecommendationForm_Cancel_Redirect_" + this.hdnParentListName.Value);

                if (String.IsNullOrEmpty(redirectUrl))
                {
                    redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");
                }

                if (!String.IsNullOrEmpty(redirectUrl))
                {
                    DisableControls();
                    Page.Response.Redirect(redirectUrl, false);
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(RecommendationForm->btnCancel_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                message_div.InnerHtml = "Something went wrong!!!  Please contact the administrator.";
                DisableControls();
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool isSaved = SaveRecommendation("Save", this.hdnRecommendationListName.Value, this.hdnParamName.Value);

                if (isSaved)
                {
                    string redirectUrl = Utility.GetRedirectUrl("RecommendationForm_Save_Redirect_" + this.hdnRecommendationListName.Value);

                    if (String.IsNullOrEmpty(redirectUrl))
                    {
                        redirectUrl = Utility.GetRedirectUrl("WorkQueue_Redirect");
                    }

                    if (!String.IsNullOrEmpty(redirectUrl))
                    {
                        DisableControls();
                        Page.Response.Redirect(redirectUrl, false);
                    }
                }
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(RecommendationForm->btnSave_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

                message_div.InnerHtml = "Something went wrong!!!  Please contact the administrator.";
                DisableControls();
            }
        }
    }
}
