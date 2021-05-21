using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace APIComm {

    public class RestApiGet : RestApi {
        public RestApiGet(Uri url) {
            Url = url;
        }
        public async Task<string> Get() {
            return await ApiRequest(Url,HttpMethod.Get, null);
        }
    }

}