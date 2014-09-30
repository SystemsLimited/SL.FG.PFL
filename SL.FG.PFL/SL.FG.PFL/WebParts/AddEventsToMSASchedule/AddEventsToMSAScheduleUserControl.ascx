<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddEventsToMSAScheduleUserControl.ascx.cs" Inherits="SL.FG.PFL.WebParts.AddEventsToMSASchedule.AddEventsToMSAScheduleUserControl" %>

<script type="text/javascript">

    function convertStringToDate(str) {
        try {
            var temp = str.split('/');

            if (temp.length > 2) {
                var d = new Date(temp[2], (parseInt(temp[1]) - 1), temp[0]);
                return d;
            }
        }
        catch (ex) {
        }
        return null;
    }

    function SaveMSASchedule() {        
        try {

            var flag = false;
            var message = "";
            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
                       
            var username = $("[id$=cppAuditotName] span.ms-entity-resolved").attr("title");

            if (username == null) {
                flag = true;
                message = 'Please enter Auditor name.';

            }

            var auditordesignation = $('[id$=txtAuditorDesignation]').val();

            if (auditordesignation == "" || auditordesignation == null) {
                flag = true;
                message = message + '\n' +
                    'Please enter Auditor designation.';
            }

            if ($('[id$=ddlAuditorDepartment] option:selected').val() == "0") {
                flag = true;
                message = message + '\n' +
                    'Please select Auditor department.';
            }

            if ($('[id$=ddlAuditorSection] option:selected').val() == "0") {
                flag = true;
                message = message + '\n' +
                    'Please select Auditor section.';
            }
            
            var startDate = convertStringToDate($("[id$=startdate_WIDate]").val());
            var endDate = convertStringToDate($("[id$=enddate_WIDate]").val());

            if (startDate == null || endDate == null)
            {
                flag = true;
                message = message = message + '\n' +
                    'Week I - Please enter at least first start date and end date.';
            }

            if (startDate != null && endDate != null && endDate < startDate) {
                flag = true;
                message = message + '\n' +
                    'Week I - End date must be greater than or equal to start date.';
            }

            if ((startDate != null && startDate < currentDate ) || (endDate != null && endDate < currentDate))  {
                flag = true;
                message = message + '\n' +
                    'Week I - Strat or End date must be greater than or equal to current date.';
            }

            if ($('[id$=ddlAreaToBeAudited_WI] option:selected').val() == "0") {
                flag = true;
                message = message + '\n' +
                    'Week I - Please select Area to be audited.';
            }

            startDate = convertStringToDate($("[id$=startdate_WIIDate]").val());
            endDate = convertStringToDate($("[id$=enddate_WIIDate]").val());

            if (startDate != null && endDate != null && endDate < startDate) {
                flag = true;
                message = message + '\n' +
                    'Week II - End date must be greater than or equal to start date.';
            }

            if ((startDate != null && startDate < currentDate) || (endDate != null && endDate < currentDate)) {
                flag = true;
                message = message + '\n' +
                    'Week II - Strat or End date must be greater than or equal to current date.';
            }

            if (startDate != null && endDate != null) {
                if ($('[id$=ddlAreaToBeAudited_WII] option:selected').val() == "0") {
                    flag = true;
                    message = message + '\n' +
                        'Week II - Please select Area to be audited.';
                }
            }

            startDate = convertStringToDate($("[id$=startdate_WIIIDate]").val());
            endDate = convertStringToDate($("[id$=enddate_WIIIDate]").val());
            if (startDate != null && endDate != null && endDate < startDate) {
                flag = true;
                message = message + '\n' +
                    'Week III - End date must be greater than or equal to start date.';
            }

            if ((startDate != null && startDate < currentDate) || (endDate != null && endDate < currentDate)) {
                flag = true;
                message = message + '\n' +
                    'Week III - Strat or End date must be greater than or equal to current date.';
            }

            if (startDate != null && endDate != null) {
                if ($('[id$=ddlAreaToBeAudited_WIII] option:selected').val() == "0") {
                    flag = true;
                    message = message + '\n' +
                        'Week III - Please select Area to be audited.';
                }
            }

            startDate = convertStringToDate($("[id$=startdate_WIVDate]").val());
            endDate = convertStringToDate($("[id$=enddate_WIVDate]").val());
            if (startDate != null && endDate != null && endDate < startDate) {
                flag = true;
                message = message + '\n' +
                    'Week IV - End date must be greater than or equal to start date.';
            }

            if ((startDate != null && startDate < currentDate) || (endDate != null && endDate < currentDate)) {
                flag = true;
                message = message + '\n' +
                    'Week IV - Strat or End date must be greater than or equal to current date.';
            }

            if (startDate != null && endDate != null) {
                if ($('[id$=ddlAreaToBeAudited_WIV] option:selected').val() == "0") {
                    flag = true;
                    message = message + '\n' +
                        'Week IV - Please select Area to be audited.';
                }
            }

            if (flag) {
                alert(message);
                return false;
            }
            else {
                return true;
            }
        }
        catch (ex) {
        }
    }
</script>

<div class="container">
    <div class="row">
        <div id="message_div" runat="server" class="messageDiv">
        </div>
        <div class="col-lg-12">
            <div class="panel panel-success">
                <div class="panel-heading">
                    <div class="row">
                        <div class="col-lg-9">
                            <h5>Add schedule to MSA</h5>
                        </div>                                
                     </div>
                </div>
                <div class="panel-body">
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>Auditor Name:</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <SharePoint:ClientPeoplePicker runat="server" ID="cppAuditotName" Rows="1" VisibleSuggestions="3" AllowMultipleEntities="false" PrincipalAccountType="User" />
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>Auditor Designation:</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <input type="text" id="txtAuditorDesignation" runat="server" />
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>Auditor Department:</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlAuditorDepartment" runat="server" CssClass="form-control" AutoPostBack="false" />
                            </div>
                        </div>
                    </div>

                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>Auditor Section:</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlAuditorSection" runat="server" CssClass="form-control" AutoPostBack="false" />
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>Start Date:</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <SharePoint:DateTimeControl ID="startdate_WI" runat="server" CssClassTextBox="form-control" AutoPostBack="false" IsRequiredField="true" DateOnly="true" LocaleId="2057"  />                                
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>End Date:</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <SharePoint:DateTimeControl ID="enddate_WI" runat="server" CssClassTextBox="form-control" AutoPostBack="false" IsRequiredField="true" DateOnly="true" LocaleId="2057" />
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>Area to be Audited:</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlAreaToBeAudited_WI" runat="server" CssClass="form-control" AutoPostBack="false" />
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>Start Date:</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <SharePoint:DateTimeControl ID="startdate_WII" runat="server" CssClassTextBox="form-control" AutoPostBack="false" IsRequiredField="true" DateOnly="true" LocaleId="2057" />
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>End Date:</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <SharePoint:DateTimeControl ID="enddate_WII" runat="server" CssClassTextBox="form-control" AutoPostBack="false" IsRequiredField="true" DateOnly="true" LocaleId="2057" />
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>Area to be Audited:</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlAreaToBeAudited_WII" runat="server" CssClass="form-control" AutoPostBack="false" />
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>Start Date:</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <SharePoint:DateTimeControl ID="startdate_WIII" runat="server" CssClassTextBox="form-control" AutoPostBack="false" IsRequiredField="true" DateOnly="true" LocaleId="2057" />
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>End Date:</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <SharePoint:DateTimeControl ID="enddate_WIII" runat="server" CssClassTextBox="form-control" AutoPostBack="false" IsRequiredField="true" DateOnly="true" LocaleId="2057" />
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>Area to be Audited:</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlAreaToBeAudited_WIII" runat="server" CssClass="form-control" AutoPostBack="false" />
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>Start Date:</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <SharePoint:DateTimeControl ID="startdate_WIV" runat="server" CssClassTextBox="form-control" AutoPostBack="false" IsRequiredField="true" DateOnly="true" LocaleId="2057" />
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>End Date:</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <SharePoint:DateTimeControl ID="enddate_WIV" runat="server" CssClassTextBox="form-control" AutoPostBack="false" IsRequiredField="true" DateOnly="true" LocaleId="2057" />
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>Area to be Audited:</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlAreaToBeAudited_WIV" runat="server" CssClass="form-control" AutoPostBack="false" />
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</label>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <asp:Button ID="btnSave" runat="server" Text="Upload to MSA Schedule" OnClick="btnSave_Click" OnClientClick="return SaveMSASchedule();" CssClass="btnSave" /> 
                            </div>
                        </div>
                    </div>

                </div>
            </div>
         </div>    
    </div>
</div>
<link href="/_layouts/15/SL.FG.PFL/CSS/FGStyle.css" rel="stylesheet" />
<script src="/_layouts/15/SL.FG.PFL/Scripts/jQuery.js"></script>
