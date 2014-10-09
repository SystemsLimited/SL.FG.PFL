function init() {

    var clientContext1 = new SP.ClientContext();
    var targetListName1 = "Department";
    var targetList1 = clientContext1.get_web().get_lists().getByTitle(targetListName1);

    var query1 = "<View>\
                    <Query>\
                        <Where>\
                            <Eq>\
                                <FieldRef Name='DepartmentDescription' />\
                                <Value Type='Note'>HOD</Value>\
                            </Eq>\
                        </Where>\
                        <OrderBy>\
                            <FieldRef Name='Title' Ascending='TRUE'/>\
                        </OrderBy>\
                    </Query>\
                </View>";

    var camlQuery1 = new SP.CamlQuery();
    camlQuery1.set_viewXml(query1);

    var targetListItems1 = targetList1.getItems(camlQuery1);
    clientContext1.load(targetListItems1, 'Include(ID,Title)');

    clientContext1.executeQueryAsync(
        Function.createDelegate(this, function () { success_Department(targetListItems1); }),
        Function.createDelegate(this, this.failed));


    //var clientContext2 = new SP.ClientContext();
    //var targetListName2 = "Section";
    //var targetList2 = clientContext2.get_web().get_lists().getByTitle(targetListName2);

    //var query2 = "<View>\
    //                <Query>\
    //                    <OrderBy>\
    //                        <FieldRef Name='Title' Ascending='TRUE'/>\
    //                    </OrderBy>\
    //                </Query>\
    //            </View>";

    //var camlQuery2 = new SP.CamlQuery();
    //camlQuery2.set_viewXml(query2);

    //var targetListItems2 = targetList2.getItems(camlQuery2);

    //clientContext2.load(targetListItems2, 'Include(ID,Title)');

    //clientContext2.executeQueryAsync(
    //    Function.createDelegate(this, function () { success_Section(targetListItems2); }),
    //    Function.createDelegate(this, this.failed));
}



function success_Department(targetListItems) {

    var listItems = "<option value='0'>Please Select</option>";

    var listItemEnumerator = targetListItems.getEnumerator();

    var oListItem = listItemEnumerator.get_current();

    while (listItemEnumerator.moveNext()) {
        var oListItem = listItemEnumerator.get_current();
        var ID = oListItem.get_item('ID');
        var Title = oListItem.get_item('Title')
        listItems += "<option value=" + ID + ">" + Title + "</option>";
    }

    $('[id$=responsibleDepartment_ddl]').html(listItems);
}

function success_Section(targetListItems) {

    var listItems = "<option value='0'>Please Select</option>";

    var listItemEnumerator = targetListItems.getEnumerator();

    var oListItem = listItemEnumerator.get_current();

    while (listItemEnumerator.moveNext()) {
        var oListItem = listItemEnumerator.get_current();
        var ID = oListItem.get_item('ID');
        var Title = oListItem.get_item('Title')
        listItems += "<option value=" + ID + ">" + Title + "</option>";
    }
    $('[id$=responsibleSection_ddl]').html(listItems);

}

function success_ObservationB(targetListItems) {

    var listItems = "<option value='0'>Please Select</option>";

    var listItemEnumerator = targetListItems.getEnumerator();

    var oListItem = listItemEnumerator.get_current();

    while (listItemEnumerator.moveNext()) {
        var oListItem = listItemEnumerator.get_current();
        var ID = oListItem.get_item('Value');
        var Title = oListItem.get_item('Value');
        listItems += "<option value=" + ID + ">" + Title + "</option>";
    }

    $('[id$=observationCategoryB_ddl]').html(listItems);
}

function failed(sender, args) {
    //alert('Request failed. \nError: ' + args.get_message() + '\nStackTrace: ' + args.get_stackTrace());
}


init();