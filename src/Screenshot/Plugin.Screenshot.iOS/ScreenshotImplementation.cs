using Foundation;
using Plugin.Screenshot.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;
using UIKit;

namespace Plugin.Screenshot
{
    /// <summary>
    /// Implementation for Screenshot
    /// </summary>
    public class ScreenshotImplementation : IScreenshot
    {   
        public async Task<byte[]> CaptureAsync()
        {
            var view = UIApplication.SharedApplication.KeyWindow.RootViewController.View;

            UIGraphics.BeginImageContext(view.Frame.Size);
            view.DrawViewHierarchy(view.Frame, true);
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            using (var imageData = image.AsPNG())
            {
                var bytes = new byte[imageData.Length];
                System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, bytes, 0, Convert.ToInt32(imageData.Length));
                return bytes;
            }
        }

        public async Task CaptureAndSaveAsync()
        {
            var bytes = await CaptureAsync();
            var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string date = DateTime.Now.ToString().Replace("/", "-").Replace(":", "-");
            string localPath = System.IO.Path.Combine(documentsDirectory, "Screnshot-" + date + ".png");

            var chartImage = new UIImage(NSData.FromArray(bytes));
            chartImage.SaveToPhotosAlbum((image, error) =>
            {
                //you can retrieve the saved UI Image as well if needed using
                //var i = image as UIImage;
                if (error != null)
                {
                    Console.WriteLine(error.ToString());
                }
            });
        }
    }
}