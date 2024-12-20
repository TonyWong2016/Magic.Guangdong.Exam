import { marked } from '../plugins/marked/lib/marked.esm.min.js'
const chatBox = document.getElementById('chat-box');
const userInput = document.getElementById('user-input');
const sendButton = document.getElementById('send-button');

// 模拟用户提问
let questionNumber = 0;
let isDone = true;
let retryCount = 0;
function getSseResp() {
    let rboxId = 'response-box-' + new Date().getTime();
    const responseBox = document.createElement('div');
    responseBox.className = 'response-box';
    responseBox.id = rboxId;
    chatBox.appendChild(responseBox);
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
                //console.log(choices[i]);
                if (choices[i].FinishReason !== "stop") {
                    //messageElement.innerText += choices[i].Delta.Content;
                    // 逐步更新当前回答框的内容
                    responseBox.innerHTML += choices[i].Delta.Content;

                    // 滚动到最新消息
                    responseBox.scrollTop = responseBox.scrollHeight;
                } else {
                    isDone = true;
                    responseBox.innerHTML += `<br><span style="font-size:small;font-style:italic">--${new Date(json.Created * 1000).toLocaleTimeString()},累计消耗【${json.Usage.TotalTokens}】tokens,输入:${json.Usage.PromptTokens},输出:${json.Usage.CompletionTokens}</span>`;
                    setTimeout(() => {
                        marked(responseBox.innerHTML);
                        
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
        if (retryCount < 3) {
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
