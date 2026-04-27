namespace BusinessCardManager.Api.Validators;

public class BusinessCardValidator : IBusinessCardValidator
{
    private const int MaxPhotoBytes = 1 * 1024 * 1024;

    public bool IsValidBase64Photo(string? photoBase64, out string? error)
    {
        error = null;

        if (string.IsNullOrWhiteSpace(photoBase64))
        {
            return true;
        }

        try
        {
            var commaIndex = photoBase64.IndexOf(',');
            var base64Payload = commaIndex >= 0 ? photoBase64[(commaIndex + 1)..] : photoBase64;
            var bytes = Convert.FromBase64String(base64Payload);

            if (bytes.Length > MaxPhotoBytes)
            {
                error = "Photo size must not exceed 1MB.";
                return false;
            }

            return true;
        }
        catch (FormatException)
        {
            error = "Photo must be a valid Base64 string.";
            return false;
        }
    }
}
