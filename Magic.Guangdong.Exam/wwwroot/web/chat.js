import { marked } from '../plugins/marked/lib/marked.esm.min.js'
const chatBox = document.getElementById('chat-box');
const userPrompt = document.getElementById('txtUserPrompt');
const btnAskAi = document.getElementById('btnAskAi');
const childWindow = document.getElementById('main-container').contentWindow;

// 模拟用户提问
let questionNumber = 0;
let isDone = true;
let retryCount = 0;
let lastId = '';
let chatType = true;
let initiator = '';
let lastResponseContent = '';
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
                    if (!choices[i].Delta || !choices[i].Delta.Content)
                        continue;
                    // 逐步更新当前回答框的内容
                    responseBox.innerHTML += choices[i].Delta.Content;

                    // 滚动到最新消息
                    responseBox.scrollTop = responseBox.scrollHeight;
                } else {
                    isDone = true;
                    localStorage.removeItem('lastRboxId');
                    responseBox.innerHTML += `<br>--end--<br><span style="font-size:small;font-style:italic">--${new Date(json.Created * 1000).toLocaleTimeString()},累计消耗【${json.Usage.TotalTokens}】tokens,输入:${json.Usage.PromptTokens},输出:${json.Usage.CompletionTokens}</span>`;
                    setTimeout(() => {
                        responseBox.innerHTML = marked(responseBox.innerHTML);
                        lastResponseContent = btoa(encodeURIComponent(responseBox.innerHTML));
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
        if (retryCount < 10) {
            retryCount++;
            setTimeout(() => {
                getSseResp();
            },3000)
            
        }
        eventSource.close();
    };
}

// 提交用户输入
btnAskAi.addEventListener('click', async function () {
    if (!isDone) {
        layer.tips('请等待AI回复完成', this, {
            tips: [2, 'var(--main-bg-color)'],
            fixed: true
        });
        
        return;
    }
    
    isDone = false;
    const message = userPrompt.value.trim();
    if (!message) {
        layer.msg('请输入问题', { icon: 2 });
        return;
    }

    if (message.length > 200) {
        layer.msg('请输入200字以内的问题', { icon: 4 });
        return;
    }
    const userMessageElement = document.createElement('p');
    userMessageElement.className = 'request-box';
    userMessageElement.textContent = `${localStorage.getItem('userName')}: ${message}`;
    chatBox.appendChild(userMessageElement);
    initiator = localStorage.getItem('initiator') ?? location.href;
    var formData = objectToFormData(
        {
            'prompt': message,
            'model': document.getElementById('aimodel').value,
            'admin': localStorage.getItem('userName'),
            'chatType': chatType,
            'initiator': initiator,
            '__RequestVerificationToken': requestToken
        }
    );
    var ret = await request('POST', '/ai/hunyuan/SimpleChat', formData, { 'Content-Type': 'multipart/form-data' });
    if (ret.code == 0) {
        let index = layer.load(2);
        setTimeout(() => {
            layer.close(index);
            getSseResp();
        }, 2500)
    }
    userPrompt.value = ''; // 清空输入框
});

const messageHandlers = {}; // 用于存储不同 action 的防抖计时器
const applyBtn = $('#btnAdopt');
const applyDisabledBtn = $('#btnAdoptDisabled');

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
            console.log('wrong token.');
            return;
        }
        console.log(event.data);
        console.log('Bingo.');
        setTimeout(() => {

            handleIncomingMessage(action, otherParams);
        }, 300);
        
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
        case 'generateAnalysis':
            $('.request-box').remove();
            $('.response-box').remove();
            openDiv('答案解析', 'chatView', '1000px', '640px');
            userPrompt.value = params.prompt;
            chatType = false;
            btnAskAi.click();
            applyBtn.show();
            $('.request-box').hide();
            applyDisabledBtn.hide();
            break;
        // 可以添加更多 action 的处理逻辑
        default:
            console.log('Unknown action:', action, 'with params:', params);
            break;
    }
}

$('#btnAdopt').click(async () => {
    if (!isDone) {
        layer.tips('请等待AI回复完成', '#btnAdopt', {
            tips: [2, 'var(--main-bg-color)'],
            fixed: true
        });

        return;
    }
    if (!chatType && initiator.indexOf('pasteAnalysis') > -1) {
        await childWindow.adoptResponse(lastResponseContent);
        layer.closeAll();
    }
});



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

