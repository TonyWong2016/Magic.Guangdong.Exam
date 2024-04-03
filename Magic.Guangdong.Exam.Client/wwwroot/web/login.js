
function log() {
    document.getElementById('results').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
        document.getElementById('results').innerText += msg + '\r\n';
    });
}
document.getElementById("login").addEventListener("click", login, false);
document.getElementById("api").addEventListener("click", api, false);
document.getElementById("api2").addEventListener("click", api2, false);
document.getElementById("logout").addEventListener("click", logout, false);



//配置获取token的请求模型，注意和服务端的配置保持一致，scope范围不要丢啦
var config = {
    authority: "http://login.xiaoxiaotong.org",
    client_id: "checkinsystem2021",
    client_secret: "checkinsystem2021",
    redirect_uri: "http://localhost:5000/user/test",
    //redirect_uri: "http://localhost:44333/SignInCallback",
    response_type: "id_token",
    scope: "openid profile",
    //scope: "openid profile",
    post_logout_redirect_uri: "http://localhost:5000/user/test",
};
var mgr = new Oidc.UserManager(config);

mgr.getUser().then(function (user) {
    if (user) {
        //登录成功，获取到用户信息
        log("User logged in", user.profile);
    }
    else {
        log("User not logged in");
    }
});

function login() {
    mgr.signinRedirect();//
}

function api() {
    mgr.getUser().then(function (user) {
        var url = "http://localhost:6002/identity";
        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            log(xhr.status, JSON.parse(xhr.responseText));
        }
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    });
}

function api2() {
    mgr.getUser().then(function (user) {
        var url = "http://localhost:6002/test";
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