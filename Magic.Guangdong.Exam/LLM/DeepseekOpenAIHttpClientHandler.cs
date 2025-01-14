namespace Magic.Guangdong.Exam.LLM
{
    public class DeepseekOpenAIHttpClientHandler : HttpClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            switch (request.RequestUri?.LocalPath)
            {
                case "/v1/chat/completions":
                    var uriBuilder = new UriBuilder(request.RequestUri)
                    {
                        Scheme = "https",
                        Host = "api.deepseek.com",
                        Path = "chat/completions",//其他与OpenAI对话请求接口兼容的模型平台，一般只需要修改host即可，不需要修改path
                    };
                    request.RequestUri = uriBuilder.Uri;
                    break;
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            return response;
        }
    }
}
