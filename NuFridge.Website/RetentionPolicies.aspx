<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RetentionPolicies.aspx.cs" Inherits="NuFridge.Website.RetentionPolicies" %>
<asp:Content runat="server" ID="HeaderContent" ContentPlaceHolderID="HeaderContent">
    <script src="Scripts/RetentionPolicies.js"></script>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h3>
        <a href="#" style="padding: 5px;"><strong><i class="glyphicon glyphicon-cog"></i> Retention Policies</strong></a></h3>
    <hr>
    <div class="row" runat="server" id="editPanel">
        <div class="col-md-6">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4>
                        Edit Retention Policies</h4>
                </div>
                <div class="panel-body">
                    <p>
                        This section allows you to select a feed and edit its retention policy. 
                    </p><p>Click on a feed in the grid below, then edit the retention policy in the section to the right.</p>
                    <table id="retentionPolicyTable" class="table table-condensed table-responsive table-bordered feedTable">
                        <thead>
                            <tr>
                                <th>
                                    Name
                                </th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
        </div>
        <div class="col-md-6" id="feedMoreDetailsPanel" style="display: none;">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4>
                        <span id="feedDetailsName" class="ellipsis"></span>
                    </h4>
                </div>
                <div class="panel-body">
                    <ul class="nav nav-tabs" id="myTab">
                        <li class="active"><a href="#general" data-toggle="tab">Retention Policy</a></li>
                    </ul>
                    <div class="tab-content">
                        <div class="tab-pane active" id="general">
                                                    <p>
                             <span data-toggle="tooltip" title="Enables or disables the retention policy for packages on this feed"> Policy enabled:</span> <input type="checkbox" name="retentionPolicyEnabled" class="retentionPolicyEnabled">
                            </p>
                                                       <p>
                                 <span data-toggle="tooltip" title="The number of days to keep packages in this feed for before they are deleted"> Number of days to keep packages:</span> <input type="number" id="retentionPolicyDaysToKeep" value="0" min="0" max="365" />
                            </p>
                            <p>
                                 <span data-toggle="tooltip" title="The number of package versions to keep in this feed before they are deleted"> Number of package versions to keep: </span> <input type="number" id="retentionPolicyVersionsToKeep" value="0" min="0" max="100" />
                            </p>
                            <p>
                                 <span data-toggle="tooltip" title="A comma separated list of package ids to exclude (partial matches)"> Excluded packages:</span> 
                            </p>
                            <p>
                                                 <table id="excludedPackagesTable" class="table table-condensed table-responsive table-bordered">
                        <thead>
                            <tr>
                                <th>
                                    Package ID
                                </th>
                                <th>
                                    Exact Match
                                </th>
                                <th></th>
                            </tr>
                        </thead>
                        <tfoot>
                            <tr>
                                <td><a href="#" id="addExcludedPackageButton" class="btn btn-primary btn-sm"><i class="glyphicon glyphicon-plus">
                </i>

                    Add Package</a></td>
                            </tr>
                        </tfoot>
                    </table>
                            </p>
                            <p>
                                <button type="button" id="saveRetentionPolicyButton" class="btn btn-primary btn-sm"><i class="glyphicon glyphicon-ok">
                </i> Save</button>
                            </p>
                        <p id="saveRetentionPolicyButtonNumericError" style="display: none;">Please enter only numeric values in the 'Numbers of days to keep' field</p>
                        <p id="saveRetentionPolicyButtonFailedMessage" style="display: none;">An error occurred trying to save the retention policy</p>
                        <p id="saveRetentionPolicyButtonSuccessMessage" style="display: none;">Successfully updated the retention policy</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
        <div class="row">
        <div class="col-md-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4>
                        Retention Policy History</h4>
                </div>
                <div class="panel-body">
                    <p>
                        This section allows you to view the last time a retention policy was applied to a feed.
                    </p>
                    <p>Retention policies are set to be run at 11:00PM daily.</p>
                 <table id="retentionPolicyHistoryTable" class="table table-condensed table-responsive table-bordered">
                        <thead>
                            <tr>
                                <th>
                                    Date
                                </th>
                                <th>
                                    Feed
                                </th>
                                <th>Result</th>
                           <th>Packages deleted</th>
                                <th>Completed in</th>
                                <th></th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
        </div>
        </div>
</asp:Content>