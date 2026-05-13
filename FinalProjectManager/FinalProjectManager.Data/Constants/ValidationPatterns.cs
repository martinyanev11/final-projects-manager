namespace FinalProjectManager.Data.Constants;

public static class ValidationPatterns
{
    // Cyrillic and Latin letters, spaces and hyphens — for name fields
    public const string BulgarianName = @"^[A-Za-zА-Яа-я\s\-]+$";

    // Cyrillic and Latin letters, spaces, digits and common punctuation — for required text fields
    public const string BulgarianText = @"^[A-Za-zА-Яа-я\s\d\-,.!?()]+$";

    // Same but allows empty value — for optional text fields
    public const string BulgarianTextOptional = @"^[A-Za-zА-Яа-я\s\d\-,.!?()]*$";

    public const string BulgarianNameMessage = "Полето приема само букви.";
    public const string BulgarianTextMessage = "Полето приема само валиден текст.";
}
