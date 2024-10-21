checkLoginStatus();

function xxtLogout() {
    deleteCookie('accountId');
    deleteCookie('accountName');
    deleteCookie('idToken');
    //location.href = '/account/Logout?idToken=' + idToken + '&redirectUrl=' + location.protocol + '//' + location.host + '/account/me';
    location.href = `/account/Logout?idToken=${idToken}&redirectUrl=${location.protocol}//${location.host}/account/me`;

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
function headerTopMenuEvent() {
    $('#xxtMenu').on('mouseover', 'li', function () {
        $(this).css('background', 'url(/img/menuSelect.png)'); // 修改背景色

    }).on('mouseout', 'li', function () {
        $(this).css('background', ''); // 恢复默认背景色
    });
}