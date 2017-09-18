using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using static ToyTrainProject.Shared.AppConfiguration;
using static ToyTrainProject.Shared.AppConfiguration.ComputerVisionAPI.DescribeImage;

namespace ToyTrainProject.Models
{
    internal class ComputerVision_DescribeImage : AnalyticsWrapper, IAnalyseService
    {
        public ComputerVision_DescribeImage() : base(SubscriptionKey, UriBase, "application/octet-stream")
        {
        }

        public async Task<string> callService(System.Drawing.Bitmap bitMap, int maxCandidates = 1)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            queryString["maxCandidates"] = maxCandidates.ToString();

            var body = new System.Drawing.ImageConverter().ConvertTo(bitMap, typeof(byte[])) as byte[];

            HttpResponseMessage response = await callEndpoint("describe", queryString, body);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        public async Task<string> callService(System.Drawing.Bitmap bitMap) => await callService(bitMap, maxCandidates);
    }
}