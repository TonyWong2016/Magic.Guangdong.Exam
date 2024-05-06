let tinymceUserId = getCookie('userId');
if (tinymceUserId)
    tinymceUserId = atob(tinymceUserId);
//增加一个富文本编辑器，tinymce，中文文档：http://tinymce.ax-z.cn/
function InitTinymce(elemId, content, file_base_url, base_child ='/FileAttach') {
    //let file_base_url = "http://v.superzb.cn/fileattach/";
    if (!base_ajax_url) {
        file_base_url = window.location.protocol + "//" + window.location.host + base_child;
    }
    tinymce.init({
        selector: '#' + elemId,
        license_key:'gpl',
        language: 'zh_CN',//注意大小写
        height: 600,
        skin: 'dark',
        skin_url:'/plugins/tinymce/skins/ui/oxide',
        //inline: true,
        plugins: 'code link image axupimgs autolink autosave autoresize charmap hr print preview imagetools insertdatetime searchreplace wordcount kityformula-editor table media codesample lists advlist textpattern',
        toolbar: 'code undo redo | link image axupimgs table kityformula-editor charmap insertdatetime| bold italic underline alignleft alignright aligncenter searchreplace  styleselect| hr bullist numlist  |preview print',
        automatic_uploads: false,
        setup: function (editor) {
            editor.on('change', function () { editor.save(); });
            //editor.on('init', function (e) {
            //    this.getBody().style.fontSize = '14pt';
            //    this.getBody().style.fontFamily = '微软雅黑';
            //});
        },
        images_upload_handler: function (blobInfo, succFun, failFun) {
            var xhr, formData;
            var file = blobInfo.blob();//转化为易于理解的file对象
            xhr = new XMLHttpRequest();
            xhr.withCredentials = false;
            xhr.open('POST', '/file/upload?userId=' + tinymceUserId);
            xhr.setRequestHeader('Authorization', localStorage.getItem('accessToken')); // 设置自定义头

            xhr.onload = function () {
                var json;
                if (xhr.status != 200) {
                    failFun('HTTP Error: ' + xhr.status);
                    console.log('HTTP Error: ' + xhr.status);
                    return;
                }
                json = JSON.parse(xhr.responseText);
                //if (!json || typeof json.location != 'string') {
                if (!json) {
                    //failFun('Invalid JSON: ' + xhr.responseText);
                    console.log('Invalid JSON: ' + xhr.responseText);
                    return;
                }
                succFun(file_base_url + json.data);
                //console.log(json);
            };
            //console.log(file.size);
            if (file.size && file.size > 1024 * 1024 * 50) {
                //Toaster("error", "文件大小不可高于50M", "错误");
                TT.error("文件大小不可高于50M");
            } else {
                formData = new FormData();
                formData.append('file', file, file.name);//此处与源文档不一样
                formData.append('__RequestVerificationToken', requestToken);
                xhr.send(formData);
            }
        },
        file_picker_callback: function (callback, value, meta) {
            //文件分类
            var filetype = '.pdf, .txt, .zip, .rar, .7z, .doc, .docx, .xls, .xlsx, .ppt, .pptx, .jpg, .png, .jpeg, .mp4';
            //后端接收上传文件的地址            
            var upurl = '/file/upload?userId=' + tinymceUserId;
            //模拟出一个input用于添加本地文件
            var input = document.createElement('input');
            input.setAttribute('type', 'file');
            input.setAttribute('accept', filetype);
            input.click();
            input.onchange = function () {
                var file = this.files[0];
                var xhr, formData;
                console.log(file.name);
                xhr = new XMLHttpRequest();
                xhr.wUrlthCredentials = false;
                xhr.open('POST', upurl);
                xhr.setRequestHeader('Authorization', localStorage.getItem('accessToken')); // 设置自定义头

                xhr.onload = function () {
                    var json;
                    if (xhr.status != 200) {
                        console.log('HTTP Error: ' + xhr.status);
                        return;
                    }
                    json = JSON.parse(xhr.responseText);
                    if (!json) {
                        console.log('Invalid JSON: ' + xhr.responseText);
                        return;
                    }
                    callback(file_base_url + json.data);
                    //$('img')
                };
                
                if (file.size && file.size > 1024 * 1024 * 50) {
                    /*Toaster("error", "文件大小不可高于20M", "错误");*/
                    TT.error("文件大小不可高于50M");
                } else {
                    formData = new FormData();
                    formData.append('file', file, file.name);
                    formData.append('__RequestVerificationToken', requestToken)
                    xhr.send(formData);
                }
            };
        },
        init_instance_callback: function (editor) {
            console.log("初始化完成");
            tinyMCE.activeEditor.insertContent(content);
            $('.tox-promotion').hide();
            $('.tox-statusbar__branding').hide();
        }
    });
}


function InitTinymcePro(obj) {
    if (!obj.base_ajax_url) {
        obj.file_base_url = window.location.protocol + "//" + window.location.host + obj.base_child;
    }
    tinymce.init({
        selector: '#' + obj.elemId,
        license_key: 'gpl|m1v4j8ztq1huct9lx2srh2jhkhpwrrxunlopgdbuclu6rxz9',
        language: 'zh_CN',//注意大小写
        height: 600,
        plugins: 'code link image axupimgs autosave print preview imagetools searchreplace wordcount kityformula-editor table media codesample',
        toolbar: 'code undo redo | styleselect | bold italic underline alignleft alignright aligncenter searchreplace wordcount| link image axupimgs |preview print |table',

        setup: function (editor) {
            editor.on('change', function () { editor.save(); });
            
        },
        images_upload_handler: function (blobInfo, succFun, failFun) {
            var xhr, formData;
            var file = blobInfo.blob();//转化为易于理解的file对象
            xhr = new XMLHttpRequest();
            xhr.withCredentials = false;
            xhr.open('POST', '/file/upload?userId=' + tinymceUserId);
            xhr.setRequestHeader('Authorization', localStorage.getItem('accessToken')); // 设置自定义头


            xhr.onload = function () {
                var json;
                if (xhr.status != 200) {
                    failFun('HTTP Error: ' + xhr.status);
                    console.log('HTTP Error: ' + xhr.status);
                    return;
                }
                json = JSON.parse(xhr.responseText);
                if (!json) {
                    console.log('Invalid JSON: ' + xhr.responseText);
                    return;
                }
                succFun(obj.file_base_url + json.data);
            };
            if (file.size && file.size > 1024 * 1024 * 50) {
                TT.error("文件大小不可高于50M");
            } else {
                formData = new FormData();
                formData.append('file', file, file.name);//此处与源文档不一样
                xhr.send(formData);
            }
        },
        file_picker_callback: function (callback, value, meta) {
            //文件分类
            var filetype = '.pdf, .txt, .zip, .rar, .7z, .doc, .docx, .xls, .xlsx, .ppt, .pptx, .jpg, .png, .jpeg';
            //后端接收上传文件的地址            
            var upurl = '/file/upload?userId=' + tinymceUserId;
            //模拟出一个input用于添加本地文件
            var input = document.createElement('input');
            input.setAttribute('type', 'file');
            input.setAttribute('accept', filetype);
            input.click();
            input.onchange = function () {
                var file = this.files[0];
                var xhr, formData;
                console.log(file.name);
                xhr = new XMLHttpRequest();
                xhr.wUrlthCredentials = false;
                xhr.open('POST', upurl);
                xhr.setRequestHeader('Authorization', localStorage.getItem('accessToken')); // 设置自定义头

                xhr.onload = function () {
                    var json;
                    if (xhr.status != 200) {
                        console.log('HTTP Error: ' + xhr.status);
                        return;
                    }
                    json = JSON.parse(xhr.responseText);
                    if (!json) {
                        console.log('Invalid JSON: ' + xhr.responseText);
                        return;
                    }
                    callback(obj.file_base_url + json.data);//不表现在页面上，而是以附件形式展示
                    //console.log(value);
                    if (typeof (obj.callback) == 'function') {
                        obj.callback(obj.file_base_url + json.data);
                    }
                };

                if (file.size && file.size > 1024 * 1024 * 50) {
                    TT.error("文件大小不可高于50M");
                } else {
                    formData = new FormData();
                    formData.append('file', file, file.name);
                    formData.append('__RequestVerificationToken', requestToken)
                    xhr.send(formData);
                }
            };
        },
        init_instance_callback: function (editor) {
            console.log("初始化完成");
            
            tinyMCE.activeEditor.insertContent(obj.content);
        }
    });
}

function ClearTinymce(type) {
    if (type == "div") {
        tinymce.remove('div');
        return;
    }
    if (type == "textarea") { 
        tinymce.remove('textarea');
        return;
    }
    if (type == "all") {
        tinymce.remove();
        return;
    }
    tinymce.remove('#' + type);
    
}