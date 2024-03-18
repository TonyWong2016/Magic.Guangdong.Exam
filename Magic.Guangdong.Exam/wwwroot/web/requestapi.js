// 假设你在全局作用域已经引入了axios库
// 例如通过script标签引入或者AMD/CommonJS等方式加载了axios
window.axios = axios; // 确保axios全局可用

// 创建一个 Axios 实例，方便全局配置和拦截器
var axiosInstance = axios.create({
    baseURL: window.location.origin,
    timeout: 10000, // 设置超时时间
});

// 添加请求拦截器（例如：全局添加 Token）
axiosInstance.interceptors.request.use(function (config) {
    var token = localStorage.getItem('accessToken'); // 假设从localStorage获取token
    if (token) {
        config.headers.Authorization = 'Bearer ' + token;
    }
    return config;
}, function (error) {
    return Promise.reject(error);
});

// 封装通用请求方法
function request(method, url, data = null, headers = {}) {
    var config = {
        method,
        url,
        headers: Object.assign({}, axiosInstance.defaults.headers.common, headers),
    };

    // 根据 Content-Type 和 method 类型处理 data
    if (['post', 'put', 'patch'].indexOf(method.toLowerCase()) !== -1) {
        if (headers['Content-Type'] === 'application/x-www-form-urlencoded') {
            config.data = new URLSearchParams(data);
        } else if (headers['Content-Type'] === 'multipart/form-data') {
            // 对于 multipart/form-data，通常需要借助 FormData 对象或其他库（如 formidable）
            // 以下仅为示例，实际场景需根据具体文件上传需求处理
            if (typeof data === 'object' && !(data instanceof FormData)) {
                var formData = new FormData();
                for (var key in data) {
                    formData.append(key, data[key]);
                }
                config.data = formData;
            } else {
                config.data = data;
            }
        } else {
            config.data = JSON.stringify(data);
            config.headers['Content-Type'] = 'application/json';
        }
    } else if (method.toLowerCase() === 'get') {
        config.params = data;
    }

    return axiosInstance(config)
        .then(function (response) {
            return response.data;
        })
        .catch(function (error) {
            throw error.response ? error.response.data : error;
        });
}


// 使用示例 - 表单数据
//async function sendFormData() {
//    const formData = new FormData();
//    formData.append('username', 'test');
//    formData.append('avatar', fileInputElement.files[0]); // 假设fileInputElement是HTML input[type=file]元素

//    try {
//        const result = await request('POST', '/api/user', formData, { 'Content-Type': 'multipart/form-data' });
//        console.log(result);
//    } catch (error) {
//        console.error('Error sending form data:', error);
//    }
//}
// 使用示例 - 请求数据
//async function fetchData() {
//    try {
//        const result = await request('GET', '/api/user', { id: 123 }, { 'X-Custom-Header': 'value' });
//        console.log(result);
//    } catch (error) {
//        console.error('Error fetching data:', error);
//    }
//}