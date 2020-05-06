using Manager.SharedLibs.Logging;
using Manager.WebApp.Settings;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace Manager.WebApp.Helpers
{
    public class FileUploadHelper
    {
        private static readonly ILog logger = LogProvider.For<FileUploadHelper>();
        public static char DirSeparator = System.IO.Path.DirectorySeparatorChar;
        //public static string FilesPath = HttpContext.Current.Server.MapPath("~\\Content" + DirSeparator + "Uploads" + DirSeparator + "{0}");

        private static readonly int exifOrientationID = 0x112; //274

        public static void ExifRotate(Image img)
        {
            if (!img.PropertyIdList.Contains(exifOrientationID))
                return;

            var prop = img.GetPropertyItem(exifOrientationID);
            int val = BitConverter.ToUInt16(prop.Value, 0);
            var rot = RotateFlipType.RotateNoneFlipNone;

            if (val == 3 || val == 4)
                rot = RotateFlipType.Rotate180FlipNone;
            else if (val == 5 || val == 6)
                rot = RotateFlipType.Rotate90FlipNone;
            else if (val == 7 || val == 8)
                rot = RotateFlipType.Rotate270FlipNone;

            if (val == 2 || val == 4 || val == 5 || val == 7)
                rot |= RotateFlipType.RotateNoneFlipX;

            if (rot != RotateFlipType.RotateNoneFlipNone)
                img.RotateFlip(rot);
        }

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

        public static string UploadPostedFile(HttpPostedFile file, string folderName, bool includeDatePath = false)
        {
            if (includeDatePath)
                folderName = folderName + "/" + DateTime.Now.ToString("dd-MM-yyyy");

            var folderPath = SystemSettings.MediaFileUrl + "/" + folderName;
            // Check if we have a file
            if (null == file) return "";
            // Make sure the file has content
            if (!(file.ContentLength > 0)) return "";

            var fileExt = Path.GetExtension(file.FileName);
            var fileName = TextHelpers.ConvertToUrlFriendly(Path.GetFileNameWithoutExtension(file.FileName), "img");

            var newFilePrefix = EpochTime.GetIntDate(DateTime.Now) + "_";

            var newFileName = newFilePrefix + fileName + fileExt;
            var absolutePath = folderName + "/" + newFileName;

            // Make sure we were able to determine a proper extension
            if (null == fileExt) return "";

            // Check if the directory we are saving to exists
            if (!Directory.Exists(folderPath))
            {
                // If it doesn't exist, create the directory
                Directory.CreateDirectory(folderPath);
            }

            // Set our full path for saving
            string path = folderPath + DirSeparator + newFileName;

            // Save our file
            try
            {
                var myImg = Image.FromStream(file.InputStream, true, true);
                ExifRotate(myImg);

                //Decrease size
                var newImgBytes = ResizeImage(myImg, myImg.Width, myImg.Height);
                var newImg = (Bitmap)((new ImageConverter()).ConvertFrom(newImgBytes));

                newImg.Save(Path.GetFullPath(path));
                //
                //var myImg = Image.FromStream(file.InputStream, true, true);
                //var newImgBytes = ResizeImage(myImg, myImg.Width, myImg.Height);
                //var newImg = (Bitmap)((new ImageConverter()).ConvertFrom(newImgBytes));

                //if (SystemSettings.WatermarkEnabled)
                //{
                //    AddWatermark((Image)newImg, SystemSettings.WatermarkPosition);
                //    //AddStringWatermark((Image)newImg, "Halo VN", ImageFormat.Jpeg);
                //}

                //newImg.Save(Path.GetFullPath(path));

                //if (img.Width > 1000)
                //    img.Resize(800, 600);

                //img.Save(Path.GetFullPath(path), null, false);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed for UploadPostedFile: {0}", ex.ToString());
                logger.Error(strError);

                return string.Empty;
            }

            // Return the filename
            return absolutePath;
        }

        public static string UploadPostedFile(HttpPostedFileBase file, string folderName, bool includeDatePath = false)
        {
            if (includeDatePath)
                folderName = folderName + "/" + DateTime.Now.ToString("dd-MM-yyyy");

            //var folderPath = SystemSettings.MediaFileUrl + "/" + folderName;
            var folderPath = HttpContext.Current.Server.MapPath("~\\Media" + DirSeparator + "Uploads" + DirSeparator + folderName);
            // Check if we have a file
            if (null == file) return "";
            // Make sure the file has content
            if (!(file.ContentLength > 0)) return "";

            var fileExt = Path.GetExtension(file.FileName);
            var fileName = TextHelpers.ConvertToUrlFriendly(Path.GetFileNameWithoutExtension(file.FileName), "img");

            var newFilePrefix = EpochTime.GetIntDate(DateTime.Now) + "_";

            var newFileName = newFilePrefix + fileName + fileExt;
            var absolutePath = "Media/Uploads/" + folderName + "/" + newFileName;

            // Make sure we were able to determine a proper extension
            if (null == fileExt) return "";

            // Check if the directory we are saving to exists
            if (!Directory.Exists(folderPath))
            {
                // If it doesn't exist, create the directory
                Directory.CreateDirectory(folderPath);
            }

            // Set our full path for saving
            string path = folderPath + DirSeparator + newFileName;

            // Save our file
            try
            {
                var myImg = Image.FromStream(file.InputStream, true, true);
                ExifRotate(myImg);

                //Decrease size
                var newImgBytes = ResizeImage(myImg, myImg.Width, myImg.Height);
                var newImg = (Bitmap)((new ImageConverter()).ConvertFrom(newImgBytes));

                newImg.Save(Path.GetFullPath(path));
                //
                //var myImg = Image.FromStream(file.InputStream, true, true);
                //var newImgBytes = ResizeImage(myImg, myImg.Width, myImg.Height);
                //var newImg = (Bitmap)((new ImageConverter()).ConvertFrom(newImgBytes));

                //if (SystemSettings.WatermarkEnabled)
                //{
                //    AddWatermark((Image)newImg, SystemSettings.WatermarkPosition);
                //    //AddStringWatermark((Image)newImg, "Halo VN", ImageFormat.Jpeg);
                //}

                //newImg.Save(Path.GetFullPath(path));

                //if (img.Width > 1000)
                //    img.Resize(800, 600);

                //img.Save(Path.GetFullPath(path), null, false);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed for UploadPostedFile: {0}", ex.ToString());
                logger.Error(strError);

                return string.Empty;
            }

            // Return the filename
            return absolutePath;
        }

        private static Image AddWatermark(Image image, string position = "top-left")
        {
            var x = 0;
            var y = 0;
            using (Image watermarkImage = Image.FromFile(HttpContext.Current.Server.MapPath("~\\Content\\images\\watermark.png")))
            using (Graphics imageGraphics = Graphics.FromImage(image))
            using (TextureBrush watermarkBrush = new TextureBrush(watermarkImage))
            {
                //int x = (image.Width / 2 - watermarkImage.Width / 2);
                //int y = (image.Height / 2 - watermarkImage.Height / 2);

                if (position == "top-left")
                {
                    x = watermarkImage.Width / 2; y = watermarkImage.Height / 2;
                }
                else if (position == "top-center")
                {
                    x = (image.Width / 2 - watermarkImage.Width / 2); y = watermarkImage.Height / 2;
                }
                else if (position == "top-right")
                {
                    x = (image.Width - (watermarkImage.Width + (watermarkImage.Width / 2))); y = watermarkImage.Height / 2;
                }
                else if (position == "middle-left")
                {
                    x = watermarkImage.Width / 2; y = (image.Height / 2 - watermarkImage.Height / 2);
                }
                else if (position == "middle-center")
                {
                    x = (image.Width / 2 - watermarkImage.Width / 2);
                    y = (image.Height / 2 - watermarkImage.Height / 2);
                }
                else if (position == "middle-right")
                {
                    x = (image.Width - (watermarkImage.Width + (watermarkImage.Width / 2))); y = (image.Height / 2 - watermarkImage.Height / 2);
                }
                else if (position == "bottom-left")
                {
                    x = watermarkImage.Width / 2; y = (image.Height - (watermarkImage.Height + (watermarkImage.Height / 2)));
                }
                else if (position == "bottom-center")
                {
                    x = (image.Width / 2 - watermarkImage.Width / 2); y = (image.Height - (watermarkImage.Height + (watermarkImage.Height / 2)));
                }
                else if (position == "bottom-right")
                {
                    x = (image.Width - (watermarkImage.Width + (watermarkImage.Width / 2)));
                    y = (image.Height - (watermarkImage.Height + (watermarkImage.Height / 2)));
                }
                else
                {
                    x = watermarkImage.Width / 2; y = watermarkImage.Height / 2;
                }

                watermarkBrush.TranslateTransform(x, y);
                imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width, watermarkImage.Height)));
            }

            return image;
        }

        public static Image AddStringWatermark(Image image, string text, ImageFormat fmt)
        {

            try
            {
                // open source image as stream and create a memorystream for output
                Stream output = new MemoryStream();
                // choose font for text
                Font font = new Font("Arial", (int)((float)image.Width / 20), FontStyle.Bold, GraphicsUnit.Pixel);

                //choose color and transparency
                Color color = Color.FromArgb(100, 255, 0, 0);

                //location of the watermark text in the parent image
                Point pt = new Point(100, 50);
                SolidBrush brush = new SolidBrush(color);

                //draw text on image
                Graphics graphics = Graphics.FromImage(image);
                graphics.DrawString(text, font, brush, pt);
                graphics.Dispose();

                //update image memorystream
                //image.Save(output, fmt);
                // Image imgFinal = Image.FromStream(output);

                //write modified image to file
                //Bitmap bmp = new System.Drawing.Bitmap(image.Width, image.Height, image.PixelFormat);
                //Graphics graphics2 = Graphics.FromImage(bmp);
                //graphics2.DrawImage(imgFinal, new Point(0, 0));                

            }
            catch (Exception ex)
            {

            }

            return image;
        }


        public static byte[] ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.Default;
                graphics.InterpolationMode = InterpolationMode.Default;
                graphics.SmoothingMode = SmoothingMode.Default;
                graphics.PixelOffsetMode = PixelOffsetMode.Default;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            MemoryStream ms = new MemoryStream();
            destImage.Save(ms, ImageFormat.Png);

            return ms.ToArray();
        }

        //public static string UploadVideo(HttpPostedFile file, string folderName, out string cover)
        //{
        //    cover = string.Empty;
        //    folderName = folderName + "/" + DateTime.Now.ToString("dd-MM-yyyy");
        //    var folderPath = SystemSettings.MediaFileUrl + "/" + folderName;
        //    // Check if we have a file
        //    if (null == file) return "";
        //    // Make sure the file has content
        //    if (!(file.ContentLength > 0)) return "";

        //    //string fileName = DateTime.Now.Millisecond + file.FileName;
        //    var fileExt = Path.GetExtension(file.FileName);
        //    var fileName = TextHelpers.ConvertToUrlFriendly(Path.GetFileNameWithoutExtension(file.FileName));
        //    var newFilePrefix = EpochTime.GetIntDate(DateTime.Now) + "_";
        //    var newFileName = newFilePrefix + fileName + fileExt;
        //    var absolutePath = folderName + "/" + newFileName;

        //    // Make sure we were able to determine a proper extension
        //    if (null == fileExt) return "";

        //    // Check if the directory we are saving to exists
        //    if (!Directory.Exists(folderPath))
        //    {
        //        // If it doesn't exist, create the directory
        //        Directory.CreateDirectory(folderPath);
        //    }

        //    // Set our full path for saving
        //    string path = folderPath + DirSeparator + newFileName;

        //    // Save our file
        //    try
        //    {
        //        var fullPath = Path.GetFullPath(path);
        //        file.SaveAs(fullPath);

        //        //Make sure the file was uploaded succesfully
        //        if (System.IO.File.Exists(fullPath))
        //        {
        //            //After upload success
        //            cover = folderName + "/" + fileName + "_thumb.png";
        //            var coverPath = folderPath + DirSeparator + newFilePrefix + fileName + "_thumb.png";
        //            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();

        //            //Take the image after 5 seconds
        //            ffMpeg.GetVideoThumbnail(fullPath, coverPath, 5);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = string.Format("Failed for UploadVideo: {0}", ex.ToString());
        //        logger.Error(strError);
        //    }

        //    // Return the filename
        //    return absolutePath;
        //}

        public static string UploadPostedFileBase(HttpPostedFileBase file, string folderName)
        {
            var folderPath = SystemSettings.MediaFileUrl + "/" + folderName;
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

        public static void ResizeImage(HttpPostedFileBase file, int width, int height, string folderPath)
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

        public static string ResizeImageByScreenCroping(HttpPostedFileBase file, int screenWidth, string folderPath)
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
            var newHeight = (int)(screenWidth * ratio);

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