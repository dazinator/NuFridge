$(function () {

    GetFeeds();


    $(document).ready(function () {
        $("[data-toggle=tooltip]").tooltip({
            placement: 'top'
        });
    });

    $('.retentionPolicyEnabled').change(function () {
        EnableSaveRetentionPolicyButton();
    });

    $('#addExcludedPackageButton').click(function () {
        ShowAddExcludePackageModal();
    });

    $('#retentionPolicyDaysToKeep,#retentionPolicyVersionsToKeep').keyup(function () {
        EnableSaveRetentionPolicyButton();
    });

    $('#retentionPolicyExcludePackageNames').keyup(function () {
        EnableSaveRetentionPolicyButton();
    });


    $('#retentionPolicyDaysToKeep,#retentionPolicyVersionsToKeep').bind('change', function () {
        EnableSaveRetentionPolicyButton();
    });

    $('#saveRetentionPolicyButton').click(function () {
        SaveRetentionPolicy();
    });
    GetHistory();
});

function ShowAddExcludePackageModal() {

    BootstrapDialog.show({
        title: "Excluded Packages",
        message: GetPackageNameTextboxPromptHtml(""),
        buttons: [{
            id: 'modalAddPackageButton',
            icon: 'glyphicon glyphicon-plus',
            label: 'Add Package',
            cssClass: 'btn-primary',
            autospin: false,
            action: function (dialog) {
                var packageName = $('#packageNameTextBox').val();
                dialog.getButton('modalAddPackageButton').spin();
                dialog.enableButtons(false);
                dialog.setClosable(false);
                dialog.getModalBody().html('Please wait...');

                excludedPackagesDataTable.fnAddData([packageName, "False", "<button type=\"button\" class=\"btn btn-danger btn-sm\" onclick=\"excludedPackagesDataTable.fnDeleteRow(excludedPackagesDataTable.fnGetPosition( $(this).closest('tr').get(0) )); EnableSaveRetentionPolicyButton()\"><i class=\"glyphicon glyphicon-remove\"></i> Remove</button>"]);
                dialog.close();
                EnableSaveRetentionPolicyButton();

            }
        }, {
            label: 'Cancel',
            action: function (dialog) {
                dialog.close();
            }
        }]
    });

}

function GetPackageNameTextboxPromptHtml(textBoxValue) {
    return "Package Name: <input id=\"packageNameTextBox\" type=\"text\" size=\"70\" maxlength=\"40\" value=\"" + textBoxValue + "\">";
}


var feedsDataTable;

var lastFeed;

function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

function SaveRetentionPolicy() {
    var feedName = $("#feedDetailsName").text();
    var enabledValue = $('.retentionPolicyEnabled').prop('checked');
    var daysToKeep = $('#retentionPolicyDaysToKeep').val();
    var versionsToKeep = $('#retentionPolicyVersionsToKeep').val();

    if (!isNumber(daysToKeep)) {
        DisableSaveRetentionPolicyButton();
        $("#saveRetentionPolicyButtonNumericError").show();
        return;
    }

    var request = new NuFridge.Website.Services.Messages.SaveRetentionPolicyRequest();
    request.FeedName = feedName;
    request.Enabled = enabledValue;
    request.DaysToKeepPackages = daysToKeep;
    request.VersionsToKeep = versionsToKeep;
    request.ExcludedPackages = new Array();
    $(window.excludedPackagesTable).find('tr.odd,tr.even').each(function (index, element) {
        if ($(element).find(".dataTables_empty").length == 0) {
            request.ExcludedPackages.push({ PackageId: $(element).find('td:eq(0)').text(), PartialMatch: $(element).find('td:eq(1)').text() });
        }
    });

    NuFridge.Website.Services.FeedService.SaveRetentionPolicy(request, function (response) {
        if (response && response.Success) {
            DisableSaveRetentionPolicyButton();
            $("#saveRetentionPolicyButtonFailedMessage").hide();
            $("#saveRetentionPolicyButtonSuccessMessage").show();
            $("#saveRetentionPolicyButtonNumericError").hide();


        } else {
            $("#saveRetentionPolicyButtonFailedMessage").show();
            $("#saveRetentionPolicyButtonSuccessMessage").hide();
            $("#saveRetentionPolicyButtonNumericError").hide();
        }
    }, function () {
        $("#saveRetentionPolicyButtonFailedMessage").show();
        $("#saveRetentionPolicyButtonSuccessMessage").hide();
        $("#saveRetentionPolicyButtonNumericError").hide();
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

    feedsDataTable = $('#retentionPolicyTable').dataTable({
        "iDisplayStart": 0,
        "aLengthMenu": [
            [10, 20, 50, -1],
            [10, 20, 50, 'All']
        ],
        "sPaginationType": "full_numbers",
        "aoColumns": [{
            "sClass": "ellipsis"
        }]
    });

    $(feedsDataTable).bind('draw', FeedTableDrawEvent);
}

function FeedTableDrawEvent() {
    lastFeed = null;
    DisableSaveRetentionPolicyButton();
    SetupFeedsRowClickEvent();

    $('#retentionPolicyTable tr').each(function (index, element) {
        $(element).removeClass("selected");
    });



    $('#feedMoreDetailsPanel').hide();
    $("#feedMoreDetailsPanel .panel-body").hide();
}

function ShowFeedDetails(feedRow) {
    var feedName = $(feedRow).find("td:first").text();
    if (lastFeed == feedName) {
        return;
    }

    DisableSaveRetentionPolicyButton();


    $('#feedMoreDetailsPanel').show();
    $("#feedDetailsName").text("Please wait...");
    $("#feedMoreDetailsPanel .panel-body").hide();

    var request = new NuFridge.Website.Services.Messages.GetRetentionPolicyRequest();
    request.FeedName = feedName;

    NuFridge.Website.Services.FeedService.GetRetentionPolicy(request, function (response) {
        if (response && response.Success) {
            $("#feedDetailsName").text(feedName);
            lastFeed = feedName;

            var enabled = false;
            var daysToKeep = 0;
            var excludedPackages = new Array();
            var versionsToKeep = 0;

            if (response.RetentionPolicy) {
                enabled = response.RetentionPolicy.Enabled;
                daysToKeep = response.RetentionPolicy.DaysToKeepPackages;
                versionsToKeep = response.RetentionPolicy.VersionsToKeep;

                if (response.RetentionPolicy.ExcludedPackages) {
                    excludedPackages = response.RetentionPolicy.ExcludedPackages;
                }
            }

            $('.retentionPolicyEnabled').prop('checked', enabled);
            $('#retentionPolicyDaysToKeep').val(daysToKeep);
            $('#retentionPolicyVersionsToKeep').val(versionsToKeep);

            if (excludedPackagesDataTable == null) {

                excludedPackagesDataTable = $('#excludedPackagesTable').dataTable({
                    "bPaginate": false,
                    "bFilter": false,
                    "bInfo": false
                });

            }


            $("#excludedPackagesTable").dataTable().fnClearTable();

            $.each(excludedPackages, function (index, element) {
                var rowData = [];
                rowData.push(element.PackageId);
                rowData.push(element.PartialMatch);
                rowData.push("<button type=\"button\" class=\"btn btn-danger btn-sm\" onclick=\"excludedPackagesDataTable.fnDeleteRow(excludedPackagesDataTable.fnGetPosition( $(this).closest('tr').get(0) )); EnableSaveRetentionPolicyButton()\"><i class=\"glyphicon glyphicon-remove\"></i> Remove</button>");
                $("#excludedPackagesTable").dataTable().fnAddData(rowData);
            });

            
            $("#feedMoreDetailsPanel .panel-body").show();
        } else {
            $("#feedMoreDetailsPanel .panel-body").hide();
            $("#feedDetailsName").text("Failed to get the retention policy...");
        }
    }, function () {

    });

}


function DisableSaveRetentionPolicyButton() {
    $('#saveRetentionPolicyButton').addClass('disabled');
    $('#saveRetentionPolicyButton').prop('disabled', true);
    $("#saveRetentionPolicyButtonFailedMessage").hide();
    $("#saveRetentionPolicyButtonSuccessMessage").hide();
    $("#saveRetentionPolicyButtonNumericError").hide();
    window.onbeforeunload = null;
 
}

function EnableSaveRetentionPolicyButton() {
    $('#saveRetentionPolicyButton').removeClass('disabled');
    $('#saveRetentionPolicyButton').prop('disabled', false);
    $("#saveRetentionPolicyButtonFailedMessage").hide();
    $("#saveRetentionPolicyButtonSuccessMessage").hide();
    $("#saveRetentionPolicyButtonNumericError").hide();
    window.onbeforeunload = function (e) {
        return "If you leave this page, any changes you have made will be lost. Are you sure you wish to leave this page?";
    };

}

var historyDataTable;

var excludedPackagesDataTable;

function GetHistory() {


    if (historyDataTable == null) {


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

        $.fn.dataTableExt.oPagination.iFullNumbersShowPages = 5;

        historyDataTable = $('#retentionPolicyHistoryTable').dataTable({
            "aaSorting": [[ 0, "desc" ]] ,
            "iDisplayStart": 0,
            "aLengthMenu": [
                [5, 10, 25, -1],
                [5, 10, 25, 'All']
            ],
            "sPaginationType": "full_numbers",
            "iDisplayLength": 5,
            "bAutoWidth": true,
            "aoColumns": [{
                // "sWidth": "20%",
                "bAutoWidth": true,
                "bSortable": true
            }, {
                "bAutoWidth": true
                //  "sWidth": "34%"
            }, {
                "bAutoWidth": true,
                // "sWidth": "12%",
                "bSortable": false
            }, {
                "bAutoWidth": true,
                // "sWidth": "18%",
                "bSortable": false
            }, {
                "bAutoWidth": true,
                //  "sWidth": "16%",
                "bSortable": false
            },
            {
                "bAutoWidth": true,
                "bSortable": false
            }],
        });

    }


    $("#retentionPolicyHistoryTable").dataTable().fnClearTable();

    NuFridge.Website.Services.FeedService.GetRetentionPolicyHistoryList(function (response) {
        if (response && response.Entries) {
            historyResults = response.Entries;
                $(response.Entries).each(function (index, history) {
                var rowData = [];
                rowData.push(history.Date);
                rowData.push(history.FeedName);
                rowData.push(history.Result == true ? "Success" : "Failure");
                rowData.push(history.PackagesDeleted + ' (' + history.DiskSpaceDeleted + ')');
                rowData.push(history.TimeRunning);
                rowData.push("<button type='button' onclick='showLogPopup(\"" + history.FeedName + "\",\"" + history.Id + "\",\"" + history.Date + "\")' class='btn btn-primary btn-sm'><i class='glyphicon glyphicon-list-alt'></i> View Log</button>");
                $("#retentionPolicyHistoryTable").dataTable().fnAddData(rowData);
            });
        }
    },
        function () {
            //     alert("There was an error trying to load the feeds.");
        });
}

var historyResults;

function fixNewLines(str) {
    if (str)
    {
        str = "<p style='word-wrap: break-word;'>" + str;
        str = str.replace(/\r?\n/g, "</p><p style='word-wrap: break-word;'>");
        return str + "</p>";
    }
    return "";
}

var logMessage = "";
var viewLogFileId = "viewLogFileMessage";

function showLogPopup(FeedName, logId, Date) {
    logMessage = "";
    

    
    BootstrapDialog.show({
        title: "Viewing log for " + FeedName + " - " + Date,
        message:  '<p id="' + viewLogFileId + '">Loading the log file. Please wait...</p>',
        buttons: [{
            label: 'Close',
            action: function (dialog) {
                dialog.close();
            },
        }]
    });
    
    NuFridge.Website.Services.FeedService.GetRetentionPolicyHistoryListLog(logId, function (response) {
        if (response) {
            logMessage = fixNewLines(response);
            setTimeout(function () { $("#" + viewLogFileId).html(logMessage); }, 1200);
        } else {
            logMessage = "An error occurred calling the service. No response was received from the server.";
            setTimeout(function () { $("#" + viewLogFileId).html(logMessage); }, 1200);
        }
    },
function () {
    logMessage = "An unexpected error occurred calling the service.";
    setTimeout(function () { $("#" + viewLogFileId).html(logMessage); }, 1200);
});
}

function GetFeeds() {
    EnablePagingOnFeedsTable();

    if ($("#retentionPolicyTable").length > 0)
        {

        $("#retentionPolicyTable").dataTable().fnClearTable();
        NuFridge.Website.Services.FeedService.GetFeedNames(function (response) {
            if (response && response.Feeds && response.Success === true) {

                $('#manageFeedMenuCount').text(response.Feeds.length);
                $(response.Feeds).each(function (index, feed) {
                    $("#retentionPolicyTable").dataTable().fnAddData([feed]);
                });
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
}

function SetupFeedsRowClickEvent() {
    $('#retentionPolicyTable tbody tr').filter(function () {
        return !($(this).children().is('.dataTables_empty'));
    }).off("click").click(function () {

        $('#retentionPolicyTable tr').each(function (index, element) {
            $(element).removeClass("selected");
        });
        $(this).addClass("selected");
        ShowFeedDetails(this);

    });
}