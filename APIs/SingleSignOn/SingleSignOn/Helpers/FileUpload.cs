using SingleSignOn.Caching;
using SingleSignOn.Models;
using SingleSignOn.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace SingleSignOn.Helpers
{
    public static class FileUploadHelper
    {
        public static char DirSeparator = System.IO.Path.DirectorySeparatorChar;
        //public static string FilesPath = HttpContext.Current.Server.MapPath("~\\Content" + DirSeparator + "Uploads" + DirSeparator + "{0}");

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

        public static string UploadPostedFile(HttpPostedFile file, string folderName)
        {
            folderName = folderName + "/" + DateTime.Now.ToString("dd-MM-yyyy");
            var folderPath = SystemSettings.MediaFileFolder + "/" + folderName;            
            // Check if we have a file
            if (null == file) return "";
            // Make sure the file has content
            if (!(file.ContentLength > 0)) return "";

            //string fileName = DateTime.Now.Millisecond + file.FileName;
            var fileName = file.FileName;
            var absolutePath = folderName + "/" + file.FileName;
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
            try
            {
                WebImage img = new WebImage(file.InputStream);
                if (img.Width > 1000)
                    img.Resize(800, 600);

                img.Save(Path.GetFullPath(path));
            }
            catch
            {

            }

            // Return the filename
            return absolutePath;
        }

        public static string UploadPostedFileBase(HttpPostedFileBase file, string folderName)
        {
            var folderPath = SystemSettings.MediaFileFolder + "/" + folderName;
            // Check if we have a file
            if (null == file) return "";
            // Make sure the file has content
            if (!(file.ContentLength > 0)) return "";

            string fileName = DateTime.Now.Millisecond + file.FileName;
            var absolutePath = folderName + "/" + file.FileName;
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

            // Return the filename
            return absolutePath;
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

        public static void ResizeImage(HttpPostedFileBase file,int width, int height, string folderPath)
        {
            string thumbnailDirectory = String.Format(@"{0}{1}", folderPath, DirSeparator);

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

        public static string ResizeImageByScreenCroping(HttpPostedFileBase file,int screenWidth, string folderPath)
        {
            var newFileName = string.Empty;
            newFileName = file.FileName;

            string thumbnailDirectory = String.Format(@"{0}", folderPath);

            // Check if the directory we are saving to exists
            if (!Directory.Exists(thumbnailDirectory))
            {
                // If it doesn't exist, create the directory
                Directory.CreateDirectory(thumbnailDirectory);
            }

            // Final path we will save our thumbnail
            //string imagePath = String.Format(@"{0}{1}", thumbnailDirectory, file.FileName);
            string imagePath = String.Format(@"{0}{1}", thumbnailDirectory, newFileName);
            imagePath = imagePath.Replace("//", "/");
            // Create a stream to save the file to when we're done resizing
            FileStream stream = new FileStream(Path.GetFullPath(imagePath), FileMode.OpenOrCreate);

            // Convert our uploaded file to an image
            Image OrigImage = Image.FromStream(file.InputStream);

            var ratio = OrigImage.Height / (double)OrigImage.Width;
            var newWidth = screenWidth;
            var newHeight =(int)(screenWidth * ratio);

            // Create a new bitmap with the size of our thumbnail
            Bitmap TempBitmap = new Bitmap(newWidth, newHeight);

            // Create a new image that contains are quality information
            Graphics NewImage = Graphics.FromImage(TempBitmap);
            NewImage.CompositingQuality = CompositingQuality.HighQuality;
            NewImage.SmoothingMode = SmoothingMode.HighQuality;
            NewImage.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Create a rectangle and draw the image
            Rectangle imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
            NewImage.DrawImage(OrigImage, imageRectangle);

            // Save the final file
            TempBitmap.Save(stream, OrigImage.RawFormat);

            // Clean up the resources
            NewImage.Dispose();
            TempBitmap.Dispose();
            OrigImage.Dispose();
            stream.Close();
            stream.Dispose();

            return newFileName;
        }
    }
}