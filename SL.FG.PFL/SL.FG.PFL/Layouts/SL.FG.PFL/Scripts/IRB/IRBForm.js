
$(document).ready(function () {

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

    function deleteRecommendation() {
        if ($('[id$=hdnIsChangesAllowed]').val() == "0") {
            return;
        }
        var par = $(this).closest('tr');
        par.remove();

        var count = $("[id$=recommendationDetails_table] tr.recommendationItem").length;
        $('[id$=noOfRecommendations_span]').text(count);
    }

    function deleteKeyFindings() {
        if ($('[id$=hdnIsChangesAllowed]').val() == "0") {
            return;
        }
        var par = $(this).closest('tr');
        par.remove();

        var count = $("[id$=keyFindings_table] tr.keyFindingsItem").length;
        $('[id$=noOfKeyFindings_span]').text(count);
    }

    function deletePeopleInterviewed() {
        if ($('[id$=hdnIsChangesAllowed]').val() == "0") {
            return;
        }
        var par = $(this).closest('tr');
        par.remove();

        var count = $("[id$=peopleInterviewed_table] tr.peopleInterviewedItem").length;
        $('[id$=noOfPeopleInterviewed_span]').text(count);
    }

    function deleteRootCauses() {
        if ($('[id$=hdnIsChangesAllowed]').val() == "0") {
            return;
        }
        var par = $(this).closest('tr');
        par.remove();

        var count = $("[id$=rootCauses_table] tr.rootCausesItem").length;
        $('[id$=noOfRootCauses_span]').text(count);
    }

    function updateRecommendationControls() {
        var html = $("[id$=responsiblePerson_PeopleEditor]").html();
        var emailList = extractEmails(html);

        if (emailList != 'undefined' && emailList != "" && emailList != null) {
            $('#responsiblePersonEmail_tf').val(emailList[0]);
        }

        var username = $('[id*=responsiblePerson_PeopleEditor_TopSpan_i]').attr('sid');

        if (username != 'undefined' && username != null && username != "") {
            var temp = username.split('|');

            if (temp.length > 1) {
                username = temp[1];
            }
        }

        if (username != 'undefined' && username != "" && username != null) {
            $('#responsiblePersonUsername_hd').val(username);
        }
    }

    function addKeyFindings() {
        if ($('[id$=hdnIsChangesAllowed]').val() == "0") {
            return;
        }
        var keyFinding = $('[id$=keyFindings_tf]').val();

        if (keyFinding != 'undefined' && keyFinding != "") {

            var count = $("[id$=keyFindings_table] tr.keyFindingsItem").length + 1;
            var actions = "<span class='btn btn-default editKeyFindings'><i class='glyphicon glyphicon-pencil'></i></span><span class='btn btn-danger removeKeyFindings'><i class='glyphicon glyphicon-remove'></i></span>";
            var data = "<tr class='keyFindingsItem'><td>" + count + "</td><td><span class='keyFindingsDescription'>" + keyFinding + "</span></td><td>" + actions + "</td></tr>"
            $("[id$=keyFindings_table]").append(data);
            $('[id$=keyFindings_tf]').val("");

            $('[id$=noOfKeyFindings_span]').text(count);
        }
        else {
            alert("Please enter valid data");
            $('[id$=keyFindings_tf]').focus();
        }
    }

    function addPeopleInterviewed() {
        if ($('[id$=hdnIsChangesAllowed]').val() == "0") {
            return;
        }
        var peopleInterviewed = $('[id$=peopleInterviewed_tf]').val();

        if (peopleInterviewed != 'undefined' && peopleInterviewed != "") {

            var count = $("[id$=peopleInterviewed_table] tr.peopleInterviewedItem").length + 1;
            var actions = "<span class='btn btn-default editPeopleInterviewed'><i class='glyphicon glyphicon-pencil'></i></span><span class='btn btn-danger removePeopleInterviewed'><i class='glyphicon glyphicon-remove'></i></span>";
            var data = "<tr class='peopleInterviewedItem'><td>" + count + "</td><td><span class='peopleInterviewedDescription'>" + peopleInterviewed + "</span></td><td>" + actions + "</td></tr>"
            $("[id$=peopleInterviewed_table]").append(data);
            $('[id$=peopleInterviewed_tf]').val("");

            $('[id$=noOfPeopleInterviewed_span]').text(count);
        }
        else {
            alert("Please enter valid data");
            $('[id$=peopleInterviewed_tf]').focus();
        }
    }

    function addRootCauses() {
        if ($('[id$=hdnIsChangesAllowed]').val() == "0") {
            return;
        }
        var rootCauses = $('[id$=rootCauses_tf]').val();

        if (rootCauses != 'undefined' && rootCauses != "") {

            var count = $("[id$=rootCauses_table] tr.rootCausesItem").length + 1;
            var actions = "<span class='btn btn-default editRootCauses'><i class='glyphicon glyphicon-pencil'></i></span><span class='btn btn-danger removeRootCauses'><i class='glyphicon glyphicon-remove'></i></span>";
            var data = "<tr class='rootCausesItem'><td>" + count + "</td><td><span class='rootCausesDescription'>" + rootCauses + "</span></td><td>" + actions + "</td></tr>"
            $("[id$=rootCauses_table]").append(data);
            $('[id$=rootCauses_tf]').val("");

            $('[id$=noOfRootCauses_span]').text(count);
        }
        else {
            alert("Please enter valid data");
            $('[id$=rootCauses_tf]').focus();
        }
    }

    $("[id$=recommendationDetails_table]").on("click", ".removeRecommendation", deleteRecommendation);
    $("[id$=rootCauses_table]").on("click", ".removeRootCauses", deleteRootCauses);
    $("[id$=peopleInterviewed_table]").on("click", ".removePeopleInterviewed", deletePeopleInterviewed);
    $("[id$=keyFindings_table]").on("click", ".removeKeyFindings", deleteKeyFindings);

    //Attachment remove
    $('span.removeLink').on('click', function () {
        var par = $(this).closest('tr');
        var fileName = par.find('span.fileName');

        if (fileName != 'undefined' && fileName != "" && fileName != null) {
            var filenames = $('[id$=hdnFilesNames]').val();
            filenames += "~" + fileName.text();

            $('[id$=hdnFilesNames]').val(filenames);
        }
        par.remove();
    });

    //Add Key Findings in grid
    $('[id$=keyFindings_span]').on('click', function () {
        addKeyFindings();
    });
    $('[id$=keyFindings_tf]').on('keypress', function (e) {
        if (e.which == 13) {
            addKeyFindings();
            e.preventDefault();
        }
    });

    //Add People Interviewed in grid
    $('[id$=peopleInterviewed_span]').on('click', function () {
        addPeopleInterviewed();
    });
    $('[id$=peopleInterviewed_tf]').on('keypress', function (e) {
        if (e.which == 13) {
            addPeopleInterviewed();
            e.preventDefault();
        }
    });

    //Add Root Causes in grid
    $('[id$=rootCauses_span]').on('click', function () {
        addRootCauses();
    });
    $('[id$=rootCauses_tf]').on('keypress', function (e) {
        if (e.which == 13) {
            addRootCauses();
            e.preventDefault();
        }
    });

    //Get username and email of the selected user
    $('[id$=responsiblePerson_PeopleEditor]').on('focusout', function () {
        updateRecommendationControls();
    });

    //Add Recommendation in Grid
    $('[id$=addRecommendation_btn]').on('click', function () {
        if ($('[id$=hdnIsChangesAllowed]').val() == "0") {
            return;
        }
        updateRecommendationControls();

        var controlList = '';
        var errorFlag = false;
        var message = '**** Please Provide value for the required fields ****';

        if ($('[id$=responsibleDepartment_ddl] option:selected').val() == "0") {
            errorFlag = true;
            var controlName = "Responsible Department";
            controlList += controlName + ": ";
        }
        if ($('[id$=responsibleSection_ddl] option:selected').val() == "0") {
            errorFlag = true;
            var controlName = "Responsible Section";
            controlList += controlName + ": ";
        }
        if ($("[id$=responsiblePersonUsername_hd]").val() == "") {
            errorFlag = true;
            var controlName = "Responsible Person";
            controlList += controlName + ": ";
        }
        if ($("[id$=targetDate_dtcDate]").val() == "") {
            errorFlag = true;
            var controlName = "Target Date";
            controlList += controlName + ": ";
        }
        if ($("[id$=description_ta]").val() == "") {
            errorFlag = true;
            var controlName = "Description";
            controlList += controlName + ": ";
        }

        if ($("[id$=targetDate_dtcDate]").val() == "") {
            errorFlag = true;
            var controlName = "Target Date";
            controlList += controlName + ": ";
        }

        if (errorFlag == false && $("[id$=targetDate_dtcDate]").val() != "") {
            try {
                var targetDate = convertStringToDate($("[id$=targetDate_dtcDate]").val());

                if (targetDate == null) {
                    errorFlag = true;
                    message = 'Target date must be Valid';
                }
                else {
                    if ($("[id$=FRTargetDate_dtcDate]").val() != "") {

                        var FRDate = convertStringToDate($("[id$=FRTargetDate_dtcDate]").val());

                        if (targetDate != null && FRDate != null && FRDate > targetDate) {
                            errorFlag = true;
                            alert("IR Recommendation/Suggestion Target Date should be greater than Flash Report Target Date");

                            message = "";
                        }
                    }
                }
            }
            catch (ex) {
                errorFlag = true;
                message += '**** Enter Valid Date ****';
            }
        }

        if (errorFlag == false) {

            var responsiblePersonUsername = $('[id$=responsiblePersonUsername_hd]').val()
            var responsiblePersonEmail = $('[id$=responsiblePersonEmail_tf]').val();
            var description = $('[id$=description_ta]').val();
            var status = $('[id$=status_ddl] option:selected').val();

            var responsibleDepartment = $('[id$=responsibleDepartment_ddl] option:selected').text();
            var responsibleDepartmentId = $('[id$=responsibleDepartment_ddl] option:selected').val();

            if (responsibleDepartmentId != 'undefined' && responsibleDepartmentId == "0") {
                responsibleDepartmentId = "0";
                responsibleDepartment = "";
            }

            var responsibleSection = $('[id$=responsibleSection_ddl] option:selected').text();
            var responsibleSectionId = $('[id$=responsibleSection_ddl] option:selected').val();

            if (responsibleSectionId != 'undefined' && responsibleSectionId == "0") {
                responsibleSectionId = "0";
                responsibleSection = "";
            }

            var recommendationId = $('#recommendationId_hd').val();;

            var type = "Recommendation";

            var selected1 = $("input[type='radio'][name='type']:checked");
            if (selected1.length > 0) {
                type = selected1.val();
            }

            var targetDate = $('[id$=targetDate_dtcDate]').val();


            //add recommendation in grid
            if (true) {
                var count = $("[id$=recommendationDetails_table] tr.recommendationItem").length + 1;
                var actions = "<span class='btn btn-default editRecommendation' ><i class='glyphicon glyphicon-pencil'></i></span><span class='btn btn-danger removeRecommendation'><i class='glyphicon glyphicon-remove'></i></span>";
                var data = "<tr class='recommendationItem'><td>" + count + "</td><td style='display:none;'><span class='recommendationId'>" + recommendationId + "</span></td><td class='td-description'><span class='description'>" + description + "</span></td><td><span class='username'>" + responsiblePersonUsername + "</span></td><td style='display:none;'><span class='email'>" + responsiblePersonEmail + "</td><td><span class='sectionName'>" + responsibleSection + "</span></td><td style='display:none'><span class='sectionId'>" + responsibleSectionId + "</span></td><td><span class='departmentName'>" + responsibleDepartment + "</span></td><td style='display:none'><span class='departmentId'>" + responsibleDepartmentId + "</span></td><td><span class='targetDate'>" + targetDate + "</span></td><td><span class='type'>" + type + "</span></td><td><span class='status'>" + status + "</span></td><td style='display:none;'><span class='recommendationNo'>" + "" + "</span></td><td>" + actions + "</td></tr>";
                $("[id$=recommendationDetails_table]").append(data);

                $('[id$=recommendationNo_tf]').val("");

                $('[id$=noOfRecommendations_span]').text(count);
            }


            $('[id$=description_ta]').val("");
            $('[id$=responsibleDepartment_ddl]').val("0");
            $('[id$=responsibleSection_ddl]').val("0");
            //clear client people picker
            $('.sp-peoplepicker-delImage[id$=_DeleteUserLink]').trigger('click');
            $('[id$=responsiblePersonEmail_tf]').val("");
            $('[id$=responsiblePersonUsername_hd]').val("");
            $('[id$=typeRecommendation_rb]').prop("checked", "checked");

            var currentDateTime = new Date();
            var currentDate = currentDateTime.format("dd/MM/yyyy");

            if (currentDate != null && currentDate != "") {
                $('[id$=targetDate_dtcDate]').val(currentDate);
            }

            $('#collapse3').collapse('hide');
            $('#panel-title3').attr('data-toggle', 'collapse');


            alert('IR-Recommendation/Suggestion added successfully!');
        }
        else
            ValidationSummary(message, controlList);
    });


    $('.panel-collapse').collapse('hide');
    $('.panel-title').attr('data-toggle', 'collapse');

    var count = $("[id$=keyFindings_table] tr.keyFindingsItem").length;
    $('[id$=noOfKeyFindings_span]').text(count);

    count = $("[id$=recommendationDetails_table] tr.recommendationItem").length;
    $('[id$=noOfRecommendations_span]').text(count);

    count = $("[id$=peopleInterviewed_table] tr.peopleInterviewedItem").length;
    $('[id$=noOfPeopleInterviewed_span]').text(count);

    count = $("[id$=rootCauses_table] tr.rootCausesItem").length;
    $('[id$=noOfRootCauses_span]').text(count);

    $('[id$=basicActivityInProgress_ddl]').on('change', function () {
        $("[id$=basicActivityInProgress_ddl]").each(function () {
            $("[id$=hdnBasicActivityInProgress]").val($(this).val());
        });
    });

    $('[id$=causeOfIncident_PR_ddl]').on('change', function () {
        $("[id$=causeOfIncident_PR_ddl]").each(function () {
            $("[id$=hdnCauseOfIncident_PR]").val($(this).val());
        });
    });

    $('[id$=causeOfIncident_ER_ddl]').on('change', function () {
        $("[id$=causeOfIncident_ER_ddl]").each(function () {
            $("[id$=hdnCauseOfIncident_ER]").val($(this).val());
        });
    });

    $('[id$=supervisionAtTimeOfIncident_ddl]').on('change', function () {
        $("[id$=supervisionAtTimeOfIncident_ddl]").each(function () {
            $("[id$=hdnSupervisionAtTimeOfIncident]").val($(this).val());
        });
    });

    $('[id$=procedureRelatedCause_Proc_R_ddl]').on('change', function () {
        $("[id$=procedureRelatedCause_Proc_R_ddl]").each(function () {
            $("[id$=hdnProcedureRelatedCause_Proc_R]").val($(this).val());
        });
    });

    $('[id$=procedureRelatedCause_Per_R_ddl]').on('change', function () {
        $("[id$=procedureRelatedCause_Per_R_ddl]").each(function () {
            $("[id$=hdnProcedureRelatedCause_Per_R]").val($(this).val());
        });
    });

    $('[id$=PSMsViolated_ddl]').on('change', function () {
        $("[id$=PSMsViolated_ddl]").each(function () {
            $("[id$=hdnPSMsViolated]").val($(this).val());
        });
    });

    $('[id$=resultantHealthEffect_ddl]').on('change', function () {
        $("[id$=resultantHealthEffect_ddl]").each(function () {
            $("[id$=hdnResultantHealthEffect]").val($(this).val());
        });
    });

    if ($("[id$=FRTargetDate_dtcDate]").val() != "" && $("[id$=approvalDate_dtcDate]").val() != "") {

        var FRDate = convertStringToDate($("[id$=FRTargetDate_dtcDate]").val());
        var ApprovalDate = convertStringToDate($("[id$=approvalDate_dtcDate]").val());

        if (ApprovalDate != null && FRDate != null && FRDate < ApprovalDate) {
            $("[id$=LateSubmition_spn]").show("fast");
        }
        else $("[id$=LateSubmition_spn]").hide("fast");
    }

    $(function () {
        if (!$.support.placeholder) {
            var active = document.activeElement;
            $(':text').focus(function () {
                if ($(this).attr('placeholder') != '' && $(this).val() == $(this).attr('placeholder')) {
                    $(this).val('').removeClass('hasPlaceholder');
                }
            }).blur(function () {
                if ($(this).attr('placeholder') != '' && ($(this).val() == '' || $(this).val() == $(this).attr('placeholder'))) {
                    $(this).val($(this).attr('placeholder')).addClass('hasPlaceholder');
                }
            });
            $(':text').blur();
            $(active).focus();
            $('form').submit(function () {
                $(this).find('.hasPlaceholder').each(function () { $(this).val(''); });
            });
        }
    });


    // Capturing when the user modifies a field
    var warnMessage = 'You have unsaved changes on this page!';
    var formModified = new Boolean();
    formModified = false;
    $('input:not(:button,:submit),textarea,select').on('change', function () {
        formModified = true;
    });
    // Checking if the user has modified the form upon closing window
    $('input:submit').on('click', function (e) {
        formModified = false;
    });
    window.onbeforeunload = function () {
        if (formModified != false) return warnMessage;
    }
});