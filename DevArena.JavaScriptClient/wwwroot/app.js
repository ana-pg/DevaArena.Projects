/// <reference path="oidc-client.js" />

function log() {
    document.getElementById('results').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
        document.getElementById('results').innerHTML += msg + '\r\n';
    });
}

document.getElementById("login").addEventListener("click", login, false);
document.getElementById("admin").addEventListener("click", full_access, false);
document.getElementById("guest").addEventListener("click", limited_access, false);
document.getElementById("logout").addEventListener("click", logout, false);

var config = {
    authority: "http://localhost:5001",
    client_id: "js.clientid",
    redirect_uri: "http://localhost:58076/callback.html",
    response_type: "id_token token",
    scope: "openid profile devarena.api",
    post_logout_redirect_uri: "http://localhost:58076/index.html",
};
var mgr = new Oidc.UserManager(config);

mgr.getUser().then(function (user) {
    if (user) {
        log("User logged in", user.profile);
    }
    else {
        log("User not logged in");
    }
});

function login() {
    mgr.signinRedirect();
}

function full_access() {
    mgr.getUser().then(function (user) {

        
        var url = "http://localhost:53377/admin";

        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            log(xhr.status, JSON.parse(xhr.responseText));
        }
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    });
}

function limited_access() {
    mgr.getUser().then(function (user) {


        var url = "http://localhost:53377/guest";

        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            log(xhr.status, JSON.parse(xhr.responseText));
        }
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    });
}

function logout() {
    mgr.signoutRedirect();
}