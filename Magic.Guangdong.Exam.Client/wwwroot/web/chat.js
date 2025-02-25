//创建连接对象connection
const signalr_connection = new signalR.HubConnectionBuilder()
    .withUrl("/MyHub")
    .withAutomaticReconnect() // 自动断开重连
    .configureLogging(signalR.LogLevel.Information)
    .build();

//启动connection
signalr_connection.start()
    .then(function () {        
        TT.success("考试中心连接成功");
    }).catch(function (ex) {
        TT.error("考试中心连接失败，请刷新当前页面重试");
        console.log(ex);

    });

signalr_connection.on("ReceiveMessage", function (msg, roomId, userId) {
    if (roomId && roomId != getQueryString("roomId"))
        return;
    if (userId && userId != getQueryString("userId"))
        return;

    if (!roomId && !userId) {
        TT.info("收到了广播消息：" + msg);
    }

    if (userId == getQueryString("userId")) {
        TT.notify("收到消息：" + msg);
    }
    
    //if (msg && roomId && userId) {
    //    TT.notify("收到了来自房间【" + roomId + "】的消息：" + msg);
    //    if (msg == "removeuser") {
    //        leaveRoom()
    //    }
    //} else if (msg && roomId) {        
    //    if (msg.indexOf("removeuser") > -1) {
    //        let parts = msg.split("|");
    //        if (parts.length > 1 && getQueryString("userId") == parts[1]) {
    //            leaveRoom();
    //            TT.warning("您已被踢出房间");
    //            setTimeout(u => {
    //                location.reload(true);
    //            }, 1200);                
    //        }
    //    }
    //    else if (msg == "dismissroom") {
    //        try {
    //            TT.warning("当前房间已解散");
    //            leaveRoom();                
    //        } catch {
    //            console.error("解散失败");
    //        }
    //        location.reload(true);
    //    } else {
    //        TT.notify("收到了来自房间【" + roomId + "】的广播消息：" + msg);
    //    }
    //} else if (msg) {
    //    TT.notify("收到了广播消息：" + msg);
    //}
});


