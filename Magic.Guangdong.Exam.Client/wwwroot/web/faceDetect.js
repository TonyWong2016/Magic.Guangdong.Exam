
//第一步， 检测图片是否包含人脸
//function detectFace(image, type, url, token) {
function detectFace(config) {
    let faceModel = {
        "image": config.image,
        "image_type": config.type,
        "liveness_control": config.liveness,
    }
    let params = {
        "__RequestVerificationToken": config.token,
        "model": faceModel
    };
    
    TT.info("正在检测图片是否包含人脸...");
    $.post(config.detectUrl, params, function (data) {
        var json = JSON.parse(data.data);
        if (json.error_code == 0) {
            let result = json.result;
            if (result.face_num == 1) {
                console.log(result);
                TT.info("检测到人脸信息");
                faceToken = result.face_list[0].face_token;                
                config.image = faceToken;
                config.type = "FACE_TOKEN";
                if (!config.userId)
                    config.userId = faceToken;
                queryFace(config);
            } else {
                TT.info("检测到多张人脸信息，请重新上传只包含单张人脸的图片");
            }
        } else {
            console.log(json.error_msg);
            TT.error("检测失败，" + json.error_msg);
        }
    })
}
//第二步， 检测是否已经注册过人脸
function queryFace(config) {
    let faceModel = {
        "image": config.image,
        "image_type": config.type,
        "group_id_list": config.groupId,
        "user_id": config.userId
    };
    let params = {
        "__RequestVerificationToken": config.token,
        "model": faceModel
    };
    TT.info("正在检测人脸是否已被注册");
    $.post(config.queryUrl, params, function (data) {
        //console.log(json);
        var json = JSON.parse(data.data);
        if (json.error_code == 0 && json.result.user_list) {
            console.log(json.result);
            for (var i = 0; i < json.result.user_list.length; i++) {
                let item = json.result.user_list[i];
                if (item.score < 80) {
                    TT.info("人脸未被注册，即将注册人脸信息");
                    addFace(config);
                    return;
                }
            }
            if (config.actionType && config.actionType == "update") {
                //TT.error("人脸已被注册");
                updateFace(config);
            } else {
                TT.error("人脸已被注册");
                $(".save").hide();
                $(".saveDisabled").show();
            }
        } else if (json.error_code == 222207) {
            TT.info("人脸未被注册，即将注册人脸信息");
            addFace(config);
        }
    });
}
//第三步，注册人脸
function addFace(config) {
    let faceModel = {
        "image": config.image,
        "image_type": config.type,
        "group_id": config.groupId,
        "user_id": config.userId,
        "user_info": config.userInfo
    };
    let params = {
        "__RequestVerificationToken": config.token,
        "model": faceModel
    };
    $.post(config.addUrl, params, function (json) {
        if (json.code == 1) {
            TT.info("人脸注册成功");
        } else {
            TT.error(json.msg);
        }
    });
}
//第三部，或者更新人脸
function updateFace(config) {
    let faceModel = {
        "image": config.image,
        "image_type": config.type,
        "group_id": config.groupId,
        "user_id": config.userId,
        "user_info": config.userInfo,
        //"action_type": "REPLACE"
    };
    let params = {
        "__RequestVerificationToken": config.token,
        "model": faceModel
    };
    $.post(config.updateUrl, params, function (json) {
        if (json.code == 1) {
            TT.info("人脸信息更新成功");
        } else {
            TT.error(json.msg);
        }
    });
}