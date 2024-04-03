const videoElement = document.getElementsByTagName('video')[0];
const cameraSelector = document.getElementById('cameraSelector');
let cameraStream = null;
document.addEventListener('DOMContentLoaded', async function () {
    getCameras()
})



async function getCameras() {
    try {
         // 获取设备列表
        let devices = await navigator.mediaDevices.enumerateDevices();
        // 筛选出视频输入设备
        let videoDevices = devices.filter(device => device.kind === 'videoinput');

        if (/iPhone|iPod|Macintosh/i.test(navigator.userAgent)) {
            const _stream = await navigator.mediaDevices.getUserMedia({ video: true });
            const _tracks = _stream.getTracks();
            _tracks.forEach(track => {
                const option = document.createElement('option');
                option.value = track.id;
                option.text = track.label || `摄像头 ${cameraList.children.length + 1}`;
                cameraSelector.appendChild(option);
            });
            // 默认选择第一个摄像头
            if (_tracks.length > 0) {
                cameraStream = await getCameraStream();
                videoElement.srcObject = cameraStream;
                videoElement.play()
            }
            // 关闭获取的流
            //_stream.getTracks().forEach(track => track.stop());
        } else {
            // 添加选项到选择器
            videoDevices.forEach(device => {
                const option = document.createElement('option');
                option.value = device.deviceId;
                option.text = device.label || `摄像头 ${cameraSelector.length + 1}`;
                cameraSelector.appendChild(option);
            });
            // 默认选择第一个摄像头
            if (videoDevices.length > 0) {
                cameraStream = await getCameraStream(videoDevices[0].deviceId);
                videoElement.srcObject = cameraStream;
                videoElement.play()
            }
        }
        

        // 监听选择器变化事件，切换摄像头
        cameraSelector.addEventListener('change', async function () {
            const selectedDeviceId = cameraSelector.value;
            await startCamera(selectedDeviceId);
        });        

    } catch (error) {        
        console.error('获取设备列表失败:', error);
        layer.msg('获取设备列表失败:', { icon: 2 }, () => {
            disabledBtn("您的设备不支持人脸登录，请使用证件号登录。")
        });
    }
}

// 获取摄像头流
async function getCameraStream(deviceId) {
    try {
        const stream = await navigator.mediaDevices.getUserMedia({
            video: { deviceId: { exact: deviceId } }
        });
        if (deviceId && typeof (deviceId) != undefined)
            localStorage.setItem("lastCameraDeviceId", deviceId)

        return stream;
    } catch (error) {
        localStorage.removeItem("lastCameraDeviceId")
        console.error('获取摄像头流时出现错误:', error);
        layer.msg('获取摄像头流时出现错误:', { icon: 2 }, () => {
            disabledBtn("您的设备不支持人脸登录，请使用证件号登录。")
        });
    }
}

async function startCamera(selectedDeviceId) {

    stopCamera();
    if (!selectedDeviceId || typeof (selectedDeviceId) == undefined ) {
        cameraStream = await getCameraStream();
    } else
        cameraStream = await getCameraStream(selectedDeviceId);
    videoElement.srcObject = cameraStream;
    videoElement.play()
}

// 停止获取摄像头流
function stopCamera() {
    if (cameraStream) {
        const tracks = cameraStream.getTracks();
        tracks.forEach(track => track.stop());
        videoElement.srcObject = null;
    }
}
function camvas(config) {
    var self = this
    self.convas = document.getElementById(config.canvasId)
    self.ctx = self.convas.getContext('2d');
    self.config = config
    self.isStop = false;

    //video节点ID
    self.video = document.getElementById(self.config.videoId)

    //video 显示尺寸
    self.video.setAttribute('width', this.config.video.width)
    self.video.setAttribute('height', this.config.video.height)

        
    //拍照，base64/image/png
    this.drawImage = function (callback) {
        if (!self.isStop) {
            self.ctx.drawImage(self.video, 0, 0, self.config.video.width, self.config.video.height);
            var base64URL = self.convas.toDataURL('image/' + self.config.imgType, self.config.quality);
            callback && callback(base64URL);
        }
    }
}