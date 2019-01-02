using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using W;

namespace W.YoudaoSDK
{
    public class TextTranslateService
    {
        const string AppId = "your Id", AppSecret = "your secret";
        public TranslateResult Translate(string query)
        {
            var client = new RestSharp.RestClient("http://openapi.youdao.com/api/");
            string salt = WUtil.NewSalt();
            string sign = WUtil.NewSign(AppId, query, salt, AppSecret).Replace("-", "");

            string lang = IdentifyLanguage(query);
            string tLang = GetTargetLanguage(lang);
            string url = string.Format("http://openapi.youdao.com/api?appKey={0}&q={1}&from={2}&to={3}&sign={4}&salt={5}",
                AppId, System.Web.HttpUtility.UrlDecode(query, Encoding.GetEncoding("UTF-8")), lang, tLang, sign, salt);
            var req = new RestSharp.RestRequest(url);

            var response = client.ExecuteAsGet<TranslateResult>(req, "GET");
            if (!response.IsSuccessful)
                throw response.ErrorException;
            return response.Data;
        }


        internal static string IdentifyLanguage(string text)
        {
            if (text.Any(c => (c >= 65 && c < 91) || (c >= 97 && c < 123)))
            {
                return "EN";
            }

            return "zh-CHS";
        }

        internal static string GetTargetLanguage(string sourceLang)
        {
            if (sourceLang.Equals("zh-CHS", StringComparison.OrdinalIgnoreCase))
            {
                return "EN";
            }

            return "zh-CHS";
        }
    }

    public class TranslateResult
    {
        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("translation")]
        public string Translation { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("basic")]
        public string Basic { get; set; }

        [JsonProperty("web")]
        public string Web { get; set; }

        [JsonProperty("speakUrl")]
        public string SpeakUrl { get; set; }

        [JsonProperty("tSpeakUrl")]
        public string TSpeakUrl { get; set; }

        public override string ToString()
        {
            if (Translation == null)
                return "ErrorCode: " + ErrorCode ?? string.Empty;


            var translations = JsonConvert.DeserializeObject<string[]>(Translation);

            if (string.IsNullOrWhiteSpace(Basic))
                return string.Join(";", translations);

            var details = JsonConvert.DeserializeObject<TranslateResultDetail>(Basic);

            var resultBuilder = new StringBuilder();
            resultBuilder.AppendLine(string.Join(";", translations));
            resultBuilder.AppendLine("============================");
            resultBuilder.AppendLine(details.ToString());
            return resultBuilder.ToString();
        }
    }

    public class TranslateResultDetail
    {
        [JsonProperty("exam_type")]
        public string[] 考试词库 { get; set; }

        [JsonProperty("us-phonetic")]
        public string 美式音标 { get; set; }

        [JsonProperty("uk-phonetic")]
        public string 英式音标 { get; set; }

        [JsonProperty("explains")]
        public string[] 详细释义 { get; set; }

        public override string ToString()
        {
            var resultBuilder = new StringBuilder();
            resultBuilder.AppendLine(nameof(考试词库) + ": " + string.Join(";", 考试词库 ?? new string[0]));
            resultBuilder.AppendLine(nameof(美式音标) + ": " + 美式音标);
            resultBuilder.AppendLine(nameof(英式音标) + ": " + 英式音标);
            resultBuilder.AppendLine(nameof(详细释义) + ": " + string.Join(Environment.NewLine, 详细释义 ?? new string[0]));
            return resultBuilder.ToString();
        }
    }
}
