using System.Threading.Tasks;

namespace ToyTrainProject.Models
{
   public interface IAnalyseService
    {
        Task<string> callService(System.Drawing.Bitmap bitMap);
    }
}
