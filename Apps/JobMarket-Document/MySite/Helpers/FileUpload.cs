using MySite.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace MySite.Helpers
{
    public class FileUpload
    {
        private static readonly ILog logger = LogProvider.For<FileUpload>();

        public static char DirSeparator = System.IO.Path.DirectorySeparatorChar;

        public static string UploadFile(HttpPostedFileBase file, string folderName)
        {
            var folderPath = HttpContext.Current.Server.MapPath("~\\Content" + DirSeparator + "Uploads" + DirSeparator + folderName);
            // Check if we have a file
            if (null == file) return "";
            // Make sure the file has content
            if (!(file.ContentLength > 0)) return "";

            string fileName = DateTime.Now.Millisecond + file.FileName;
            string fileExt = Path.GetExtension(file.FileName);

            // Make sure we were able to determine a proper extension
            if (null == fileExt) return "";

            // Check if the directory we are saving to exists
            if (!Directory.Exists(folderPath))
            {
                // If it doesn't exist, create the directory
                Directory.CreateDirectory(folderPath);
            }

            // Set our full path for saving
            string path = folderPath + DirSeparator + fileName;

            // Save our file
            file.SaveAs(Path.GetFullPath(path));

            // Save our thumbnail as well
            ResizeImage(file, 70, 70, folderPath);

            // Return the filename
            return fileName;
        }

        public static void DeleteFile(string fileName, string folderPath)
        {
            // Don't do anything if there is no name
            if (fileName.Length == 0) return;

            // Set our full path for deleting
            string path = folderPath + DirSeparator + fileName;
            string thumbPath = folderPath + DirSeparator + "Thumbnails" + DirSeparator + fileName;

            RemoveFile(path);
            RemoveFile(thumbPath);
        }

        private static void RemoveFile(string path)
        {
            // Check if our file exists
            if (File.Exists(Path.GetFullPath(path)))
            {
                // Delete our file
                File.Delete(Path.GetFullPath(path));
            }
        }

        public static void ResizeImage(HttpPostedFileBase file, int width, int height, string folderPath)
        {
            string thumbnailDirectory = String.Format(@"{0}{1}{2}", folderPath, DirSeparator, "Thumbnails");

            // Check if the directory we are saving to exists
            if (!Directory.Exists(thumbnailDirectory))
            {
                // If it doesn't exist, create the directory
                Directory.CreateDirectory(thumbnailDirectory);
            }

            // Final path we will save our thumbnail
            string imagePath = String.Format(@"{0}{1}{2}", thumbnailDirectory, DirSeparator, file.FileName);
            // Create a stream to save the file to when we're done resizing
            FileStream stream = new FileStream(Path.GetFullPath(imagePath), FileMode.OpenOrCreate);

            // Convert our uploaded file to an image
            Image OrigImage = Image.FromStream(file.InputStream);
            // Create a new bitmap with the size of our thumbnail
            Bitmap TempBitmap = new Bitmap(width, height);

            // Create a new image that contains are quality information
            Graphics NewImage = Graphics.FromImage(TempBitmap);
            NewImage.CompositingQuality = CompositingQuality.HighQuality;
            NewImage.SmoothingMode = SmoothingMode.HighQuality;
            NewImage.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Create a rectangle and draw the image
            Rectangle imageRectangle = new Rectangle(0, 0, width, height);
            NewImage.DrawImage(OrigImage, imageRectangle);

            // Save the final file
            TempBitmap.Save(stream, OrigImage.RawFormat);

            // Clean up the resources
            NewImage.Dispose();
            TempBitmap.Dispose();
            OrigImage.Dispose();
            stream.Close();
            stream.Dispose();
        }

        public static Image ConvertHttpPostedFileBaseToImage(HttpPostedFileBase myFile)
        {
            try
            {
                var myImage = Image.FromStream(myFile.InputStream, true, true);

                return myImage;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not ConvertHttpPostedFileBaseToImage because: {0}", ex));
            }

            return null;
        }

        public static string ImageToBase64(Image image, ImageFormat format)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Convert Image to byte[]
                    image.Save(ms, format);
                    byte[] imageBytes = ms.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not ConvertImageToBase64 because: {0}", ex));
            }

            return string.Empty;
        }

        public static Image Base64ToImage(string base64String)
        {
            try
            {
                // Convert Base64 String to byte[]
                if (!string.IsNullOrEmpty(base64String))
                {
                    base64String = base64String.Replace("data:image/png;base64,", String.Empty).Replace("data:image/jpeg;base64,", String.Empty).Replace("data:image/jpg;base64,", String.Empty).Replace("data:image/bmp;base64,", String.Empty);
                }

                byte[] imageBytes = Convert.FromBase64String(base64String);
                MemoryStream ms = new MemoryStream(imageBytes, 0,
                  imageBytes.Length);

                // Convert byte[] to Image
                ms.Write(imageBytes, 0, imageBytes.Length);
                Image image = Image.FromStream(ms, true);
                return image;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not ConvertBase64ToImage because: {0}", ex));
            }

            return null;
        }
    }
}