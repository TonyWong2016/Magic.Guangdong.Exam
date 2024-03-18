
layui.use(['element', 'layer', 'util'], function () {
    var element = layui.element;
    var layer = layui.layer;
    var util = layui.util;
    var $ = layui.$;

    //头部事件
    util.event('lay-header-event', {
        menuLeft: function (othis) { // 左侧菜单事件
            layer.msg('展开左侧菜单的操作', { icon: 0 });
        },
        menuRight: function () {  // 右侧菜单事件
            layer.open({
                type: 1,
                title: '更多',
                content: '<div style="padding: 15px;">处理右侧面板的操作</div>',
                area: ['260px', '100%'],
                offset: 'rt', // 右上角
                anim: 'slideLeft', // 从右侧抽屉滑出
                shadeClose: true,
                scrollbar: false
            });
        }
    });
});

function executeFunctions(funcs) {
    const results = [];

    return funcs.reduce((promiseChain, func) => {
        return promiseChain.then(() => (
            new Promise((resolve, reject) => {
                // 如果函数返回Promise，则等待其resolve/reject
                if (func.constructor.name === 'AsyncFunction' || func instanceof Promise) {
                    func().then(result => {
                        results.push({ status: 'success', data: result });
                        resolve();
                    }).catch(err => {
                        results.push({ status: 'error', error: err });
                        resolve(); // 即使出错也继续执行后续函数
                    });
                }
                // 如果函数是普通函数（同步函数），则立即执行并收集结果
                else {
                    try {
                        const result = func();
                        results.push({ status: 'success', data: result });
                        resolve();
                    } catch (err) {
                        results.push({ status: 'error', error: err });
                        resolve(); // 同样即使出错也继续执行后续函数
                    }
                }
            })
        ));
    }, Promise.resolve())
        .then(() => results);
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
            resolve("渲染完成");
        }
        reject("渲染失败");
    })
    
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