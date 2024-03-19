
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
            resolve(true);
        }
        reject(false);
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


// 定义一个函数帮助我们找到对象并删除
function removeObjectFromArray(array, key, value) {
    for (let i = 0; i < array.length; i++) {
        if (array[i][key] === value) {
            array.splice(i, 1); // 删除找到的对象
            break; // 找到并删除后就可以退出循环了
        }
    }
}

function setBreadCrumb() {
    let breadArr = JSON.parse(localStorage.getItem('breadcrumb'));
    var breadCrumb = document.getElementById('breadcrumb');
    breadCrumb.innerHTML = '';
    breadArr.forEach(function (item) {
        breadCrumb.innerHTML += '<a href="javascript">' + item.name + '</a>';
    })
    let element = layui.element;
    element.render('breadcrumb', 'breadcrumb');
}

function listToTree(items) {
    // 创建一个临时映射表，以加快查找速度
    const map = new Map();
    items.forEach(item => {
        if (!map.has(item.id)) {
            map.set(item.id, { ...item, children: [] });
        }
    });

    // 遍历数组，将每个元素附加到其父节点的children数组中
    items.forEach(item => {
        if (item.parentId !== 0 && map.has(item.parentId)) {
            map.get(item.parentId).children.push(map.get(item.id));
        }
    });

    // 找到根节点（即parentId为0的节点）
    const roots = [];
    map.forEach((value, key) => {
        if (value.parentId === 0) {
            roots.push(value);
        }
    });

    return roots;
}
