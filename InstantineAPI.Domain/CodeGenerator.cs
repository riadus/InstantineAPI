using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using InstantineAPI.Core.Domain;
using ZXing;

namespace InstantineAPI.Domain
{
    public class CodeGenerator : ICodeGenerator
    {

        public byte[] GenerateImageFromCode(string code)
        {
            BarcodeWriterPixelData writer = new BarcodeWriterPixelData()
            {
                Format = BarcodeFormat.QR_CODE
            };
            var pixelData = writer.Write(code);

            using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
            {
                using (var ms = new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                    try
                    {
                        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }

                    bitmap.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }
    }
}