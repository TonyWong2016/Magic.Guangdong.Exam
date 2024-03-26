let randomInt = Math.floor(Math.random() * 1000);

function refreshRandomInt() {
    randomInt = Math.floor(Math.random() * 1000);
}

//自动创建面包屑(临时)
function setBreadCrumb() {
    let breadArr = JSON.parse(localStorage.getItem('breadcrumb'));
    var breadCrumb = document.getElementById('breadcrumb');
    breadCrumb.innerHTML = '';
    breadArr.forEach(function (item) {
        breadCrumb.innerHTML += '<a href="javascript:;">' + item.name + '</a>';
    })
    let element = layui.element;
    element.render('breadcrumb', 'breadcrumb');
}

//自动给main-form类名的form表单添加lay-verify属性，以便验证
function autoCheckFormRequired() {
    var formsWithClass = document.querySelectorAll('form.main-form');

    formsWithClass.forEach(function (form) {
        // 对每个找到的form表单
        Array.from(form.elements).forEach(function (element) {
            // 判断元素是否有name属性
            if (element.hasAttribute('name') &&
                element.hasAttribute('data-val-required') &&
                element.getAttribute('data-val-required').includes('is required') &&
                !element.hasAttribute('lay-verify')
            ) {
                element.setAttribute('lay-verify', 'required');
            }
        });
    });
}

function renderTpl(tplid, viewid, checkbarParams, append) {
    return new Promise((resolve, reject) => {
        if (tplid && viewid) {
            var tpl = document.getElementById(tplid).innerHTML, view = document.getElementById(viewid);
            layui.use('laytpl', function () {
                var laytpl = layui.laytpl;
                laytpl(tpl).render(checkbarParams, function (html) {
                    if (append)
                        view.innerHTML += html;
                    else
                        view.innerHTML = html;
                })
            });
            resolve(true);
        }
        reject(false);
    })

}

function renderTplNow(tplid, viewid, checkbarParams, append) {
    if (tplid && viewid) {
        var tpl = document.getElementById(tplid).innerHTML, view = document.getElementById(viewid);
        layui.use('laytpl', function () {
            var laytpl = layui.laytpl;
            laytpl(tpl).render(checkbarParams, function (html) {
                if (append)
                    view.innerHTML += html;
                else
                    view.innerHTML = html;
            })
        });
    }
}



function renderTplByClass(tplid, viewclass, index, checkbarParams, append) {
    if (tplid && viewclass) {
        var tpl = document.getElementById(tplid).innerHTML, view = document.getElementsByClassName(viewclass)[index];
        layui.use('laytpl', function () {
            var laytpl = layui.laytpl;
            laytpl(tpl).render(checkbarParams, function (html) {
                if (append)
                    view.innerHTML += html;
                else
                    view.innerHTML = html;
            })
        });
    }
}


function renderLayuiFormElem(params) {
    let form = layui.form;
    if (!params.time)
        params.time = 500;
    setTimeout(() => {
        if (params.elemType && params.elemFilter)
            form.render(params.elemType, params.elemFilter);
        else if (params.elemType && !params.elemFilter)
            form.render(params.elemType);
        else if (!params.elemType && params.elemFilter)
            form.render(null, params.elemFilter);
        else
            form.render();
    }, params.time)

}

function successMsg(msg, callback = '') {
    if (typeof (callback) == 'function') {
        layer.msg(msg, { icon: 1 }, () => {
            callback();
        });
    } else
        layer.msg(msg, { icon: 1 });
}
function errorMsg(msg, callback = '') {
    if (typeof (callback) == 'function') {
        layer.msg(msg, { icon: 2 }, () => {
            callback();
        });
    } else
        layer.msg(msg, { icon: 2 });
}
function warnMsg(msg, callback = '') {
    if (typeof (callback) == 'function') {
        layer.msg(msg, { icon: 0 }, () => {
            callback();
        });
    } else
        layer.msg(msg, { icon: 0 });
}

//弹出层-初级封装
function openDiv(obj, cancleCallback = '') {
    var area = ['800px', '500px'];
    if (obj.width && obj.height) {
        area = [obj.width, obj.height];
    }
    var content = $('#' + obj.elem + '');
    if (obj.istpl)
        content = $('#' + obj.elem + '').html();


    if (obj.url) {
        layer.open({
            type: 2,
            title: obj.title,
            shadeClose: true,
            shade: false,
            maxmin: true, //开启最大化最小化按钮
            area: area,
            content: url,
            cancel: function (index) {

                if (typeof (cancleCallback) == 'function') {
                    cancleCallback();
                }
            }
        });
    } else {
        layer.open({
            type: 1,
            shade: [0.7, '#393D49'],
            title: obj.title, //不显示标题
            area: area, //宽高
            anim: 2,
            shadeClose: false, //是否开启遮罩关闭
            content: content, //捕获的元素，注意：最好该指定的元素要存放在body最外层，否则可能被其它的相对元素所影响
            cancel: function (index) {
                $('#' + obj.elem + '').hide();
                if (typeof (cancleCallback) == 'function') {
                    cancleCallback();
                }
            }
        });
    }
}

function removeItem(obj) {
    if (!obj.msg)
        obj.msg = "确定要删除当前项目吗？";
    if (obj.router && obj.removeId) {
        layer.confirm(obj.msg, { icon: 0 }, async function () {
            let formData = new FormData();
            formData.append('id', obj.removeId);
            formData.append('__RequestVerificationToken', requestToken);
            try {
                const result = await request('POST', obj.router, formData, { 'Content-Type': 'multipart/form-data' });
                if (result.code == 0) {
                    successMsg('操作成功', () => {
                        if (obj.callback && typeof (obj.callback) == 'function')
                            obj.callback();
                        else
                            console.log('success');
                        if (obj.refresh) {
                            location.reload();
                        }
                    })
                }

            } catch (error) {
                errorMsg('操作失败' + error);
            }

        }, function () {
            if (obj.cancel && typeof (obj.cancel) == 'function') {
                obj.cancel();

            } else
                layer.closeAll();
        })
    }
}

//渲染table
function getTable(params, callBack = '') {
    if (!params.height)
        params.height = 860;
    if (!params.method)
        params.method = "get";
    if (!params.page)
        params.limit = 1000
    else if (!params.limit)
        params.limit = 10
    layui.use('table', function () {
        var table = layui.table;
        //第一个实例
        table.render({
            elem: params.elem
            , height: params.height
            , url: params.url //数据接口
            , method: params.method
            , page: params.page //开启分页
            , headers: params.headers
            , limit: params.limit
            //, skin: 'row' //行边框风格
            , toolbar: params.tool
            , defaultToolbar: params.defaultToolbar
            , size: params.size
            , where: params.where
            , parseData: function (res) { //res 即为原始返回的数据
                //console.log(res)
                if (res.code == 0) {
                    if (params.save && params.local_name) {
                        if (params.local)
                            localStorage.setItem(params.local_name, JSON.stringify(res.data));
                        else
                            sessionStorage.setItem(params.local_name, JSON.stringify(res.data));
                    }
                    return {
                        "code": res.code, //解析接口状态
                        "msg": res.msg, //解析提示文本
                        "count": res.data.total, //解析数据长度
                        "data": res.data.items, //解析数据列表
                        "other": res.data.other
                    }
                }
                else if (res.code != 0) {
                    if (params.nomsg) {
                        return;
                    } else {
                        layer.msg("" + "" + res.msg + "", { icon: 0 });
                        return;
                    }
                }

            }
            , request: {
                pageName: 'pageindex' //页码的参数名称，默认：page
                , limitName: 'pagesize' //每页数据量的参数名，默认：limit
            }
            , response: {
                statusCode: 0 //规定成功的状态码，默认：0 
            }
            , cols: [params.cols]
            , done: function (res, curr, count) {
                layer.closeAll("loading");
                if (count == 0 && !params.nomsg) {

                    layer.msg("暂无数据...", { icon: 0 });
                }
                if (params.tpl) {
                    for (var i = 0; i < count; i++) {
                        renderTpl(params.tpl, params.view, i + 1, false);
                    }
                }


                //答题：选题的时候显示总分数
                if (params.elemId) {
                    $(params.elemId).html(res.msg);
                }
                if (params.specialStorage) {
                    //console.log(res);
                    sessionStorage.setItem(params.specialStorage, JSON.stringify(res.data));
                }

                if (typeof (callBack) == 'function') {
                    callBack(res);
                }
            }
        });
    });
}

function getTableNoUrl(params, callBack = '') {
    if (!params.height)
        params.height = 860;
    //if (!params.width)
    //    params.width = 1000;
    if (!params.method)
        params.method = "get";
    if (!params.page)
        params.limit = 1000
    else if (!params.limit)
        params.limit = 10
    layui.use('table', function () {
        var table = layui.table;
        //第一个实例
        table.render({
            elem: params.elem
            , height: params.height
            , width: params.width
            //, url: params.url //数据接口
            , method: params.method
            , page: params.page //开启分页
            , headers: params.headers
            , limit: params.limit
            //, skin: 'row' //行边框风格
            , toolbar: params.tool
            , size: params.size
            , cols: [params.cols]
            , data: params.data
            , done: function (res, curr, count) {
                layer.closeAll("loading");
                if (typeof (callBack) == 'function') {
                    callBack(res);
                }
            }
        });
    });
}