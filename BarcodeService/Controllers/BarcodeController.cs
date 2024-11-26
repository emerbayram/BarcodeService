using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Windows.Compatibility;



namespace BarcodeService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BarcodeController : Controller
    {
        /// <summary>
        /// Generate barcode image from data 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("GenerateBarcode")]
        public IActionResult GenerateBarcode([FromQuery] string data, [FromQuery] string code = "code128")
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return BadRequest("Barcode value is required");
            }

            try
            {
                // default code128
                BarcodeFormat barcodeFormat = code.ToLower() switch
                {
                    "code128" => BarcodeFormat.CODE_128,
                    "ean13" => BarcodeFormat.EAN_13,
                    "qrcode" => BarcodeFormat.QR_CODE,
                    _ => BarcodeFormat.CODE_128
                };
                var barcodeWriter = new BarcodeWriter<Bitmap>
                {
                    Format = barcodeFormat, 
                    Options = new ZXing.Common.EncodingOptions
                    {
                        PureBarcode = false,
                        Height = 150, 
                        Width = 300,  
                        Margin = 10   
                    },
                    Renderer = new BitmapRenderer()
                };

                Bitmap bitmap = barcodeWriter.Write(data);
                using (var ms = new MemoryStream())
                {
                    //bitmap.RotateFlip(RotateFlipType.Rotate270FlipX); //this can be taken as a parameter
                    bitmap.Save(ms, ImageFormat.Png);
                    var base64String = Convert.ToBase64String(ms.ToArray()); 
                    return File(ms.ToArray(), "image/png");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"One or more error occured: {ex.Message}");                
            }
        }
                
    }
}
