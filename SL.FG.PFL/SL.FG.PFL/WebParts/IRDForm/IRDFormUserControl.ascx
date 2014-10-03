<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IRDFormUserControl.ascx.cs" Inherits="SL.FG.PFL.WebParts.IRDForm.IRDFormUserControl" %>


<link href="/_layouts/15/SL.FG.FFL/CSS/FGStyle.css" rel="stylesheet" />

<script type="text/javascript">
    function isActionConfirmed(action) {

        var message = "IR05 Detailed Investigation: Are you sure you want to perform this action?";

        if (typeof action != 'undefined' && action != null && action != "") {
            if (action == "Save") {
                message = "Do you want to Save IR05 Detailed Investigation?";
            }
            else if (action == "SaveAsDraft") {
                message = "Do you want to Save IR05 Detailed Investigation as Draft?";
            }
            else if (action == "Approve") {
                message = "Do you want to Approve IR05 Detailed Investigation?";
            }
            else if (action == "Reject") {
                message = "Do you want to Reject IR05 Detailed Investigation?";
            }
            else if (action == "Forward") {
                message = "Do you want to Forward IR05 Detailed Investigation?";
            }
            else if (action == "Submit") {
                message = "Do you want to Submit IR05 Detailed Investigation?";
            }
        }

        var confirm = window.confirm(message);
        if (!confirm) {
            return false;
        }
        return true;
    }

    //Extract Email from content
    function extractEmails(text) {
        return text.match(/([a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z0-9._-]+)/gi);
    }

    //Extract Username from content
    function extractUsernames(option) {
        var username;
        if (option == "1") {
            username = $("[id$=responsiblePerson_PeopleEditor] span.ms-entity-resolved").attr("title");
        }
        return username;
    }

    function ValidationHandler(control, controlName) {
        alert(controlName + ":  " + "Please Provide value for the required field.");
        control.focus();
    }

    function ValidationSummary(message, controls) {
        if (message != "" && controls != "") {
            alert("Message: " + message + "\n\n" + controls);
        }
    }

    function SaveIRDIDetails(isSavedAsDraft, action, validate) {
        var errorFlag = false;
        var controlList = '';
        var message = '';
        var validateResult = true;

        if (validate) {

            validateResult = Save_Click();
        }

        if (validateResult) {

            if (isSavedAsDraft == false) {
                if (errorFlag == true) {
                    message += "**** Please Provide value for the required fields ****";
                }

            }

            if (errorFlag == false) {

                if (!isActionConfirmed(action)) {
                    return false;
                }

                var recommendationList = '';
                $("[id$=recommendationDetails_table] tr.recommendationItem").each(function () {
                    $this = $(this)
                    var recommendationId = $this.find("span.recommendationId").html();
                    var recommendationNo = $this.find("span.recommendationNo").html();
                    var description = $this.find("span.description").html();
                    var username = $this.find("span.username").html();
                    var email = $this.find("span.email").html();
                    var sectionId = $this.find("span.sectionId").html();
                    var sectionName = $this.find("span.sectionName").html();
                    var departmentId = $this.find("span.departmentId").html();
                    var departmentName = $this.find("span.departmentName").html();
                    var targetDate = $this.find("span.targetDate").html();
                    var observationSpot = $this.find("span.type").html();
                    var status = $this.find("span.status").html();

                    if (typeof recommendationId == 'undefined') {
                        recommendationId = 0;
                    }
                    if (typeof recommendationNo == 'undefined') {
                        recommendationNo = "";
                    }
                    if (typeof description == 'undefined') {
                        description = "";
                    }
                    if (typeof username == 'undefined') {
                        username = "";
                    }
                    if (typeof email == 'undefined') {
                        email = "";
                    }
                    if (typeof sectionId == 'undefined') {
                        sectionId = '0';
                    }
                    if (typeof sectionName == 'undefined') {
                        sectionName = "";
                    }

                    if (typeof departmentId == 'undefined') {
                        departmentId = '0';
                    }
                    if (typeof departmentName == 'undefined') {
                        departmentName = "";
                    }
                    if (typeof targetDate == 'undefined') {
                        targetDate = "";
                    }
                    if (typeof type == 'undefined') {
                        type = "no";
                    }
                    if (typeof status == 'undefined') {
                        status = "";
                    }

                    recommendationList = recommendationList +
                        recommendationId + "*|*" +
                        description + "*|*" +
                        username + "*|*" +
                        email + "*|*" +
                        sectionId + "*|*" +
                        sectionName + "*|*" +
                        departmentId + "*|*" +
                        departmentName + "*|*" +
                        targetDate + "*|*" +
                        type + "*|*" +
                        status + "*|*" +
                        recommendationNo + "*|*" +
                        isSavedAsDraft + "~|~";
                });

                $("[id$=hdnRecommendationList]").val(recommendationList);
                return true;
            }
            else {
                ValidationSummary(message, controlList);
                return false;
            }

        }
        else {
            return false;
        }
    }
</script>


<style type="text/css">
    .ms-webpart-chrome-title {
        display: none !important;
    }

    .editRecommendation {
        display: none !important;
    }
</style>



<div class="container">
    <div class="row">
        <div class="col-sm-12">
            <div id="message_div" runat="server" class="messageDiv">
            </div>
            <div class="panel panel-success">
                <div class="panel-heading">
                    IR-05
                </div>
                <div class="panel-body">
                    <div class="form-group">
                        <div class="row">
                            <div class="col-lg-6">
                                <label>Incident Category<span style="color: red">&nbsp;*</span></label>
                                <select id="IncidentCategory_ddl" class="select2 col-lg-12 form-control" multiple="true" runat="server" visible="false">
                                </select>
                                <textarea id="IncidentCategory_ta" class="form-control" runat="server" readonly></textarea>
                                <label id="IncidentCategory_msg" hidden style="color: red">You can't leave this empty.</label>
                                <input class="form-control" id="IncidentCategory_hdn" placeholder="Enter text" type="hidden" runat="server" />
                            </div>
                            <div class="col-lg-6">
                                <label>Unit/Area of Incident<span style="color: red">&nbsp;*</span></label>
                                <br />
                                <select id="Unit_Area_ddl" class="select2 col-lg-12 form-control" runat="server" disabled>
                                </select>
                                <label id="Unit_Area_msg" hidden style="color: red">You can't leave this empty.</label>
                                <input class="form-control" id="Unit_Area_hdn" placeholder="Enter text" type="hidden" runat="server">
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <label>Date of Incident<span style="color: red">&nbsp;*</span></label>
                            <div class="form-group">
                                <SharePoint:DateTimeControl ID="DateOfIncident_dtc" runat="server" DateOnly="true" CssClassTextBox="form-control" AutoPostBack="false" UseTimeZoneAdjustment="false" LocaleId="2057" />
                                <label id="DateOfIncident_msg" hidden style="color: red">You can't leave this empty.</label>
                            </div>

                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>Time of Incident<span style="color: red">&nbsp;*</span></label>
                                <div class="form-group">
                                    <SharePoint:DateTimeControl ID="TimeOfIncident_dtc" runat="server" TimeOnly="true" CssClassTextBox="form-control" AutoPostBack="false" />
                                    <label id="TimeOfIncident_msg" hidden style="color: red">You can't leave this empty.</label>
                                </div>

                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="row">
                            <div class="col-lg-6">
                                <label>Title<span style="color: red">&nbsp;*</span></label>
                                <input type="text" id="Title_tf" class="form-control" placeholder="Enter text" runat="server" readonly>
                                <label id="Title_msg" hidden style="color: red">You can't leave this empty.</label>
                            </div>
                            <div class="col-lg-6">
                                <label>Employee Type<span style="color: red">&nbsp;*</span></label>
                                <select id="EmployeeType_ddl" class="form-control" runat="server" disabled>
                                    <option value="0">Please Select</option>
                                    <option>FFL</option>
                                    <option>Contractor</option>
                                </select>
                                <label id="EmployeeType_msg" hidden style="color: red">You can't leave this empty.</label>
                            </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="row">
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
                        <div class="col-lg-6">
                        </div>
                    </div>
                </div>

                <div id="Violation_div" style="display: none" runat="server">
                    <div class="panel panel-success">
                        <div class="panel-heading">
                            In case of traffic violation/vehicle incident
                        </div>
                        <div class="panel-body">
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-lg-6">
                                        <label>Employee Name<span style="color: red">&nbsp;*</span></label>
                                        <input type="text" class="form-control" id="EmployeeName_tf" placeholder="Enter text" readonly runat="server" />
                                    </div>
                                    <label id="EmployeeName_msg" hidden style="color: red">You can't leave this empty.</label>
                                    <div class="col-lg-6">
                                        <label>Violation by<span style="color: red">&nbsp;*</span></label>
                                        <SharePoint:ClientPeoplePicker runat="server" ID="ViolationBy_PeopleEditor" Rows="1" VisibleSuggestions="3" AllowMultipleEntities="false" PrincipalAccountType="User" />
                                        <label id="ViolationBy_msg" hidden style="color: red">You can't leave this empty.</label>
                                    </div>

                                </div>
                            </div>
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-lg-6">
                                        <label>Vehicle no<span style="color: red">&nbsp;*</span></label>
                                        <input type="text" class="form-control" id="VehicleNo_tf" placeholder="Enter text" runat="server">
                                        <label id="VehicleNo_msg" hidden style="color: red">You can't leave this empty.</label>
                                    </div>
                                    <div class="col-lg-6">
                                        <label>Vehicle Category<span style="color: red">&nbsp;*</span></label>
                                        <input type="text" class="form-control" id="VehicleCategory_tf" placeholder="Enter text" runat="server" />
                                        <label id="VehicleCategory_msg" hidden style="color: red">You can't leave this empty.</label>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-lg-6">
                                        <label>Type of Violation<span style="color: red">&nbsp;*</span></label>
                                        <select id="TypeOfViolation_ddl" class="form-control" runat="server">
                                            <option value="0">Please Select</option>
                                            <option>Over Speeding</option>
                                            <option>Under Age Driving</option>
                                            <option>Driving Without License</option>
                                            <option>Wrong Parking</option>
                                            <option>Driving Without Seatbelt</option>
                                            <option>Use Of Mobile Phone While Driving</option>
                                            <option>Driving Without Helmet</option>
                                            <option>Others</option>
                                        </select>
                                    </div>
                                    <label id="TypeOfViolation_msg" hidden style="color: red">You can't leave this empty.</label>
                                    <div class="col-lg-6">
                                        <label>Section<span style="color: red">&nbsp;*</span></label>
                                        <select id="Section_Violation_ddl" class="select2 col-lg-12 form-control" runat="server">
                                        </select>
                                        <input type="hidden" class="form-control" id="Violation_Section_hdn" placeholder="Enter text" runat="server" />
                                        <label id="Section_Violation_msg" hidden style="color: red">You can't leave this empty.</label>
                                    </div>

                                </div>
                            </div>
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-lg-6">
                                        <label>Department<span style="color: red">&nbsp;*</span></label>
                                        <select id="Department_Violation_ddl" class="select2 col-lg-12 form-control" runat="server">
                                        </select>
                                        <input type="hidden" class="form-control" id="Violation_Departmentt_hdn" placeholder="Enter text" runat="server" />
                                        <label id="Violation_Departmentt_msg" hidden style="color: red">You can't leave this empty.</label>
                                    </div>
                                    <div class="col-lg-6">
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
                <div id="Injury_div" style="display: none" runat="server">
                    <div class="panel panel-success">
                        <div class="panel-heading">
                            In case of injury
                        </div>
                        <div class="panel-body">
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-lg-6">
                                        <label>Name of Injured<span style="color: red">&nbsp;*</span></label>
                                        <SharePoint:ClientPeoplePicker runat="server" ID="NameOfInjured_PeopleEditor" Rows="1" VisibleSuggestions="3" AllowMultipleEntities="false" PrincipalAccountType="User" />
                                    </div>
                                    <label id="NameOfInjured_msg" hidden style="color: red">You can't leave this empty.</label>
                                    <div class="col-lg-6">
                                        <label>P/No.<span style="color: red">&nbsp;*</span></label>
                                        <input type="text" class="form-control" id="PNO_tf" placeholder="Enter text" runat="server" />
                                        <label id="PNO_msg" hidden style="color: red">You can't leave this empty.</label>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-lg-6">
                                        <label>Occupation/Trade<span style="color: red">&nbsp;*</span></label>
                                        <input type="text" class="form-control" id="OccupationTrade_tf" placeholder="Enter text" runat="server" />
                                        <label id="OccupationTrade_msg" hidden style="color: red">You can't leave this empty.</label>
                                    </div>
                                    <div class="col-lg-6">
                                        <label>Section<span style="color: red">&nbsp;*</span></label>
                                        <select id="Section_Injury_ddl" class="select2 col-lg-12 form-control" runat="server">
                                        </select>
                                        <input type="hidden" class="form-control" id="Injury_Section_hdn" placeholder="Enter text" runat="server" />
                                        <label id="Injury_Section_msg" hidden style="color: red">You can't leave this empty.</label>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-lg-6">
                                        <label>Department<span style="color: red">&nbsp;*</span></label>
                                        <select id="Department_Injury_ddl" class="select2 col-lg-12 form-control" runat="server">
                                        </select>
                                        <input type="hidden" class="form-control" id="Injury_Department_hdn" placeholder="Enter text" runat="server" />
                                        <label id="Injury_Department_msg" hidden style="color: red">You can't leave this empty.</label>
                                    </div>
                                    <div class="col-lg-6">
                                        <label>Injury Category<span style="color: red">&nbsp;*</span></label>
                                        <br />
                                        <select id="InjuryCategory_ddl" class="select2 col-lg-12 form-control" multiple="true" runat="server">
                                        </select>
                                        <textarea id="InjuryCategory_ta" class="form-control" visible="false" runat="server"></textarea>
                                        <label id="InjuryCategory_msg" hidden style="color: red">You can't leave this empty.</label>
                                        <input type="hidden" class="form-control" id="InjuryCategory_hdn" placeholder="Enter text" runat="server" />
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-lg-12">
                        <label>Description<span style="color: red">&nbsp;*</span></label>
                        <textarea id="Description_ta" class="form-control" runat="server"></textarea>
                        <label id="Description_msg" hidden style="color: red">You can't leave this empty.</label>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-lg-12">
                        <label>Action taken<span style="color: red">&nbsp;*</span></label>
                        <textarea id="ActionTaken_ta" class="form-control" runat="server"></textarea>
                        <label id="ActionTaken_msg" hidden style="color: red">You can't leave this empty.</label>
                    </div>
                </div>
                <!--Recommendation Started From Here-->
                <div class="panel panel-success">
                    <div class="panel-heading">
                        <div class="row">
                            <div class="col-lg-9">
                                <h5>Recommendation Entry</h5>
                            </div>
                            <div class="col-lg-3">
                                <span id="panel-title3" class="panel-title pull-right"
                                    data-toggle="collapse"
                                    data-target="#collapse3">
                                    <i class='glyphicon glyphicon-sort'></i>
                                </span>
                            </div>
                        </div>
                    </div>
                    <div id="collapse3" class="panel-collapse collapse">
                        <div class="panel-body">
                            <div class="form-group">
                                <label>Description</label>
                                <textarea class="form-control" id="description_ta"></textarea>
                            </div>
                            <div class="form-group row">
                                <div class="col-lg-6 table-responsive">
                                    <label>Responsible Person<span style="color: red">&nbsp;*</span></label>
                                    <SharePoint:ClientPeoplePicker runat="server" ID="responsiblePerson_PeopleEditor" Rows="1" VisibleSuggestions="3" AllowMultipleEntities="false" PrincipalAccountType="User" />
                                    <input id="responsiblePersonUsername_hd" type="hidden" value="" />
                                    <input id="recommendationId_hd" type="hidden" value="0" />
                                </div>
                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Responsible Department<span style="color: red">&nbsp;*</span></label>
                                        <select name="responsibleDepartment_ddl" class="form-control" id="responsibleDepartment_ddl" runat="server">
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Responsible Unit/Section</label>
                                        <select name="responsibleSection_ddl" class="form-control" id="responsibleSection_ddl" runat="server">
                                        </select>
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Responsible Person Email</label>
                                        <input id="responsiblePersonEmail_tf" type='text' class="form-control disabled" disabled />
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <div class="col-lg-6">
                                    <label>Do you have concurrence of responsible individuals on action items completion dates</label>
                                    <div class="form-group">
                                        <div class="form-inline col-lg-6">
                                            <label>Yes</label>
                                            <input type="radio" name="concurrenceOfRP" id="concurrenceOfRPYes_rb" value="Yes">
                                        </div>
                                        <div class="form-inline col-lg-6">
                                            <label>No</label>
                                            <input type="radio" name="concurrenceOfRP" id="concurrenceOfRPNo_rb" value="No" checked>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <label>Target Date<span style="color: red">&nbsp;*</span></label>
                                    <div class="form-group">
                                        <SharePoint:DateTimeControl ID="targetDate_dtc" runat="server" DateOnly="true" CssClassTextBox="form-control" AutoPostBack="false" UseTimeZoneAdjustment="false" LocaleId="2057" />
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <div class="col-lg-6">
                                    <label>Status</label>
                                    <select class="form-control" id="status_ddl" disabled>
                                        <option>Pending</option>
                                        <option>In Progress</option>
                                        <option>Completed</option>
                                    </select>
                                </div>
                                <div class="col-lg-6">
                                    <label>&nbsp</label>
                                    <div class="form-group">
                                        <input type="button" value="Add" class="btnAdd pull-right" id="addRecommendation_btn" />
                                    </div>
                                </div>
                            </div>
                            <div class="panel panel-success">
                                <div class="panel-body">
                                    <div class="form-group table-responsive" style="overflow-x: scroll;">
                                        <table class="table" id="recommendationDetails_table" runat="server">
                                            <tr>
                                                <th>No</th>
                                                <th>Description</th>
                                                <th>Responsible Person</th>
                                                <th>Responsible Unit</th>
                                                <th>Responsible Department</th>
                                                <th>Target Date</th>
                                                <th>Concurrence Of RP</th>
                                                <th>Status</th>
                                                <th>Actions</th>
                                            </tr>
                                        </table>
                                        No. of Recommendations <span id="noOfRecommendations_span" runat="server"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="panel-body">
                    <div class="form-group row" id="">
                        <div class="col-lg-6">
                            <label>Approval Authority<span style="color: red">&nbsp;*</span></label>
                            <input type='text' class="form-control" id="approvedBy_tf" runat="server" disabled />
                            <label id="approvedBy_msg" hidden style="color: red">You can't leave this empty.</label>
                        </div>
                        <div id="approvalDate_div" runat="server" style="display: none">
                            <div class="col-lg-6">
                                <label>Approval Date<span style="color: red">&nbsp;*</span></label>
                                <div class="form-group">
                                    <SharePoint:DateTimeControl ID="approvalDate_dtc" runat="server" DateOnly="true" CssClassTextBox="form-control" AutoPostBack="false" Enabled="false" UseTimeZoneAdjustment="false" LocaleId="2057" />
                                    <label id="approvalDate_msg" hidden style="color: red">You can't leave this empty.</label>
                                </div>
                            </div>
                            <div class="form-group row">
                                <div class="col-lg-6">
                                    <span class="errorMsg" id="LateSubmition_spn">Late Submission</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>


                <div class="panel panel-success" id="HSEDepartment_div" runat="server" visible="false">
                    <div class="panel-heading">
                        <div class="row">
                            <div class="col-lg-9">
                                <h5>HSE Department</h5>
                            </div>
                            <div class="col-lg-3">
                                <span class="panel-title pull-right"
                                    data-toggle="collapse"
                                    data-target="#collapse4">
                                    <i class='glyphicon glyphicon-sort'></i>
                                </span>
                            </div>
                        </div>
                    </div>
                    <div id="collapse4" class="panel-collapse collapse">
                        <div class="panel-body">
                            <div class="form-group row">
                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Report Reviewed (HSE Engineer/Unit Manager)<span style="color: red">&nbsp;*</span></label>
                                        <textarea class="form-control" id="rvf_reportViewed_ta" runat="server" rows="3" maxlength="30"></textarea>

                                        <label id="rvf_reportViewed_msg" hidden style="color: red">You can't leave this empty.</label>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label>UM HSE comments<span style="color: red">&nbsp;*</span></label>
                                <textarea class="form-control" id="UM_HSE_Comments_ta" runat="server" rows="3"></textarea>
                                <label id="UM_HSE_Comments_msg" hidden style="color: red">You can't leave this empty.</label>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-6" id="FRTagetDate_div" runat="server" style="display: none;">
                    <SharePoint:DateTimeControl ID="FRTargetDate_dtc" runat="server" DateOnly="true" CssClassTextBox="form-control" AutoPostBack="false" Enabled="false" UseTimeZoneAdjustment="false" LocaleId="2057" />
                </div>
                <!--Recommendation End Here-->

                <asp:HiddenField ID="hdnSentFrom" runat="server" Value="" />
                <asp:HiddenField ID="hdnIsChangesAllowed" runat="server" Value="1" />
                <asp:HiddenField ID="hdnApprovalAuthority" runat="server" Value="" />
                <asp:HiddenField ID="hdnFRID" runat="server" Value="" />
                <asp:HiddenField ID="hdnIR05ID" runat="server" Value="" />
                <asp:HiddenField ID="hdnIRR01DI_Id" runat="server" Value="" />
                <asp:HiddenField ID="hdnIdList" runat="server" Value="" />
                <asp:HiddenField ID="hdnKeyFindingsList" runat="server" Value="" />
                <asp:HiddenField ID="hdnPeopleInterviewedList" runat="server" Value="" />
                <asp:HiddenField ID="hdnRootCausesList" runat="server" Value="" />
                <asp:HiddenField ID="hdnRecommendationList" runat="server" Value="" />
                <asp:HiddenField ID="hdnFRTargetDate" runat="server" Value="" />
                <br />
                <br />

            </div>

        </div>


    </div>

</div>




<div class="form-group pull-right">
    <asp:Button ID="btnSaveAsDraft" runat="server" Text="Save" OnClick="btnSaveAsDraft_Click" OnClientClick="return SaveIRDIDetails(true, 'SaveAsDraft', false);" CssClass="btnSaveAsDraft" />
    <asp:Button ID="btnSave" runat="server" Visible="false" Text="Submit" OnClick="btnSave_Click" CssClass="btnSave" OnClientClick="return SaveIRDIDetails(true, 'Submit' ,true);" />
    <asp:Button ID="btnApprovingAuthoritySave" runat="server" Visible="false" Text="Save" OnClick="btnApprovingAuthoritySave_Click" CssClass="btnApprove" OnClientClick="return SaveIRDIDetails(true, 'Save' , true);" />
    <asp:Button ID="btnApprovingAuthorityApprove" runat="server" Visible="false" Text="Approve" OnClick="btnApprovingAuthorityApprove_Click" CssClass="btnApprove" OnClientClick="return SaveIRDIDetails(true, 'Approve', true);" />
    <asp:Button ID="btnApprovingAuthorityDisApprove" runat="server" Visible="false" Text="Reject" OnClick="btnApprovingAuthorityDisapprove_Click" CssClass="btnReject" OnClientClick="return SaveIRDIDetails(true, 'Reject' ,true);" />
    <asp:Button ID="btnHSEApprove" runat="server" Visible="false" Text="Approve And Send Recommendation" OnClick="btnHSEApprove_Click" CssClass="btnApprove" OnClientClick="return SaveIRDIDetails(false, 'Approve' ,true);" />
    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" CssClass="btnCancel" />
</div>



<script src="/_layouts/15/SL.FG.FFL/Scripts/jQuery.js"></script>
<script src="/_layouts/15/SL.FG.FFL/Scripts/MicrosoftAjax.js" type="text/javascript">
</script>
<script
    type="text/javascript"
    src="/_layouts/15/sp.runtime.js">
</script>
<script
    type="text/javascript"
    src="/_layouts/15/sp.js">
</script>



<script src="/_layouts/15/SL.FG.FFL/Scripts/IRR05DI/IR5OffForm.js"></script>



<script src="/_layouts/15/SL.FG.FFL/Scripts/IRR05DI/IR5OffForm_JSOM.js"></script>


<script src="/_layouts/15/SL.FG.FFL/Scripts/Validation/IR05.js"></script>


<script type="text/javascript">



    $('[id$=IncidentCategory_ddl]').on('change', function () {

        $("[id$=IncidentCategory_ddl]").each(function () {

            $("[id$=IncidentCategory_hdn]").val($(this).val());




        });



        var Injury = false;

        var array = $("[id$=IncidentCategory_hdn]").val().split(",");

        for (var i in array) {

            if (array[i] == "Injury") {

                Injury = true;

            }


        }

        if (Injury) {

            $("[id$=Violation_div]").hide("fast");

            $("[id$=Injury_div]").show("fast");

        }
        else {

            $("[id$=Injury_div]").hide("fast");

            $("[id$=Violation_div]").show("fast");
        }

    })




    $('[id$=InjuryCategory_ddl]').on('change', function () {

        $("[id$=InjuryCategory_ddl]").each(function () {

            $("[id$=InjuryCategory_hdn]").val($(this).val());

        });


    })


    $('[id$=Unit_Area_ddl]').on('change', function () {

        $("[id$=Unit_Area_hdn]").val($(this).val());

    })


    $('[id$=Section_Violation_ddl]').on('change', function () {

        $("[id$=Violation_Section_hdn]").val($(this).val());

    })

    $('[id$=Department_Violation_ddl]').on('change', function () {

        $("[id$=Violation_Departmentt_hdn]").val($(this).val());

    })


    $('[id$=Section_Injury_ddl]').on('change', function () {

        $("[id$=Injury_Section_hdn]").val($(this).val());

    })


    $('[id$=Department_Injury_ddl]').on('change', function () {

        $("[id$=Injury_Department_hdn]").val($(this).val());

    })



</script>

<script>

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

    $(document).ready(function () {

        $("[id$=LateSubmition_spn]").hide("fast");

        $('[id$=approvalDate_dtcDate]').attr("disabled", "disabled");

        if ($("[id$=FRTargetDate_dtcDate]").val() != "" && $("[id$=approvalDate_dtcDate]").val() != "") {

            var FRDate = convertStringToDate($("[id$=FRTargetDate_dtcDate]").val());
            var ApprovalDate = convertStringToDate($("[id$=approvalDate_dtcDate]").val());

            if (ApprovalDate != null && FRDate != null && FRDate < ApprovalDate) {
                $("[id$=LateSubmition_spn]").show("fast");
            }
            else $("[id$=LateSubmition_spn]").hide("fast");
        }

    });
</script>


