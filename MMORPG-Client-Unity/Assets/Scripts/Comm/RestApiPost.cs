using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Comm {

    public class RestApiPost : RestApi {
        private readonly object _obj;
        public RestApiPost(Uri url, object obj) {
            Url = url;
            _obj = obj;
        }

        public async Task<string> Post() {
            return await ApiRequest(Url,HttpMethod.Post, _obj);
        }
    }

}