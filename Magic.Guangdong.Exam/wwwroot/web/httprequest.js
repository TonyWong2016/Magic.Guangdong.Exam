/// <reference path="axios/axios.js" />

//获取服务端数据
//params = {
//"router":字符串，如："home/getlist","user/getlist"
//"query":字符串，如："id=1"
//"queryObj":对象,如：{id:"123","name":title}
//"arae":字符串，如："system","exam",""
//}


function GetServerData(router, query, area='') {
   
    //先统一处理，再匹配是否正确
    let requestUrl = router;
    if (requestUrl.startWith("/")) {
        requestUrl = requestUrl.substr(1);
    }
    if (area!=='') {
        area = area.replace("/", "");
        requestUrl = `${area}/${router}`;
    }
    const pathPattern = /^\/[a-z0-9]+(\/[a-z0-9]+)*$/i;
    if (!pathPattern.test(requestUrl)) {
        return "非法请求";
    }
    let queryStr = query;
    if (isStringRepresentingAnObject(query)) {
        queryStr = objectToQueryString(query)
    }
    if (!isValidQueryParameter(queryStr)) {
        return "非法的请求参数";
    }
    if (queryStr === '')
        return axios.get(`${requestUrl}`);
    return axios.get(`${requestUrl}?${queryStr}`);
}

//get方式请求服务端数据
function GetServerData(router, query, headers, area='') {
    const pathPattern = /^\/[a-z0-9]+(\/[a-z0-9]+)*$/i;
    //先统一处理，再匹配是否正确
    let requestUrl = router;
    if (requestUrl.startWith("/")) {
        requestUrl = requestUrl.substr(1);
    }
    if (area) {
        area = area.replace("/", "");
        requestUrl = `${area}/${router}`;
    }
    if (!pathPattern.test(requestUrl)) {
        return "非法请求";
    }
    let queryStr = query;
    if (isStringRepresentingAnObject(query)) {
        queryStr = objectToQueryString(query)
    }
    if (!isValidQueryParameter(queryStr)) {
        return "非法的请求参数";
    }

    if (queryStr === '')
        return axios.get(`${requestUrl}`);
    return axios.get(`${requestUrl}?${queryStr}`, { headers: headers });
}



//对象转为url参数
function objectToQueryString(obj) {
    return Object.keys(obj)
        .map(key => `${encodeURIComponent(key)}=${encodeURIComponent(obj[key])}`)
        .join('&');
}

//url参数转换为对象
function queryParamsToObject(queryString) {
    const params = {};

    // 将查询参数字符串分割成键值对数组
    queryString.split('&').forEach((param) => {
        // 分割键和值
        const [key, value] = param.split('=');

        // URL解码键和值
        const decodedKey = decodeURIComponent(key);
        let decodedValue = decodeURIComponent(value);

        // 如果URL中有多个相同的键，则将值放入数组
        if (params[decodedKey]) {
            if (!Array.isArray(params[decodedKey])) {
                params[decodedKey] = [params[decodedKey]];
            }
            params[decodedKey].push(decodedValue);
        } else {
            params[decodedKey] = decodedValue;
        }
    });

    return params;
}

//是否可以转换成数组；
function isStringRepresentingAnObject(str) {
    try {
        JSON.parse(str);
        // 如果能成功转化为JSON对象，则可能是对象或数组
        return typeof JSON.parse(str) === 'object' && JSON.parse(str) !== null;
    } catch (e) {
        // 如果转化失败，说明不是一个有效的JSON字符串对象
        return false;
    }
}

//验证query参数是否合理
function isValidQueryParameter(str) {
    // 空字符串被认为是有效的（即没有键值对）
    if (str === '') {
        return true;
    }
    // 考虑到URL编码，这里的正则表达式会允许任何URL编码字符
    // 在key和value中，但不会验证URL编码的合法性，仅仅是结构上符合key=value形式
    const regex = /^%(?:[0-9A-Fa-f]{2})+=[^&=]*$/; // 包含URL编码的键
    const nonEncodedRegex = /^[^&=]*=[^&=]*$/; // 非URL编码的键

    return (regex.test(decodeURIComponent(str)) || nonEncodedRegex.test(str)) || str === '';
}


// 添加请求拦截器以全局设置Token
axios.interceptors.request.use(config => {
    const token = localStorage.getItem('accessToken'); // 假设从localStorage获取token
    if (token) {
        config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
});
