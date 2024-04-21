
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