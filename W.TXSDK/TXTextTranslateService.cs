using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using W;
using W.Common;

namespace W.TXSDK
{
    /// <summary>
    /// 垃圾，接口贼难接入！！！！
    /// </summary>
    public class TXTextTranslateService
    {
        const string AppId = "yourId", AppSecret = "yourSecret";
        public TXTranslateResult Translate(string query)
        {
            var client = new RestSharp.RestClient("https://api.ai.qq.com/");
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            keyValues.Add("app_id", AppId);
            keyValues.Add("nonce_str", Utils.NewNonce());
            keyValues.Add("time_stamp", Utils.GetCurrentTimeStamp().ToString());
            keyValues.Add("sign", string.Empty);
            keyValues.Add("type", "0");
            keyValues.Add("text", query);

            string newsign = Utils.NewTXSign(keyValues, AppSecret);
            keyValues["sign"] = newsign;

            string url = string.Format("https://api.ai.qq.com/fcgi-bin/nlp/nlp_texttrans");
            var req = new RestSharp.RestRequest(url);
            foreach (var item in keyValues)
            {
                req.AddParameter(item.Key, item.Value, RestSharp.ParameterType.GetOrPost);
            }
            var response = client.ExecuteAsPost<TXTranslateResult>(req, "POST");
            if (!response.IsSuccessful)
                throw response.ErrorException;
            return response.Data;
        }

    }

    public class TXTranslateResult
    {
        [JsonProperty("data")]
        public TranslateResultDetail Data { get; set; }

        [JsonProperty("ret")]
        public int ErrorCode { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }

        public override string ToString()
        {
            string preText = "Tencent AI: ";
            if (ErrorCode != 0)
                return preText + Environment.NewLine + "ErrorCode: " + ErrorCode ?? string.Empty + Message ?? string.Empty;

            return preText + Environment.NewLine + Data.ToString();
        }
    }

    public class TranslateResultDetail
    {
        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("org_text")]
        public string OriginalText { get; set; }

        [JsonProperty("trans_text")]
        public string Translation { get; set; }

        public override string ToString()
        {
            return Translation;
        }
    }
}
