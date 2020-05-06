using FileReading.Logging;
using FileReading.Models;
using FileReading.Settings;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace FileReading.Helpers
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

        public static string ImageToBase64(Image image,
          System.Drawing.Imaging.ImageFormat format)
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

        public static Image Base64ToImage(string base64String)
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

        public static string UploadSingleImage(HttpPostedFile file, string folderName, bool includeDatePath = true)
        {
            var dtNow = DateTime.Now;
            if (includeDatePath)
                folderName = folderName + "/" + dtNow.ToString("dd-MM-yyyy");

            var folderPath = SystemSettings.MediaFileUrl + "/" + folderName;
            // Check if we have a file
            if (null == file) return "";
            // Make sure the file has content
            if (!(file.ContentLength > 0)) return "";

            var fileExt = Path.GetExtension(file.FileName);
            var fileName = TextHelpers.ConvertToUrlFriendly(Path.GetFileNameWithoutExtension(file.FileName), "img");

            //var newFilePrefix = EpochTime.GetIntDate(DateTime.Now) + "_";

            var newFileName = fileName + "_" + EpochTime.GetIntDate(dtNow) + fileExt;
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

                // Assumes myImage is the PNG you are converting
                using (var b = new Bitmap(myImg.Width, myImg.Height))
                {
                    b.SetResolution(myImg.HorizontalResolution, myImg.VerticalResolution);

                    using (var g = Graphics.FromImage(b))
                    {
                        g.Clear(Color.White);
                        g.DrawImageUnscaled(myImg, 0, 0);
                    }

                    // Now save b as a JPEG like you normally would
                    b.Save(Path.GetFullPath(path), ImageFormat.Jpeg);
                }

                //var newWidth = (int)(myImg.Width * 1);
                //var newHeight = (int)(myImg.Height * 1);
                //var thumbnailImg = new Bitmap(newWidth, newHeight);
                //var thumbGraph = Graphics.FromImage(thumbnailImg);
                //thumbnailImg.SetTransparencyKey()
                //thumbGraph.CompositingQuality = CompositingQuality.Default;
                //thumbGraph.SmoothingMode = SmoothingMode.Default;
                //thumbGraph.InterpolationMode = InterpolationMode.Default;
                //var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                //thumbGraph.DrawImage(myImg, imageRectangle);
                //thumbnailImg.Save(targetPath, image.RawFormat);

                //myImg.Save(Path.GetFullPath(path), ImageFormat.Jpeg);

                //Decrease size
                //var newImgBytes = ResizeImage(myImg, myImg.Width, myImg.Height);
                //newImgBytes.Save(Path.GetFullPath(path));
                //var newImg = (Image)((new ImageConverter()).ConvertFrom(newImgBytes));                


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

        public static FileUploadResponseModel UploadPostedFile(HttpPostedFile file, string folderName, bool includeDatePath = true)
        {
            FileUploadResponseModel returnFile = new FileUploadResponseModel();
            var dtNow = DateTime.Now;
            if (includeDatePath)
                folderName = folderName + "/" + dtNow.ToString("dd-MM-yyyy");

            var folderPath = SystemSettings.MediaFileUrl + "/" + folderName;

            // Make sure the file has content
            if (file == null) return null;

            var absolutePath = string.Empty;

            // Save our file
            try
            {
                // Check if the directory we are saving to exists
                if (!Directory.Exists(folderPath))
                {
                    // If it doesn't exist, create the directory
                    Directory.CreateDirectory(folderPath);
                }

                var myImg = Image.FromStream(file.InputStream, true, true);

                var fileExt = Path.GetExtension(file.FileName);
                var fileName = TextHelpers.ConvertToUrlFriendly(Path.GetFileNameWithoutExtension(file.FileName), "img");
                // Make sure we were able to determine a proper extension
                if (null == fileExt) return null;

                //string fileName = Path.ChangeExtension(
                //    Path.GetRandomFileName(),
                //    fileExt
                //);

                var newFileName = fileName + "_" + EpochTime.GetIntDate(dtNow) + fileExt;
                absolutePath = folderName + "/" + newFileName;

                // Set our full path for saving
                string path = folderPath + DirSeparator + newFileName;

                ExifRotate(myImg);

                // Assumes myImage is the PNG you are converting
                using (var b = new Bitmap(myImg.Width, myImg.Height))
                {
                    b.SetResolution(myImg.HorizontalResolution, myImg.VerticalResolution);

                    using (var g = Graphics.FromImage(b))
                    {
                        g.Clear(Color.White);
                        g.DrawImageUnscaled(myImg, 0, 0);
                    }

                    // Now save b as a JPEG like you normally would
                    b.Save(Path.GetFullPath(path), ImageFormat.Jpeg);
                }

                returnFile.FileName = newFileName;
                returnFile.Path = absolutePath;
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed for UploadPostedFile: {0}", ex.ToString());
                logger.Error(strError);

                return null;
            }

            // Return the path
            return returnFile;
        }

        public static FileUploadResponseModel UploadFrombase64(string file, string folderName, bool includeDatePath = true)
        {
            FileUploadResponseModel returnFile = new FileUploadResponseModel();
            var dtNow = DateTime.Now;
            if (includeDatePath)
                folderName = folderName + "/" + dtNow.ToString("dd-MM-yyyy");

            var folderPath = SystemSettings.MediaFileUrl + "/" + folderName;

            // Make sure the file has content
            if (string.IsNullOrEmpty(file)) return null;

            var absolutePath = string.Empty;

            // Save our file
            try
            {              
                // Check if the directory we are saving to exists
                if (!Directory.Exists(folderPath))
                {
                    // If it doesn't exist, create the directory
                    Directory.CreateDirectory(folderPath);
                }

                var myImg = Base64ToImage(file);

                var fileExt = ".jpg";
                // Make sure we were able to determine a proper extension
                if (null == fileExt) return null;

                string fileName = Path.ChangeExtension(
                    Path.GetRandomFileName(),
                    fileExt
                );

                var newFileName = fileName;
                absolutePath = folderName + "/" + newFileName;

                // Set our full path for saving
                string path = folderPath + DirSeparator + newFileName;

                ExifRotate(myImg);

                // Assumes myImage is the PNG you are converting
                using (var b = new Bitmap(myImg.Width, myImg.Height))
                {
                    b.SetResolution(myImg.HorizontalResolution, myImg.VerticalResolution);

                    using (var g = Graphics.FromImage(b))
                    {
                        g.Clear(Color.White);
                        g.DrawImageUnscaled(myImg, 0, 0);
                    }

                    // Now save b as a JPEG like you normally would
                    b.Save(Path.GetFullPath(path), ImageFormat.Jpeg);
                }

                returnFile.FileName = newFileName;
                returnFile.Path = absolutePath;
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed for UploadPostedFile: {0}", ex.ToString());
                logger.Error(strError);

                return null;
            }

            // Return the path
            return returnFile;
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


        //public static Image ResizeImage(Image image, int width, int height)
        //{
        //    var destRect = new Rectangle(0, 0, width, height);
        //    var destImage = new Bitmap(width, height);

        //    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        //    using (var graphics = Graphics.FromImage(destImage))
        //    {
        //        graphics.CompositingMode = CompositingMode.SourceCopy;
        //        graphics.CompositingQuality = CompositingQuality.Default;
        //        graphics.InterpolationMode = InterpolationMode.Default;
        //        graphics.SmoothingMode = SmoothingMode.Default;
        //        graphics.PixelOffsetMode = PixelOffsetMode.Default;

        //        using (var wrapMode = new ImageAttributes())
        //        {
        //            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
        //            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
        //        }
        //    }
        //    MemoryStream ms = new MemoryStream();
        //    destImage.Save(ms, ImageFormat.Jpeg);

        //    return (Image)destImage;
        //}

        //public static Image ResizeImage(Image image, int width, int height)
        //{
        //    var destRect = new Rectangle(0, 0, width, height);
        //    var destImage = new Bitmap(width, height);

        //    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        //    using (var graphics = Graphics.FromImage(destImage))
        //    {
        //        graphics.CompositingMode = CompositingMode.SourceCopy;
        //        graphics.CompositingQuality = CompositingQuality.Default;
        //        graphics.InterpolationMode = InterpolationMode.Default;
        //        graphics.SmoothingMode = SmoothingMode.Default;
        //        graphics.PixelOffsetMode = PixelOffsetMode.Default;

        //        using (var wrapMode = new ImageAttributes())
        //        {
        //            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
        //            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
        //        }
        //    }
        //    MemoryStream ms = new MemoryStream();
        //    destImage.Save(ms, ImageFormat.Jpeg);

        //    Image img = Image.FromStream(ms);
        //    return img;

        //    //return ms.ToArray();
        //}

        public static Image ResizeImageFromStream(Stream fromStream, double scaleFactor)
        {
            var toStream = fromStream;

            var image = Image.FromStream(fromStream, true, true);
            ExifRotate(image);

            //var image = Image.FromStream(fromStream, true, true);
            var newWidth = (int)(image.Width * scaleFactor);
            var newHeight = (int)(image.Height * scaleFactor);
            var thumbnailBitmap = new Bitmap(newWidth, newHeight);

            var thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
            thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
            thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
            thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
            thumbnailGraph.DrawImage(image, imageRectangle);

            thumbnailBitmap.Save(toStream, image.RawFormat);

            thumbnailGraph.Dispose();
            thumbnailBitmap.Dispose();
            image.Dispose();

            return Image.FromStream(toStream, true, true);
        }

        public static string UploadVideo(HttpPostedFile file, string folderName, out string cover, bool generateThumb = false, bool includeDatePath = false)
        {
            cover = string.Empty;

            if (includeDatePath)
                folderName = folderName + "/" + DateTime.Now.ToString("dd-MM-yyyy");

            var folderPath = SystemSettings.MediaFileUrl + "/" + folderName;
            // Check if we have a file
            if (null == file) return "";
            // Make sure the file has content
            if (!(file.ContentLength > 0)) return "";

            //string fileName = DateTime.Now.Millisecond + file.FileName;
            var fileExt = Path.GetExtension(file.FileName);
            var fileName = TextHelpers.ConvertToUrlFriendly(Path.GetFileNameWithoutExtension(file.FileName));
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
                var fullPath = Path.GetFullPath(path);
                file.SaveAs(fullPath);

                if (generateThumb)
                {
                    //Make sure the file was uploaded succesfully
                    if (System.IO.File.Exists(fullPath))
                    {
                        //After upload success
                        cover = folderName + "/" + fileName + "_thumb.png";
                        var coverPath = folderPath + DirSeparator + newFilePrefix + fileName + "_thumb.png";
                        var ffMpeg = new NReco.VideoConverter.FFMpegConverter();

                        //Take the image after 5 seconds
                        ffMpeg.GetVideoThumbnail(fullPath, coverPath, 5);
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed for UploadVideo: {0}", ex.ToString());
                logger.Error(strError);
            }

            // Return the filename
            return absolutePath;
        }

        public static string UploadVideoWithCompress(HttpPostedFile file, string folderName, out string cover, bool generateThumb = false, bool includeDatePath = false)
        {
            cover = string.Empty;

            if (includeDatePath)
                folderName = folderName + "/" + DateTime.Now.ToString("dd-MM-yyyy");

            var folderPath = SystemSettings.MediaFileUrl + "/" + folderName;
            // Check if we have a file
            if (null == file) return "";
            // Make sure the file has content
            if (!(file.ContentLength > 0)) return "";

            //string fileName = DateTime.Now.Millisecond + file.FileName;
            var fileExt = Path.GetExtension(file.FileName);
            var fileName = TextHelpers.ConvertToUrlFriendly(Path.GetFileNameWithoutExtension(file.FileName));
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
            string tmpPath = folderPath + DirSeparator + "tmp_" + newFileName;
            string path = folderPath + DirSeparator + newFileName;
            // Save our file
            try
            {
                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();

                var fullPath = Path.GetFullPath(path);
                var tmpFullPath = Path.GetFullPath(tmpPath);

                file.SaveAs(tmpFullPath);
                
                ffMpeg.ConvertMedia(tmpFullPath, fullPath, NReco.VideoConverter.Format.mp4);

                //NReco.VideoConverter.ConvertSettings cvSetting = new NReco.VideoConverter.ConvertSettings();
                //cvSetting.CustomOutputArgs = " -b:v 64k -bufsize 64k ";

                //new NReco.VideoConverter.FFMpegConverter().ConvertMedia(tmpFullPath, null, fullPath, null, cvSetting);

                if (generateThumb)
                {
                    //Make sure the file was uploaded succesfully
                    if (System.IO.File.Exists(fullPath))
                    {
                        //After upload success
                        cover = folderName + "/" + fileName + "_thumb.png";
                        var coverPath = folderPath + DirSeparator + newFilePrefix + fileName + "_thumb.png";                        

                        //Take the image after 5 seconds
                        ffMpeg.GetVideoThumbnail(fullPath, coverPath, 5);
                    }
                }

                if (File.Exists(tmpFullPath))
                    File.Delete(tmpFullPath);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed for UploadVideo: {0}", ex.ToString());
                logger.Error(strError);
            }

            // Return the filename
            return absolutePath;
        }

        public static string CompressFiles(HttpPostedFile file,string rawFile, string folderName, out string cover, bool generateThumb = false, bool includeDatePath = false)
        {
            cover = string.Empty;

            if (includeDatePath)
                folderName = folderName + "/" + DateTime.Now.ToString("dd-MM-yyyy");

            var folderPath = SystemSettings.MediaFileUrl + "/" + folderName;
            // Check if we have a file
            if (null == file) return "";
            // Make sure the file has content
            if (!(file.ContentLength > 0)) return "";

            //string fileName = DateTime.Now.Millisecond + file.FileName;
            var fileExt = Path.GetExtension(file.FileName);
            var fileName = TextHelpers.ConvertToUrlFriendly(Path.GetFileNameWithoutExtension(file.FileName));
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
            string tmpPath = folderPath + DirSeparator + "tmp_" + newFileName;
            // Save our file
            try
            {
                var fullPath = Path.GetFullPath(path);
                var fullTmpPath = Path.GetFullPath(tmpPath);
                file.SaveAs(fullTmpPath);

                FileInfo fileToCompress = new FileInfo(fullTmpPath);
                using (MemoryStream compressedMemStream = new MemoryStream())
                {
                    using (FileStream originalFileStream = fileToCompress.OpenRead())
                    using (GZipStream compressionStream = new GZipStream(
                        compressedMemStream,
                        CompressionMode.Compress,
                        leaveOpen: true))
                    {
                        originalFileStream.CopyTo(compressionStream);
                    }
                    compressedMemStream.Seek(0, SeekOrigin.Begin);


                    FileStream compressedFileStream = File.Create(fileToCompress.FullName + ".gz");
                    //Eventually this will be the AWS transfer, but that's not important here
                    compressedMemStream.WriteTo(compressedFileStream);
                    compressedFileStream.Close();

                    //Remove old
                    File.Delete(fullTmpPath);
                }

                if (generateThumb)
                {
                    //Make sure the file was uploaded succesfully
                    if (System.IO.File.Exists(fullPath))
                    {
                        //After upload success
                        cover = folderName + "/" + fileName + "_thumb.png";
                        var coverPath = folderPath + DirSeparator + newFilePrefix + fileName + "_thumb.png";
                        var ffMpeg = new NReco.VideoConverter.FFMpegConverter();

                        //Take the image after 5 seconds
                        ffMpeg.GetVideoThumbnail(fullPath, coverPath, 5);
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed for UploadVideo: {0}", ex.ToString());
                logger.Error(strError);
            }

            // Return the filename
            return absolutePath;
        }

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