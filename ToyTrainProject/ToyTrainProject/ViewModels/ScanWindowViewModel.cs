using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using ToyTrainProject.Models;

namespace ToyTrainProject.ViewModels
{
    public class ScanWindowViewModel:ViewModelBase
    {
        private const string SubscriptionKey = "";
        const string UriBase = "";
        private const string TemporaryImagePath = "";
        
        private DeviceInfo _selectedDevice;
        public DeviceInfo SelectedDevice
        {
            get { return _selectedDevice; }
            set { _selectedDevice = value; OnPropertyChanged(); }
        }

        private string _cameraUsbId;
        public string CameraUsbId
        {
            get { return _cameraUsbId; }
            set { _cameraUsbId = value; OnPropertyChanged(); }
        }

        private string _responseText;
        public string ResponseText
        {
            get { return _responseText; }
            set { _responseText = value; OnPropertyChanged();}
        }
  
        private RelayCommand _submitSnapShotCommand;
        public RelayCommand SubmitSnapShotCommand => _submitSnapShotCommand ?? (_submitSnapShotCommand = new RelayCommand(SubmitSnapShot));
        
        private ImageSource _snapshotImageSource;
        public ImageSource SnapshotImageSource
        {
            get { return _snapshotImageSource; }
            set { _snapshotImageSource = value; OnPropertyChanged(); }
        }
        
        private Bitmap _snapshotBitmap;
        public Bitmap SnapshotBitmap
        {
            get { return _snapshotBitmap; }
            set
            {
                if (SetField(ref _snapshotBitmap, value, "SnapshotBitmap"))
                {
                    SnapshotImageSource = ConvertToImageSource(SnapshotBitmap);
                }
            }
        }

        public ScanWindowViewModel()
        {

        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private  void SubmitSnapShot()
        {
             MakeAnalysisRequest(TemporaryImagePath);
        }
        
        private async void MakeAnalysisRequest(string imageFilePath)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);

            string requestParameters = "visualFeatures=Categories,Description,Color&language=en";

            string uri = UriBase + "?" + requestParameters;

            if (SnapshotBitmap != null)
            {
               
            byte[] byteData = ImageToByte(SnapshotBitmap);
            //byte[] byteData = GetImageAsByteArray(imageFilePath);

            HttpResponseMessage response;

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // This example uses content type "application/octet-stream".
                    // The other content types you can use are "application/json" and "multipart/form-data".
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    // Execute the REST API call.
                    response = await client.PostAsync(uri, content);

                    // Get the JSON response.
                    string contentString = await response.Content.ReadAsStringAsync();

                    ResponseText = JsonPrettyPrint(contentString);
                }
            }        
        }
        
        //get data from taken snapshot
        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
     
        static byte[] GetImageAsByteArray(string imagePath)
        {
            FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
        
        public static ImageSource ConvertToImageSource(Bitmap bitmap)
        {
            var imageSourceConverter = new ImageSourceConverter();
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
                var snapshotBytes = memoryStream.ToArray();
                return (ImageSource)imageSourceConverter.ConvertFrom(snapshotBytes); ;
            }
        }

        static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");

            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            int offset = 0;
            int indentLength = 3;

            foreach (char ch in json)
            {
                switch (ch)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\'':
                        if (quote) ignore = !ignore;
                        break;
                }

                if (quote)
                    sb.Append(ch);
                else
                {
                    switch (ch)
                    {
                        case '{':
                        case '[':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', ++offset * indentLength));
                            break;
                        case '}':
                        case ']':
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', --offset * indentLength));
                            sb.Append(ch);
                            break;
                        case ',':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', offset * indentLength));
                            break;
                        case ':':
                            sb.Append(ch);
                            sb.Append(' ');
                            break;
                        default:
                            if (ch != ' ') sb.Append(ch);
                            break;
                    }
                }
            }

            return sb.ToString().Trim();
        }
    }
}

