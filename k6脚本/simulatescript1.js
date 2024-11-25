import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {

    // A number specifying the number of VUs to run concurrently.
    vus: 1,
    // A string specifying the total duration of the test run.
    duration: '3s',
    thresholds: {
        checks: ['rate>0.95'],  // 至少 95% 的请求必须成功
        http_req_duration: ['p(95)<500']  // 95% 的请求响应时间小于 500 毫秒
    }
}

export default function () {
    //const url = 'http://192.168.0.46:5001/simulate/GetExamInfo?examId=49D90000-4C24-00FF-84A0-08DD0926B694';
    const url = 'https://exam.xiaoxiaotong.org/simulate/GetExamInfo?examId=0C140000-569B-0050-51DC-08DD093F8ED7';
    let params = {
        headers: {
            'Content-Type': 'application/json'
        }
    };

    let res = http.get(url, params);
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