using ZXing;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CinephoriaServer.Models.PostgresqlDb;
using ZXing.Common;



namespace CinephoriaServer.Services
{

}
public class QRCodeService
{
    /// <summary>
    /// Génère un QRCode pour une réservation spécifique.
    /// </summary>
    /// <param name="reservation">La réservation pour laquelle générer le QRCode.</param>
    /// <returns>Le QRCode sous forme de tableau de bytes (image).</returns>
    public byte[] GenerateQRCode(Reservation reservation)
    {
        if (reservation == null)
            throw new ArgumentNullException(nameof(reservation));

        var qrCodeData = $"ReservationId:{reservation.ReservationId};ShowtimeId:{reservation.ShowtimeId};AppUserId:{reservation.AppUserId}";

        var writer = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new EncodingOptions
            {
                Width = 300,
                Height = 300,
                Margin = 1
            }
        };

        var pixelData = writer.Write(qrCodeData);

        using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
        {
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }
    }
}