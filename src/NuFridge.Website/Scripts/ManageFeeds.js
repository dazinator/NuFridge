$(function () {
    DisableFeedDependantButtons();
    GetFeeds();
    $('#deleteFeedButton').click(function () {
        ShowDeleteFeedModal();
    });
    $('#addFeedButton').click(function () {
        ShowAddFeedModal();
    });

    $("#packageSearchButton").click(function () {
        DoPackageSearch(this);
    });

    $('#packageSearchInput').bind("enterKey", function (e) {
        DoPackageSearch(this);
    });
    $('#packageSearchInput').keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).trigger("enterKey");
        }
    });

    $("#packageImportButton").click(function () {
        ShowImportPackagesModal(this);
    });

    $("[data-toggle=tooltip]").tooltip({ placement: 'top' });
});

var feedsDataTable;
var isAllowedToDeleteFeed;

var hubs = {};
var status = {};
var hub;
var isConnected;
var isConnecting;
var lastFeed;

function DisableFeedDependantButtons() {
    $('#deleteFeedButton').addClass('disabled');
    $('#deleteFeedButton').prop('disabled', true);
    $('#packageImportButton').addClass('disabled');
    $('#packageImportButton').prop('disabled', true);
}


function ConnectToSignalROnFeed(baseurl) {
    if (isConnected || isConnecting) {
        hub.connection.stop();
    }

    var signalR = $.signalR;

    $.ajax({
        url: baseurl + "/api/signalr/hubs",
        dataType: 'script',
        async: true
    }).done(function () {

        for (var i in signalR) {
            var prop = signalR[i];
            if (typeof prop === 'object' && 'hubName' in prop) {
                hubs[prop.hubName] = prop;
            }
        }

        hub = hubs["status"];

        var setStatusCallback = function (statusupdate) {
            status = statusupdate;
            if (statusupdate.totalPackages != null) {
                EnableFeedDependantButtons();
                $("#feedDetailsTotalPackages").text(statusupdate.totalPackages.toString());
            }
            if (statusupdate.packagesToIndex != null) {
                $("#feedDetailsPackagesToIndex").text(statusupdate.packagesToIndex.toString());
            }
            if (statusupdate.synchronizationState != null) {
                $("#feedDetailsSyncState").text(statusupdate.synchronizationState.toString());
            }
            if (statusupdate.indexingState != null) {
                $("#feedDetailsIndexerState").text(statusupdate.indexingState.toString());
            }
        };


        hub.client.updateStatus = setStatusCallback;

        hub.connection.stateChanged(function (change) {
            isConnected = change.newState === signalR.connectionState.connected;

            if (isConnected) {
                hub.server.getStatus().then(setStatusCallback);
            } else {
                setStatusCallback({});
            }
        });

        isConnecting = true;
        hub.connection.start().done(function () {
            // console.log('Connection Established.');
            isConnecting = false;
            // console.log('Connecting to SignalR indexing status hub', signalR.version, baseurl + "/api/signalr");

        }).fail(function () {
            isConnecting = false;
            $("#feedDetailsTotalPackages").text("0");
            //console.log('Could not connect'); 
        });
    });
}

function DoPackageSearch(sender) {
    var searchTerm = $("#packageSearchInput").val();
    $('#packageSearchTable tbody tr').remove();
    var url = $("#feedDetailsPublishPackages").text() + "?query=" + searchTerm + "&offset=0&count=10&originFilter=Any&sort=Score&Order=Ascending";


    $("#searchingForPackages").show();

    $.ajax({
        url: url,
        async: true
    }).done(function (response) {
        if (response.hits.length > 0) {
            $("#noPackagesFoundOnSearch").hide();
            $("#searchingForPackages").hide();
            $("#packageSearchTable").show();
            $("#noPackagesFoundErrorOnSearch").hide();

            var newrows;
            $(response.hits).each(function (index, hit) {
                var name = hit.id;
                if (hit.searchTitle != null) {
                    name = hit.searchTitle;
                }
                newrows += "<tr><td class=\"name\"><a href=\"" + $("#feedDetailsBrowsePackages").text() + "/#/packages/" + hit.id + "/" + hit.version + "\" target=\"_blank\">" + name + "</a></td><td class=\"version\">" + hit.version + "</td></tr>";
            });
            //Append rows outside of loop so we dont redraw the page multiple times adding to the DOM
            $('#packageSearchTable').append(newrows);

        } else {
            $("#noPackagesFoundOnSearch").show();
            $("#searchingForPackages").hide();
            $("#packageSearchTable").hide();
            $("#noPackagesFoundErrorOnSearch").hide();
        }
    }).fail(function () {
        $("#noPackagesFoundOnSearch").hide();
        $("#searchingForPackages").hide();
        $("#packageSearchTable").hide();
        $("#noPackagesFoundErrorOnSearch").show();
    });
}

function EnablePagingOnFeedsTable() {
    if (feedsDataTable) {
        return;
    }

    $.fn.dataTableExt.oApi.fnPagingInfo = function (oSettings) {
        return {
            "iStart": oSettings._iDisplayStart,
            "iEnd": oSettings.fnDisplayEnd(),
            "iLength": oSettings._iDisplayLength,
            "iTotal": oSettings.fnRecordsTotal(),
            "iFilteredTotal": oSettings.fnRecordsDisplay(),
            "iPage": oSettings._iDisplayLength === -1 ?
                0 : Math.ceil(oSettings._iDisplayStart / oSettings._iDisplayLength),
            "iTotalPages": oSettings._iDisplayLength === -1 ?
                0 : Math.ceil(oSettings.fnRecordsDisplay() / oSettings._iDisplayLength)
        };
    };

    $.fn.dataTableExt.oPagination.iFullNumbersShowPages = 3;

    feedsDataTable = $('#manageFeedTable').dataTable({
        "iDisplayStart": 0,
        "aLengthMenu": [[10, 20, 50, -1], [10, 20, 50, 'All']],
        "sPaginationType": "full_numbers",
        "aoColumns": [
			{ "sClass": "ellipsis" }
        ]
    });

    $(feedsDataTable).bind('draw', FeedTableDrawEvent);
}

function FeedTableDrawEvent() {
    lastFeed = null;
    DisableFeedDependantButtons();
    SetupFeedsRowClickEvent();

    $("#manageFeedTable tr").each(function (index, element) {
        $(element).removeClass("selected");
    });



    $('#feedMoreDetailsPanel').hide();
    $("#feedMoreDetailsPanel .panel-body").hide();
}

function GetFeedNameTextboxPromptHtml(textBoxValue) {
    return "Feed Name: <input id=\"feedNameTextBox\" type=\"text\" size=\"70\" maxlength=\"40\" value=\"" + textBoxValue + "\">";
}

function GetFeedUrlTextboxPromptHtml(textBoxValue) {
    return "Feed URL: <input id=\"feedUrlTextBox\" type=\"text\" size=\"70\" maxlength=\"255\" value=\"" + textBoxValue + "\">";
}

function ShowAddFeedModal() {

    BootstrapDialog.show({
        title: "Create Feed",
        message: GetFeedNameTextboxPromptHtml(""),
        buttons: [{
            id: 'modalAddFeedButton',
            icon: 'glyphicon glyphicon-plus',
            label: 'Create Feed',
            cssClass: 'btn-primary',
            autospin: false,
            action: function (dialog) {
                var feedName = $('#feedNameTextBox').val();
                dialog.getButton('modalAddFeedButton').spin();
                dialog.enableButtons(false);
                dialog.setClosable(false);
                dialog.getModalBody().html('Please wait...');

                var request = new NuFridge.Website.Services.CreateFeedRequest();
                request.FeedName = feedName;

                var modalHtml = GetFeedNameTextboxPromptHtml(feedName);
                var message = "There was an error trying to create the feed.";

                NuFridge.Website.Services.FeedService.CreateFeed(request, function (response) {
                    if (response && response.Success) {
                        dialog.close();
                        GetFeeds();
                    } else {
                        if (response && response.Message) {
                            message = "Error: " + response.Message;
                        }
                        ShowErrorInDialog(dialog, 'modalAddFeedButton', message, modalHtml);
                    }
                }, function () {
                    ShowErrorInDialog(dialog, 'modalAddFeedButton', message, modalHtml);
                });


            }
        }, {
            label: 'Cancel',
            action: function (dialog) {
                dialog.close();
            }
        }]
    });
}

function ShowImportPackagesModal() {

    BootstrapDialog.show({
        title: "Import Packages From an External Feed",
        message: GetFeedUrlTextboxPromptHtml("http://"),
        buttons: [{
            id: 'modalImportFeedButton',
            icon: 'glyphicon glyphicon-upload',
            label: 'Import Packages',
            cssClass: 'btn-primary',
            autospin: false,
            action: function (dialog) {
                var feedName = $('#feedDetailsName').text();
                var sourceFeedUrl = $('#feedUrlTextBox').val();

                dialog.getButton('modalImportFeedButton').spin();
                dialog.enableButtons(false);
                dialog.setClosable(false);
                dialog.getModalBody().html('Please wait...');

                var request = new NuFridge.Website.Services.ImportFeedRequest();
                request.FeedName = feedName;
                request.SourceFeedUrl = sourceFeedUrl;
                request.ApiKey = "TODO";

                var modalHtml = GetFeedUrlTextboxPromptHtml(feedName);
                var message = "There was an error scheduling the import of the feed.";

                NuFridge.Website.Services.FeedService.ImportFeed(request, function (response) {
                    if (response && response.Success) {
                        dialog.close();
                    } else {
                        if (response && response.Message) {
                            message = "Error: " + response.Message;
                        }
                        ShowErrorInDialog(dialog, 'modalImportFeedButton', message, modalHtml);
                    }
                }, function () {
                    ShowErrorInDialog(dialog, 'modalImportFeedButton', message, modalHtml);
                });


            }
        }, {
            label: 'Cancel',
            action: function (dialog) {
                dialog.close();
            }
        }]
    });
}

function ShowErrorInDialog(dialog, buttonId, message, footerHtml) {
    dialog.setClosable(true);
    dialog.enableButtons(true);
    dialog.getButton(buttonId).stopSpin();
    dialog.getModalBody().html("<p class=\"alert alert-danger\">" + message + "</p>" + footerHtml);
}

function ShowDeleteFeedModal() {
    if ($(this).attr("disabled") == true) {
        return;
    }

    var selectedRow = $('#manageFeedTable tr.selected:first');
    if (selectedRow.length > 0) {
        var name = selectedRow.find("td:first");
        var feedName = name.text();
        BootstrapDialog.show({
            title: "Delete Feed - " + feedName,
            message: "<p class=\"alert alert-danger\">Are you sure you want to delete this feed? <br><br> Any packages stored in the feed will be lost.</p>",
            buttons: [{
                id: 'modalDeleteFeedButton',
                icon: 'glyphicon glyphicon-remove',
                label: 'Delete Feed',
                cssClass: 'btn-danger',
                autospin: false,
                action: function (dialog) {
                    dialog.getButton('modalDeleteFeedButton').spin();
                    dialog.enableButtons(false);
                    dialog.setClosable(false);
                    dialog.getModalBody().html('Please wait...');

                    var request = new NuFridge.Website.Services.DeleteFeedRequest();
                    request.FeedName = feedName;

                    var message = "There was an error trying to delete the feed.";

                    NuFridge.Website.Services.FeedService.DeleteFeed(request, function (response) {
                        if (response && response.Success) {
                            dialog.close();
                            GetFeeds();
                        } else {
                            if (response && response.Message) {
                                message = "Error: " + response.Message;
                            }
                            ShowErrorInDialog(dialog, 'modalDeleteFeedButton', message, "");
                        }
                    }, function () {
                        ShowErrorInDialog(dialog, 'modalDeleteFeedButton', message, "");
                    });
                }
            }, {
                label: 'Cancel',
                action: function (dialog) {
                    dialog.close();
                }
            }]
        });
    } else {
        DisableFeedDependantButtons();
    }
}

function EnableFeedDependantButtons() {
    $('#deleteFeedButton').removeClass('disabled');
    $('#deleteFeedButton').prop('disabled', false);

    $('#packageImportButton').removeClass('disabled');
    $('#packageImportButton').prop('disabled', false);
}

function ShowFeedDetails(feedRow) {
    var feedName = $(feedRow).find("td:first").text();
    if (lastFeed == feedName) {
        return;
    }

    $('#feedMoreDetailsPanel .nav-tabs a[href=#general]').tab('show');

    $('#packageSearchTable tbody tr').remove();
    $("#packageSearchInput").val("");
    $("#noPackagesFoundOnSearch").hide();
    $("#packageSearchTable").hide();
    $("#noPackagesFoundErrorOnSearch").hide();
    $('#feedMoreDetailsPanel').show();
    $("#feedDetailsName").text("Please wait...");
    $("#feedMoreDetailsPanel .panel-body").hide();

    var request = new NuFridge.Website.Services.GetFeedRequest();

    request.FeedName = feedName;

    NuFridge.Website.Services.FeedService.GetFeed(request, function (response) {
        if (response && response.Success) {
            $("#feedDetailsName").text(response.Feed.Name);
            $("#feedDetailsBrowsePackages").html(MakeHyperlinkFromUrl(response.Feed.BrowsePackagesUrl));
            $("#feedDetailsDownloadPackages").html(MakeHyperlinkFromUrl(response.Feed.DownloadPackagesUrl));
            $("#feedDetailsPublishPackages").html(MakeHyperlinkFromUrl(response.Feed.PublishPackagesUrl));
            $("#feedDetailsSymbolServer").html(MakeHyperlinkFromUrl(response.Feed.SymbolServerUrl));
            $("#feedDetailsTotalPackages").text("Connecting...");
            $("#feedDetailsPackagesToIndex").text("Connecting...");
            $("#feedDetailsSyncState").text("Connecting...");
            $("#feedDetailsIndexerState").text("Connecting...");
            lastFeed = feedName;
            $("#feedMoreDetailsPanel .panel-body").show();
            ConnectToSignalROnFeed(response.Feed.BrowsePackagesUrl);
        } else {
            $("#feedMoreDetailsPanel .panel-body").hide();
            $("#feedDetailsName").text("Failed to get the feed...");
        }
    }, function () {

    });

}

function MakeHyperlinkFromUrl(url) {
    return "<a href=\"" + url + "\" target=\"_blank\">" + url + "</a>";
}

function GetFeeds() {

    EnablePagingOnFeedsTable();

    $("#manageFeedTable").dataTable().fnClearTable();

    NuFridge.Website.Services.FeedService.GetFeedNames(function (response) {
        if (response && response.Feeds && response.Success === true) {

            $('#manageFeedMenuCount').text(response.Feeds.length);
            $(response.Feeds).each(function (index, feed) {
                $("#manageFeedTable").dataTable().fnAddData([feed]);
            });
            DisableFeedDependantButtons();
            SetupFeedsRowClickEvent();
        } else if (response && response.Success === false) {
            BootstrapDialog.show({
                title: "An error has occurred loading the feeds",
                message: "<p>" + response.Message + "</p>",
                buttons: [{
                    label: 'Close',
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
        }
    },
        function () {
            //     alert("There was an error trying to load the feeds.");
        });
}

function SetupFeedsRowClickEvent() {
    $('#manageFeedTable tbody tr').filter(function () { return !($(this).children().is('.dataTables_empty')); }).off("click").click(function () {
        $("#manageFeedTable tr").each(function (index, element) {
            $(element).removeClass("selected");
        });
        $(this).addClass("selected");
        DisableFeedDependantButtons();
        ShowFeedDetails(this);

    });
}