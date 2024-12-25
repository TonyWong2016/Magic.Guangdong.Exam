import { marked } from '../plugins/marked/lib/marked.esm.min.js'
const chatBox = document.getElementById('chat-box');
const userInput = document.getElementById('user-input');
const sendButton = document.getElementById('send-button');

// 模拟用户提问
let questionNumber = 0;
let isDone = true;
let retryCount = 0;
let lastId = '';
function getSseResp() {
    let rboxId = 'response-box-' + new Date().getTime();
    
    lastId = localStorage.getItem('lastRboxId');
    let responseBox;
    if (isDone || !lastId) {
        responseBox = document.createElement('div');
        responseBox.className = 'response-box';
        responseBox.id = rboxId;
        chatBox.appendChild(responseBox);
    } else {
        responseBox = document.getElementById(lastId);
    }
    if (!localStorage.getItem('lastRboxId'))
        localStorage.setItem('lastRboxId', rboxId);
    const eventSource = new EventSource('/airesp?admin=' + localStorage.getItem('userName'));
    // 标记是否接收到了完成信号
    isDone = false;
    eventSource.onmessage = function (event) {
        // messageElement.textContent = `AI: `;
        
        isDone = false;
        const message = event.data;
        let json = JSON.parse(message);
        //console.log(json);
        let choices = json.Choices;
        if (choices.length > 0) {
            retryCount = 0;
            for (let i = 0; i < choices.length; i++) {
                if (choices[i].FinishReason !== "stop") {
                    //messageElement.innerText += choices[i].Delta.Content;
                    // 逐步更新当前回答框的内容
                    responseBox.innerHTML += choices[i].Delta.Content;

                    // 滚动到最新消息
                    responseBox.scrollTop = responseBox.scrollHeight;
                } else {
                    isDone = true;
                    localStorage.removeItem('lastRboxId');
                    responseBox.innerHTML += `<br><span style="font-size:small;font-style:italic">--${new Date(json.Created * 1000).toLocaleTimeString()},累计消耗【${json.Usage.TotalTokens}】tokens,输入:${json.Usage.PromptTokens},输出:${json.Usage.CompletionTokens}</span>`;
                    setTimeout(() => {
                        responseBox.innerHTML = marked(responseBox.innerHTML);                        
                    },300);
                    eventSource.close();
                }
            }

        }
    };

    eventSource.onerror = function (error) {
        // 检查是否是正常的关闭
        if (isDone || eventSource.readyState === EventSource.CLOSED) {
            console.log('Connection closed normally.');
            return;
        }
        console.error('EventSource failed:', error);
        if (retryCount < 5) {
            retryCount++;
            setTimeout(() => {
                getSseResp();
            },2000)
            
        }
        eventSource.close();
    };
}

// 提交用户输入
sendButton.addEventListener('click', async function () {
    if (!isDone) {
        layer.tips('请等待AI回复完成', this, {
            tips: [2, 'var(--main-bg-color)'],
            fixed: true
        });
        
        return;
    }
    isDone = false;
    const message = userInput.value.trim();
    if (message) {
        const userMessageElement = document.createElement('p');
        userMessageElement.className = 'request-box';
        userMessageElement.textContent = `${localStorage.getItem('userName')}: ${message}`;
        chatBox.appendChild(userMessageElement);
        var formData = objectToFormData({ 'prompt': message, 'model': document.getElementById('aimodel').value, 'admin': localStorage.getItem('userName'), '__RequestVerificationToken': requestToken });
        var ret = await request('POST', '/ai/hunyuan/SimpleChat', formData, { 'Content-Type': 'multipart/form-data' });
        if (ret.code == 0) {
            let index = layer.load(2);
            setTimeout(() => {
                layer.close(index);
                getSseResp();
            }, 2500)
        }
        userInput.value = ''; // 清空输入框
    }
});

const messageHandlers = {}; // 用于存储不同 action 的防抖计时器
const applyBtn = $('#apply-button');
const applyDisabledBtn = $('#apply-disabled-button');

// 监听来自子页面的消息
function setupMessageListener() {
    window.addEventListener('message', function (event) {
        // 检查消息来源的安全性，确保它来自同一个源
        if (event.origin !== window.location.origin) return;

        applyBtn.hide();
        applyDisabledBtn.show();
        // 尝试获取锁
        //const lockKey = 'messageHandlerLock';
        //if (!tryAcquireLock(lockKey)) {
        //    console.log('Another page is handling the message.');
        //    return;
        //}
       
        // 解析消息数据
        const { action, accessToken, ...otherParams } = event.data;
        
        // 检查是否包含 action 参数
        if (!action) {
            //console.log('Message does not contain an action parameter.');
            return;
        }
       
        if (accessToken != localStorage.getItem('accessToken')) {
            console.log('It is not my msg.');
            return;
        }
        console.log(event.data);
        console.log('Bingo.');
        setTimeout(() => {

            handleIncomingMessage(action, otherParams);
        }, 100);
        
        // 释放锁
        //releaseLock(lockKey);
    });
}


function handleIncomingMessage(action, params) {
    switch (action) {
        case 'showApplyBtn':
            applyBtn.show();
            applyDisabledBtn.hide();
            break;
        case 'hideElement':
            document.getElementById('targetElement').style.display = 'none';
            console.log('Received message with action:', action, 'and params:', params);
            break;
        // 可以添加更多 action 的处理逻辑
        default:
            console.log('Unknown action:', action, 'with params:', params);
            break;
    }
}


function tryAcquireLock(key) {
    // 尝试设置锁，如果已经存在则返回 false
    return localStorage.getItem(key) === null && localStorage.setItem(key, 'locked');
}

function releaseLock(key) {
    // 释放锁
    localStorage.removeItem(key);
}

// 确保在 DOM 完全加载后执行
document.addEventListener('DOMContentLoaded', () => {
    setupMessageListener();
});

