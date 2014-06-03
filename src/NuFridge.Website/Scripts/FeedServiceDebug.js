Type.registerNamespace('NuFridge.Website.Services');
NuFridge.Website.Services.FeedService = function () {
    NuFridge.Website.Services.FeedService.initializeBase(this);
    this._timeout = 0;
    this._userContext = null;
    this._succeeded = null;
    this._failed = null;
}
NuFridge.Website.Services.FeedService.prototype = {
    _get_path: function () {
        var p = this.get_path();
        if (p) return p;
        else return NuFridge.Website.Services.FeedService._staticInstance.get_path();
    },
    GetFeed: function (request, succeededCallback, failedCallback, userContext) {
        /// <param name="request" type="NuFridge.Website.Services.GetFeedRequest">NuFridge.Website.Services.GetFeedRequest</param>
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'GetFeed', false, { request: request }, succeededCallback, failedCallback, userContext);
    },
    GetRetentionPolicy: function (request, succeededCallback, failedCallback, userContext) {
        /// <param name="request" type="NuFridge.Website.Services.GetRetentionPolicyRequest">NuFridge.Website.Services.GetRetentionPolicyRequest</param>
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'GetRetentionPolicy', false, { request: request }, succeededCallback, failedCallback, userContext);
    },
    SaveRetentionPolicy: function (request, succeededCallback, failedCallback, userContext) {
        /// <param name="request" type="NuFridge.Website.Services.SaveRetentionPolicyRequest">NuFridge.Website.Services.SaveRetentionPolicyRequest</param>
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'SaveRetentionPolicy', false, { request: request }, succeededCallback, failedCallback, userContext);
    },
    GetFeedNames: function (succeededCallback, failedCallback, userContext) {
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'GetFeedNames', false, {}, succeededCallback, failedCallback, userContext);
    },
    CreateFeed: function (request, succeededCallback, failedCallback, userContext) {
        /// <param name="request" type="NuFridge.Website.Services.CreateFeedRequest">NuFridge.Website.Services.CreateFeedRequest</param>
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'CreateFeed', false, { request: request }, succeededCallback, failedCallback, userContext);
    },
    GetRetentionPolicyHistoryList: function (succeededCallback, failedCallback, userContext) {
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'GetRetentionPolicyHistoryList', false, {}, succeededCallback, failedCallback, userContext);
    },
    GetRetentionPolicyHistoryListLog: function (id, succeededCallback, failedCallback, userContext) {
        /// <param name="id" type="String">System.Guid</param>
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'GetRetentionPolicyHistoryListLog', false, { id: id }, succeededCallback, failedCallback, userContext);
    },
    DeleteFeed: function (request, succeededCallback, failedCallback, userContext) {
        /// <param name="request" type="NuFridge.Website.Services.DeleteFeedRequest">NuFridge.Website.Services.DeleteFeedRequest</param>
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'DeleteFeed', false, { request: request }, succeededCallback, failedCallback, userContext);
    },
    ImportFeed: function (request, succeededCallback, failedCallback, userContext) {
        /// <param name="request" type="NuFridge.Website.Services.ImportFeedRequest">NuFridge.Website.Services.ImportFeedRequest</param>
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'ImportFeed', false, { request: request }, succeededCallback, failedCallback, userContext);
    }
}
NuFridge.Website.Services.FeedService.registerClass('NuFridge.Website.Services.FeedService', Sys.Net.WebServiceProxy);
NuFridge.Website.Services.FeedService._staticInstance = new NuFridge.Website.Services.FeedService();
NuFridge.Website.Services.FeedService.set_path = function (value) {
    NuFridge.Website.Services.FeedService._staticInstance.set_path(value);
}
NuFridge.Website.Services.FeedService.get_path = function () {
    /// <value type="String" mayBeNull="true">The service url.</value>
    return NuFridge.Website.Services.FeedService._staticInstance.get_path();
}
NuFridge.Website.Services.FeedService.set_timeout = function (value) {
    NuFridge.Website.Services.FeedService._staticInstance.set_timeout(value);
}
NuFridge.Website.Services.FeedService.get_timeout = function () {
    /// <value type="Number">The service timeout.</value>
    return NuFridge.Website.Services.FeedService._staticInstance.get_timeout();
}
NuFridge.Website.Services.FeedService.set_defaultUserContext = function (value) {
    NuFridge.Website.Services.FeedService._staticInstance.set_defaultUserContext(value);
}
NuFridge.Website.Services.FeedService.get_defaultUserContext = function () {
    /// <value mayBeNull="true">The service default user context.</value>
    return NuFridge.Website.Services.FeedService._staticInstance.get_defaultUserContext();
}
NuFridge.Website.Services.FeedService.set_defaultSucceededCallback = function (value) {
    NuFridge.Website.Services.FeedService._staticInstance.set_defaultSucceededCallback(value);
}
NuFridge.Website.Services.FeedService.get_defaultSucceededCallback = function () {
    /// <value type="Function" mayBeNull="true">The service default succeeded callback.</value>
    return NuFridge.Website.Services.FeedService._staticInstance.get_defaultSucceededCallback();
}
NuFridge.Website.Services.FeedService.set_defaultFailedCallback = function (value) {
    NuFridge.Website.Services.FeedService._staticInstance.set_defaultFailedCallback(value);
}
NuFridge.Website.Services.FeedService.get_defaultFailedCallback = function () {
    /// <value type="Function" mayBeNull="true">The service default failed callback.</value>
    return NuFridge.Website.Services.FeedService._staticInstance.get_defaultFailedCallback();
}
NuFridge.Website.Services.FeedService.set_enableJsonp = function (value) { NuFridge.Website.Services.FeedService._staticInstance.set_enableJsonp(value); }
NuFridge.Website.Services.FeedService.get_enableJsonp = function () {
    /// <value type="Boolean">Specifies whether the service supports JSONP for cross domain calling.</value>
    return NuFridge.Website.Services.FeedService._staticInstance.get_enableJsonp();
}
NuFridge.Website.Services.FeedService.set_jsonpCallbackParameter = function (value) { NuFridge.Website.Services.FeedService._staticInstance.set_jsonpCallbackParameter(value); }
NuFridge.Website.Services.FeedService.get_jsonpCallbackParameter = function () {
    /// <value type="String">Specifies the parameter name that contains the callback function name for a JSONP request.</value>
    return NuFridge.Website.Services.FeedService._staticInstance.get_jsonpCallbackParameter();
}
NuFridge.Website.Services.FeedService.set_path("/Services/FeedService.asmx");
NuFridge.Website.Services.FeedService.GetFeed = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="NuFridge.Website.Services.GetFeedRequest">NuFridge.Website.Services.GetFeedRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    NuFridge.Website.Services.FeedService._staticInstance.GetFeed(request, onSuccess, onFailed, userContext);
}
NuFridge.Website.Services.FeedService.GetRetentionPolicy = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="NuFridge.Website.Services.GetRetentionPolicyRequest">NuFridge.Website.Services.GetRetentionPolicyRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    NuFridge.Website.Services.FeedService._staticInstance.GetRetentionPolicy(request, onSuccess, onFailed, userContext);
}
NuFridge.Website.Services.FeedService.SaveRetentionPolicy = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="NuFridge.Website.Services.SaveRetentionPolicyRequest">NuFridge.Website.Services.SaveRetentionPolicyRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    NuFridge.Website.Services.FeedService._staticInstance.SaveRetentionPolicy(request, onSuccess, onFailed, userContext);
}
NuFridge.Website.Services.FeedService.GetFeedNames = function (onSuccess, onFailed, userContext) {
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    NuFridge.Website.Services.FeedService._staticInstance.GetFeedNames(onSuccess, onFailed, userContext);
}
NuFridge.Website.Services.FeedService.CreateFeed = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="NuFridge.Website.Services.CreateFeedRequest">NuFridge.Website.Services.CreateFeedRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    NuFridge.Website.Services.FeedService._staticInstance.CreateFeed(request, onSuccess, onFailed, userContext);
}
NuFridge.Website.Services.FeedService.GetRetentionPolicyHistoryList = function (onSuccess, onFailed, userContext) {
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    NuFridge.Website.Services.FeedService._staticInstance.GetRetentionPolicyHistoryList(onSuccess, onFailed, userContext);
}
NuFridge.Website.Services.FeedService.GetRetentionPolicyHistoryListLog = function (id, onSuccess, onFailed, userContext) {
    /// <param name="id" type="String">System.Guid</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    NuFridge.Website.Services.FeedService._staticInstance.GetRetentionPolicyHistoryListLog(id, onSuccess, onFailed, userContext);
}
NuFridge.Website.Services.FeedService.DeleteFeed = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="NuFridge.Website.Services.DeleteFeedRequest">NuFridge.Website.Services.DeleteFeedRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    NuFridge.Website.Services.FeedService._staticInstance.DeleteFeed(request, onSuccess, onFailed, userContext);
}
NuFridge.Website.Services.FeedService.ImportFeed = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="NuFridge.Website.Services.ImportFeedRequest">NuFridge.Website.Services.ImportFeedRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    NuFridge.Website.Services.FeedService._staticInstance.ImportFeed(request, onSuccess, onFailed, userContext);
}
var gtc = Sys.Net.WebServiceProxy._generateTypedConstructor;
if (typeof (NuFridge.Website.Services.GetFeedRequest) === 'undefined') {
    NuFridge.Website.Services.GetFeedRequest = gtc("NuFridge.Website.Services.GetFeedRequest");
    NuFridge.Website.Services.GetFeedRequest.registerClass('NuFridge.Website.Services.GetFeedRequest');
}
if (typeof (NuFridge.Website.Services.GetFeedResponse) === 'undefined') {
    NuFridge.Website.Services.GetFeedResponse = gtc("NuFridge.Website.Services.GetFeedResponse");
    NuFridge.Website.Services.GetFeedResponse.registerClass('NuFridge.Website.Services.GetFeedResponse');
}
if (typeof (NuFridge.Website.Services.GetRetentionPolicyRequest) === 'undefined') {
    NuFridge.Website.Services.GetRetentionPolicyRequest = gtc("NuFridge.Website.Services.GetRetentionPolicyRequest");
    NuFridge.Website.Services.GetRetentionPolicyRequest.registerClass('NuFridge.Website.Services.GetRetentionPolicyRequest');
}
if (typeof (NuFridge.Website.Services.GetRetentionPolicyResponse) === 'undefined') {
    NuFridge.Website.Services.GetRetentionPolicyResponse = gtc("NuFridge.Website.Services.GetRetentionPolicyResponse");
    NuFridge.Website.Services.GetRetentionPolicyResponse.registerClass('NuFridge.Website.Services.GetRetentionPolicyResponse');
}
if (typeof (NuFridge.Website.Services.SaveRetentionPolicyRequest) === 'undefined') {
    NuFridge.Website.Services.SaveRetentionPolicyRequest = gtc("NuFridge.Website.Services.SaveRetentionPolicyRequest");
    NuFridge.Website.Services.SaveRetentionPolicyRequest.registerClass('NuFridge.Website.Services.SaveRetentionPolicyRequest');
}
if (typeof (NuFridge.Website.Services.SaveRetentionPolicyResponse) === 'undefined') {
    NuFridge.Website.Services.SaveRetentionPolicyResponse = gtc("NuFridge.Website.Services.SaveRetentionPolicyResponse");
    NuFridge.Website.Services.SaveRetentionPolicyResponse.registerClass('NuFridge.Website.Services.SaveRetentionPolicyResponse');
}
if (typeof (NuFridge.Website.Services.GetFeedsResponse) === 'undefined') {
    NuFridge.Website.Services.GetFeedsResponse = gtc("NuFridge.Website.Services.GetFeedsResponse");
    NuFridge.Website.Services.GetFeedsResponse.registerClass('NuFridge.Website.Services.GetFeedsResponse');
}
if (typeof (NuFridge.Website.Services.CreateFeedRequest) === 'undefined') {
    NuFridge.Website.Services.CreateFeedRequest = gtc("NuFridge.Website.Services.CreateFeedRequest");
    NuFridge.Website.Services.CreateFeedRequest.registerClass('NuFridge.Website.Services.CreateFeedRequest');
}
if (typeof (NuFridge.Website.Services.CreateFeedResponse) === 'undefined') {
    NuFridge.Website.Services.CreateFeedResponse = gtc("NuFridge.Website.Services.CreateFeedResponse");
    NuFridge.Website.Services.CreateFeedResponse.registerClass('NuFridge.Website.Services.CreateFeedResponse');
}
if (typeof (NuFridge.Website.Services.GetRetentionPolicyHistoryResponse) === 'undefined') {
    NuFridge.Website.Services.GetRetentionPolicyHistoryResponse = gtc("NuFridge.Website.Services.GetRetentionPolicyHistoryResponse");
    NuFridge.Website.Services.GetRetentionPolicyHistoryResponse.registerClass('NuFridge.Website.Services.GetRetentionPolicyHistoryResponse');
}
if (typeof (NuFridge.Website.Services.DeleteFeedRequest) === 'undefined') {
    NuFridge.Website.Services.DeleteFeedRequest = gtc("NuFridge.Website.Services.DeleteFeedRequest");
    NuFridge.Website.Services.DeleteFeedRequest.registerClass('NuFridge.Website.Services.DeleteFeedRequest');
}
if (typeof (NuFridge.Website.Services.DeleteFeedResponse) === 'undefined') {
    NuFridge.Website.Services.DeleteFeedResponse = gtc("NuFridge.Website.Services.DeleteFeedResponse");
    NuFridge.Website.Services.DeleteFeedResponse.registerClass('NuFridge.Website.Services.DeleteFeedResponse');
}
if (typeof (NuFridge.Website.Services.ImportFeedRequest) === 'undefined') {
    NuFridge.Website.Services.ImportFeedRequest = gtc("NuFridge.Website.Services.ImportFeedRequest");
    NuFridge.Website.Services.ImportFeedRequest.registerClass('NuFridge.Website.Services.ImportFeedRequest');
}
if (typeof (NuFridge.Website.Services.ImportFeedResponse) === 'undefined') {
    NuFridge.Website.Services.ImportFeedResponse = gtc("NuFridge.Website.Services.ImportFeedResponse");
    NuFridge.Website.Services.ImportFeedResponse.registerClass('NuFridge.Website.Services.ImportFeedResponse');
}