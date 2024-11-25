import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {

    // A number specifying the number of VUs to run concurrently.
    vus: 1,
    // A string specifying the total duration of the test run.
    duration: '5s',
    thresholds: {
        checks: ['rate>0.95'],  // 至少 95% 的请求必须成功
        http_req_duration: ['p(95)<500']  // 95% 的请求响应时间小于 500 毫秒
    },// 设置并发用户数
    // stages: [
    //     { duration: '20s', target: 10 }, // 起始阶段，10个线程运行60秒
    //     { duration: '20s', target: 100 }, // 在接下来的30秒内将线程数增加到100
    //     { duration: '20s', target: 300 }, // 再接下来的30秒内将线程数增加到300
    //    // { duration: '20s', target: 500 }, // 再接下来的30秒内将线程数增加到500
    //    // { duration: '20s', target: 800 }, // 再接下来的30秒内将线程数增加到800
    //    // { duration: '30s', target: 1000 }, // 再接下来的30秒内将线程数增加到1000
    //     //{ duration: '120s', target: 1000 } // 保持1000个线程运行120秒
    //   ],
}

export default function () {
    //const url = 'http://192.168.0.46:5001/simulate/SaveDraft';
    const url = 'https://exam.xiaoxiaotong.org/simulate/SaveDraft';
    let params = {
        headers: {
            'Content-Type': 'application/json'
        }
    };

    let payLoad ={
        answer:"[]",
    }

    let res = http.post(url,payLoad, params);
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