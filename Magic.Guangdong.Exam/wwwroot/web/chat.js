import { marked } from '../plugins/marked/lib/marked.esm.min.js'
const chatBox = document.getElementById('chat-box');
const userPrompt = document.getElementById('txtUserPrompt');
const btnAskAi = document.getElementById('btnAskAi');
const btnClear = document.getElementById('btnClear');
const childWindow = document.getElementById('main-container').contentWindow;


let isDone = true;
let retryCount = 0;
let lastId = '';
let chatType = true;
let initiator = '';
let lastResponseContent = '';
let eventSource;
let btnAskAiDefaultHtml = '发送 <i class="layui-icon layui-icon-release"></i>';

//获取sse响应数据
function getSseResp() {
    eventSource = new EventSource('/airesp?admin=' + localStorage.getItem('userName'))

    // 标记是否接收到了完成信号
    isDone = false;
    eventSource.onmessage = function (event) {

        isDone = false;
        const message = event.data;
        let json = JSON.parse(message);
        //renderResponse(json, responseBox);
        renderResponse(json);
    };

    eventSource.onerror = function (error) {
        // 检查是否是正常的关闭
        if (isDone || eventSource.readyState === EventSource.CLOSED) {
            console.log('Connection closed normally.');
            return;
        }
        console.error('EventSource failed:', error);
        if (retryCount < 30) {
            retryCount++;
            setTimeout(() => {
                getSseResp();
            }, 1000)
        }
        eventSource.close();
    };
}

//渲染回复内容
function renderResponse(json) {
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

    let type = 'hunyuan';
    let choices = {};
    if (json.Choices)
        choices = json.Choices;
    else if (json.choices) {
        choices = json.choices;
        type = 'deepseek';
    }
    else {
        responseBox.innerHTML = "无有效输出";
        if (eventSource && eventSource.readyState !== EventSource.CLOSED)
            eventSource.close();
        btnAskAi.innerHTML = btnAskAiDefaultHtml;
        return;
    }
    
    if (choices.length > 0) {
        retryCount = 0;
        if (type == 'hunyuan') {
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
                    }, 300);
                    elemStatusSwitch(true);
                    eventSource.close();
                }
            }
        } else if (type == 'deepseek') {

            //data: {"id":"f07c2ca9-f1ed-49db-a4e6-dcfe2c297f88","object":"chat.completion.chunk","created":1735544096,"model":"deepseek-chat","system_fingerprint":"fp_f1afce2943","choices":[{"index":0,"delta":{"content":"。"},"logprobs":null,"finish_reason":null}]}
            //{"id":"37be5e8b-9b0d-4650-a1b0-13237d1eedec","object":"chat.completion.chunk","created":1735549888,"model":"deepseek-chat","system_fingerprint":"fp_f1afce2943","choices":[{"index":0,"delta":{"role":"assistant","content":""},"logprobs":null,"finish_reason":null}]}
            //data: {"id":"f07c2ca9-f1ed-49db-a4e6-dcfe2c297f88","object":"chat.completion.chunk","created":1735544096,"model":"deepseek-chat","system_fingerprint":"fp_f1afce2943","choices":[{"index":0,"delta":{"content":""},"logprobs":null,"finish_reason":"stop"}],"usage":{"prompt_tokens":13,"completion_tokens":381,"total_tokens":394,"prompt_cache_hit_tokens":0,"prompt_cache_miss_tokens":13}}
            for (let i = 0; i < choices.length; i++) {
                if (choices[i].finish_reason && choices[i].finish_reason === "stop") {
                    console.log(json);
                    isDone = true;
                    localStorage.removeItem('lastRboxId');
                    responseBox.innerHTML += `<br>--end--<br><span style="font-size:small;font-style:italic">--${new Date(json.created * 1000).toLocaleTimeString()},累计消耗【${json.usage.total_tokens}】tokens,输入:${json.usage.prompt_tokens},输出:${json.usage.completion_tokens}</span>`;
                    setTimeout(() => {
                        responseBox.innerHTML = marked(responseBox.innerHTML);
                        lastResponseContent = btoa(encodeURIComponent(responseBox.innerHTML));
                    }, 300);
                    //btnAskAi.innerHTML = btnAskAiDefaultHtml;
                    elemStatusSwitch(true);
                    eventSource.close();
                } else {

                    //messageElement.innerText += choices[i].Delta.Content;
                    //console.log(choices[i]);
                    if (!choices[i].delta)
                        continue;
                    // 逐步更新当前回答框的内容
                    responseBox.innerHTML += choices[i].delta.content;

                    // 滚动到最新消息
                    responseBox.scrollTop = responseBox.scrollHeight;
                   
                }
            }
        }

        
    }
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

    let index = layer.load(2);
    let messageLog = extractChatMessagesModern();
    initiator = localStorage.getItem('initiator') ?? location.href;
    let formData = objectToFormData(
        {
            'prompt': message,
            'model': document.getElementById('aimodel').value,
            'admin': localStorage.getItem('userName'),
            'chatType': chatType,
            'initiator': initiator,
            'messages': JSON.stringify(messageLog),
            '__RequestVerificationToken': requestToken
        }
    );


    const userMessageElement = document.createElement('p');
    userMessageElement.className = 'request-box';
    userMessageElement.textContent = `${localStorage.getItem('userName')} : ${message}`;
    chatBox.appendChild(userMessageElement);


    var ret = await request('POST', '/ai/magic/SimpleChat', formData, { 'Content-Type': 'multipart/form-data' });
    if (ret.code == 0) {

        setTimeout(() => {
            elemStatusSwitch(false);
            layer.close(index);
            getSseResp();
        }, 2500)
    }
    userPrompt.value = ''; // 清空输入框
});

//清空话题
btnClear.addEventListener('click', async function () {
    await clearChatBox();
})

let layuiform = layui.form;
layuiform.on('select(aimodel)', async function (data) {
    layer.msg('模型已切换', { icon: 0 });
    await clearChatBox()
})

function elemStatusSwitch(flag) {
    if (flag) {
        $('#aimodel').removeAttr('disabled');
        $('#btnClear').show();
        btnAskAi.innerHTML = btnAskAiDefaultHtml;

    } else {
        $('#aimodel').attr('disabled', true);
        $('#btnClear').hide();
        btnAskAi.innerHTML = '思考中 <i class="layui-icon layui-icon-loading layui-anim layui-anim-rotate layui-anim-loop"></i>'
    }
    layuiform.render('select');
}

async function clearChatBox() {
    $('.request-box').remove();
    $('.response-box').remove();
    btnAskAi.innerHTML = btnAskAiDefaultHtml; 
    localStorage.removeItem('lastRboxId');
    isDone = true;
    userPrompt.value = ''; // 清空输入框
    const formData = objectToFormData(
        {
            'admin': localStorage.getItem('userName'),
            '__RequestVerificationToken': requestToken
        }
    );
    await request('POST', '/ai/magic/ClearChat', formData, { 'Content-Type': 'multipart/form-data' });

}

const applyBtn = $('#btnAdopt');
const applyDisabledBtn = $('#btnAdoptDisabled');

// 监听来自子页面的消息
function setupMessageListener() {
    window.addEventListener('message', function (event) {
        // 检查消息来源的安全性，确保它来自同一个源
        if (event.origin !== window.location.origin) return;

        applyBtn.hide();
        applyDisabledBtn.show();

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
            clearChatBox()
            openDiv('答案解析', 'chatView', '1000px', '640px');
            userPrompt.value = params.prompt;
            chatType = false;
            btnAskAi.click();
            applyBtn.show();            
            applyDisabledBtn.hide();
            $('.request-box').hide();
            setTimeout(() => {
                $('.request-box').remove();
            }, 500);
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

//组合聊天记录
function extractChatMessagesModern() {
    const chatBox = document.querySelector('#chat-box');
    if (!chatBox) return [];

    // 使用 Array.from 将 NodeList 转换为数组，并映射其内容
    //const requestBoxes = Array.from(chatBox.querySelectorAll('.request-box'), box => box.textContent.split(' : ')[1].trim());
    //const responseBoxes = Array.from(chatBox.querySelectorAll('.response-box'), box => box.textContent.split('--end--')[0].trim());
    // 获取所有 request-box 和 response-box 元素，并转换为数组
    const requestBoxes = Array.from(chatBox.querySelectorAll('.request-box'));
    const responseBoxes = Array.from(chatBox.querySelectorAll('.response-box'));

    // 创建一个用于存储消息对的数组
    let messagePairs = [];

    // 遍历 request-boxes 并尝试找到对应的 response-box
    for (let i = 0; i < requestBoxes.length; i++) {
        // 假设 request 和 response 是一一对应的，并且顺序相同
        if (i < responseBoxes.length) {
            // 提取文本内容并去除多余的空白字符
            const requestContent = requestBoxes[i].textContent.split(' : ')[1].trim();
            const responseContent = responseBoxes[i].textContent.split('--end--')[0].trim();

            // 将每一对消息封装成对象，并添加到数组中
            messagePairs.push({ role: 'user', content: requestContent });
            messagePairs.push({ role: 'assistant', content: responseContent });
        }
    }

    // 返回包含所有消息对的数组(最多40条)
    return messagePairs.length > 40 ? messagePairs.slice(-40) : messagePairs;
}


// 确保在 DOM 完全加载后执行
document.addEventListener('DOMContentLoaded', () => {
    setupMessageListener();
});

