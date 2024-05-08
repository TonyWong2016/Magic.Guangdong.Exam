
function startCountdown(seconds, countdownElementId, onCountdownEnd) {
    let remainingTime = seconds;
    let lastTimestamp = null;

    function updateCountdown(timestamp) {
        if (lastTimestamp !== null) {
            const deltaTime = (timestamp - lastTimestamp) / 1000;
            remainingTime -= deltaTime;
        }

        const hours = Math.floor(remainingTime / 3600);
        const minutes = Math.floor((remainingTime % 3600) / 60);
        const remainingSeconds = Math.floor(remainingTime % 60);

        function formatTime(num) {
            return num.toString().padStart(2, '0');
        }

        const countdownElement = document.getElementById(countdownElementId);
        countdownElement.textContent = `${formatTime(hours)}:${formatTime(minutes)}:${formatTime(remainingSeconds)}`;

        if (remainingTime > 60 && remainingTime < 300) {
            countdownElement.style.color = 'pico-color-amber-200';
        } else if (remainingTime > 0 && remainingTime < 60) {
            countdownElement.style.color = 'pico-color-red-500';
        }  

        if (remainingTime > 0) {
            lastTimestamp = timestamp;
            requestAnimationFrame(updateCountdown);
        } else {
            if (typeof onCountdownEnd === 'function') {
                onCountdownEnd();
            }
        }
    }

    requestAnimationFrame(updateCountdown);

    // 返回一个函数，用于获取剩余时间
    return {
        getRemainingTime: function () {
            return Math.floor(remainingTime);
        }
    };
}

// 使用示例：传入剩余秒数、倒计时标签ID和结束时的回调函数
//function countdownEnded() {
//    console.log('Countdown has ended!');
//}

//// 调用startCountdown函数，获取返回的getRemainingTime函数
//const countdown = startCountdown(3900, 'countdownTimer', countdownEnded);

//// 在函数外部获取剩余时间
//console.log('Initial remaining time:', countdown.getRemainingTime());

//// ...其他操作...

//// 在任意时刻获取剩余时间
//console.log('Current remaining time:', countdown.getRemainingTime());


function executeAtInterval(intervalMs, callback) {
    // 使用 setInterval 创建定时器，每隔 intervalMs 毫秒执行一次 callback 函数
    const intervalId = setInterval(() => {
        callback();
    }, intervalMs);

    // 返回一个函数，用于清除定时器，防止内存泄漏
    return function clearIntervalWrapper() {
        clearInterval(intervalId);
    };
}

// 使用示例
//function myCallback() {
//    console.log('Callback executed at:', new Date());
//}
//const stopExecution = executeAtInterval(2000, myCallback); // 每隔2秒执行一次 myCallback
// 在需要停止执行时，调用返回的清除函数
// stopExecution();

function processTitle(title, customIndex) {
    // 使用正则表达式去除HTML标签
    const strippedTitle = title.replace(/<[^>]+>/g, '');

    // 使用正则表达式去除以数字加顿号（如 "1、"）开头的部分
    const cleanedTitle = strippedTitle.replace(/^\d+\u3001/, '');

    // 拼接自定义序号
    const formattedTitle = `${customIndex}、${cleanedTitle}`;

    return formattedTitle;
}

function monitorAswer(elemId, callback) {
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
        }, 200);
    });
}

// 在调用页面引入了Monaco Editor库
//function createMonacoEditor(containerId, initialLanguage, defaultCode) {
function createMonacoEditor(edtiorParmas) {
    const container = document.getElementById(edtiorParmas.containerId);

    if (!container) {
        throw new Error(`Container with id "${edtiorParmas.containerId}" not found.`);
    }

    return {
        async init() {
            // 加载Monaco编辑器核心模块和语言包
            await new Promise((resolve) => {
                require.config({ paths: { 'vs': '/plugins/monaco-editor/min/vs' } });
                require(['vs/editor/editor.main'], resolve);
            });

            // 初始化Monaco编辑器配置和默认值
            const editorOptions = {
                language: edtiorParmas.initialLanguage || 'javascript',// 默认为JavaScript，若传入语言则使用传入的语言
                theme: edtiorParmas.theme||'vs-dark',
                automaticLayout: true,
            };

            // 创建Monaco编辑器实例
            this.currentEditor = monaco.editor.create(container, editorOptions);
            // 如果提供了默认代码，则写入编辑器
            if (edtiorParmas.defaultCode) {
                this.currentEditor.setValue(edtiorParmas.defaultCode);
            }

            
        },

        /**
         * 获取编辑器内的代码内容
         * returns {string} 编辑器内的代码文本
         */
        getCodeContent() {
            if (!this.currentEditor) {
                throw new Error('Editor has not been initialized yet.');
            }
            return this.currentEditor.getValue();
        },
        /**
         * 设置编辑器的语言
         * param {string} language - 编辑器的新语言
         */
        setLanguage(language) {
            const currentEditor = monaco.editor.getModels()[0];
            //currentEditor.setModelLanguage(language);
            monaco.editor.setModelLanguage(currentEditor, language);
        },
        /**
         * 设置编辑器内的代码内容
         * param {string} codeContent - 新的代码内容
         */
        setCodeContent(codeContent) {
            this.currentEditor.setValue(codeContent);
        },

        monitorCode(id) {
            this.currentEditor.onDidChangeModelContent((e) => {

                sessionStorage.setItem("currCode_"+id, this.currentEditor.getValue());
            });
        }
    };
}


//// 使用示例
//const editorManager = createMonacoEditor('your-container-id');

//// 在需要时获取编辑器内的代码内容
//const codeContent = editorManager.getCodeContent();
//console.log(codeContent);

//净化一下选项
function confirmAnswerArr() {
    if (!paperDetail || !paperDetail.Questions) {
        return '';//试卷没加载上
    }
    let currAnswer = sessionStorage.getItem("answerArr");
    if (!currAnswer || !isStringConvertibleToObject(currAnswer)) {
        return '';//还没答题
    }
    let currAnswerArr = JSON.parse(currAnswer);
    let tmpCurrAnswerArr = [];
    //if (currAnswerArr.length > paperDetail.questions.length) {

    //}

    for (let i = 0; i < currAnswerArr.length; i++) {
        if (tmpCurrAnswerArr.filter(u => u.id == u.id == currAnswerArr[i].questionId).length == 1) {
            continue;
        }
        if (paperDetail.Questions.filter(u => u.Id == currAnswerArr[i].questionId).length == 1) {
            tmpCurrAnswerArr.push(currAnswerArr[i]);
        }
    }

    for (let i = 0; i < sessionStorage.length; i++) {
        const key = sessionStorage.key(i);

        if (key.startsWith("currCode_")) {
            const value = sessionStorage.getItem(key);
            const questionId = key.replace("currCode_", "");
            if (tmpCurrAnswerArr.filter(u => u.questionId == parseFloat(questionId)).length > 0)
                continue;
            tmpCurrAnswerArr.push({ questionId: parseFloat(questionId), userAnswer: [value] })
            
        }
    }

    return JSON.stringify(tmpCurrAnswerArr);

}

//判定是否可转换为数组
function isStringConvertibleToObject(str) {
    try {
        JSON.parse(str);
        return true;
    } catch (e) {
        return false;
    }
}