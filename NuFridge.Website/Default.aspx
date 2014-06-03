<%@ Page Title="Manage Feeds" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="NuFridge.Website._Default" %>

<asp:Content runat="server" ID="HeaderContent" ContentPlaceHolderID="HeaderContent">
    <script src="Scripts/ManageFeeds.js"></script>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h3>
        <a href="#" style="padding: 5px;"><strong><i class="glyphicon glyphicon-list"></i>Manage
            Feeds <span id="adminMode"></span></strong></a></h3>
    <hr>
    <div class="row">
        <div class="col-md-12" runat="server" id="adminButtons" visible="false">
            <div class="btn-group btn-group-justified">
                <a href="#" id="addFeedButton" class="btn btn-primary col-sm-3"><i class="glyphicon glyphicon-plus"></i>
                    <br>
                    Create NuGet Feed </a><a href="#" id="deleteFeedButton" class="btn btn-danger col-sm-3"><i
                        class="glyphicon glyphicon-remove"></i>
                        <br>
                        Delete Feed </a>
            </div>
        </div>
        <div class="col-md-5 nopaddingright">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4>NuGet Feeds</h4>
                </div>
                <div class="panel-body">
                    <p>This section allows you to view the list of NuGet feeds. </p>
                    <p>
                        Clicking on a feed in the grid below will display a section on the right for the feed details.
                    </p>
                    <table id="manageFeedTable" class="table table-condensed table-responsive table-bordered feedTable">
                        <thead>
                            <tr>
                                <th>Name
                                </th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
        </div>
        <div class="col-md-7 nopaddingleft" id="feedMoreDetailsPanel" style="display: none;">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4><span id="feedDetailsName" class="ellipsis"></span>
                    </h4>
                </div>
                <div class="panel-body">
                    <ul class="nav nav-tabs" id="myTab">
                        <li class="active"><a href="#general" data-toggle="tab">General</a></li>
                        <li><a href="#settings" data-toggle="tab">Search Packages</a></li>
                    </ul>
                    <div class="tab-content">
                        <div class="tab-pane active" id="general">
                            <p>
                                Total packages: <span id="feedDetailsTotalPackages"></span>
                            </p>
                            <p>
                                Packages to index: <span id="feedDetailsPackagesToIndex"></span>
                            </p>
                            <p>
                                Synchronizer State: <span id="feedDetailsSyncState"></span>
                            </p>
                            <p>
                                Indexer State: <span id="feedDetailsIndexerState"></span>
                            </p>
                            <p>
                                <span>Browse packages: <i class="glyphicon glyphicon-info-sign" data-toggle="tooltip" title="The URL to browse the feed packages"></i></span>

                                <br />
                                <span id="feedDetailsBrowsePackages" class="ellipsis"></span>
                            </p>
                            <p>
                                Download packages from: <i class="glyphicon glyphicon-info-sign" data-toggle="tooltip" title="The URL to use in Visual Studio / Octopus when downloading packages"></i>
                                <br />
                                <span id="feedDetailsDownloadPackages" class="ellipsis"></span>
                            </p>
                            <p>
                                Publish packages to: <i class="glyphicon glyphicon-info-sign" data-toggle="tooltip" title="The URL to use when uploading packages to the feed"></i>
                                <br />
                                <span id="feedDetailsPublishPackages" class="ellipsis"></span>
                            </p>
                            <p>
                                Symbol server: <i class="glyphicon glyphicon-info-sign" data-toggle="tooltip" title="The URL to the symbol server which can be added in Visual Studio"></i>
                                <br />
                                <span id="feedDetailsSymbolServer" class="ellipsis"></span>
                            </p>
                             <p runat="server" id="pnlImportPackages" Visible="false">
                                <button type="button" id="packageImportButton" class="btn btn-primary btn-sm">
                                    <span class="glyphicon glyphicon-upload"></span>&nbsp;Import Packages
                                </button>
                            </p>
                        </div>
                        <div class="tab-pane" id="settings">
                            <p>
                                Search Term:
                                <input type="text" id="packageSearchInput" />
                                <button type="button" id="packageSearchButton" class="btn btn-primary btn-sm">
                                    <span class="glyphicon glyphicon-search"></span>Search
                                </button>
                            </p>
                            <p id="searchingForPackages" style="display: none;">Searching for packages...</p>
                            <p id="noPackagesFoundOnSearch" style="display: none;">No packages were found matching the search term</p>
                            <p id="noPackagesFoundErrorOnSearch" style="display: none;">An error occurred trying to search for packages</p>
                            <table id="packageSearchTable" class="table table-condensed table-responsive table-bordered" style="display: none;">
                                <thead>
                                    <tr>
                                        <th>Package Name
                                        </th>
                                        <th>Version
                                        </th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
