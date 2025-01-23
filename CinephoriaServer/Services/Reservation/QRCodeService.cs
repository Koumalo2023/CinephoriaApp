using ZXing;
using ZXing.QrCode;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CinephoriaServer.Models.PostgresqlDb;

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
        {
            throw new ArgumentNullException(nameof(reservation));
        }

        // Créer les données à encoder dans le QR code
        string qrCodeData = $"ReservationId:{reservation.ReservationId};ShowtimeId:{reservation.ShowtimeId};UserId:{reservation.AppUserId}";

        // Configurer le writer pour générer un QR code
        var writer = new BarcodeWriter<Bitmap>
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Width = 300,  
                Height = 300, 
                Margin = 1 
            }
        };

        // Générer le QR code en tant qu'image Bitmap
        using (Bitmap bitmap = writer.Write(qrCodeData))
        using (MemoryStream stream = new MemoryStream())
        {
            // Convertir l'image en tableau de bytes (format PNG)
            bitmap.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }
    }
}