using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using static ToyTrainProject.Shared.AppConfiguration;
using static ToyTrainProject.Shared.AppConfiguration.ComputerVisionAPI.AnalyzeImage;

namespace ToyTrainProject.Models
{
    internal class ComputerVision_AnalyseImage : AnalyticsWrapper
    {
        public ComputerVision_AnalyseImage() : base(SubscriptionKey, UriBase, "application/octet-stream")
        {
        }

        public async Task<string> callService(System.Drawing.Bitmap bitMap, string visualFeatures = "", string details = "", string language = "")
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            if (visualFeatures.Length > 0)
            {
                queryString["visualFeatures"] = visualFeatures;
            }

            if (details.Length > 0)
            {
                queryString["details"] = details;
            }

            if (language.Length > 0)
            {
                queryString["language"] = language;
            }

            var body = new System.Drawing.ImageConverter().ConvertTo(bitMap, typeof(byte[])) as byte[];

            HttpResponseMessage response = await callEndpoint("analyze", queryString, body);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        public async Task<string> callService(System.Drawing.Bitmap bitMap) => await callService(bitMap, visualFeatures, details, language);
    }
}