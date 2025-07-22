using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Web;
using System.Text;

namespace EAP.Dashboard.Utils
{
    public class WebApiHelper
    {

        /// <summary>
        /// Post访问方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parseData"></param>
        /// <param name="result"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool PostWithProxy(string url, string parseData, out string result, out string errMsg)
        {
            errMsg = string.Empty;
            result = string.Empty;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //请求添加特殊key
                    //string url = GetLastUrl(baseUrl, dictParam);
                    StringContent content = new StringContent(parseData, Encoding.UTF8);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage tmpResult = client.PostAsync(url, content).Result;
                    tmpResult.EnsureSuccessStatusCode();
                    result = tmpResult.Content.ReadAsStringAsync().Result;
                    return true;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }
    }
}