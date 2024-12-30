handlers.VerifyNonceOnLogin = async function (args, context) {
    try {
        var method = "post";
        var contentBody = {"content": JSON.stringify(context.playStreamEvent.CustomTags)}
        var contentType = "application/json";
        var headers = {};
        var nonce = context.playStreamEvent.CustomTags.nonce
        var userId = context.playStreamEvent.CustomTags.userId

        const oculusAppId = "yourappid";
        const oculusAppSecret = "yourappsecret";
        const apiUrl = 'https://graph.oculus.com/user_nonce_validate';

        const postData = {
            nonce: nonce,
            user_id: userId,
            access_token: `OC|${oculusAppId}|${oculusAppSecret}`
        };
        
        var result = http.request(`${apiUrl}?access_token=${postData.access_token}&nonce=${postData.nonce}&user_id=${postData.user_id}`, method, "", "application/json", {}, true)
        var json = JSON.parse(result)

        if (json.is_valid) {
            return { status: "success", message: "Nonce verified", userId: result.user_id };
        } else {
            var reason = "invalid nonce contact the discord to fix" 
            Ban(context.playerProfile.PlayerId, 72, reason, context.playStreamEvent.IPV4Address)
        }
        
    } catch (error) {
        var reason = "verification failed, contact the discord to fix"
        Ban(context.playerProfile.PlayerId, 72, reason, context.playStreamEvent.IPV4Address)
    }
};

handlers.UnbanUser = async function(args, context) {
    var unbanRequest = {
        PlayFabId: context.playerProfile.PlayerId
    }

    server.RevokeAllBansForUser(unbanRequest)
}

function Ban(playerId, hours, reason, ip){
    var banRequest = {
        "Bans": [
            {
                DurationInHours: hours,
                PlayFabId: playerId,
                Reason: reason,
                IPAddress: ip
            }
        ]
    }
    
    server.BanUsers(banRequest);
}
