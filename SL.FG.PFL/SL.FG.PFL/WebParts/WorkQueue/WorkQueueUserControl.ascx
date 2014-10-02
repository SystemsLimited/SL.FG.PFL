<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkQueueUserControl.ascx.cs" Inherits="SL.FG.PFL.WebParts.WorkQueue.WorkQueueUserControl" %>



<link href="/_layouts/15/SL.FG.PFL/CSS/FGStyle.css" rel="stylesheet" />

<div class="container">
    <div class="row">
        <div id="message_div" runat="server" class="messageDiv">
        </div>
        <div class="col-lg-12">
            <div class="panel panel-success">
                <div class="panel-heading">
                    <div class="row">
                        <div class="col-lg-9">
                            <h5>MSA</h5>
                        </div>
                        <div class="col-lg-3">
                            <span class="panel-title pull-right"
                                data-toggle="collapse"
                                data-target="#collapse_01">
                                <i class='glyphicon glyphicon-sort'></i>
                            </span>
                        </div>
                    </div>
                </div>
                <div id="collapse_01" class="panel-collapse collapse">
                    <div class="panel-body" style="overflow-y: scroll;">
                        <div class="col-lg-12">
                            <div class="panel panel-success">
                                <div class="panel-heading">
                                    <div class="row">
                                        <div class="col-lg-9">
                                            <h5>MSA Schedule</h5>
                                        </div>
                                        <div class="col-lg-3">
                                            <span class="panel-title pull-right"
                                                data-toggle="collapse"
                                                data-target="#collapse3">
                                                <i class='glyphicon glyphicon-sort'></i>
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div id="collapse3" class="panel-collapse collapse">
                                    <div class="panel-body" style="height: 200px; overflow-y: scroll;">
                                        <div class="row">
                                            <div style="margin: 10px;">
                                                <input type="text" id="searchInput3" placeholder="Search..." class="form-control" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <asp:GridView ID="grdMSAScheduled" runat="server" AutoGenerateColumns="false" CssClass="GridViewStyle" GridLines="Both" HeaderStyle-BackColor="AliceBlue" Width="100%" CellPadding="10" CellSpacing="10">
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-12">
                            <div class="panel panel-success">
                                <div class="panel-heading">
                                    <div class="row">
                                        <div class="col-lg-9">
                                            <h5>MSA (Saved as draft)</h5>
                                        </div>
                                        <div class="col-lg-3">
                                            <span class="panel-title pull-right"
                                                data-toggle="collapse"
                                                data-target="#collapse1">
                                                <i class='glyphicon glyphicon-sort'></i>
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div id="collapse1" class="panel-collapse collapse">
                                    <div class="panel-body" style="height: 200px; overflow-y: scroll;">
                                        <div class="row">
                                            <div style="margin: 10px;">
                                                <input type="text" id="searchInput1" placeholder="Search..." class="form-control" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <asp:GridView ID="grdMSATask" runat="server" AutoGenerateColumns="false" CssClass="GridViewStyle" GridLines="Both" HeaderStyle-BackColor="AliceBlue" Width="100%" CellPadding="10" CellSpacing="10">
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-12">
                            <div class="panel panel-success">
                                <div class="panel-heading">
                                    <div class="row">
                                        <div class="col-lg-9">
                                            <h5>MSA Recommendations</h5>
                                        </div>
                                        <div class="col-lg-3">
                                            <span class="panel-title pull-right"
                                                data-toggle="collapse"
                                                data-target="#collapse2">
                                                <i class='glyphicon glyphicon-sort'></i>
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div id="collapse2" class="panel-collapse collapse">
                                    <div class="panel-body" style="height: 200px; overflow-y: scroll;">
                                        <div class="row">
                                            <div style="margin: 10px;">
                                                <input type="text" id="searchInput2" placeholder="Search..." class="form-control" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <asp:GridView ID="grdMSARecommendationTask" runat="server" AutoGenerateColumns="false" CssClass="GridViewStyle" GridLines="Both" HeaderStyle-BackColor="AliceBlue" Width="100%" CellPadding="10" CellSpacing="10">
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-12">
            <div class="panel panel-success">
                <div class="panel-heading">
                    <div class="row">
                        <div class="col-lg-9">
                            <h5>IR-On-Job</h5>
                        </div>
                        <div class="col-lg-3">
                            <span class="panel-title pull-right"
                                data-toggle="collapse"
                                data-target="#collapse_02">
                                <i class='glyphicon glyphicon-sort'></i>
                            </span>
                        </div>
                    </div>
                </div>
                <div id="collapse_02" class="panel-collapse collapse">
                    <div class="panel-body" style="overflow-y: scroll;">
                        <div class="col-lg-12">
                            <div class="panel panel-success">
                                <div class="panel-heading">
                                    <div class="row">
                                        <div class="col-lg-9">
                                            <h5>IR-B On Job</h5>
                                        </div>
                                        <div class="col-lg-3">
                                            <span class="panel-title pull-right"
                                                data-toggle="collapse"
                                                data-target="#collapse_21">
                                                <i class='glyphicon glyphicon-sort'></i>
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div id="collapse_21" class="panel-collapse collapse">
                                    <div class="panel-body" style="height: 200px; overflow-y: scroll;">
                                        <div class="row">
                                            <div style="margin: 10px;">
                                                <input type="text" id="searchInput_21" placeholder="Search..." class="form-control" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <asp:GridView ID="grdIRBTasks" runat="server" AutoGenerateColumns="false" CssClass="GridViewStyle" GridLines="Both" HeaderStyle-BackColor="AliceBlue" Width="100%" CellPadding="10" CellSpacing="10">
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-12">
                            <div class="panel panel-success">
                                <div class="panel-heading">
                                    <div class="row">
                                        <div class="col-lg-9">
                                            <h5>IR Recommendation/Suggestion On Job</h5>
                                        </div>
                                        <div class="col-lg-3">
                                            <span class="panel-title pull-right"
                                                data-toggle="collapse"
                                                data-target="#collapse_22">
                                                <i class='glyphicon glyphicon-sort'></i>
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div id="collapse_22" class="panel-collapse collapse">
                                    <div class="panel-body" style="height: 200px; overflow-y: scroll;">
                                        <div class="row">
                                            <div style="margin: 10px;">
                                                <input type="text" id="searchInput_22" placeholder="Search..." class="form-control" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <asp:GridView ID="grdIRRecommendationsOnJob" runat="server" AutoGenerateColumns="false" CssClass="GridViewStyle" GridLines="Both" HeaderStyle-BackColor="AliceBlue" Width="100%" CellPadding="10" CellSpacing="10">
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<script src="/_layouts/15/SL.FG.PFL/Scripts/jQuery.js"></script>

<script src="/_layouts/15/SL.FG.PFL/Scripts/WorkQueue/WorkQueue.js"></script>



