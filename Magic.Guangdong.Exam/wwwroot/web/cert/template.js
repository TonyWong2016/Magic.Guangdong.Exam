var colorpicker = layui.colorpicker;
//渲染
colorpicker.render({
    elem: '#colorpicker'  //绑定元素
    , color: '#1e9fff' // 设置默认色
    , change: function (color) {
        console.log(color)
    }
    , done: function (color) {
        //console.log(color)
        $('#fontColor').val(color)
        //譬如你可以在回调中把得到的 color 赋值给表单
    }
});



function loadImg() {
    getImageSize(document.getElementById('template')).then(data => {
        console.log(data)
        //$('#fabricDiv').append(`<canvas width="${data.width + 100}" height="${data.height + 100}" id="c" style="border: 1px solid #ccc;"></canvas>`)
        canvas.setWidth(data.width + 100)
        canvas.setHeight(data.height + 100)
        certParam.certTempUrl = $("#template").attr("src")

    }).then(data2 => {
        fabric.Image.fromURL(
            path, // 参数1：图片路径
            img => { // 参数2：图片加载完成后的回调函数

                // 设置图片在画布中的位置
                img.top = padding
                img.left = padding
                img.setControlsVisibility({
                    mt: false,
                    mb: false,
                    ml: false,
                    mr: false,
                    bl: false,
                    br: false,
                    tl: false,
                    tr: false,
                    mtr: false,
                });//隐藏多个拖拽点，禁止拖拽，因为要固定1080p
                img.lockMovementX = true; //不许左右移动
                img.lockMovementY = true; //不许上下移动
                // 将图片添加到画布中
                canvas.add(img)

            });
    }).then(() => {
        if (localStorage.getItem('canvasJson')) {
            canvas.loadFromJSON(localStorage.getItem('canvasJson'))
            
        }
    })

}

// 鼠标在画布上的点击事件
function canvasOnMouseDown(opt) {
    // 判断：右键，且在元素上右键
    // opt.button: 1-左键；2-中键；3-右键
    // 在画布上点击：opt.target 为 null
    if (opt.button === 3 && opt.target) {
        // 获取当前元素
        activeEl = opt.target

        menu.domReady = function () {
            //console.log(123)
        }

        // 显示菜单，设置右键菜单位置
        // 获取菜单组件的宽高
        const menuWidth = menu.offsetWidth
        const menuHeight = menu.offsetHeight

        // 当前鼠标位置
        pointX = opt.pointer.x 
        pointY = opt.pointer.y

        // 计算菜单出现的位置
        // 如果鼠标靠近画布右侧，菜单就出现在鼠标指针左侧
        if (canvas.width - pointX <= menuWidth) {
            pointX -= menuWidth
        }
        // 如果鼠标靠近画布底部，菜单就出现在鼠标指针上方
        if (canvas.height - pointY <= menuHeight) {
            pointY -= menuHeight
        }

        // 将菜单展示出来
        menu.style = `
                      visibility: visible;
                      left: ${pointX + padding}px;
                      top: ${pointY + padding*3}px;
                      z-index: 100;
                    `
    } else {
        hiddenMenu()
    }
}

function handleObjectMoving(event) {
    const object = event.target;
    const canvas = object.canvas;

    // 获取画布边界
    const canvasWidth = canvas.getWidth();
    const canvasHeight = canvas.getHeight();

    // 获取对象边界及其左上角坐标
    const objectBoundingRect = object.getBoundingRect();
    const objectLeft = object.left;
    const objectTop = object.top;

    // 检查对象右边界和下边界是否超出画布
    const maxX = canvasWidth - objectBoundingRect.width;
    const maxY = canvasHeight - objectBoundingRect.height;

    // 如果超出边界，则将对象位置限定在最大允许值内
    object.set({
        left: Math.min(Math.max(objectLeft, 0), maxX),
        top: Math.min(Math.max(objectTop, 0), maxY),
    });

    // 阻止事件冒泡，防止默认行为（即移动到画布外）
    event.e.stopPropagation();
}

// 隐藏菜单
function hiddenMenu() {
    menu.style.display = 'none';
    activeEl = null
}

// 删除元素
function delEl() {
    if (activeEl._element && activeEl._element.tagName === 'IMG') {
        warnMsg('画布不可以删除')
        return;
    }
    //console.log(activeEl)
    //if (activeEl.text && valueArr.includes(activeEl.text)) {
    //    valueArr = removeElementFromArrayByValue(valueArr, activeEl.text);
    //}
    canvas.remove(activeEl)
    hiddenMenu()
    if (number > 1)
        number--;
}

//获取图片尺寸
function getImageSize(imgElement) {
    return new Promise((resolve, reject) => {
        if (imgElement.complete) {
            resolve({ width: imgElement.naturalWidth, height: imgElement.naturalHeight });
        } else {
            imgElement.addEventListener('load', () => {
                resolve({ width: imgElement.naturalWidth, height: imgElement.naturalHeight });
            });
            imgElement.addEventListener('error', reject);
        }
    });
}





function preview() {
    // 实现预览功能，如全屏显示或在新窗口打开
}



function getRandomHexColor() {
    const r = Math.floor(Math.random() * 256).toString(16).padStart(2, '0');
    const g = Math.floor(Math.random() * 256).toString(16).padStart(2, '0');
    const b = Math.floor(Math.random() * 256).toString(16).padStart(2, '0');
    return `#${r}${g}${b}`;
}