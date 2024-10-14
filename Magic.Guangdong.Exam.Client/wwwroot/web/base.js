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
    if (location.host.indexOf('localhost') > -1) {
        checkEnv(false);
    } else {
        checkEnv(true);
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

    //if (parent.location) {
    //    parent.location.href = "/account/login"
    //}
    //window.location.href = "/account/login";
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