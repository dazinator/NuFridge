Type.registerNamespace('FeedManagerWebsite.Services');
FeedManagerWebsite.Services.FeedService = function () {
    FeedManagerWebsite.Services.FeedService.initializeBase(this);
    this._timeout = 0;
    this._userContext = null;
    this._succeeded = null;
    this._failed = null;
}
FeedManagerWebsite.Services.FeedService.prototype = {
    _get_path: function () {
        var p = this.get_path();
        if (p) return p;
        else return FeedManagerWebsite.Services.FeedService._staticInstance.get_path();
    },
    GetFeed: function (request, succeededCallback, failedCallback, userContext) {
        /// <param name="request" type="FeedManagerWebsite.Services.GetFeedRequest">FeedManagerWebsite.Services.GetFeedRequest</param>
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'GetFeed', false, { request: request }, succeededCallback, failedCallback, userContext);
    },
    GetRetentionPolicy: function (request, succeededCallback, failedCallback, userContext) {
        /// <param name="request" type="FeedManagerWebsite.Services.GetRetentionPolicyRequest">FeedManagerWebsite.Services.GetRetentionPolicyRequest</param>
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'GetRetentionPolicy', false, { request: request }, succeededCallback, failedCallback, userContext);
    },
    SaveRetentionPolicy: function (request, succeededCallback, failedCallback, userContext) {
        /// <param name="request" type="FeedManagerWebsite.Services.SaveRetentionPolicyRequest">FeedManagerWebsite.Services.SaveRetentionPolicyRequest</param>
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
        /// <param name="request" type="FeedManagerWebsite.Services.CreateFeedRequest">FeedManagerWebsite.Services.CreateFeedRequest</param>
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
    DeleteFeed: function (request, succeededCallback, failedCallback, userContext) {
        /// <param name="request" type="FeedManagerWebsite.Services.DeleteFeedRequest">FeedManagerWebsite.Services.DeleteFeedRequest</param>
        /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
        /// <param name="userContext" optional="true" mayBeNull="true"></param>
        return this._invoke(this._get_path(), 'DeleteFeed', false, { request: request }, succeededCallback, failedCallback, userContext);
    }
}
FeedManagerWebsite.Services.FeedService.registerClass('FeedManagerWebsite.Services.FeedService', Sys.Net.WebServiceProxy);
FeedManagerWebsite.Services.FeedService._staticInstance = new FeedManagerWebsite.Services.FeedService();
FeedManagerWebsite.Services.FeedService.set_path = function (value) {
    FeedManagerWebsite.Services.FeedService._staticInstance.set_path(value);
}
FeedManagerWebsite.Services.FeedService.get_path = function () {
    /// <value type="String" mayBeNull="true">The service url.</value>
    return FeedManagerWebsite.Services.FeedService._staticInstance.get_path();
}
FeedManagerWebsite.Services.FeedService.set_timeout = function (value) {
    FeedManagerWebsite.Services.FeedService._staticInstance.set_timeout(value);
}
FeedManagerWebsite.Services.FeedService.get_timeout = function () {
    /// <value type="Number">The service timeout.</value>
    return FeedManagerWebsite.Services.FeedService._staticInstance.get_timeout();
}
FeedManagerWebsite.Services.FeedService.set_defaultUserContext = function (value) {
    FeedManagerWebsite.Services.FeedService._staticInstance.set_defaultUserContext(value);
}
FeedManagerWebsite.Services.FeedService.get_defaultUserContext = function () {
    /// <value mayBeNull="true">The service default user context.</value>
    return FeedManagerWebsite.Services.FeedService._staticInstance.get_defaultUserContext();
}
FeedManagerWebsite.Services.FeedService.set_defaultSucceededCallback = function (value) {
    FeedManagerWebsite.Services.FeedService._staticInstance.set_defaultSucceededCallback(value);
}
FeedManagerWebsite.Services.FeedService.get_defaultSucceededCallback = function () {
    /// <value type="Function" mayBeNull="true">The service default succeeded callback.</value>
    return FeedManagerWebsite.Services.FeedService._staticInstance.get_defaultSucceededCallback();
}
FeedManagerWebsite.Services.FeedService.set_defaultFailedCallback = function (value) {
    FeedManagerWebsite.Services.FeedService._staticInstance.set_defaultFailedCallback(value);
}
FeedManagerWebsite.Services.FeedService.get_defaultFailedCallback = function () {
    /// <value type="Function" mayBeNull="true">The service default failed callback.</value>
    return FeedManagerWebsite.Services.FeedService._staticInstance.get_defaultFailedCallback();
}
FeedManagerWebsite.Services.FeedService.set_enableJsonp = function (value) { FeedManagerWebsite.Services.FeedService._staticInstance.set_enableJsonp(value); }
FeedManagerWebsite.Services.FeedService.get_enableJsonp = function () {
    /// <value type="Boolean">Specifies whether the service supports JSONP for cross domain calling.</value>
    return FeedManagerWebsite.Services.FeedService._staticInstance.get_enableJsonp();
}
FeedManagerWebsite.Services.FeedService.set_jsonpCallbackParameter = function (value) { FeedManagerWebsite.Services.FeedService._staticInstance.set_jsonpCallbackParameter(value); }
FeedManagerWebsite.Services.FeedService.get_jsonpCallbackParameter = function () {
    /// <value type="String">Specifies the parameter name that contains the callback function name for a JSONP request.</value>
    return FeedManagerWebsite.Services.FeedService._staticInstance.get_jsonpCallbackParameter();
}
FeedManagerWebsite.Services.FeedService.set_path("/Services/FeedService.asmx");
FeedManagerWebsite.Services.FeedService.GetFeed = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="FeedManagerWebsite.Services.GetFeedRequest">FeedManagerWebsite.Services.GetFeedRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    FeedManagerWebsite.Services.FeedService._staticInstance.GetFeed(request, onSuccess, onFailed, userContext);
}
FeedManagerWebsite.Services.FeedService.GetRetentionPolicy = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="FeedManagerWebsite.Services.GetRetentionPolicyRequest">FeedManagerWebsite.Services.GetRetentionPolicyRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    FeedManagerWebsite.Services.FeedService._staticInstance.GetRetentionPolicy(request, onSuccess, onFailed, userContext);
}
FeedManagerWebsite.Services.FeedService.SaveRetentionPolicy = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="FeedManagerWebsite.Services.SaveRetentionPolicyRequest">FeedManagerWebsite.Services.SaveRetentionPolicyRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    FeedManagerWebsite.Services.FeedService._staticInstance.SaveRetentionPolicy(request, onSuccess, onFailed, userContext);
}
FeedManagerWebsite.Services.FeedService.GetFeedNames = function (onSuccess, onFailed, userContext) {
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    FeedManagerWebsite.Services.FeedService._staticInstance.GetFeedNames(onSuccess, onFailed, userContext);
}
FeedManagerWebsite.Services.FeedService.CreateFeed = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="FeedManagerWebsite.Services.CreateFeedRequest">FeedManagerWebsite.Services.CreateFeedRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    FeedManagerWebsite.Services.FeedService._staticInstance.CreateFeed(request, onSuccess, onFailed, userContext);
}
FeedManagerWebsite.Services.FeedService.GetRetentionPolicyHistoryList = function (onSuccess, onFailed, userContext) {
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    FeedManagerWebsite.Services.FeedService._staticInstance.GetRetentionPolicyHistoryList(onSuccess, onFailed, userContext);
}
FeedManagerWebsite.Services.FeedService.DeleteFeed = function (request, onSuccess, onFailed, userContext) {
    /// <param name="request" type="FeedManagerWebsite.Services.DeleteFeedRequest">FeedManagerWebsite.Services.DeleteFeedRequest</param>
    /// <param name="succeededCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="failedCallback" type="Function" optional="true" mayBeNull="true"></param>
    /// <param name="userContext" optional="true" mayBeNull="true"></param>
    FeedManagerWebsite.Services.FeedService._staticInstance.DeleteFeed(request, onSuccess, onFailed, userContext);
}
var gtc = Sys.Net.WebServiceProxy._generateTypedConstructor;
if (typeof (FeedManagerWebsite.Services.GetFeedRequest) === 'undefined') {
    FeedManagerWebsite.Services.GetFeedRequest = gtc("FeedManagerWebsite.Services.GetFeedRequest");
    FeedManagerWebsite.Services.GetFeedRequest.registerClass('FeedManagerWebsite.Services.GetFeedRequest');
}
if (typeof (FeedManagerWebsite.Services.GetFeedResponse) === 'undefined') {
    FeedManagerWebsite.Services.GetFeedResponse = gtc("FeedManagerWebsite.Services.GetFeedResponse");
    FeedManagerWebsite.Services.GetFeedResponse.registerClass('FeedManagerWebsite.Services.GetFeedResponse');
}
if (typeof (FeedManagerWebsite.Services.GetRetentionPolicyRequest) === 'undefined') {
    FeedManagerWebsite.Services.GetRetentionPolicyRequest = gtc("FeedManagerWebsite.Services.GetRetentionPolicyRequest");
    FeedManagerWebsite.Services.GetRetentionPolicyRequest.registerClass('FeedManagerWebsite.Services.GetRetentionPolicyRequest');
}
if (typeof (FeedManagerWebsite.Services.GetRetentionPolicyResponse) === 'undefined') {
    FeedManagerWebsite.Services.GetRetentionPolicyResponse = gtc("FeedManagerWebsite.Services.GetRetentionPolicyResponse");
    FeedManagerWebsite.Services.GetRetentionPolicyResponse.registerClass('FeedManagerWebsite.Services.GetRetentionPolicyResponse');
}
if (typeof (FeedManagerWebsite.Services.SaveRetentionPolicyRequest) === 'undefined') {
    FeedManagerWebsite.Services.SaveRetentionPolicyRequest = gtc("FeedManagerWebsite.Services.SaveRetentionPolicyRequest");
    FeedManagerWebsite.Services.SaveRetentionPolicyRequest.registerClass('FeedManagerWebsite.Services.SaveRetentionPolicyRequest');
}
if (typeof (FeedManagerWebsite.Services.SaveRetentionPolicyResponse) === 'undefined') {
    FeedManagerWebsite.Services.SaveRetentionPolicyResponse = gtc("FeedManagerWebsite.Services.SaveRetentionPolicyResponse");
    FeedManagerWebsite.Services.SaveRetentionPolicyResponse.registerClass('FeedManagerWebsite.Services.SaveRetentionPolicyResponse');
}
if (typeof (FeedManagerWebsite.Services.GetFeedsResponse) === 'undefined') {
    FeedManagerWebsite.Services.GetFeedsResponse = gtc("FeedManagerWebsite.Services.GetFeedsResponse");
    FeedManagerWebsite.Services.GetFeedsResponse.registerClass('FeedManagerWebsite.Services.GetFeedsResponse');
}
if (typeof (FeedManagerWebsite.Services.CreateFeedRequest) === 'undefined') {
    FeedManagerWebsite.Services.CreateFeedRequest = gtc("FeedManagerWebsite.Services.CreateFeedRequest");
    FeedManagerWebsite.Services.CreateFeedRequest.registerClass('FeedManagerWebsite.Services.CreateFeedRequest');
}
if (typeof (FeedManagerWebsite.Services.CreateFeedResponse) === 'undefined') {
    FeedManagerWebsite.Services.CreateFeedResponse = gtc("FeedManagerWebsite.Services.CreateFeedResponse");
    FeedManagerWebsite.Services.CreateFeedResponse.registerClass('FeedManagerWebsite.Services.CreateFeedResponse');
}
if (typeof (FeedManagerWebsite.Services.GetRetentionPolicyHistoryResponse) === 'undefined') {
    FeedManagerWebsite.Services.GetRetentionPolicyHistoryResponse = gtc("FeedManagerWebsite.Services.GetRetentionPolicyHistoryResponse");
    FeedManagerWebsite.Services.GetRetentionPolicyHistoryResponse.registerClass('FeedManagerWebsite.Services.GetRetentionPolicyHistoryResponse');
}
if (typeof (FeedManagerWebsite.Services.DeleteFeedRequest) === 'undefined') {
    FeedManagerWebsite.Services.DeleteFeedRequest = gtc("FeedManagerWebsite.Services.DeleteFeedRequest");
    FeedManagerWebsite.Services.DeleteFeedRequest.registerClass('FeedManagerWebsite.Services.DeleteFeedRequest');
}
if (typeof (FeedManagerWebsite.Services.DeleteFeedResponse) === 'undefined') {
    FeedManagerWebsite.Services.DeleteFeedResponse = gtc("FeedManagerWebsite.Services.DeleteFeedResponse");
    FeedManagerWebsite.Services.DeleteFeedResponse.registerClass('FeedManagerWebsite.Services.DeleteFeedResponse');
}