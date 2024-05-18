using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Threading;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace _1221
{
    public partial class Form1 : Form
    {
        //我是用怡蓁的，因為他的更精確一些
        //辨識車牌位置的自訂視覺服務位置及金鑰(custom-vision)
        const string endpoint = "https://yz-1222.cognitiveservices.azure.com/customvision/v3.0/Prediction/8b6bcac8-c6eb-4c1c-9b15-e4a95cf61473/detect/iterations/Iteration1/image";
        const string key = "952761d591d84bf3889e43507b36f4bc";

        //ocr服務的端點及金鑰(computer-vision)
        const string ocr_endpoint = "https://325-computervision.cognitiveservices.azure.com/";
        const string ocr_key = "68b279ab63b2412da7afdb620c331389";

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string imgPath;
            //找到檔案
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                imgPath = openFileDialog1.FileName;
                pictureBox1.Image = new Bitmap(imgPath);

                // 創建一個資料夾來存放處理過的圖片
                string processedImagesFolder = Path.Combine(Path.GetDirectoryName(imgPath), "ProcessedImages");
                Directory.CreateDirectory(processedImagesFolder);

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Prediction-key", key);

                FileStream fileStream = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
                BinaryReader reader = new BinaryReader(fileStream);
                byte[] buffer = reader.ReadBytes((int)fileStream.Length);
                ByteArrayContent content = new ByteArrayContent(buffer);
                //依指示將conttenttype的header設為"application/octet-stream"
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                //將圖片資料透過 filestream 傳遞給 endpoint api並等待其回傳偵測結果
                HttpResponseMessage responseMessage = await client.PostAsync(endpoint, content);

                string jsonStr = await responseMessage.Content.ReadAsStringAsync();
                richTextBox1.Text = JObject.Parse(jsonStr).ToString();

                Carplate_info carplate_Info = JsonConvert.DeserializeObject<Carplate_info>(jsonStr);
                string message = "";
                message += $"共抓到的車牌有{carplate_Info.predictions.Count}個\n";

                Graphics g = pictureBox1.CreateGraphics();
                Pen p = new Pen(Color.Red, 1);
                int pb_Width = pictureBox1.Width;
                int pb_Height = pictureBox1.Height;

                int count = 0;
                double pro = 0.5;
                Bitmap originalImage = new Bitmap(imgPath);

                foreach (Prediction prediction in carplate_Info.predictions)
                {
                    if (prediction.probability >= pro)
                    {
                        int left = (int)(prediction.boundingBox.left * pb_Width);
                        int top = (int)(prediction.boundingBox.top * pb_Height);
                        int width = (int)(prediction.boundingBox.width * pb_Width);
                        int height = (int)(prediction.boundingBox.height * pb_Height);
                        g.DrawRectangle(p, new Rectangle(left, top, width, height));

                        // 擷取車牌區域
                        Bitmap croppedImage = new Bitmap(width , height+20);
                        using (Graphics cropGraphics = Graphics.FromImage(croppedImage))
                        {
                            cropGraphics.DrawImage(originalImage, 0, 0, new Rectangle(left - 40, top - 12, width, height), GraphicsUnit.Pixel);
                        }

                        // 處理過的圖片儲存到新資料夾中
                        string processedImagePath = Path.Combine(processedImagesFolder, $"cropimg_{count}.png");
                        croppedImage.Save(processedImagePath, System.Drawing.Imaging.ImageFormat.Png);

                        count++;
                        message +=
                            $"  Tag Name：{prediction.tagName}\n" +
                            $"  Tag Id：{prediction.tagId}\n" +
                            $"  Probability：{prediction.probability:p1}\n" +
                            $"  Left：{left}\n" +
                            $"  Top：{top}\n" +
                            $"  Width：{width}\n" +
                            $"  Height：{height}\n\n";

                        pictureBox2.Image = croppedImage;
                    }
                }

                message += $"共有{count}個車牌的信心度大於{pro:p0}";

                fileStream.Close();

                //ocr
                foreach (string imagePath in Directory.GetFiles(processedImagesFolder))
                {
                    fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                    ComputerVisionClient visionClient = new ComputerVisionClient(
                        new ApiKeyServiceClientCredentials(ocr_key),
                        new System.Net.Http.DelegatingHandler[] { });
                    visionClient.Endpoint = ocr_endpoint;

                    ReadInStreamHeaders textHeaders = await visionClient.ReadInStreamAsync(fileStream);
                    string operationLocation = textHeaders.OperationLocation;
                    Thread.Sleep(1000);
                    string operationId = operationLocation.Substring(operationLocation.Length - 36);

                    ReadOperationResult result = await visionClient.GetReadResultAsync(Guid.Parse(operationId));

                    if (result != null && result.Status == OperationStatusCodes.Succeeded)
                    {
                        IList<ReadResult> textUrlFileResults = result.AnalyzeResult.ReadResults;

                        string str = "";
                        foreach (ReadResult textResult in textUrlFileResults)
                        {
                            foreach (Line line in textResult.Lines)
                            {
                                str += line.Text + "\n";
                            }
                        }
                        message += "\n\n車牌號碼:\t" + str + "\n";
                        richTextBox2.Text = message;
                    }
                    else
                    {
                        richTextBox2.Text += "\n\nOCR 操作未成功完成或失敗。\n";
                    }

                   

                    fileStream.Close();
                }
            }
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
