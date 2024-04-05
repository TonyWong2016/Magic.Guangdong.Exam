let formData = new FormData();
formData.append('__RequestVerificationToken', requestToken);
async function test() {
    let ret = await request('POST', 'Index?handler=Test', formData, { 'Content-Type': 'multipart/form-data' })
    console.log(ret)
}
test()

let associationId = '';
let examId = '';
let groupCode = '';