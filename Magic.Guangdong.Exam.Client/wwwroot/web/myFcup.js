﻿//注意使用该文件前，要引用layui的element和upload和fcup2.js
//上传接口目前使用的是创新学院的接口，如后续有调整，需改url参数即可

let uploadFlag = false;
let fcupUserId = getCookie('accountId');
//if (fcupUserId)
//    fcupUserId = atob(fcupUserId);
function initFcup(elemId, types, callBackElem, progressElem = '', url = '', checkUrl = '') {
    if (url == '')
        //url = "http://manage.xiaoxiaotong.net/fileupload/upload";
        url = "/fileupload/upload";
    if (checkUrl == '')
        //checkUrl = "http://manage.xiaoxiaotong.net/fileupload/CheckFile";
        checkUrl = "/fileupload/CheckFile";
    if (progressElem)
        $('#' + progressElem).attr('max', 100);
    var up = new fcup({
        id: elemId, // 绑定id
        url: url, // url地址

        type: types, // 限制上传类型，为空不限制
        shardsize: "0.5", // 每次分片大小，单位为M，默认1M
        minsize: '', // 最小文件上传M数，单位为M，默认为无
        maxsize: "2048", // 上传文件最大M数，单位为M，默认200M
        // headers: {"version": "fcup-v2.0"}, // 附加的文件头
        // apped_data: {}, //每次上传的附加数据
        timeout: 600000,
        // 定义错误信息
        errormsg: {
            1000: "未找到上传id",
            1001: "类型不允许上传",
            1002: "上传文件过小",
            1003: "上传文件过大",
            1004: "上传请求超时"
        },

        // 错误提示
        error: (msg) => {
            TT.error(msg);
        },

        // 初始化事件
        start: () => {
            console.log('上传已准备就绪');
           
            if (progressElem)
                $('#' + progressElem).attr('value', 0);
            //element.progress(progressElem, 0);
        },

        // 等待上传事件，可以用来loading
        beforeSend: () => {
            //console.log('等待请求中');
        },

        // 上传进度事件
        progress: (num, other) => {
            //element.progress(progressElem, num + "%");
            if (progressElem)
                $('#' + progressElem).attr('value', num);
            //console.log(num);
            //console.log('上传进度' + num);
            //console.log("上传类型" + other.type);
            //console.log("已经上传" + other.current);
            //console.log("剩余上传" + other.surplus);
            //console.log("已用时间" + other.usetime);
            //console.log("预计时间" + other.totaltime);
            console.log(other);
            if (num >= 99 && !uploadFlag) {
                TT.info("已上传" + num + "%，正在进行最后的校验阶段，请耐心等待，上传完成后会出现成功提示", 1000 * 10);
                uploadFlag = true;
            }
        },
        checkurl: checkUrl, // 检查上传url地址
        // 检查地址回调,用于判断文件是否存在,类型,当前上传的片数等操作
        checksuccess: (res) => {
            let data = res ? eval('(' + res + ')') : '';
            let status = data.code;
            let msg = data.message;
            // 错误提示
            if (status == -1) {
                TT.error(msg);
                return false;
            }
            // 已经上传
            if (status == 2) {
                //element.progress(progressElem, "100%");
                if (progressElem)
                    $('#' + progressElem).attr('value', 100);
                $("#" + callBackElem).val(data.data.url);
                //layer.alert(data.msg, { icon: 0 });
                TT.notify(data.msg);//想通过toast的形式提示，需要引入相关文件
                return false;
            }

            // 如果提供了这个参数,那么将进行断点上传的准备
            if (data.data && data.data.file_index) {
               TT.info("已上传大约" + data.data.percent + "%");
                // element.progress(progressElem, data.data.percent + "%");
                if (progressElem)
                    $('#' + progressElem).attr('value', data.data.percent);
                // 起始上传的切片要从1开始
                let file_index = data.data.file_index ? parseInt(data.data.file_index) : 1;
                // 设置上传切片的起始位置
                up.setshard(file_index);
            }
            // 如果接口没有错误，必须要返回true，才不会终止上传
            return true;
        },
        // 上传成功回调，回调会根据切片循环，要终止上传循环，必须要return false，成功的情况下要始终返回true;
        success: (res) => {
            let data = res ? eval('(' + res + ')') : '';
            let url = data.url + "?" + Math.random();
            if (data.completed) {
                //$("#filepath").val(data.path);
                if (callBackElem)
                    $("#" + callBackElem).val(data.path);

                TT.success("上传完成");
                if (progressElem)
                    $('#' + progressElem).attr('value', 100);
                return false;
            }
            // 如果接口没有错误，必须要返回true，才不会终止上传循环
            return true;
        }
    })
}
var upload = layui.upload;
function initUploadFile(elemId, exts = 'jpg|pdf', url = '', callback) {
    if (url == '')
        url = "/file/upload?userId=" + fcupUserId;
    var uploadInst = upload.render({
        elem: '#' + elemId //绑定元素
        , accept: 'file'
        , exts: exts
        , url: url //上传接口
        , size: 1024 * 30 //限定30M文件大小
        , headers: { 'Authorization': localStorage.getItem('accessToken') }
        , data: {
            '__RequestVerificationToken': requestToken
        }
        , before: function (obj) { //obj参数包含的信息，跟 choose回调完全一致，可参见上文。

           console.log('准备上传')
        }
        , done: function (res) {
            //上传完毕回调

            TT.success('上传成功');
            //console.log(res);
            if (typeof (callback) == 'function') {
                callback(res);
            }
        }
        , error: function () {
            //请求异常回调
            TT.error("上传图片失败");
        }
    });
}

//分块上传
function initUploadFileChunk(elemId, types, progressElem = '', url = '', checkUrl = '', appendData = '', callback = '') {
    if (url == '')
        url = "/fileupload/upload";
    if (checkUrl == '')
        checkUrl = "/fileupload/CheckFile";
    var up = new fcup({
        id: elemId, // 绑定id
        url: url, // url地址

        type: types, // 限制上传类型，为空不限制
        shardsize: "0.5", // 每次分片大小，单位为M，默认1M
        minsize: '', // 最小文件上传M数，单位为M，默认为无
        maxsize: "2048", // 上传文件最大M数，单位为M，默认200M
        timeout: 600000,
        headers: { 'Authorization': localStorage.getItem('accessToken'), 'X-CSRF-TOKEN': requestToken },
        apped_data: appendData,
        // 定义错误信息
        errormsg: {
            1000: "未找到上传id",
            1001: "类型不允许上传",
            1002: "上传文件过小",
            1003: "上传文件过大",
            1004: "上传请求超时"
        },

        // 错误提示
        error: (msg) => {
            TT.error(msg);
        },

        // 初始化事件
        start: () => {
            console.log('上传已准备就绪');
            if (progressElem)
                $('#' + progressElem).attr('value', 0).attr('max',100);
        },

        // 等待上传事件，可以用来loading
        beforeSend: () => {
            //console.log('等待请求中');
        },

        // 上传进度事件
        progress: (num, other) => {
            if (progressElem)
                $('#' + progressElem).attr('value', num)
            //console.log(num);
            //console.log('上传进度' + num);
            //console.log("上传类型" + other.type);
            //console.log("已经上传" + other.current);
            //console.log("剩余上传" + other.surplus);
            //console.log("已用时间" + other.usetime);
            //console.log("预计时间" + other.totaltime);
            console.log(other);
            if (num >= 99 && !uploadFlag) {
                TT.info("已上传" + num + "%，正在进行最后的校验阶段，请耐心等待，上传完成后会出现成功提示", 1000 * 10);
                uploadFlag = true;
            }
        },
        checkurl: checkUrl, // 检查上传url地址
        // 检查地址回调,用于判断文件是否存在,类型,当前上传的片数等操作
        checksuccess: (res) => {
            let data = res ? eval('(' + res + ')') : '';
            let status = data.code;
            let msg = data.message;
            // 错误提示
            if (status == -1) {
                TT.error(msg);
                return false;
            }
            // 已经上传
            if (status == 2) {
                if (progressElem)
                    $('#' + progressElem).attr('value', 100)
                //$("#" + callBackElem).val(data.data.url);
                if (typeof (callback) == 'function') {
                    callback();
                }
                Toast.notify(data.msg);//想通过toast的形式提示，需要引入相关文件
                return false;
            }

            // 如果提供了这个参数,那么将进行断点上传的准备
            if (data.data && data.data.file_index) {
                TT.info("已上传大约" + data.data.percent + "%");
                if (progressElem)
                    $('#' + progressElem).attr('value', data.data.percent)
                // 起始上传的切片要从1开始
                let file_index = data.data.file_index ? parseInt(data.data.file_index) : 1;
                // 设置上传切片的起始位置
                up.setshard(file_index);
            }
            // 如果接口没有错误，必须要返回true，才不会终止上传
            return true;
        },
        // 上传成功回调，回调会根据切片循环，要终止上传循环，必须要return false，成功的情况下要始终返回true;
        success: (res) => {
            let data = res ? eval('(' + res + ')') : '';
            if (data.completed) {
                if (typeof (callback) == 'function') {
                    callback(res);
                }
                TT.info("上传完成");
                if (progressElem)
                    $('#' + progressElem).attr('value', 100)
                return false;
            }
            // 如果接口没有错误，必须要返回true，才不会终止上传循环
            return true;
        }
    })
}

//上传小文件，传入对象
function initUploadFilePro(obj) {
    if (!obj.url || obj.url == '')
        obj.url = "/file/upload?userId=" + fcupUserId;
    if (!obj.exts)
        obj.exts = 'jpg|jpeg|png';
    if (!obj.accept)
        obj.accept = 'file';
    if (!obj.size)
        obj.size = 1024 * 2;
    console.log(obj);
    var uploadInst = upload.render({
        elem: '#' + obj.elemId //绑定元素
        , accept: obj.accept//images 图片类型,file 所有文件类型,video 视频类型,audio 音频类型
        , exts: obj.exts
        , url: obj.url //上传接口
        , size: obj.size //限定30M文件大小
        , headers: { 'Authorization': localStorage.getItem('accessToken') }
        , data: {
            '__RequestVerificationToken': requestToken
        }
        , before: function (bobj) { //obj参数包含的信息，跟 choose回调完全一致，可参见上文。

            //layer.load(); //上传loading
            bobj.preview(function (index, file, result) {
                console.log(file.size + '字节');
                //console.log(file);
                //console.log(result)
                if (file.size / 1024 > obj.size) {
                    TT.warning(`请上传${obj.size}MB以内的文件`);
                    //layer.msg(`请上传${obj.size}MB以内的文件`, { icon: 2 });
                    //layer.closeAll('loading');
                    return;
                }
                if (obj.previewId) {
                    document.getElementById(obj.previewId).src = result;
                    
                }
                
            });
        }
        , choose: function (cobj) {
            //// 将每次选择的文件追加到文件队列
            //var files = obj.pushFile();

            //// 预读本地文件，如果是多文件，则会遍历。(不支持ie8/9)
            //cobj.preview(function (index, file, result) {
            //    console.log(index); // 得到文件索引
            //    console.log(file); // 得到文件对象
            //    console.log(result); // 得到文件base64编码，比如图片

            //    // obj.resetFile(index, file, '123.jpg'); // 重命名文件名

            //    // 这里还可以做一些 append 文件列表 DOM 的操作

            //    // obj.upload(index, file); // 对上传失败的单个文件重新上传，一般在某个事件中使用
            //    // delete files[index]; //删除列表中对应的文件，一般在某个事件中使用
            //});
        }
        , done: function (res) {
            //上传完毕回调

            TT.info('上传成功');
            //console.log(res);
            if (typeof (obj.callback) == 'function') {
                obj.callback(res);
            }
        }
        , error: function () {
            //请求异常回调
            TT.error("上传图片失败");
        }
    });
}