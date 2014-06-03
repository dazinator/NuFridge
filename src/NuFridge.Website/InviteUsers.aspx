<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="InviteUsers.aspx.cs" Inherits="NuFridge.Website.InviteUsers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
        <script src="Scripts/InviteUsers.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <h3>
        <a href="#" style="padding: 5px;"><strong><i class="glyphicon glyphicon-user"></i> Invite Users</strong></a></h3>
    <hr>
            <div class="row">
        <div class="col-md-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4>Send Invite</h4>
                </div>
                <div class="panel-body">
                    <p>
                       Send an invitation email to allow a user to register their own account.
                    </p>
                    <form role="form">
  <div class="form-group">
    <label for="emailAddressLabel">Email address</label>
    <input type="email" class="form-control" id="emailAddressInput" placeholder="Enter email">
  </div>
                        </form>
                    <p>
                      <button type="button" id="sendInviteButton" class="btn btn-primary">Send Invite</button>
                    </p>
                        <p id="submitMessage" style="display: none;" class="alert"></p>
                </div>
            </div>
        </div>
        </div>
</asp:Content>
