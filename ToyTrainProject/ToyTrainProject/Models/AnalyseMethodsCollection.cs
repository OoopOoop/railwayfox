using System.Collections.ObjectModel;

namespace ToyTrainProject.Models
{
   public class AnalyseMethodsCollection: ObservableCollection<AnalyseMethod>
    {
        public AnalyseMethodsCollection()
        {
            Add(new AnalyseMethod { Name = "Analyse Image" });
            Add(new AnalyseMethod { Name = "Describe Image" });
            Add(new AnalyseMethod { Name = "OCR" });
        }
    }
}
