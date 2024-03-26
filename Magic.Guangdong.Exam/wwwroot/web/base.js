
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



// 定义一个函数帮助我们找到对象并删除
function removeObjectFromArray(array, key, value) {
    for (let i = 0; i < array.length; i++) {
        if (array[i][key] === value) {
            array.splice(i, 1); // 删除找到的对象
            break; // 找到并删除后就可以退出循环了
        }
    }
}
//生成指定长度的随机字符
function generateRandomString(length) {
    let possibleChars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let randomString = '';

    for (let i = 0; i < length; i++) {
        let randomIndex = Math.floor(Math.random() * possibleChars.length);
        randomString += possibleChars.charAt(randomIndex);
    }

    return randomString;
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

function objectToFormData(obj, form, namespace) {
    const fd = form || new FormData();
    const formKey = namespace ? `${namespace}[${encodeURIComponent(String(obj))}]` : encodeURIComponent(String(obj));

    for (const property in obj) {
        if (obj.hasOwnProperty(property)) {
            const value = obj[property];
            const key = namespace ? `${formKey}[${encodeURIComponent(property)}]` : encodeURIComponent(property);

            if (value instanceof File) {
                fd.append(key, value);
            } else if (value instanceof Blob) {
                fd.append(key, value, value.name);
            } else if (Array.isArray(value)) {
                for (let i = 0; i < value.length; i++) {
                    objectToFormData(value[i], fd, `${key}[${i}]`);
                }
            } else if (value && typeof value === 'object' && !(value instanceof Date)) {
                objectToFormData(value, fd, key);
            } else {
                fd.append(key, String(value));
            }
        }
    }

    return fd;
}

function parseJwtPayload(jwt) {
    let base64Url = jwt.split('.')[1];
    let base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    return JSON.parse(window.atob(base64));
}

function setLoginInfo(jwt) {
    let base64Url = jwt.split('.')[1];
    let base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    let item = JSON.parse(window.atob(base64));
    let userId = item['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid'];
    let nameidentifier = item['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
    localStorage.setItem('userId', userId);
    localStorage.setItem('accessToken', window.btoa(nameidentifier + '|' + item.exp));
    let expDays = (item.exp - (Math.round(new Date() / 1000))) / 86400;
    setCookie('userId', userId, expDays)
    setCookie('examToken', jwt, expDays);

}


// 函数：设置（写入）cookie，支持SameSite属性
function setCookie(name, value, daysToExpire, sameSite = 'Lax') {
    // HTTP-only标志必须在服务器端设置，这里仅做提示
    //console.warn('请注意：HTTP-only标志应由服务器端设置，客户端无法直接设置。');

    let cookieText = `${name}=${encodeURIComponent(value)};`;

    // 有效期
    if (daysToExpire) {
        const date = new Date();
        date.setTime(date.getTime() + (daysToExpire * 24 * 60 * 60 * 1000));
        cookieText += `Expires=${date.toUTCString()};`;
    }

    // SameSite属性
    cookieText += `SameSite=${sameSite.toLowerCase()};`;

    // 路径
    cookieText += 'Path=/;';

    // 将最终的cookie文本设置到document.cookie
    document.cookie = cookieText;
}

// 函数：读取cookie
function getCookie(name) {
    const nameEQ = `${name}=`;
    const cookies = document.cookie.split(';');
    for (let i = 0; i < cookies.length; i++) {
        let cookie = cookies[i].trim();
        if (cookie.startsWith(nameEQ)) {
            return decodeURIComponent(cookie.substring(nameEQ.length));
        }
    }
    return null;
}

// 函数：删除cookie，考虑到SameSite属性
function deleteCookie(name) {
    setCookie(name, '', -1, 'Lax'); // 设置有效期为过去，同时设置SameSite属性
}

function getUrlQueryParams(parameterName = null,url = window.location.href) {
    const queryStart = url.indexOf('?') + 1;
    const queryEnd = url.indexOf('#') > 0 ? url.indexOf('#') : url.length;

    const rawQuery = url.slice(queryStart, queryEnd);
    const queryParams = {};

    if (rawQuery.length > 0) {
        rawQuery.split('&').forEach(param => {
            const [key, value] = param.split('=');
            queryParams[decodeURIComponent(key)] = decodeURIComponent(value);
        });
    }

    // 如果指定了参数名称，直接返回该参数的值
    if (parameterName !== null) {
        return queryParams[decodeURIComponent(parameterName)] || null;
    }

    return queryParams;
}

function removeQueryParamFromCurrentUrl(paramName) {
    const url = new URL(window.location.href);
    const params = new URLSearchParams(url.search);

    // 移除指定的参数
    params.delete(paramName);

    // 更新URL的search部分
    url.search = params.toString();

    // 生成新的URL字符串（不能直接修改window.location.href，需要用history.replaceState）
    const newUrl = url.toString();

    // 使用History API替换当前URL（无页面跳转）
    history.replaceState({}, '', newUrl);
}

function calculateTimestampDifferenceInSeconds(timestamp1, timestamp2) {
    // 先确定哪个时间戳更早，以便始终得到正数结果
    const earlierTimestamp = Math.min(timestamp1, timestamp2);
    const laterTimestamp = Math.max(timestamp1, timestamp2);

    // 相差秒数就是后面的时间戳减去前面的时间戳
    const differenceInSeconds = laterTimestamp - earlierTimestamp;

    return differenceInSeconds;
}
