Type.registerNamespace('FeedManagerWebsite.Services');
FeedManagerWebsite.Services.UserService = function () {
    FeedManagerWebsite.Services.UserService.initializeBase(this);
    this._timeout = 0;
    this._userContext = null;
    this._succeeded = null;
    this._failed = null;
}
FeedManagerWebsite.Services.UserService.prototype = {
    _get_path: function () {
        var p = this.get_path();
        if (p) return p;
        else return FeedManagerWebsite.Services.UserService._staticInstance.get_path();
    },
    SignIn: function (request, succeededCallback, failedCallback, userContext) {
        /// <param name="request" type="FeedManagerWebsite.Services.SignInRequest">FeedManagerWebsite.Services.SignInRequest</param>
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'SignIn', false, { request: request }, succeededCallback, failedCallback, userContext);
    },
    CreateAccount: function (request, succeededCallback, failedCallback, userContext) {
        /// <param name="request" type="FeedManagerWebsite.Services.CreateAccountRequest">FeedManagerWebsite.Services.CreateAccountRequest</param>
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'CreateAccount', false, { request: request }, succeededCallback, failedCallback, userContext);
    },
    CreateInvite: function (request, succeededCallback, failedCallback, userContext) {
        /// <param name="request" type="FeedManagerWebsite.Services.CreateInviteRequest">FeedManagerWebsite.Services.CreateInviteRequest</param>
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'CreateInvite', false, { request: request }, succeededCallback, failedCallback, userContext);
    },
    GetInvite: function (request, succeededCallback, failedCallback, userContext) {
        /// <param name="request" type="FeedManagerWebsite.Services.GetInviteRequest">FeedManagerWebsite.Services.GetInviteRequest</param>
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'GetInvite', false, { request: request }, succeededCallback, failedCallback, userContext);
    }
}
FeedManagerWebsite.Services.UserService.registerClass('FeedManagerWebsite.Services.UserService', Sys.Net.WebServiceProxy);
FeedManagerWebsite.Services.UserService._staticInstance = new FeedManagerWebsite.Services.UserService();
FeedManagerWebsite.Services.UserService.set_path = function (value) {
    FeedManagerWebsite.Services.UserService._staticInstance.set_path(value);
}
FeedManagerWebsite.Services.UserService.get_path = function () {
    /// <value type="String" mayBeNull="true">The service url.</value>
    return FeedManagerWebsite.Services.UserService._staticInstance.get_path();
}
FeedManagerWebsite.Services.UserService.set_timeout = function (value) {
    FeedManagerWebsite.Services.UserService._staticInstance.set_timeout(value);
}
FeedManagerWebsite.Services.UserService.get_timeout = function () {
    /// <value type="Number">The service timeout.</value>
    return FeedManagerWebsite.Services.UserService._staticInstance.get_timeout();
}
FeedManagerWebsite.Services.UserService.set_defaultUserContext = function (value) {
    FeedManagerWebsite.Services.UserService._staticInstance.set_defaultUserContext(value);
}
FeedManagerWebsite.Services.UserService.get_defaultUserContext = function () {
    /// <value mayBeNull="true">The service default user context.</value>
    return FeedManagerWebsite.Services.UserService._staticInstance.get_defaultUserContext();
}
FeedManagerWebsite.Services.UserService.set_defaultSucceededCallback = function (value) {
    FeedManagerWebsite.Services.UserService._staticInstance.set_defaultSucceededCallback(value);
}
FeedManagerWebsite.Services.UserService.get_defaultSucceededCallback = function () {
    /// <value type="Function" mayBeNull="true">The service default succeeded callback.</value>
    return FeedManagerWebsite.Services.UserService._staticInstance.get_defaultSucceededCallback();
}
FeedManagerWebsite.Services.UserService.set_defaultFailedCallback = function (value) {
    FeedManagerWebsite.Services.UserService._staticInstance.set_defaultFailedCallback(value);
}
FeedManagerWebsite.Services.UserService.get_defaultFailedCallback = function () {
    /// <value type="Function" mayBeNull="true">The service default failed callback.</value>
    return FeedManagerWebsite.Services.UserService._staticInstance.get_defaultFailedCallback();
}
FeedManagerWebsite.Services.UserService.set_enableJsonp = function (value) { FeedManagerWebsite.Services.UserService._staticInstance.set_enableJsonp(value); }
FeedManagerWebsite.Services.UserService.get_enableJsonp = function () {
    /// <value type="Boolean">Specifies whether the service supports JSONP for cross domain calling.</value>
    return FeedManagerWebsite.Services.UserService._staticInstance.get_enableJsonp();
}
FeedManagerWebsite.Services.UserService.set_jsonpCallbackParameter = function (value) { FeedManagerWebsite.Services.UserService._staticInstance.set_jsonpCallbackParameter(value); }
FeedManagerWebsite.Services.UserService.get_jsonpCallbackParameter = function () {
    /// <value type="String">Specifies the parameter name that contains the callback function name for a JSONP request.</value>
    return FeedManagerWebsite.Services.UserService._staticInstance.get_jsonpCallbackParameter();
}
FeedManagerWebsite.Services.UserService.set_path("/Services/UserService.asmx");
FeedManagerWebsite.Services.UserService.SignIn = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="FeedManagerWebsite.Services.SignInRequest">FeedManagerWebsite.Services.SignInRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    FeedManagerWebsite.Services.UserService._staticInstance.SignIn(request, onSuccess, onFailed, userContext);
}
FeedManagerWebsite.Services.UserService.CreateAccount = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="FeedManagerWebsite.Services.CreateAccountRequest">FeedManagerWebsite.Services.CreateAccountRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    FeedManagerWebsite.Services.UserService._staticInstance.CreateAccount(request, onSuccess, onFailed, userContext);
}
FeedManagerWebsite.Services.UserService.CreateInvite = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="FeedManagerWebsite.Services.CreateInviteRequest">FeedManagerWebsite.Services.CreateInviteRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    FeedManagerWebsite.Services.UserService._staticInstance.CreateInvite(request, onSuccess, onFailed, userContext);
}
FeedManagerWebsite.Services.UserService.GetInvite = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="FeedManagerWebsite.Services.GetInviteRequest">FeedManagerWebsite.Services.GetInviteRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    FeedManagerWebsite.Services.UserService._staticInstance.GetInvite(request, onSuccess, onFailed, userContext);
}
var gtc = Sys.Net.WebServiceProxy._generateTypedConstructor;
if (typeof (FeedManagerWebsite.Services.SignInRequest) === 'undefined') {
    FeedManagerWebsite.Services.SignInRequest = gtc("FeedManagerWebsite.Services.SignInRequest");
    FeedManagerWebsite.Services.SignInRequest.registerClass('FeedManagerWebsite.Services.SignInRequest');
}
if (typeof (FeedManagerWebsite.Services.SignInResponse) === 'undefined') {
    FeedManagerWebsite.Services.SignInResponse = gtc("FeedManagerWebsite.Services.SignInResponse");
    FeedManagerWebsite.Services.SignInResponse.registerClass('FeedManagerWebsite.Services.SignInResponse');
}
if (typeof (FeedManagerWebsite.Services.CreateAccountRequest) === 'undefined') {
    FeedManagerWebsite.Services.CreateAccountRequest = gtc("FeedManagerWebsite.Services.CreateAccountRequest");
    FeedManagerWebsite.Services.CreateAccountRequest.registerClass('FeedManagerWebsite.Services.CreateAccountRequest');
}
if (typeof (FeedManagerWebsite.Services.CreateAccountResponse) === 'undefined') {
    FeedManagerWebsite.Services.CreateAccountResponse = gtc("FeedManagerWebsite.Services.CreateAccountResponse");
    FeedManagerWebsite.Services.CreateAccountResponse.registerClass('FeedManagerWebsite.Services.CreateAccountResponse');
}
if (typeof (FeedManagerWebsite.Services.CreateInviteRequest) === 'undefined') {
    FeedManagerWebsite.Services.CreateInviteRequest = gtc("FeedManagerWebsite.Services.CreateInviteRequest");
    FeedManagerWebsite.Services.CreateInviteRequest.registerClass('FeedManagerWebsite.Services.CreateInviteRequest');
}
if (typeof (FeedManagerWebsite.Services.CreateInviteResponse) === 'undefined') {
    FeedManagerWebsite.Services.CreateInviteResponse = gtc("FeedManagerWebsite.Services.CreateInviteResponse");
    FeedManagerWebsite.Services.CreateInviteResponse.registerClass('FeedManagerWebsite.Services.CreateInviteResponse');
}
if (typeof (FeedManagerWebsite.Services.GetInviteRequest) === 'undefined') {
    FeedManagerWebsite.Services.GetInviteRequest = gtc("FeedManagerWebsite.Services.GetInviteRequest");
    FeedManagerWebsite.Services.GetInviteRequest.registerClass('FeedManagerWebsite.Services.GetInviteRequest');
}