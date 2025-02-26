using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tkn_ToolBox;
using Tkn_Variable;

namespace Tkn_Images
{
    public class tImageService
    {
        tToolBox t = new tToolBox();
        
        public Image setImageDPI(Image inImage_, int newDpi, ImageFormat imageFormat_)
        {
            /// Dpi = Dots per inch , Bir inç karedeki nokta sayısı demektir. 
            /// Bir inch diye tabir edilen 2,54 cm’lik alanda bulunan nokta sayısına denir. 
            /// 2,54 cm x 2,54 cm kare içine düşen nokta sayısı. 
            /// Bir inç kare ye ne kadar çok nokta sığdırırsanız o kadar kaliteli sonuç elde edersiniz.
            ///
            /// Örnek verek olursak;
            /// 72 Dpi bir inç kare içinde 72 nokta demektir.
            /// 300 Dpi bir inç kare içinde 300 nokta demektir.
            /// 1440 Dpi bir inç kare içinde 1440 nokta demektir.

            Image resultImage = null;

            try
            {
                //Bitmap workingImage = null;
                int width = inImage_.Width;
                int height = inImage_.Height;
                //float horizontalResolation = inImage_.HorizontalResolution;
                //float verticalResolation = inImage_.VerticalResolution;

                //workingImage = new Bitmap(inImage_, width, height);

                /// dpi değişikliğin en basit hali
                /// dpi change
                /// 
                /// Bitmap _img = new Bitmap(pictureEdit1.Image, width, height);
                /// _img.SetResolution(newDpi, newDpi);

                Bitmap _img = null;
                _img = new Bitmap(inImage_, width, height);
                _img.SetResolution(newDpi, newDpi);

                string formatName = getImageFormatName(imageFormat_);

                string Images_Path = t.Find_Path("images") + t.getNewFileGuidName + "." + formatName; //"jpg"
                //string Images_Path = t.Find_Path("images") + t.getFileName(v.tFileName.setImageDPI) + "." + formatName; //"jpg"
                                
                t.DosyaVarsaSil(Images_Path);

                _img.Save(Images_Path, imageFormat_);// ImageFormat.Jpeg);

                resultImage = Image.FromFile(Images_Path);

                t.AlertMessage("Bilgilendirme", "DPI düzenlemesi yapılmıştır...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            //---

            #region başka bir yöntem
            /* 
            /// Burda gerçekleşen olay şu 
            /// bir resmi 96 dpi dan 300 dpi yüksekseltiğinde resmin width ve height de artması gerekiyor
            /// İşte burada bu büyümede gerçekleşiyor

            ///  300 > 96
            if (newDpi > horizontalResolation)
            {
               width = (int)((decimal)width * ((decimal)newDpi / (decimal)horizontalResolation));
               height = (int)((decimal)height * ((decimal)newDpi / (decimal)verticalResolation));
            }
                        
            /// yeni çizilen alan ölçüsü
            Rectangle rect = new Rectangle(0, 0, width, height);

            /// Original image
            Bitmap _img = new Bitmap(width, height);

            /// new dpi set
            _img.SetResolution(newDpi, newDpi);
                        
            /// for cropinf image
            Graphics g = Graphics.FromImage(_img);
            /// create graphics
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            
            /// set image attributes
            g.DrawImage(workingImage, 0, 0, rect, GraphicsUnit.Pixel);
            */
            #endregion

            return resultImage;

        }

        /// <summary>
        /// Resmin Quality ayarlarını değiştirir.
        /// </summary>
        public Image setImageQuality(Image inImage_, int newValue, ImageFormat imageFormat_)
        {
            Image resultImage = null;

            /// newValue 1 ile 100 arası olması gerekiyor
            ///   1 en düşük kalite
            /// 100 en yüksek kalite
            /// 

            ImageCodecInfo myImageCodecInfo;
            System.Drawing.Imaging.Encoder myEncoder;
            EncoderParameter myEncoderParameter = null;
            EncoderParameters myEncoderParameters = null;
            Bitmap workingImage = null;

            try
            {
                workingImage = new Bitmap(inImage_, inImage_.Width, inImage_.Height);

                /// önce resmin kalitesi yükseltilecek            
                myImageCodecInfo = TipBilgisi("image/jpeg");

                myEncoder = System.Drawing.Imaging.Encoder.Quality;

                myEncoderParameters = new EncoderParameters(1);

                // orjinali
                // myEncoderParameter = new EncoderParameter(myEncoder, 65L);
                myEncoderParameter = new EncoderParameter(myEncoder, (long)newValue);

                myEncoderParameters.Param[0] = myEncoderParameter;
                
                string formatName = getImageFormatName(imageFormat_);

                // Klasör oluşturulur 
                //string Images_Path = t.Find_Path("images") + t.getNewFileGuidName + ".jpg";
                string Images_Path = t.Find_Path("images") + t.getNewFileGuidName + "." + formatName;
                //string Images_Path = t.Find_Path("images") + t.getFileName(v.tFileName.setImageQuality) + "." + formatName;

                t.DosyaVarsaSil(Images_Path);

                workingImage.Save(Images_Path, myImageCodecInfo, myEncoderParameters);

                resultImage = Image.FromFile(Images_Path);

                //printImageProperties();

                t.AlertMessage("Bilgilendirme", "Resim kalitesini düzenleme çalışması yapılmıştır...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            workingImage.Dispose();
            myEncoderParameter.Dispose();
            myEncoderParameters.Dispose();

            return resultImage;
        }

        private ImageCodecInfo TipBilgisi(string mimeType)
        {
            //Üst metodun yardımcı metodu.
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        public Bitmap setImageCompress_(Bitmap workingImage, int newWidth, int newHeight)
        {
            Bitmap _img = new Bitmap(newWidth, newHeight, workingImage.PixelFormat);
            _img.SetResolution(workingImage.HorizontalResolution, workingImage.VerticalResolution);

            /// for new small image
            Graphics g = Graphics.FromImage(_img);
            /// create graphics
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            g.DrawImage(workingImage, 0, 0, newWidth, newHeight);

            return _img;
        }

        public Bitmap cropImage(Image oldImage, int x, int y, int width, int height, int newDpi)
        {

            //Rectangle rectDestination = new Rectangle(x, y, width, height);
            // kaynak okunacak alan
            Rectangle sourceRect = new Rectangle(x + 1, y + 1, width, height);
            // yeni resim çerçevesi
            Rectangle destinationRect = new Rectangle(0, 0, width, height);


            //Bitmap _newImage = new Bitmap(rectDestination.Width, rectDestination.Height);
            Bitmap _newImage = new Bitmap(sourceRect.Width, sourceRect.Height);
            _newImage.SetResolution(oldImage.HorizontalResolution, oldImage.VerticalResolution);

            Graphics gr = Graphics.FromImage(_newImage);
            gr.CompositingQuality = CompositingQuality.Default;
            gr.SmoothingMode = SmoothingMode.Default;
            gr.InterpolationMode = InterpolationMode.Bicubic;
            gr.PixelOffsetMode = PixelOffsetMode.Default;
            //gr.DrawImage(oldImage, new Rectangle(0, 0, _newImage.Width, _newImage.Height), rectDestination, GraphicsUnit.Pixel);
            //gr.DrawImage(oldImage, new Rectangle(0, 0, _newImage.Width, _newImage.Height), sourceRect, GraphicsUnit.Pixel);
            gr.DrawImage(oldImage, destinationRect, sourceRect, GraphicsUnit.Pixel);

            if (newDpi > 0)
                _newImage.SetResolution(newDpi, newDpi);

            return _newImage;
        }

        public Bitmap getImage(Image inImage_)
        {
            int oldWidth = inImage_.Width;
            int oldHeight = inImage_.Height;

            Bitmap resultImage = new Bitmap(inImage_, oldWidth, oldHeight);
            resultImage.SetResolution(inImage_.HorizontalResolution, inImage_.VerticalResolution);

            return resultImage;
        }

        public Bitmap getImage(Image inImage_, ref int oldWidth, ref int oldHeight)
        {
            oldWidth = inImage_.Width;
            oldHeight = inImage_.Height;

            Bitmap resultImage = new Bitmap(inImage_, oldWidth, oldHeight);
            resultImage.SetResolution(inImage_.HorizontalResolution, inImage_.VerticalResolution);

            return resultImage;
        }

        public void getBitmapProperties(Bitmap bitmap, ref vImageProperties tImageProperties_)
        {
            tImageProperties_.Clear();

            int horResolation = (int)bitmap.HorizontalResolution;  //pictureEdit1.Image.HorizontalResolution;
            int verResolation = (int)bitmap.VerticalResolution;    //pictureEdit1.Image.VerticalResolution;
            System.Drawing.Imaging.ImageFormat imageFormat = tGetImageFormat(bitmap.RawFormat);

            if (imageFormat == System.Drawing.Imaging.ImageFormat.MemoryBmp)
                imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;

            tImageProperties_.imageFormat = imageFormat;
            tImageProperties_.width = bitmap.Width;   // pictureEdit1.Image.Width;
            tImageProperties_.height = bitmap.Height; // pictureEdit1.Image.Height;
            tImageProperties_.horizontalResolation = horResolation; // pictureEdit1.Image.HorizontalResolution;
            tImageProperties_.verticalResolation = verResolation; // pictureEdit1.Image.VerticalResolution;
            long imageByteSize;
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, tImageProperties_.imageFormat);
                imageByteSize = ms.Length;
            }
            tImageProperties_.byteSize = imageByteSize;
            tImageProperties_.kbSize = (int)((decimal)imageByteSize / 1024);
        }
        private System.Drawing.Imaging.ImageFormat tGetImageFormat(ImageFormat RawFormat)
        {
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                return System.Drawing.Imaging.ImageFormat.Jpeg;
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                return System.Drawing.Imaging.ImageFormat.Bmp;
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                return System.Drawing.Imaging.ImageFormat.Png;
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Emf))
                return System.Drawing.Imaging.ImageFormat.Emf;
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Exif))
                return System.Drawing.Imaging.ImageFormat.Exif;
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                return System.Drawing.Imaging.ImageFormat.Gif;
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Icon))
                return System.Drawing.Imaging.ImageFormat.Icon;
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.MemoryBmp))
                return System.Drawing.Imaging.ImageFormat.MemoryBmp;
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Tiff))
                return System.Drawing.Imaging.ImageFormat.Tiff;
            else
                return System.Drawing.Imaging.ImageFormat.Wmf;
        }
        private System.Drawing.Imaging.ImageFormat tGetImageFormat(System.Drawing.Image img)
        {
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                return System.Drawing.Imaging.ImageFormat.Jpeg;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                return System.Drawing.Imaging.ImageFormat.Bmp;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                return System.Drawing.Imaging.ImageFormat.Png;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Emf))
                return System.Drawing.Imaging.ImageFormat.Emf;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Exif))
                return System.Drawing.Imaging.ImageFormat.Exif;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                return System.Drawing.Imaging.ImageFormat.Gif;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Icon))
                return System.Drawing.Imaging.ImageFormat.Icon;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.MemoryBmp))
                return System.Drawing.Imaging.ImageFormat.MemoryBmp;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Tiff))
                return System.Drawing.Imaging.ImageFormat.Tiff;
            else
                return System.Drawing.Imaging.ImageFormat.Wmf;
        }
        public string getImageFormatName(ImageFormat RawFormat)
        {
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                return "jpg";
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                return "bmp";
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                return "png";
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Emf))
                return "emf";
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Exif))
                return "exif";
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                return "gif";
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Icon))
                return "icon";
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.MemoryBmp))
                return "MemoryBmp";
            if (RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Tiff))
                return "tiff";
            else
                return "wmf";
        }
        public void getImageProperties(Image inImage_, ref vImageProperties tImageProperties_)
        {
            tImageProperties_.Clear();
            if (inImage_ == null) return;

            int horResolation = (int)inImage_.HorizontalResolution;
            int verResolation = (int)inImage_.VerticalResolution;
            System.Drawing.Imaging.ImageFormat imageFormat = tGetImageFormat(inImage_);

            if (imageFormat == System.Drawing.Imaging.ImageFormat.MemoryBmp)
                imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;

            tImageProperties_.imageFormat = imageFormat;
            tImageProperties_.width = inImage_.Width;
            tImageProperties_.height = inImage_.Height;
            tImageProperties_.horizontalResolation = horResolation; // pictureEdit1.Image.HorizontalResolution;
            tImageProperties_.verticalResolation = verResolation; // pictureEdit1.Image.VerticalResolution;
            long imageByteSize;
            using (var ms = new MemoryStream())
            {
                //pictureEdit1.Image.Save(ms, ImageFormat.Jpeg);
                inImage_.Save(ms, tImageProperties_.imageFormat);
                imageByteSize = ms.Length;
            }
            tImageProperties_.byteSize = imageByteSize;
            tImageProperties_.kbSize = (int)((decimal)imageByteSize / 1024);
        }


        public Image setImageSizeUnder100K(Image inImage_, int inDpi_, int inAutoCropHeight_, int inAutoCropWidth_, ImageFormat imageFormat_)
        {
            int oldWidth = 0;
            int oldHeight = 0;
            int newWidth = 0;
            int newHeight = 0;

            int farkValue = 0;
            int newValue = 0;

            int oldSize = 0;
            int newSize = 0;
            int farkSize = 0;

            bool onayCompress = false;

            vImageProperties tImageProperties = new vImageProperties();
            Image resultImage = null;

            /// inDpi ile resmin DPI uygun olmayabilir
            Image orginalImage = setImageDPI(inImage_, inDpi_, imageFormat_);
            getImageProperties(orginalImage, ref tImageProperties);
            
            /// resmin büyükğü nedir
            /// 
            oldSize = (int)tImageProperties.kbSize;

            if (oldSize > 100)
            {
                onayCompress = true;
            }
            else
            {
                return orginalImage;
            }

            Bitmap workingImage = null;
            Bitmap _img = null;

            if (onayCompress)
            {
                newValue = 300;
                farkValue = 10;

                t.WaitFormOpen(v.mainForm, "");
                t.WaitFormOpen(v.mainForm, "Resim 100 kb altına düşürülüyor...");

                while (onayCompress)
                {
                    workingImage = getImage(orginalImage, ref oldWidth, ref oldHeight);

                    preparingNewWidthNewHeight(newValue, oldWidth, oldHeight, ref newWidth, ref newHeight, inDpi_, inAutoCropHeight_, inAutoCropWidth_);

                    _img = setImageCompress_(workingImage, newWidth, newHeight);

                    getBitmapProperties(_img, ref tImageProperties);

                    newSize = (int)tImageProperties.kbSize;
                    farkSize = 100 - newSize; // 100 K altına düşmüşmü ?

                    if (farkSize >= 0 && farkSize <= 20 && newSize < 100)
                    {
                        resultImage = _img;
                        onayCompress = false; // işlem bitti
                    }
                    else
                    {
                        // yeni resim küçük, yalnız fazla küçülmüş 
                        if (farkSize > 20)
                        {
                            newValue = newValue - farkValue;
                            //farkValue -= 5;
                            //if (farkValue <= 0) farkValue = 10;

                            if (newValue < 10)
                            {
                                resultImage = _img;
                                onayCompress = false; // işlem bitti
                            }
                        }

                        // yeni resim halen 100 kd büyük
                        if (farkSize <= 0)
                        {
                            newValue = newValue + farkValue;
                            //farkValue += 10;

                            if (newValue > 5000)
                            {
                                resultImage = _img;
                                onayCompress = false; // işlem bitti
                            }
                        }
                    }
                }

                v.SP_OpenApplication = false;
                v.IsWaitOpen = false;
                t.WaitFormClose();

            }

            return resultImage;
        }

        public void preparingNewWidthNewHeight(int newPixel, int oldWidth, int oldHeight, ref int newWidth, ref int newHeight, int autoDPI, int autoCropHeight, int autoCropWidth)
        {
            /// Sıkıştıma işlemi için yeni newWidth ve newHeight hesaplanıyor
            /// 
            if (autoDPI == 400 && (autoCropHeight == oldHeight || autoCropWidth == oldWidth))
            {
                newWidth = oldWidth;
                newHeight = oldHeight;
                return;
            }

            Size yeni_boyut = new Size(-1, -1);
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            if ((oldWidth < 20) || (oldHeight < 20)) return;// string.Empty;

            if (newPixel < 10) newPixel = 10;

            yeni_boyut.Width = (oldWidth - newPixel);
            yeni_boyut.Height = (oldHeight - newPixel);

            nPercentW = ((float)yeni_boyut.Width / (float)oldWidth);
            nPercentH = ((float)yeni_boyut.Height / (float)oldHeight);

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
            }
            else
            {
                nPercent = nPercentW;
            }

            /// nPercent : küçültme oranı : 0.????
            /// bu sayede oldWidth küçülerek newWidth elde ediliyor

            /// tarayıcı tarafında kırıpılarak gelirse
            /// kullanıcı tarama sırasında önce önizleme yapıyor ve kırpılmış vaziyette tarama yaptırıyor ise
            /// 
            /// 600 dpi taranınca resim büyük geliyor
            if (autoDPI == 400 && (autoCropHeight != oldHeight || autoCropWidth != oldWidth))
            {
                int fark = 120;
                if (autoCropHeight == 512 && oldHeight > 512) fark = autoCropHeight + 140; // biyometrik için
                if (autoCropHeight == 472 && oldHeight > 472) fark = autoCropHeight + 10;  // imza için

                if (oldHeight > autoCropHeight && oldHeight < 2000)
                    nPercent = (float)(fark) / (float)oldHeight;  /// 650 / ??????
            }

            newWidth = (int)(oldWidth * nPercent);
            newHeight = (int)(oldHeight * nPercent);
        }

    }
}
