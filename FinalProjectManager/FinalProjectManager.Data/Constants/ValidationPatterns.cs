namespace FinalProjectManager.Data.Constants;

public static class ValidationPatterns
{
    // Cyrillic letters, spaces and hyphens — for name fields
    public const string BulgarianName = @"^[А-Яа-я\s\-]+$";

    // Cyrillic letters, spaces, digits and common punctuation — for required text fields
    public const string BulgarianText = @"^[А-Яа-я\s\d\-,.!?()]+$";

    // Same but allows empty value — for optional text fields
    public const string BulgarianTextOptional = @"^[А-Яа-я\s\d\-,.!?()]*$";

    public const string BulgarianNameMessage = "This field only accepts Bulgarian (Cyrillic) letters.";
    public const string BulgarianTextMessage = "This field only accepts Bulgarian (Cyrillic) text.";
}
