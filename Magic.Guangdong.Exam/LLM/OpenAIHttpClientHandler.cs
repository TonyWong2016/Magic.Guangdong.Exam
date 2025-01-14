namespace Magic.Guangdong.Exam.LLM
{
    public class OpenAIHttpClientHandler : HttpClientHandler
    {
        private readonly string _uri;
        private readonly string _model;
        public OpenAIHttpClientHandler(string uri,string model) 
        {
            _uri = uri.TrimEnd('/');
            _model = model;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            UriBuilder uriBuilder; 
            if (request.RequestUri?.LocalPath == "/v1/chat/completions") 
            {
                string path = SwitchchatPath(_model);
                uriBuilder = new UriBuilder(_uri + path); 
                request.RequestUri = uriBuilder.Uri; 
            } else if (request.RequestUri?.LocalPath == "/v1/embeddings") 
            { 
                uriBuilder = new UriBuilder(_uri + "/v1/embeddings"); 
                request.RequestUri = uriBuilder.Uri; 
            }
            
            return await base.SendAsync(request, cancellationToken);
        }

        private string SwitchchatPath(string modelName)
        {
            switch (modelName)
            {
                //case "deepseek-chat":
                //    return "/v1/chat/completions";
                default:
                    return "/v1/chat/completions";
            }
        }
    }
}
