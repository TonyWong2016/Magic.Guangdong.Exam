function getAiEditorConfig(elemId, obj) {
    let defaultSize = obj.defaultSize ?? 160;
    return {
        element: '#' + elemId,
        placeholder: obj.placeholder ?? "请输入内容",
        content: obj.content ?? "",
        
        image: {
            allowBase64: false,
            defaultSize: defaultSize,
            uploadUrl: obj.uploadUrl ?? "/file/upload?userId=" + (obj.userId ?? getCookie('userId')),
            uploadFormName: "image", //上传时的文件表单名称
            uploadHeaders: {
                "Authorization": localStorage.getItem('accessToken'),
            },
            uploader: (file, uploadUrl, headers, formName) => {
                const formData = new FormData();
                formData.append(formName, file);
                formData.append('__RequestVerificationToken', requestToken);
                return new Promise((resolve, reject) => {
                    fetch(uploadUrl, {
                        method: "post",
                        headers: { 'Accept': 'application/json', ...headers },
                        body: formData,
                    }).then((resp) => resp.json())
                        .then(json => {
                            resolve(json);
                        }).catch((error) => {
                            reject(error);
                        })
                });
            },
            uploaderEvent: {
                onUploadBefore: (file, uploadUrl, headers) => {
                    //监听图片上传之前，此方法可以不用回任何内容，但若返回 false，则终止上传
                    if (file.size && file.size > 1024 * 1024 * 50) {
                        //Toaster("error", "文件大小不可高于50M", "错误");
                        errorMsg("文件大小不可高于50M");
                        return;
                    }
                },
                onSuccess: (file, response) => {
                    //监听图片上传成功
                    //注意：
                    // 1、如果此方法返回 false，则图片不会被插入到编辑器
                    // 2、可以在这里返回一个新的 json 给编辑器
                    console.log(response)
                    if (response && response.code == 0) {
                        return {
                            errorCode: response.code,
                            errorMsg: response.msg,
                            data: {
                                src: obj.showThumb ? obj.resourceHost + response.data + '?width=' + defaultSize : obj.resourceHost + response.data,
                                alt: "图片 alt",
                                extendata: response.extendata
                            }

                        }
                    }
                },
                onFailed: (file, response) => {
                    errorMsg('上传失败' + response.msg);
                },
                onError: (file, error) => {
                    //监听图片上传错误，比如网络超时等
                    errorMsg('上传失败' + error);
                },
            },
            bubbleMenuItems: ["AlignLeft", "AlignCenter", "AlignRight", "delete"]
        },
        video: {
            
            uploadUrl: obj.uploadUrl ?? "/file/UploadChunk",
            uploadFormName: "video", //上传时的文件表单名称
            uploadHeaders: {
                "Authorization": localStorage.getItem('accessToken'),
            },
            uploader: async (file, uploadUrl, headers, formName) => {
                //可自定义视频上传逻辑
                await uploadFileInChunks(file, uploadUrl, headers, formName);
            },
            uploaderEvent: {
                onUploadBefore: (file, uploadUrl, headers) => {
                    //监听视频上传之前，此方法可以不用回任何内容，但若返回 false，则终止上传
                },
                onSuccess: (file, response) => {
                    //监听视频上传成功
                    //注意：
                    // 1、如果此方法返回 false，则视频不会被插入到编辑器
                    // 2、可以在这里返回一个新的 json 给编辑器
                },
                onFailed: (file, response) => {
                    //监听视频上传失败，或者返回的 json 信息不正确
                },
                onError: (file, error) => {
                    //监听视频上传错误，比如网络超时等
                },
            }
        },
        ai: {
            //自定义后端
            models: {
                custom: {
                    url: "/simplechat",
                    headers: () => {

                    },
                    wrapPayload: (message) => {
                        // 将用户输入转换为后端需要的JSON格式
                        return JSON.stringify({ prompt: message });
                    },
                    parseMessage: (message) => {
                        return {
                            role: "assistant",
                            content: message,
                            // index: number,
                            // //0 代表首个文本结果；1 代表中间文本结果；2 代表最后一个文本结果。
                            // status: 0|1|2,
                        }
                    },
                }
            }
        }
    }
}
function uploadHandleForAiEditor(file, uploadUrl, headers, formName) {
    //可自定义图片上传逻辑
    const formData = new FormData();
    formData.append(formName, file);
    formData.append('__RequestVerificationToken', requestToken);
    return new Promise((resolve, reject) => {
        fetch(uploadUrl, {
            method: "post",
            headers: { 'Accept': 'application/json', ...headers },
            body: formData,
        }).then((resp) => resp.json())
            .then(json => {
                resolve(json);
            }).catch((error) => {
                reject(error);
            })
    });
}

async function uploadFileInChunks(file, uploadUrl, headers, formName) {
    // 计算文件的MD5值
    const fileMd5 = await calculateFileMd5(file);

    // 文件名
    const fileName = file.name;

    // 文件后缀
    const fileSuffix = fileName.split('.').pop() || '';

    // 分片大小，例如1MB
    const chunkSize = 1 * 1024 * 1024; // 1 MB

    // 总分片数
    const totalChunks = Math.ceil(file.size / chunkSize);

    for (let index = 0; index < totalChunks; index++) {
        // 当前分片的开始位置
        const start = index * chunkSize;
        // 当前分片的结束位置
        const end = Math.min(start + chunkSize, file.size);
        // 创建一个Blob对象表示当前分片
        const chunk = file.slice(start, end);

        // 创建FormData对象并添加分片信息
        const formData = new FormData();
        formData.append(`file_name`, fileName);
        formData.append(`connId`, 0);
        formData.append(`adminId`, atob(getCookie("userId")));
        formData.append(`file_index`, index + 1); // 序号从1开始
        formData.append(`file_total`, totalChunks);
        formData.append(`file_md5`, fileMd5);
        formData.append(`file_chunksize`, chunkSize);
        formData.append(`file_suffix`, fileSuffix);
        formData.append(`file_data`, chunk, `${fileName}_part${index + 1}`);
        formData.append('__RequestVerificationToken', requestToken);
        // 设置请求头
        const fetchOptions = {
            method: 'POST',
            headers: {
                ...headers,
                // 注意：不要设置'Content-Type'，让浏览器自动设置为'multipart/form-data'
            },
            body: formData
        };

        try {
            // 发送分片
            const response = await fetch(uploadUrl, fetchOptions);
            const result = await response.json();

            if (!response.ok) {
                throw new Error(`Upload failed for chunk ${index + 1}: ${result.message || response.statusText}`);
            }

            console.log(`Chunk ${index + 1} of ${totalChunks} uploaded successfully.`);
        } catch (error) {
            console.error('Error uploading chunk:', error);
            break; // 或者你可以选择继续尝试上传剩余的分片
        }
    }
}

// 计算文件的MD5值
function calculateFileMd5(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        const spark = new SparkMD5.ArrayBuffer(); // 假设你已经包含了spark-md5库

        reader.onerror = () => reject(reader.error);
        reader.onload = (e) => {
            spark.append(e.target.result);
            resolve(spark.end());
        };
        reader.readAsArrayBuffer(file);
    });
}