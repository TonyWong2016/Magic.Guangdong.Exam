checkLoginStatus();

function xxtLogout(loginRedicretUrl) {
    
    deleteCookie('accountId');
    deleteCookie('accountName');
    deleteCookie('idToken');
    //location.href = '/account/Logout?idToken=' + idToken + '&redirectUrl=' + location.protocol + '//' + location.host + '/account/me';
    location.href = `/account/Logout?idToken=${idToken}&redirectUrl=${location.protocol}//${location.host}/account/me`;

    if (redirecturl)
        setCooie('loginRedicretUrl', btoa(loginRedicretUrl), 0.25);
}
//if (accountId)
//    $('#examHistoryHref').attr('href', '/exam/history?accountId=' + accountId);

function setExamPage() {
    if (localStorage.getItem('examPageConfig')) {
        const pageCfg = JSON.parse(localStorage.getItem('examPageConfig'));
        if (pageCfg.pageHost + pageCfg.pageHeader)
            $('#headerBgImg').attr('src', pageCfg.pageHost + pageCfg.pageHeader);
        if (pageCfg.pageHost + pageCfg.pageFooter)
            $('#footerBgImg').attr('src', pageCfg.pageHost + pageCfg.pageFooter);
        if (pageCfg.pageHost + pageCfg.pageBody) {
            // setTimeout(()=>{
            //     $('body').attr('background',pageCfg.pageHost+pageCfg.pageBody)
            //        .attr('style','background-position-y:'+$('#headerBgImg').innerHeight()+'px');
            // },50)

        }
    }
}
//setExamPage()
//changeTheme('jade');
//修改主题
function changeTheme(theme) {
    const themeLink = document.getElementById('theme-style');
    let themes = [
        'amber', 'blue', 'cyan', 'fuchsia',
        'green', 'grey', 'indigo', 'jade',
        'lime', 'orange', 'pink', 'pumpkin',
        'purple', 'red', 'sand', 'slate', 'violet',
        'yellow', 'zinc'
    ];

    if (theme === 'indigo' || themes.indexOf(theme) == -1) {
        return;
    }

    else {
        themeLink.href = '/lib/pico2/css/pico.' + theme + '.min.css';
    }
    // 添加更多主题选项...
}


//添加菜单点击时间
function headerTopMenuEvent(topMenuSelectedImg,topMenuBackgroundImg) {
    if (!topMenuSelectedImg)
        topMenuSelectedImg = '/img/menuSelect.png';
    if (!topMenuBackgroundImg)
        topMenuBackgroundImg = '/img/menuBg.jpg';
    $('#xxtMenu').on('mouseover', 'li', function () {
        $(this).css('background', 'url(' + topMenuSelectedImg +')'); // 修改背景色

    }).on('mouseout', 'li', function () {
        $(this).css('background', ''); // 恢复默认背景色
    });

    
    $('.xxtNavTop').css('background', 'url(' + topMenuBackgroundImg + ')')
}

// 封装的通知显示函数
//function cycleNotices(noticeList, elementId, interval = 3000) {
//    // 获取元素
//    const element = document.getElementById(elementId);

//    // 当前索引
//    let currentIndex = 0;

//    // 更新链接和文本的函数
//    function updateNotice() {
//        const currentNotice = noticeList[currentIndex];
//        element.href = currentNotice.url;
//        element.textContent = currentNotice.title;

//        // 更新索引，循环回第一个通知
//        currentIndex = (currentIndex + 1) % noticeList.length;
//    }

//    // 初始化显示第一个通知
//    updateNotice();

//    // 设置定时器，每interval毫秒切换一次通知
//    setInterval(updateNotice, interval);
//}
function cycleNotices(noticeList, elementId, interval = 3000) {
    // 获取元素
    const element = document.getElementById(elementId);

    // 当前索引
    let currentIndex = 0;

    // 更新链接和文本的函数
    function updateNotice() {
        // 先隐藏当前通知
        element.classList.remove('visible');

        // 等待一段时间让通知完全隐藏
        setTimeout(() => {
            const currentNotice = noticeList[currentIndex];
            element.href = currentNotice.url;
            element.textContent = currentNotice.title;
            if (element.textContent.length > 25) {
                element.textContent = element.textContent.substring(0, 24) + '...'
            }
            element.title = currentNotice.title;
            if (currentNotice.time) {
                element.title = currentNotice.title + '，发布时间：' + currentNotice.time;
            }
            // 显示新的通知
            element.classList.add('visible');

            // 更新索引，循环回第一个通知
            currentIndex = (currentIndex + 1) % noticeList.length;
        }, 500); // 这个值应该与 CSS 中的 transition 时间一致
    }

    // 初始化显示第一个通知
    updateNotice();

    // 设置定时器，每interval毫秒切换一次通知
    setInterval(updateNotice, interval);
}

