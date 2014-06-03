Type.registerNamespace('NuFridge.Website.Services');
NuFridge.Website.Services.UserService = function () {
    NuFridge.Website.Services.UserService.initializeBase(this);
    this._timeout = 0;
    this._userContext = null;
    this._succeeded = null;
    this._failed = null;
}
NuFridge.Website.Services.UserService.prototype = {
    _get_path: function () {
        var p = this.get_path();
        if (p) return p;
        else return NuFridge.Website.Services.UserService._staticInstance.get_path();
    },
    SignIn: function (request, succeededCallback, failedCallback, userContext) {
        /// <param name="request" type="NuFridge.Website.Services.SignInRequest">NuFridge.Website.Services.SignInRequest</param>
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'SignIn', false, { request: request }, succeededCallback, failedCallback, userContext);
    },
    CreateAccount: function (request, succeededCallback, failedCallback, userContext) {
        /// <param name="request" type="NuFridge.Website.Services.CreateAccountRequest">NuFridge.Website.Services.CreateAccountRequest</param>
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'CreateAccount', false, { request: request }, succeededCallback, failedCallback, userContext);
    },
    CreateInvite: function (request, succeededCallback, failedCallback, userContext) {
        /// <param name="request" type="NuFridge.Website.Services.CreateInviteRequest">NuFridge.Website.Services.CreateInviteRequest</param>
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'CreateInvite', false, { request: request }, succeededCallback, failedCallback, userContext);
    },
    GetInvite: function (request, succeededCallback, failedCallback, userContext) {
        /// <param name="request" type="NuFridge.Website.Services.GetInviteRequest">NuFridge.Website.Services.GetInviteRequest</param>
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'GetInvite', false, { request: request }, succeededCallback, failedCallback, userContext);
    }
}
NuFridge.Website.Services.UserService.registerClass('NuFridge.Website.Services.UserService', Sys.Net.WebServiceProxy);
NuFridge.Website.Services.UserService._staticInstance = new NuFridge.Website.Services.UserService();
NuFridge.Website.Services.UserService.set_path = function (value) {
    NuFridge.Website.Services.UserService._staticInstance.set_path(value);
}
NuFridge.Website.Services.UserService.get_path = function () {
    /// <value type="String" mayBeNull="true">The service url.</value>
    return NuFridge.Website.Services.UserService._staticInstance.get_path();
}
NuFridge.Website.Services.UserService.set_timeout = function (value) {
    NuFridge.Website.Services.UserService._staticInstance.set_timeout(value);
}
NuFridge.Website.Services.UserService.get_timeout = function () {
    /// <value type="Number">The service timeout.</value>
    return NuFridge.Website.Services.UserService._staticInstance.get_timeout();
}
NuFridge.Website.Services.UserService.set_defaultUserContext = function (value) {
    NuFridge.Website.Services.UserService._staticInstance.set_defaultUserContext(value);
}
NuFridge.Website.Services.UserService.get_defaultUserContext = function () {
    /// <value mayBeNull="true">The service default user context.</value>
    return NuFridge.Website.Services.UserService._staticInstance.get_defaultUserContext();
}
NuFridge.Website.Services.UserService.set_defaultSucceededCallback = function (value) {
    NuFridge.Website.Services.UserService._staticInstance.set_defaultSucceededCallback(value);
}
NuFridge.Website.Services.UserService.get_defaultSucceededCallback = function () {
    /// <value type="Function" mayBeNull="true">The service default succeeded callback.</value>
    return NuFridge.Website.Services.UserService._staticInstance.get_defaultSucceededCallback();
}
NuFridge.Website.Services.UserService.set_defaultFailedCallback = function (value) {
    NuFridge.Website.Services.UserService._staticInstance.set_defaultFailedCallback(value);
}
NuFridge.Website.Services.UserService.get_defaultFailedCallback = function () {
    /// <value type="Function" mayBeNull="true">The service default failed callback.</value>
    return NuFridge.Website.Services.UserService._staticInstance.get_defaultFailedCallback();
}
NuFridge.Website.Services.UserService.set_enableJsonp = function (value) { NuFridge.Website.Services.UserService._staticInstance.set_enableJsonp(value); }
NuFridge.Website.Services.UserService.get_enableJsonp = function () {
    /// <value type="Boolean">Specifies whether the service supports JSONP for cross domain calling.</value>
    return NuFridge.Website.Services.UserService._staticInstance.get_enableJsonp();
}
NuFridge.Website.Services.UserService.set_jsonpCallbackParameter = function (value) { NuFridge.Website.Services.UserService._staticInstance.set_jsonpCallbackParameter(value); }
NuFridge.Website.Services.UserService.get_jsonpCallbackParameter = function () {
    /// <value type="String">Specifies the parameter name that contains the callback function name for a JSONP request.</value>
    return NuFridge.Website.Services.UserService._staticInstance.get_jsonpCallbackParameter();
}
NuFridge.Website.Services.UserService.set_path("/Services/UserService.asmx");
NuFridge.Website.Services.UserService.SignIn = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="NuFridge.Website.Services.SignInRequest">NuFridge.Website.Services.SignInRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    NuFridge.Website.Services.UserService._staticInstance.SignIn(request, onSuccess, onFailed, userContext);
}
NuFridge.Website.Services.UserService.CreateAccount = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="NuFridge.Website.Services.CreateAccountRequest">NuFridge.Website.Services.CreateAccountRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    NuFridge.Website.Services.UserService._staticInstance.CreateAccount(request, onSuccess, onFailed, userContext);
}
NuFridge.Website.Services.UserService.CreateInvite = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="NuFridge.Website.Services.CreateInviteRequest">NuFridge.Website.Services.CreateInviteRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    NuFridge.Website.Services.UserService._staticInstance.CreateInvite(request, onSuccess, onFailed, userContext);
}
NuFridge.Website.Services.UserService.GetInvite = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="NuFridge.Website.Services.GetInviteRequest">NuFridge.Website.Services.GetInviteRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    NuFridge.Website.Services.UserService._staticInstance.GetInvite(request, onSuccess, onFailed, userContext);
}
var gtc = Sys.Net.WebServiceProxy._generateTypedConstructor;
if (typeof (NuFridge.Website.Services.SignInRequest) === 'undefined') {
    NuFridge.Website.Services.SignInRequest = gtc("NuFridge.Website.Services.SignInRequest");
    NuFridge.Website.Services.SignInRequest.registerClass('NuFridge.Website.Services.SignInRequest');
}
if (typeof (NuFridge.Website.Services.SignInResponse) === 'undefined') {
    NuFridge.Website.Services.SignInResponse = gtc("NuFridge.Website.Services.SignInResponse");
    NuFridge.Website.Services.SignInResponse.registerClass('NuFridge.Website.Services.SignInResponse');
}
if (typeof (NuFridge.Website.Services.CreateAccountRequest) === 'undefined') {
    NuFridge.Website.Services.CreateAccountRequest = gtc("NuFridge.Website.Services.CreateAccountRequest");
    NuFridge.Website.Services.CreateAccountRequest.registerClass('NuFridge.Website.Services.CreateAccountRequest');
}
if (typeof (NuFridge.Website.Services.CreateAccountResponse) === 'undefined') {
    NuFridge.Website.Services.CreateAccountResponse = gtc("NuFridge.Website.Services.CreateAccountResponse");
    NuFridge.Website.Services.CreateAccountResponse.registerClass('NuFridge.Website.Services.CreateAccountResponse');
}
if (typeof (NuFridge.Website.Services.CreateInviteRequest) === 'undefined') {
    NuFridge.Website.Services.CreateInviteRequest = gtc("NuFridge.Website.Services.CreateInviteRequest");
    NuFridge.Website.Services.CreateInviteRequest.registerClass('NuFridge.Website.Services.CreateInviteRequest');
}
if (typeof (NuFridge.Website.Services.CreateInviteResponse) === 'undefined') {
    NuFridge.Website.Services.CreateInviteResponse = gtc("NuFridge.Website.Services.CreateInviteResponse");
    NuFridge.Website.Services.CreateInviteResponse.registerClass('NuFridge.Website.Services.CreateInviteResponse');
}
if (typeof (NuFridge.Website.Services.GetInviteRequest) === 'undefined') {
    NuFridge.Website.Services.GetInviteRequest = gtc("NuFridge.Website.Services.GetInviteRequest");
    NuFridge.Website.Services.GetInviteRequest.registerClass('NuFridge.Website.Services.GetInviteRequest');
}