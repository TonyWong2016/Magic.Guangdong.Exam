let baseDownloadUrl = location.protocol + "//" + location.host;
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
            fetchKeyActions();
                 
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

function removeElementFromArrayByValue(array, value) {
    if (!Array.isArray(array)) {
        throw new Error('Invalid input: Ensure you provide a valid array.');
    }

    const filteredArray = array.filter((item) => item !== value);
    return filteredArray;
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
function objectToFormData2(obj, form, namespace) {
    var form = form || new FormData();
    var formKey;

    for (var property in obj) {
        if (obj.hasOwnProperty(property)) {

            if (Array.isArray(obj[property])) {
                // 处理数组属性
                for (var i = 0; i < obj[property].length; i++) {
                    formKey = namespace ? namespace + '[' + property + '][]' : property + '[]';
                    objectToFormData({ [property]: obj[property][i] }, form, formKey);
                }
            } else if (typeof obj[property] === 'object' && obj[property] !== null && !(obj[property] instanceof File)) {
                // 非数组且为嵌套对象的情况
                formKey = namespace ? namespace + '[' + property + ']' : property;
                objectToFormData(obj[property], form, formKey);
            } else {
                // 基本类型值或File对象
                formKey = namespace ? namespace + '[' + property + ']' : property;
                form.append(formKey, obj[property]);
            }
        }
    }

    return form;
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
    let userName = item['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
    localStorage.setItem('userId', userId);
    localStorage.setItem('userName', userName);
    localStorage.setItem('accessToken', window.btoa(nameidentifier + '|' + item.exp));
    let expDays = (item.exp - (Math.round(new Date() / 1000))) / 86400;
    setCookie('userId', userId, expDays)
    setCookie('examToken', jwt, expDays);
}



function clearLoginInfo() {
    deleteCookie('userId');
    deleteCookie('examToken');
    localStorage.removeItem('userId');
    localStorage.removeItem('accessToken');
    localStorage.removeItem('userName');
    if (parent.location) {
        parent.location.href = "/home/index"
    }
    window.location.href = "/home/index";
}

function watchInput(inputId, callback, debounce =1500) {
    var inputElement = document.getElementById(inputId);
    let flag = true;
    if (!inputElement) {
        console.error('无法找到id为' + inputId + '的元素');
        return;
    }

    // 使用防抖(debounce)技术处理连续输入的情况，避免过于频繁的回调调用
    var debounceTimeout;

    function handleInputChange() {
        flag = false;
        clearTimeout(debounceTimeout);
        debounceTimeout = setTimeout(function () {
            let value = inputElement.value.replace(/^\s+|\s+$/g, "");
            callback(value);
            flag = true;  
        }, debounce); 
        
    }

    // 监听input事件（实时输入变化）和change事件（输入框失去焦点）
    inputElement.addEventListener('input', handleInputChange);
    inputElement.addEventListener('change', handleInputChange);
    // 当文本框失去焦点时，清除定时器以防止后续触发回调
    inputElement.addEventListener('blur', function () {
        if (!flag)
            callback(inputElement.value.replace(/^\s+|\s+$/g, ""));
        clearTimeout(debounceTimeout);
       
    });
}

function autoSearch(elemId, callback) {
    let last;
    //搜索关键字
    $("#" + elemId).on('input propertychange', function (event) {
        last = event.timeStamp;
        //利用event的timeStamp来标记时间，这样每次事件都会修改last的值，注意last必需为全局变量
        setTimeout(function () {    //设时延迟x秒执行
            if (last - event.timeStamp == 0)//如果时间差为0（也就是你停止输入x秒之内都没有其它的keyup事件发生）则做你想要做的事
            {
                if (typeof (callback) == 'function') {
                    let value = $("#" + elemId).val().replace(/^\s+|\s+$/g, "");
                    callback(value);
                        
                }
            }
        }, 1500);
    });
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

function autoCheckLoginStatus() {
    if (isCookieExpired('userId') || !getCookie('userId')) {
        clearLoginInfo();
    }
    
}

function isCookieExpired(name) {
    // 获取指定名称的cookie值
    var cookie = document.cookie.split(';').find(function (cookie) {
        return cookie.trim().startsWith(name + '=');
    });

    if (!cookie) {
        // 如果找不到该cookie，则默认认为已过期
        return true;
    }

    // 分离cookie名称和值
    cookie = cookie.split('=');
    // 获取cookie的过期时间（如果有）
    var expires = cookie[1].match(/expires=([^;]+)/);

    if (!expires || !expires[1]) {
        // 如果没有expires属性，则默认认为未过期
        return false;
    }

    // 解析expires属性的UTC日期字符串为Date对象
    var expirationDate = new Date(expires[1]);
    // 检查当前时间是否晚于cookie的过期时间
    return expirationDate <= new Date();
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

//判定某个字符是否包含cnt种以上的字符
function checkStringVariety(str, cnt = 2) {
    const hasLowerCase = /[a-z]/.test(str);
    const hasUpperCase = /[A-Z]/.test(str);
    const hasNumbers = /\d/.test(str);
    const hasSpecialChars = /[!@#$%^&*(),.?":{}|<>]/.test(str); // 这里列举了一些特殊字符，你可以根据需求扩展

    let count = 0;
    count += hasLowerCase ? 1 : 0;
    count += hasUpperCase ? 1 : 0;
    count += hasNumbers ? 1 : 0;
    count += hasSpecialChars ? 1 : 0;

    return count >= 2;
}


function jVal(id) {
    return $("#" + id).val();
}
function jValSet(id, val) {
    $("#" + id).val(val);
}
function jHtml(id) {
    return $("#" + id).html();
}
function jText(id) {
    return $("#" + id).text();
}
function jHtmlSet(id, html) {
    $("#" + id).html(html);
}
function jTextSet(id, text) {
    $("#" + id).text(text);
}
//是否位数字
//lesszero：是否允许负数
function isNumeric(input, lesszero = false) {
    let numericRegex = /^\d+(\.\d+)?$/; // 匹配正数和零，不包含负号
    if (lesszero) {
        numericRegex = /^-?\d+(\.\d+)?$/; // 匹配整数或小数，允许负号和小数点       
    }
    return numericRegex.test(input);
}

function identifyStringType(input) {
    if (!input || input.trim().length === 0) {
        return "other";
    }

    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    const phoneRegex = /^(\+\d{1,4}\s?)?1\d{10}$/; // 中国手机号码（可能带国际区号）
    const idCardRegex = /^\d{6}(18|19|20)?\d{2}(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])\d{3}(\d|X|x)$/; // 简单的中国大陆18位身份证格式校验
    const pureNumberRegex = /^\d+(\.\d+)?$/; // 数字
    const amountRegex = /^(?:\d{1,3}(?:,\d{3})*|\d+)(?:\.\d{1,2})?$/; // 金额（精确到分）

    if (emailRegex.test(input)) {
        return "email";
    } else if (phoneRegex.test(input)) {
        return "phone";
    } else if (idCardRegex.test(input)) {
        return "idCard";
    } else if (/^[\u4e00-\u9fa5]+$/.test(input)) { // 纯汉字
        return "pureChinese";
    } else if (pureNumberRegex.test(input)) {
        return "pureNumber";
    } else if (amountRegex.test(input)) {
        return "amount";
    } else {
        return "string"; // 其他类型
    }
}