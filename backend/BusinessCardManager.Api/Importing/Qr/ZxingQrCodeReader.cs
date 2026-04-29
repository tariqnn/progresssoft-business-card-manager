using BusinessCardManager.Api.Importing.Exceptions;
using BusinessCardManager.Api.Importing.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using ZXing;
using ZXing.Common;

namespace BusinessCardManager.Api.Importing.Qr;

public class ZxingQrCodeReader : IQrCodeReader
{
    public async Task<string> ReadPayloadAsync(Stream imageStream, CancellationToken cancellationToken)
    {
        Image<Rgba32> image;

        try
        {
            image = await Image.LoadAsync<Rgba32>(imageStream, cancellationToken);
        }
        catch (Exception ex) when (ex is UnknownImageFormatException or InvalidImageContentException)
        {
            throw new BusinessCardImportException("QR upload must be a valid image file.", [ex.Message]);
        }

        using (image)
        {
            var pixels = new byte[image.Width * image.Height * 4];
            image.CopyPixelDataTo(pixels);

            var luminanceSource = new RGBLuminanceSource(
                pixels,
                image.Width,
                image.Height,
                RGBLuminanceSource.BitmapFormat.RGBA32);

            var reader = new BarcodeReaderGeneric
            {
                AutoRotate = true,
                Options = new DecodingOptions
                {
                    TryHarder = true,
                    TryInverted = true,
                    PossibleFormats = [BarcodeFormat.QR_CODE]
                }
            };

            var result = reader.Decode(luminanceSource);

            if (result is null || string.IsNullOrWhiteSpace(result.Text))
            {
                throw new BusinessCardImportException("No readable QR code was found in the uploaded image.");
            }

            return result.Text;
        }
    }
}
