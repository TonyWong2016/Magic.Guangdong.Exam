//构造xmselect，注意页面要引用xm-select.js
function makeXmSelect(obj) {
    $.getJSON(obj.url, obj.param, function (json) {
        if (json.code == 0) {
            return obj.result = xmSelect.render({
                el: `#${obj.elem}`,
                filterable: obj.filter ? obj.filter : true,
                tips: obj.tips,
                radio: obj.radio,
                clickClose: obj.radio,
                theme: {
                    color: obj.color ? obj.color : '#0081ff',
                },
                direction: obj.direction ? obj.direction : 'down',
                //prop: {
                //    name : 'caption',
                //    value : 'code'
                //},
                disabled: obj.disabled,
                toolbar: {
                    show: obj.tool ? obj.tool : false,
                },
                autoRow: obj.autoRow ? obj.autoRow:false,
                prop: obj.prop,
                initValue: obj.initValue,
                data: json.data,
                on: function (data) {
                    if (obj.func && typeof (obj.func == 'function')) {
                        obj.func(data);
                    }
                },
            })
        }
    })
}

function makeXmSelectCreate(obj) {
    $.getJSON(obj.url, obj.param, function (json) {
        if (json.code == 0) {
            return obj.result = xmSelect.render({
                el: `#${obj.elem}`,
                filterable: true,
                tips: obj.tips,
                radio: obj.radio,
                clickClose: obj.radio,
                theme: {
                    color: obj.color ? obj.color : '#0081ff',
                },
                direction: obj.direction ? obj.direction : 'down',
                toolbar: {
                    show: obj.tool ? obj.tool : false,
                },
                prop: obj.prop,
                initValue: obj.initValue,
                data: json.data,
                on: function (data) {
                    if (obj.func && typeof (obj.func == 'function')) {
                        obj.func(data);
                    }
                },
                //Pro版本支持创建自定义条目
                create: function (val, arr) {
                    return {
                        text: '创建-' + val,
                        name: '创建-' + val,
                        value: val
                    }
                }
            })
        }
    })
}

function makeXmSelectTree(obj, callback) {
    $.getJSON(obj.url, obj.param, function (json) {
        if (json.code == 0) {
            return obj.result = xmSelect.render({
                el: `#${obj.elem}`,
                filterable: obj.filter ? obj.filter : true,
                autoRow: true,
                tips: obj.tips,
                theme: {
                    color: obj.color ? obj.color : '#0081ff',
                },
                direction: obj.direction ? obj.direction : 'down',
                toolbar: {
                    show: obj.tool ? obj.tool : false,
                },
                height: 'auto',
                prop: obj.prop,
                disabled: obj.disabled ? obj.disabled : false,
                tree: {
                    show: true,
                    showFolderIcon: true,
                    strict: false,
                    indent: 20,
                    //点击节点是否展开
                    clickExpand: true,
                    lazy: obj.lazy ? obj.lazy : true,
                    load: function (item, cb) {
                        //$.getJSON("../code/GetSubjectCode", { "parentCode": item.id }, function(children) {
                        //    return cb(children.data);
                        //})
                        if (typeof (callback) == "function") {
                            callback(item, cb)
                        }
                    }
                },
                data: json.data,
                on: function (data) {
                    if (obj.func && typeof (obj.func == 'function')) {
                        obj.func(data);
                    }
                },
            });
        }
    });
}