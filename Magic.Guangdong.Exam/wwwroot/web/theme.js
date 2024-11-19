function mainTheme(bg) {
    if (bg && bg.startsWith('#')) {
        localStorage.setItem('main-bg-color', bg);
        const root = document.documentElement;
        let bgColor = getComputedStyle(root).getPropertyValue('--main-bg-color').trim();
        root.style.setProperty('--main-bg-color', bg);
        if (bg == '#0F3888') {
            root.style.setProperty('--main-word-bg-color', '#6699ff');
            root.style.setProperty('--main-word-color', '#fff');
            root.style.setProperty('--bg-success-color', '#3366cc');
            root.style.setProperty('--bg-normal-color', '#6699ff');
            $('#SystemTitle').css('color', '#fff');
            $('.layui-bg-green').css('background-color', 'var(--bg-success-color) !important')
        } else {
            root.style.setProperty('--main-word-bg-color', '#16baaa');
            root.style.setProperty('--status-bg-success-color', '#16baaa');
            root.style.setProperty('--main-word-color', '#fff');
            root.style.setProperty('--bg-success-color', '#16baaa');
            root.style.setProperty('--bg-normal-color', '#1e9fff');
            $('#SystemTitle').css('color', 'yellow');
        }
    }
}
mainTheme(localStorage.getItem('main-bg-color'));