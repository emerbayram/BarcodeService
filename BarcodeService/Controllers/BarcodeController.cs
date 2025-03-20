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
        /// Generate barcode image from parameters
        /// </summary>
        /// <param name="data"></param>
        /// <param name="code"></param>
        /// <param name="rotate"></param>
        /// <param name="pureBarcode"></param>
        /// <returns></returns>
        [HttpGet("GenerateBarcode")]
        public IActionResult GenerateBarcode([FromQuery] string data, [FromQuery] string code = "code128", [FromQuery] int rotate = 0, [FromQuery] bool pureBarcode = false, [FromQuery] int width=400, [FromQuery] int height=200)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return BadRequest("Barcode value is required");
            }

            if (width <= 0 || height <= 0)
            {
                return BadRequest("Width and height must be positive values.");
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
                        PureBarcode = pureBarcode,
                        Height = height,
                        Width = width,
                        Margin = 1,
                        NoPadding=true
                    },
                    Renderer = new BitmapRenderer()
                };

                Bitmap bitmap = barcodeWriter.Write(data);
                

                switch (rotate)
                {
                    case 90:
                        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 180:
                        bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 270:
                        bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                    default:
                        bitmap.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                        break;
                }

                using (var ms = new MemoryStream())
                {


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
