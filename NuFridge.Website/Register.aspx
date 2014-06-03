<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="NuFridge.Website.Register" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
            <script src="Scripts/Register.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
            <div class="row">
        <div class="col-md-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4>Complete Registration</h4>
                </div>
                <div class="panel-body">
                    <p>
                       Please fill in the form below to complete your account registration.
                    </p>
                    <form role="form">
  <div class="form-group">

           <p> <label for="firstNameInput">First Name</label>
    <input type="text" class="form-control" id="firstNameInput" autocomplete="off"></p>
              <p> <label for="lastNameInput">Last Name</label>
    <input type="text" class="form-control" id="lastNameInput" autocomplete="off"></p>
            <p>
    <label for="emailAddressInput">Email address</label>
    <input type="email" class="form-control" id="emailAddressInput" readonly="true" autocomplete="off"></p>
        <p> <label for="passwordInput">Password</label>
    <input type="password" class="form-control" id="passwordInput" autocomplete="off"></p>
  </div>
                        </form>
                    <p>
                      <button type="button" disabled="disabled" id="createAccountButton" class="btn btn-primary">Create Account</button>
                    </p>
                        <p id="submitMessage" style="display: none;" class="alert alert-danger"></p>
                </div>
            </div>
        </div>
        </div>
</asp:Content>
