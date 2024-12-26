function setupChildMessageListener() {
    window.addEventListener('message', function (event) {
        // 检查消息来源的安全性，确保它来自同一个源
        if (event.origin !== location.origin) return;

       
        // 解析消息数据
        const { action, accessToken, initiator , ...otherParams } = event.data;

        // 检查是否包含 action 参数
        if (!action) {
            //console.log('Message does not contain an action parameter.');
            return;
        }

        if (accessToken != localStorage.getItem('accessToken')) {
            console.log('wrong token.');
            return;
        }

        let currInitiator = localStorage.getItem('initiator') ?? location.href;
        if (initiator != currInitiator) {
            console.log('It is not my msg.');
            return;
        }

        console.log(event.data);
        console.log('Child Bingo.');
        setTimeout(() => {
            console.log('Child Bingo222.');
            handleChildIncomingMessage(action, otherParams);
        }, 100);

        // 释放锁
        //releaseLock(lockKey);
    });
}

function handleChildIncomingMessage(action, params) {
    switch (action) {
        case 'pasteAnalysis':
            console.log('haha')
            tinymce.get('Analysis').setContent(params.content);

            break;
        
        // 可以添加更多 action 的处理逻辑
        default:
            console.log('Unknown action:', action, 'with params:', params);
            break;
    }
}

// 确保在 DOM 完全加载后执行
document.addEventListener('DOMContentLoaded', () => {
    setupChildMessageListener();
});
