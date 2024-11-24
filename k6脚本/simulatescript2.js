import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {

    // A number specifying the number of VUs to run concurrently.
    vus: 100,
    // A string specifying the total duration of the test run.
    duration: '10s',
    thresholds: {
        checks: ['rate>0.95'],  // 至少 95% 的请求必须成功
        http_req_duration: ['p(95)<500']  // 95% 的请求响应时间小于 500 毫秒
    }
}

export default function () {
    const url = 'http://192.168.5.20:5001/simulate/VerifyInfo';
    //const url = 'https://oaexam.xiaoxiaotong.org/home/gettemporarytoken';
    let params = {
        headers: {
            'Content-Type': 'application/json'
        }
    };

    let res = http.post(url,null, params);
    // 解析响应体
    let response = JSON.parse(res.body);
    // 定义检查点
    let checks = {
        'status code is 200': (r) => r.status === 200,
        'API code is 0': (r) => response.code === 0
    };

    // 执行检查点
    check(res, checks);
    // 记录非成功的响应
    if (response.code !== 0) {
        console.log(`Error: code=${response.code}, msg=${response.msg}, data=${JSON.stringify(response.data)}`);
    }
}