namespace LibraryManagementAPI.Extensions;

public static class BookCopyExtensions
{
    /// <summary>
    /// Generates a permanent QR code string for a BookCopy based on its ID.
    /// This is deterministic - the same BookCopyId will always produce the same QR code.
    /// </summary>
    public static string GenerateQrCode(this Guid bookCopyId)
    {
        // Option 1: Simple format with prefix
        return $"COPY-{bookCopyId}";
        
        // Option 2: Base64 encoded (shorter, still reversible)
        // var bytes = bookCopyId.ToByteArray();
        // return $"BC-{Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_')}";
        
        // Option 3: With timestamp hash for extra validation (still deterministic)
        // return $"COPY-{bookCopyId:N}";  // N format removes hyphens
    }
    
    /// <summary>
    /// Extracts the BookCopyId from a QR code string.
    /// </summary>
    public static Guid? ExtractBookCopyId(string qrCode)
    {
        if (string.IsNullOrWhiteSpace(qrCode))
            return null;
            
        try
        {
            // Option 1: Simple format
            if (qrCode.StartsWith("COPY-"))
            {
                var idString = qrCode.Substring(5);
                return Guid.Parse(idString);
            }
            
            // Option 2: Base64 format
            // if (qrCode.StartsWith("BC-"))
            // {
            //     var base64 = qrCode.Substring(3).Replace('-', '+').Replace('_', '/');
            //     var padding = (4 - base64.Length % 4) % 4;
            //     base64 += new string('=', padding);
            //     var bytes = Convert.FromBase64String(base64);
            //     return new Guid(bytes);
            // }
            
            return null;
        }
        catch
        {
            return null;
        }
    }
}
