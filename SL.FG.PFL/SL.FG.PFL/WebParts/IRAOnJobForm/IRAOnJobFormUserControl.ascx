<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IRAOnJobFormUserControl.ascx.cs" Inherits="SL.FG.PFL.WebParts.IRAOnJobForm.IRAOnJobFormUserControl" %>



<link href="/_layouts/15/SL.FG.FFL/CSS/FGStyle.css" rel="stylesheet" />

<%--<script type="text/javascript">
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
        var validateResult = false;

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
                    var observationSpot = $this.find("span.concurrenceOfRP").html();
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
                    if (typeof concurrenceOfRP == 'undefined') {
                        concurrenceOfRP = "no";
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
                        concurrenceOfRP + "*|*" +
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
</script>--%>


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
                    Part A
                </div>
                <div class="panel-body">

                    <div class="form-group">
                        <div class="row">

                            <div class="col-lg-6">
                                <label>Incident Type<span style="color: red">&nbsp;*</span></label>
                                <select id="IncidentType_ddl" class="form-control" runat="server">
                                    <option value="0">Please Select</option>
                                    <option>Safety</option>
                                    <option>Environment</option>
                                </select>
                                <label id="IncidentType_msg" hidden style="color: red">You can't leave this empty.</label>
                            </div>
                            <div class="col-lg-6">
                                <label>Employee Type<span style="color: red">&nbsp;*</span></label>
                                <select id="EmployeeType_ddl" class="form-control" runat="server">
                                    <option value="0">Please Select</option>
                                    <option>PFL</option>
                                    <option>Contractor</option>
                                </select>
                                <label id="EmployeeType_msg" hidden style="color: red">You can't leave this empty.</label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="row">
                            <div class="col-lg-6">
                                <label>Incident Category<span style="color: red">&nbsp;*</span></label>
                                <select id="IncidentCategory_ddl" class="select2 col-lg-12 form-control" multiple="true" runat="server">
                                </select>
                                <textarea id="IncidentCategory_ta" class="form-control" runat="server" visible="false"></textarea>
                                <label id="IncidentCategory_msg" hidden style="color: red">You can't leave this empty.</label>
                                <input class="form-control" id="IncidentCategory_hdn" placeholder="Enter text" type="hidden" runat="server" />
                            </div>
                            <div class="col-lg-6">
                                <label>Unit/Area Of Incident<span style="color: red">&nbsp;*</span></label>
                                <br />
                                <select id="Unit_Area_ddl" class="select2 col-lg-12 form-control" runat="server">
                                </select>
                                <label id="Unit_Area_msg" hidden style="color: red">You can't leave this empty.</label>
                                <input class="form-control" id="Unit_Area_hdn" placeholder="Enter text" type="hidden" runat="server">
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-6">
                            <label>Date Of Incident<span style="color: red">&nbsp;*</span></label>
                            <div class="form-group">
                                <SharePoint:DateTimeControl ID="DateOfIncident_dtc" runat="server" DateOnly="true" CssClassTextBox="form-control" AutoPostBack="false" UseTimeZoneAdjustment="false" LocaleId="2057" />
                                <label id="DateOfIncident_msg" hidden style="color: red">You can't leave this empty.</label>
                            </div>

                        </div>
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>Time Of Incident<span style="color: red">&nbsp;*</span></label>
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
                                <input type="text" id="Title_tf" class="form-control" placeholder="Enter text" runat="server">
                                <label id="Title_msg" hidden style="color: red">You can't leave this empty.</label>
                            </div>
                            <div class="col-lg-6">
                            </div>
                        </div>
                    </div>


                </div>


                <div id="Injury_div" runat="server">
                    <div class="panel panel-success">
                        <div class="panel-heading">
                            In Case Of Injury
                        </div>
                        <div class="panel-body">
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-lg-6">
                                        <label>Injury To<span style="color: red">&nbsp;*</span></label>
                                        <select id="InjuryTo_ddl" class="form-control" runat="server" disabled>
                                            <option value="0">Please Select</option>
                                            <option>PFL</option>
                                            <option>Contractor</option>
                                        </select>
                                        <label id="InjuryTo_msg" hidden style="color: red">You can't leave this empty.</label>
                                    </div>
                                    <div class="col-lg-6">
                                        <label>Name Of Contractor<span style="color: red">&nbsp;*</span></label>
                                        <SharePoint:ClientPeoplePicker runat="server" ID="NameOfContractor_PeopleEditor" Rows="1" VisibleSuggestions="3" AllowMultipleEntities="false" PrincipalAccountType="User" />
                                    </div>
                                    <label id="NameOfContractor_msg" hidden style="color: red">You can't leave this empty.</label>
                                </div>
                            </div>

                            <div class="form-group">
                                <div class="row">
                                    <div class="col-lg-6">
                                        <label>Name Of Injured<span style="color: red">&nbsp;*</span></label>
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
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-lg-6">
                                        <label>Cause Of Injury<span style="color: red">&nbsp;*</span></label>
                                        <br />
                                        <select id="CauseOfInjury_ddl" class="select2 col-lg-12 form-control" multiple="true" runat="server">
                                        </select>
                                        <textarea id="CauseOfInjury_ta" class="form-control" visible="false" runat="server"></textarea>
                                        <label id="CauseOfInjury_msg" hidden style="color: red">You can't leave this empty.</label>
                                        <input type="hidden" class="form-control" id="CauseOfInjury_hdn" placeholder="Enter text" runat="server" />
                                    </div>
                                    <div class="col-lg-6">
                                        <label>Type Of Injury<span style="color: red">&nbsp;*</span></label>
                                        <br />
                                        <select id="TypeOfInjury_ddl" class="select2 col-lg-12 form-control" multiple="true" runat="server">
                                        </select>
                                        <textarea id="TypeOfInjury_ta" class="form-control" visible="false" runat="server"></textarea>
                                        <label id="TypeOfInjury_msg" hidden style="color: red">You can't leave this empty.</label>
                                        <input type="hidden" class="form-control" id="TypeOfInjury_hdn" placeholder="Enter text" runat="server" />
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <div class="row">
                                    <div class="form-group">
                                        <div class="col-lg-6">
                                            <div class="form-inline">
                                                <label>Person need to be send to outside medical facility &nbsp;&nbsp;</label>
                                                <input id="outside_cb" type="checkbox" runat="server">
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <%-- <div class="form-group">
                                                <label>Person need not to be send to outside medical facility &nbsp;&nbsp;</label>
                                                <input id="Notoutside_cb" type="checkbox" runat="server">
                                            </div>--%>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div id="MOName_div" runat="server">
                                <div class="form-group">

                                    <div class="row">
                                        <div class="col-lg-6">
                                            <label>MO Name<span style="color: red">&nbsp;*</span></label>
                                            <SharePoint:ClientPeoplePicker runat="server" ID="MOName_PeopleEditor" Rows="1" VisibleSuggestions="3" AllowMultipleEntities="false" PrincipalAccountType="User" />

                                            <label id="MOName_PeopleEditor_msg" hidden style="color: red">You can't leave this empty.</label>

                                            <input class="form-control" id="MOName_tf" placeholder="Enter text" visible="false" runat="server">
                                        </div>
                                        <div class="col-lg-6">

                                            <label id="MORemarks_ldl" runat="server">DR Comments<span id="MORemarks_str" runat="server" style="color: red">&nbsp;*</span></label>

                                            <textarea id="MORemarks_ta" class="form-control" runat="server"></textarea>
                                            <label id="MORemarks_msg" hidden style="color: red">You can't leave this empty.</label>

                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-lg-12">
                        <label>Description/Details Of Incident<span style="color: red">&nbsp;*</span></label>
                        <textarea id="Description_ta" class="form-control" runat="server"></textarea>
                        <label id="Description_msg" hidden style="color: red">You can't leave this empty.</label>
                    </div>
                </div>

                <div class="panel-body">
                    <div class="form-group row" id="">
                        <div class="col-lg-6">
                            <label>Date Of Occurence Of Incident<span style="color: red">&nbsp;*</span></label>
                            <div class="form-group">
                                <SharePoint:DateTimeControl ID="DateOfOccurenceOfIncident_dtc" runat="server" DateOnly="true" CssClassTextBox="form-control" AutoPostBack="false" UseTimeZoneAdjustment="false" LocaleId="2057" />
                                <label id="DateOfOccurenceOfIncident_msg" hidden style="color: red">You can't leave this empty.</label>
                            </div>

                        </div>

                        <div class="col-lg-6">

                            <label id="Label1" runat="server">Reasone for sending the report late<span id="Span1" runat="server" style="color: red">&nbsp;*</span></label>

                            <textarea id="ReasoneSendingReportlate_ta" class="form-control" runat="server"></textarea>
                            <label id="ReasoneSendingReportlate_msg" hidden style="color: red">You can't leave this empty.</label>

                        </div>

                        <div class="form-group row">
                            <span class="errorMsg" id="LateSubmition_spn">Late Submission</span>
                        </div>

                    </div>

                    <div class="form-group row">
                        <div class="col-lg-6 table-responsive">

                            <label>Submitted By<span style="color: red">&nbsp;*</span></label>
                            <div id="SubmittedBy_div" runat="server">
                                <SharePoint:ClientPeoplePicker runat="server" ID="SubmittedBy_PeopleEditor" Rows="1" VisibleSuggestions="3" AllowMultipleEntities="false" PrincipalAccountType="User" Enabled="false" />
                            </div>
                            <input class="form-control" id="SubmittedBy_tf" placeholder="Enter text" visible="false" runat="server">
                            <label id="SubmittedBy_PeopleEditor_msg" hidden style="color: red">You can't leave this empty.</label>
                        </div>
                        <div class="col-lg-6">
                            <label>Submission Date<span style="color: red">&nbsp;*</span></label>
                            <div class="form-group">
                                <SharePoint:DateTimeControl ID="SubmissionDate_dtc" runat="server" DateOnly="true" CssClassTextBox="form-control" AutoPostBack="false" UseTimeZoneAdjustment="false" LocaleId="2057" />
                            </div>
                            <label id="SubmissionDate_msg" hidden style="color: red">You can't leave this empty.</label>
                        </div>
                    </div>
                </div>



                <div class="col-lg-6" id="FRTagetDate_div" runat="server" style="display: none;">
                    <SharePoint:DateTimeControl ID="FRTargetDate_dtc" runat="server" DateOnly="true" CssClassTextBox="form-control" AutoPostBack="false" Enabled="false" UseTimeZoneAdjustment="false" LocaleId="2057" />
                </div>
                <!--Recomendation End Here-->

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




    <div class="form-group pull-right">
        <asp:Button ID="btnSaveAsDraft" runat="server" Text="Save" CssClass="btnSaveAsDraft" />
        <asp:Button ID="btnSave" runat="server" Text="Submit" CssClass="btnSave" />
         <asp:Button ID="btnMOSave" runat="server" Text="Submit" OnClick="btnMOSave_Click" Visible="false" CssClass="btnSave" />
        <%--    <asp:Button ID="btnApprovingAuthoritySave" runat="server" Visible="false" Text="Save" CssClass="btnApprove" />
        <asp:Button ID="btnApprovingAuthorityApprove" runat="server" Visible="false" Text="Approve" CssClass="btnApprove" />
        <asp:Button ID="btnApprovingAuthorityDisApprove" runat="server" Visible="false" Text="Reject" CssClass="btnReject" />
        <asp:Button ID="btnHSEApprove" runat="server" Visible="false" Text="Approve And Send Recomendation" CssClass="btnApprove" />--%>
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btnCancel" />
    </div>

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

    <%--
<script src="/_layouts/15/SL.FG.FFL/Scripts/Validation/IR05.js"></script>--%>



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

        $('[id$=CauseOfInjury_ddl]').on('change', function () {

            $("[id$=CauseOfInjury_hdn]").val($(this).val());

        })

        $('[id$=TypeOfInjury_ddl]').on('change', function () {

            $("[id$=TypeOfInjury_hdn]").val($(this).val());

        })

    </script>

    <%--<script>

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

    --%>
