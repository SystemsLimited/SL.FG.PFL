using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using SL.FG.PFL.Layouts.SL.FG.PFL.Common;
using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace SL.FG.PFL.WebParts.IRAForm
{
    public partial class IRAFormUserControl : UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {



                    String IRAID = Page.Request.QueryString["IRAID"];

                    FillDropdowns();

                    if (!String.IsNullOrEmpty(IRAID))
                    {
                        // PageLoadOnUserBases();
                    }
                    else
                    {
                        this.btnSave.Visible = false;
                        this.btnSaveAsDraft.Visible = false;
                        //this.btnMOSave.Visible = false;
                        //this.btnHSESave.Visible = false;

                    }
                }

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAForm->Page_Load)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

            }
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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAForm->FillArea)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAForm->FillIncidentCategory)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAForm->FillInjuryCategory)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAForm->FillSection)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAForm->FillSection)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
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
                            //SPListItemCollection IR_1infoList = oWebSite.Lists["IR-1-Off"].Items;

                            string listName = "IRAOnJob";

                            // Fetch the List
                            SPList list = oWebSite.GetList(string.Format("{0}/Lists/{1}/AllItems.aspx", oWebSite.Url, listName));
                            SPListItem spListItem = null;

                            String IRID = Page.Request.QueryString["IRAID"];
                            int ItemID = Convert.ToInt32(IRID);

                            if (ItemID != 0 && list != null)
                            {

                                spListItem = list.Items.GetItemById(ItemID);

                                if (spListItem != null)
                                {

                                    //           UpdateIR_AValues(spListItem, false, oWebSite, false);

                                }
                            }

                            else if (list != null)
                            {
                                spListItem = list.Items.Add();


                                if (spListItem != null)
                                {

                                    //           UpdateIR_AValues(spListItem, false, oWebSite, false);

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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAForm->btnSave_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAForm->btnCancel_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);

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
                            //SPListItemCollection IR_1infoList = oWebSite.Lists["IR-1-Off"].Items;

                            string listName = "IR-1-Off";

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

                                    //       UpdateIR_1Values(spListItem, true, oWebSite, false);

                                }
                            }


                            else if (list != null)
                            {
                                spListItem = list.Items.Add();


                                if (spListItem != null)
                                {

                                    //      UpdateIR_1Values(spListItem, true, oWebSite, false);

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
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SL.FG.PFL(IRAForm->btnSaveAsDraft_Click)", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, ex.Message, ex.StackTrace);
            }
        }

        protected void DisableControls()
        {
            this.btnSave.Visible = false;
            this.btnSaveAsDraft.Visible = false;
        }

    }
}
