<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FlashReportOnJobFormUserControl.ascx.cs" Inherits="SL.FG.PFL.WebParts.FlashReportOnJobForm.FlashReportOnJobFormUserControl" %>




<link href="/_layouts/15/SL.FG.FFL/CSS/FGStyle.css" rel="stylesheet" />

<div class="container">
    <div class="row">
        <div class="col-sm-12">
            <div class="panel panel-success">
                <div class="panel-heading">
                    Flash Report
                </div>
                <div class="panel-body">
                    <div class="form-group row">
                        <div class="col-lg-8">
                            <span class="col-lg-4">Flash Report ID:</span>
                            <span class="col-lg-4">PSI-01-MSE-2014 </span>
                        </div>
                        <div class="col-lg-4">
                            <a href="" runat="server" id="IR_link">View IR-1</a>
                        </div>
                    </div>
                    
                        <div class="form-group row">
                            <div class="col-lg-6">
                                <label>IR-I receiving Date</label>
                                <div class="form-group">
                                    <SharePoint:DateTimeControl ID="IR_IReceivingDate_dtc" runat="server" DateOnly="true" CssClassTextBox="form-control" AutoPostBack="false" UseTimeZoneAdjustment="false" LocaleId="2057" />
                                    <label id="IR_IReceivingDate_msg" hidden style="color: red">You can't leave this empty.</label>
                                </div>

                            </div>
                            <div class="col-lg-6">
                                <div class="form-group">
                                    <label>Flash issue Date</label>
                                    <div class="form-group">
                                        <SharePoint:DateTimeControl ID="FlashIssueDate_dtc" runat="server" DateOnly="true" CssClassTextBox="form-control" AutoPostBack="false" UseTimeZoneAdjustment="false" LocaleId="2057" />
                                        <label id="FlashIssueDate_msg" hidden style="color: red">You can't leave this empty.</label>
                                    </div>

                                </div>
                            </div>
                        </div>
                        <div class="form-group row">
                            <div class="col-lg-6">
                                <label>Unit/Section</label>
                                <select id="Unit_Section_ddl" class="select2 col-lg-12 form-control" runat="server">
                                </select>
                                <input class="form-control" id="Unit_Section_hdn" placeholder="Enter text" type="hidden" runat="server">
                                <label id="Unit_Section_msg" hidden style="color: red">You can't leave this empty.</label>
                            </div>
                            <div class="col-lg-6">
                                <label>Date of Incident</label>
                                <div class="form-group">
                                    <SharePoint:DateTimeControl ID="DateOfIncident_dtc" runat="server" DateOnly="true" CssClassTextBox="form-control" AutoPostBack="false" UseTimeZoneAdjustment="false" LocaleId="2057" />
                                    <label id="DateOfIncident_msg" hidden style="color: red">You can't leave this empty.</label>
                                </div>

                            </div>
                        </div>
                        <div class="form-group row">
                            <div class="col-lg-6">
                                <label>Time of Incident</label>
                                <div class="form-group">
                                    <SharePoint:DateTimeControl ID="TimeOfIncident_dtc" runat="server" TimeOnly="true" CssClassTextBox="form-control" AutoPostBack="false" />
                                    <label id="TimeOfIncident_msg" hidden style="color: red">You can't leave this empty.</label>
                                </div>

                            </div>
                            <div class="col-lg-6">
                                <label>Attachment</label>
                                <div>
                                    <table id="grdAttachments" runat="server">
                                    </table>
                                </div>
                                <asp:FileUpload ID="fileUploadControl" runat="server" AllowMultiple="true" />
                                <asp:HiddenField ID="hdnFilesNames" runat="server" Value="" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label>Description</label>
                            <textarea class="form-control" id="Description1_ta" runat="server"></textarea>
                            <label id="Description1_msg" hidden style="color: red">You can't leave this empty.</label>
                        </div>
                        <div class="form-group">
                            <label>Action taken</label>
                            <textarea class="form-control" id="ActionTaken_ta" runat="server"></textarea>
                            <label id="ActionTaken_msg" hidden style="color: red">You can't leave this empty.</label>
                        </div>


                        <div class="panel panel-success">
                            <div class="panel-heading">
                                Risk based classification of process incident
                            </div>
                            <div class="panel-body">
                                <div class="form-group row">
                                    <div class="col-lg-6">
                                        <label>Incident Score</label>
                                        <input type='text' class="form-control" id="IncidentScore_tf" runat="server" />
                                        <label id="IncidentScore_msg" hidden style="color: red">You can't leave this empty.</label>
                                    </div>
                                    <div class="col-lg-6">
                                        <label>Action Required</label>
                                        <asp:DropDownList ID="ActionRequired_Unit_ddl" runat="server" CssClass="form-control" AutoPostBack="false">
                                            <asp:ListItem>IR-1 with Section A & B Filled</asp:ListItem>
                                            <asp:ListItem>IR-3 with detailed RCA (attachment is must on IR-3 form)</asp:ListItem>

                                        </asp:DropDownList>
                                        <label id="ActionRequired_Unit_msg" hidden style="color: red">You can't leave this empty.</label>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <div class="col-lg-6">
                                        <label>Responsible Section/Unit</label>

                                        <select id="ResponsibleSection_Unit_ddl" class="select2 col-lg-12 form-control" runat="server">
                                        </select>
                                        <input class="form-control" id="ResponsibleSection_Unit_hdn" placeholder="Enter text" type="hidden" runat="server">
                                        <label id="ResponsibleSection_Unit_msg" hidden style="color: red">You can't leave this empty.</label>
                                    </div>
                                    <div class="col-lg-6">
                                        <label>Responsible Department</label>

                                        <select id="ResponsibleDepartmentt_ddl" class="select2 col-lg-12 form-control" runat="server">
                                        </select>
                                        <input class="form-control" id="ResponsibleDepartmentt_hdn" placeholder="Enter text" type="hidden" runat="server">
                                        <label id="ResponsibleDepartmentt_msg" hidden style="color: red">You can't leave this empty.</label>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <div class="col-lg-6">
                                        <label>Target Date</label>
                                        <div class="form-group">
                                            <SharePoint:DateTimeControl ID="TargetDate_dtc" runat="server" DateOnly="true" CssClassTextBox="form-control" AutoPostBack="false" UseTimeZoneAdjustment="false" LocaleId="2057" />
                                            <label id="TargetDate_msg" hidden style="color: red">You can't leave this empty.</label>
                                        </div>

                                    </div>
                                    <div class="col-lg-6">
                                        <div class="form-group">
                                            <label>Approving Authority</label>
                                            <SharePoint:ClientPeoplePicker runat="server" ID="ApprovingAuthority_PeopleEditor" Rows="1" VisibleSuggestions="3" AllowMultipleEntities="True" PrincipalAccountType="User" />
                                            <asp:HiddenField ID="ApprovingAuthority_hdn" runat="server" Value="" />
                                            <label id="ApprovingAuthority_msg" hidden style="color: red">You can't leave this empty.</label>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <div class="col-lg-6">
                                        <label>Team Lead</label>
                                        <SharePoint:ClientPeoplePicker runat="server" ID="TeamLead_PeopleEditor" Rows="1" VisibleSuggestions="3" AllowMultipleEntities="false" PrincipalAccountType="User" />
                                        <asp:HiddenField ID="TeamLead_hdn" runat="server" Value="" />
                                        <label id="TeamLead_msg" hidden style="color: red">You can't leave this empty.</label>
                                    </div>
                                    <div class="col-lg-6">
                                        <label>Team Members</label>
                                        <%-- <SharePoint:PeopleEditor runat="server" ID="TeamMembers_PeopleEditor" AllowEmpty="false" SelectionSet="User"
                                                Rows="1" MultiSelect="true" />--%>
                                        <SharePoint:ClientPeoplePicker runat="server" ID="TeamMembers_PeopleEditor" Rows="1" VisibleSuggestions="3" AllowMultipleEntities="True" PrincipalAccountType="User" />

                                        <asp:HiddenField ID="TeamMembers_hdn" runat="server" Value="" />
                                        <label id="TeamMembers_msg" hidden style="color: red">You can't leave this empty.</label>
                                    </div>

                                </div>
                                <div class="form-group row">
                                    <div class="col-lg-6">
                                    </div>
                                    <div class="col-lg-6">
                                        <label>
                                            Remarks
                                        </label>
                                        <div class="form-group">
                                            <textarea class="form-control" rows="6" id="Description2_ta" runat="server"></textarea>
                                            <label id="Description2_msg" hidden style="color: red">You can't leave this empty.</label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <asp:HiddenField ID="hdnIRID" Value="" runat="server" />

                        <div class="form-group pull-right">
                            <asp:Button ID="btnSaveAsDraft" runat="server" Text="Save As Draft" OnClick="btnSaveAsDraft_Click" CssClass="btnSaveAsDraft" />
                            <asp:Button ID="btnSave" runat="server" Text="Submit" OnClick="btnSave_Click" OnClientClick="return Save_Click();" CssClass="btnSave" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" CssClass="btnCancel" />
                        </div>
                   
                </div>
            </div>
        </div>
    </div>
</div>

<script src="/_layouts/15/SL.FG.FFL/Scripts/jQuery.js"></script>

<script src="/_layouts/15/SL.FG.FFL/Scripts/Validation/FlashReportOn.js"></script>

<script type="text/javascript">





    $('[id$=Unit_Section_ddl]').on('change', function () {

        $("[id$=Unit_Section_hdn]").val($(this).val());



    })

    $('[id$=ResponsibleSection_Unit_ddl]').on('change', function () {

        $("[id$=ResponsibleSection_Unit_hdn]").val($(this).val());


    })


    $('[id$=ResponsibleDepartmentt_ddl]').on('change', function () {

        $("[id$=ResponsibleDepartmentt_hdn]").val($(this).val());

    })


</script>

<script>

    $(document).ready(function () {

        $('[id$=IR_IReceivingDate_dtcDate] ').attr("disabled", "disabled");
    });

</script>

