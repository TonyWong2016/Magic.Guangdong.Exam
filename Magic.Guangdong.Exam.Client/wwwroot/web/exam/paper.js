function startCountdown1(seconds, countdownElementId, onCountdownEnd) {
    let remainingTime = seconds;

    function updateCountdown() {
        // 将剩余秒数转换为小时、分钟和秒
        const hours = Math.floor(remainingTime / 3600);
        const minutes = Math.floor((remainingTime % 3600) / 60);
        const remainingSeconds = remainingTime % 60;

        // 格式化时间显示，补足两位数
        function formatTime(num) {
            return Math.floor(num).toString().padStart(2, '0');
        }

        // 更新倒计时标签内容
        const countdownElement = document.getElementById(countdownElementId);
        countdownElement.textContent = `${formatTime(hours)}:${formatTime(minutes)}:${formatTime(remainingSeconds)}`;

        // 如果剩余时间大于0，继续调用requestAnimationFrame更新倒计时
        if (remainingTime > 0) {
            remainingTime -= 1/60;
            requestAnimationFrame(updateCountdown);
        } else {
            // 计时结束，执行用户提供的回调函数
            if (typeof onCountdownEnd === 'function') {
                onCountdownEnd();
            }
        }
    }

    // 启动倒计时
    updateCountdown();
}

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
