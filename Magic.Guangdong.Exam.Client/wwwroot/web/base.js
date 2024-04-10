let fileBaseUrl='https://localhost:7188'
function parseJwtPayload(jwt) {
    let base64Url = jwt.split('.')[1];
    let base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    return JSON.parse(window.atob(base64));
}
function checkLoginStatus() {
    if (!getCookie('accountId') || isCookieExpired('accountId')) {
        clearLoginInfo()
    }
    
    console.log('login ok');
}
function setLoginInfo(jwt) {
    //let base64Url = jwt.split('.')[1];
    //let base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    //let item = JSON.parse(window.atob(base64));
    //let userId = item['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid'];
    //let nameidentifier = item['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
    //localStorage.setItem('userId', userId);
    //localStorage.setItem('accessToken', window.btoa(nameidentifier + '|' + item.exp));
    //let expDays = (item.exp - (Math.round(new Date() / 1000))) / 86400;
    //setCookie('userId', userId, expDays)
    //setCookie('examToken', jwt, expDays);
   
}


function clearLoginInfo() {
    deleteCookie('accountId');
    deleteCookie('accessToken');
    //localStorage.removeItem('accessToken');

    if (parent.location) {
        parent.location.href = "/account/login"
    }
    window.location.href = "/account/login";
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

function listenChange(elementId, onChangeCallback) {
    const element = document.getElementById(elementId);

    if (!element) {
        console.error(`Element with ID "${elementId}" not found.`);
        return;
    }

    // 按类型区分处理
    switch (element.tagName.toLowerCase()) {
        case 'input':
            const inputType = element.type.toLowerCase();

            if (['text', 'password', 'number', 'email', 'url'].includes(inputType)) {
                // 文本类输入框
                element.addEventListener('input', onChangeCallback);
            } else if (inputType === 'checkbox' || inputType === 'radio') {
                // 单选框和复选框
                element.addEventListener('change', onChangeCallback);
            }
            break;
        case 'select':
            // 下拉选择框
            element.addEventListener('change', onChangeCallback);
            break;
        case 'textarea':
            // 多行文本框
            element.addEventListener('input', onChangeCallback);
            break;
        default:
            console.warn(`Unsupported element type "${element.tagName}" for change listener.`);
    }
}

function removeListen(elementId, onChangeCallback) {
    const element = document.getElementById(elementId);

    if (!element) {
        console.error(`Element with ID "${elementId}" not found.`);
        return;
    }

    // 按类型区分处理
    switch (element.tagName.toLowerCase()) {
        case 'input':
            const inputType = element.type.toLowerCase();

            if (['text', 'password', 'number', 'email', 'url'].includes(inputType)) {
                // 文本类输入框
                element.removeEventListener('input', onChangeCallback);
            } else if (inputType === 'checkbox' || inputType === 'radio') {
                // 单选框和复选框
                element.removeEventListener('change', onChangeCallback);
            }
            break;
        case 'select':
            // 下拉选择框
            element.removeEventListener('change', onChangeCallback);
            break;
        case 'textarea':
            // 多行文本框
            element.removeEventListener('input', onChangeCallback);
            break;
        default:
            console.warn(`Unsupported element type "${element.tagName}" for removing change listener.`);
    }
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


function getUrlQueryParams(parameterName = null, url = window.location.href) {
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
        return queryParams[decodeURIComponent(parameterName)] || '';
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

function buildFormData(...args) {
    const formData = new FormData();

    for (const arg of args) {
        const type = typeof arg;
        formData.append(type, JSON.stringify(arg));
    }

    return formData;
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
    if (requestToken)
        fd.append('__RequestVerificationToken', requestToken);

    return fd;
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


function SFID(card) {

    var vcity = {
        11: "北京", 12: "天津", 13: "河北", 14: "山西", 15: "内蒙古",
        21: "辽宁", 22: "吉林", 23: "黑龙江", 31: "上海", 32: "江苏",
        33: "浙江", 34: "安徽", 35: "福建", 36: "江西", 37: "山东", 41: "河南",
        42: "湖北", 43: "湖南", 44: "广东", 45: "广西", 46: "海南", 50: "重庆",
        51: "四川", 52: "贵州", 53: "云南", 54: "西藏", 61: "陕西", 62: "甘肃",
        63: "青海", 64: "宁夏", 65: "新疆", 71: "台湾", 81: "香港", 82: "澳门", 91: "国外"
    };
    //检查号码是否符合规范，包括长度，类型
    var isCardNo = function (card) {
        //身份证号码为15位或者18位，15位时全为数字，18位前17位为数字，最后一位是校验位，可能为数字或字符X
        var reg = /(^\d{15}$)|(^\d{17}(\d|X)$)/;
        if (reg.test(card) === false) {
            return false;
        }
        return true;
    };
    //取身份证前两位,校验省份
    var checkProvince = function (card) {
        var province = card.substr(0, 2);
        if (vcity[province] == undefined) {
            return false;
        }
        return true;
    };
    //校验性别
    var checkSex = function (card, sex) {

        var sexStr = 0;
        if (card.length == 15) {
            sexStr = parseInt(card.substr(14, 1), 10) % 2 ? "1" : "2";
        } else {
            sexStr = parseInt(card.substr(16, 1), 10) % 2 ? "1" : "2";
        }

        if (sexStr == sex) {
            return true;
        }
        else {
            return false;
        }
    }
    //检查生日是否正确
    var checkBirthday = function (card) {
        var len = card.length;
        //身份证15位时，次序为省（3位）市（3位）年（2位）月（2位）日（2位）校验位（3位），皆为数字
        if (len == '15') {
            var re_fifteen = /^(\d{6})(\d{2})(\d{2})(\d{2})(\d{3})$/;
            var arr_data = card.match(re_fifteen);
            var year = arr_data[2];
            var month = arr_data[3];
            var day = arr_data[4];
            var birthday = new Date('19' + year + '/' + month + '/' + day);
            return verifyBirthday('19' + year, month, day, birthday);
        }
        //身份证18位时，次序为省（3位）市（3位）年（4位）月（2位）日（2位）校验位（4位），校验位末尾可能为X
        if (len == '18') {
            var re_eighteen = /^(\d{6})(\d{4})(\d{2})(\d{2})(\d{3})([0-9]|X)$/;
            var arr_data = card.match(re_eighteen);
            var year = arr_data[2];
            var month = arr_data[3];
            var day = arr_data[4];
            var birthday = new Date(year + '/' + month + '/' + day);
            return verifyBirthday(year, month, day, birthday);
        }
        return false;
    };
    //校验日期
    var verifyBirthday = function (year, month, day, birthday) {
        var now = new Date();
        var now_year = now.getFullYear();
        //年月日是否合理
        if (birthday.getFullYear() == year && (birthday.getMonth() + 1) == month && birthday.getDate() == day) {
            //判断年份的范围（3岁到100岁之间)
            var time = now_year - year;
            if (time >= 3 && time <= 100) {
                return true;
            }
            return false;
        }
        return false;
    };
    //校验位的检测
    var checkParity = function (card) {
        //15位转18位
        card = changeFivteenToEighteen(card);
        var len = card.length;
        if (len == '18') {
            var arrInt = new Array(7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2);
            var arrCh = new Array('1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2');
            var cardTemp = 0, i, valnum;
            for (i = 0; i < 17; i++) {
                cardTemp += card.substr(i, 1) * arrInt[i];
            }
            valnum = arrCh[cardTemp % 11];
            if (valnum == card.substr(17, 1)) {
                return true;
            }
            return false;
        }
        return false;
    };
    //15位转18位身份证号
    var changeFivteenToEighteen = function (card) {
        if (card.length == '15') {
            var arrInt = new Array(7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2);
            var arrCh = new Array('1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2');
            var cardTemp = 0, i;
            card = card.substr(0, 6) + '19' + card.substr(6, card.length - 6);
            for (i = 0; i < 17; i++) {
                cardTemp += card.substr(i, 1) * arrInt[i];
            }
            card += arrCh[cardTemp % 11];
            return card;
        }
        return card;
    };

    //是否为空
    if (card === '') {
        return '请输入身份证号，身份证号不能为空';
        //return false;
    }
    //校验长度，类型
    if (isCardNo(card) === false) {
        return '您输入的身份证号码不正确，请重新输入';
        //return false;
    }
    //检查省份
    if (checkProvince(card) === false) {
        return '您输入的身份证号码不正确,请重新输入';
        //return false;
    }
    //校验生日
    if (checkBirthday(card) === false) {
        return '您输入的身份证号码生日不正确,请重新输入';
        //return false;
    }
    //校验性别
    //if (checkSex(card, sex) == false) {
    //    return '您的身份证与性别不一致,请重新输入';
    //}
    //检验位的检测
    if (checkParity(card) === false) {
        //  mui.alert('您的身份证校验位不正确,请重新输入');
        return '您的身份证校验位不正确,请重新输入';
        //return false;
    }
    return "OK";
    //return true;
};

const TT = {
    tips: function (msg, url) {
        Toastify({
            text: msg,
            destination: url,
            newWindow: true,
            duration: 1000 * 30,
            gravity: 'top', // `top` or `bottom`
            position: 'right', // `left`, `center` or `right`
            style:{
                background: '#16baaa'
            },
            close: true
        }).showToast();
    },
    tips2: function (msg, url) {
        Toastify({
            text: msg,
            destination: url,
            newWindow: true,
            duration: 1000 * 30,
            gravity: 'top', // `top` or `bottom`
            position: 'right', // `left`, `center` or `right`
            style:{
                background: '#16baaa'
            },
            close: true
        }).showToast();
    },
    success: function (msg, cb) {
        
        Toastify({
            text: msg,
            duration: 1000 * 3,
            gravity: 'top', // `top` or `bottom`
            position: 'right', // `left`, `center` or `right`
            style:{
                background: '#16b777'
            },
            close: true,
        }).showToast();
    },
    info: function (msg, time = 5000) {
        Toastify({
            text: msg,
            duration: time,
            gravity: 'top', // `top` or `bottom`
            position: 'right', // `left`, `center` or `right`
            style:{
                background: '#1e9fff'
            },        
            })
            .showToast();
    },
    notify: function (msg) {
        Toastify({
            text: msg,
            duration: 5000,
            gravity: 'top', // `top` or `bottom`
            position: 'right', // `left`, `center` or `right`
            style:{
                background: '#31bdec'
            },  
            close: true
        }).showToast();
    },
    error: function (msg) {
        Toastify({
            text: msg,
            duration: 5000,
            gravity: 'top', // `top` or `bottom`
            position: 'right', // `left`, `center` or `right`
            style:{
                background: '#ff5722'
            }, 
            close: true
        }).showToast();
    },
    warning: function (msg) {
        Toastify({
            text: msg,
            duration: 5000,
            gravity: 'top', // `top` or `bottom`
            position: 'right', // `left`, `center` or `right`
            style:{
                background: '#ffb800'
            },
            close: true
        }).showToast();
    }
};

function getFormData(form) {
    const formData = {};

    for (const element of form.elements) {
        // 忽略禁用的元素、按钮（除提交按钮）和其他非交互元素
        if (element.disabled || element.type === 'button' && element.type !== 'submit') {
            continue;
        }

        const name = element.name;
        const value = getInputElementValue(element);

        if (name) {
            if (Array.isArray(formData[name])) {
                formData[name].push(value);
            } else if (formData.hasOwnProperty(name)) {
                formData[name] = [formData[name], value];
            } else {
                formData[name] = value;
            }
        }
    }

    return formData;
}

function getInputElementValue(element) {
    switch (element.type) {
        case 'checkbox':
        case 'radio':
            return element.checked;
        case 'select-multiple':
            return Array.from(element.options).filter(option => option.selected).map(option => option.value);
        default:
            return element.value;
    }
}

function interceptFormSubmit(form, onSubmitCallback) {
    form.addEventListener('submit', (event) => {
        event.preventDefault(); // 阻止默认提交动作

        const formData = getFormData(form);

        // 在此处根据业务需求处理formData，例如动态修改表单内容
        // 示例：若某个字段值为空，则添加错误提示
        //if (!formData.someField) {
        //    const someFieldError = document.getElementById('some-field-error');
        //    someFieldError.style.display = 'block'; // 显示错误提示
        //    someFieldError.textContent = 'This field is required.';
        //}

        // 调用业务处理回调函数，传递formData作为参数
        onSubmitCallback(formData);

        // 根据业务处理结果决定是否提交表单，例如：
        if (shouldSubmit) {
            form.submit(); // 手动提交表单
        }
    });
}

//倒计时
//参数elementId，用于指定要显示倒计时的HTML元素ID。
//获取该元素，并在开始前检查其是否存在，若不存在则抛出错误。
//定义一个新的内部函数updateElementContent，它接受剩余时间作为参数，根据元素类型（input或非input）更新其内容。对于input元素，设置其value属性；对于其他元素，更新其textContent属性。
//在countdown函数中，调用updateElementContent来实时更新元素内容。
//在开始倒计时前，先将初始倒计时数值（完整秒数）显示在目标元素上。
//现在，您可以传入一个元素ID，当该元素是button、a（链接）、span等非input元素时，倒计时进度会显示在其textContent中；如果是input元素，则显示在value属性中。
function accurateCountdown(seconds, elementId, callback) {
    const startTimestamp = Date.now();
    const targetElement = document.getElementById(elementId);

    if (!targetElement) {
        throw new Error(`Element with ID "${elementId}" not found.`);
    }

    function updateElementContent(remainingTime) {
        const isInput = targetElement.tagName.toLowerCase() === 'input';

        if (isInput) {
            targetElement.value = remainingTime;
        } else {
            targetElement.textContent = remainingTime;
        }
    }

    function countdown() {
        const remainingSeconds = Math.max(0, seconds - Math.floor((Date.now() - startTimestamp) / 1000));
        if (remainingSeconds === 0) {
            if (typeof callback === 'function') {
                callback();
            }
            return;
        }

        updateElementContent(`${remainingSeconds} 秒`);

        requestAnimationFrame(countdown);
    }

    updateElementContent(`${seconds} 秒`);
    countdown();
}