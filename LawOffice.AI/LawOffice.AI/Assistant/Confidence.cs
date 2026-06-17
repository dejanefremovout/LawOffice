namespace LawOffice.AI.Assistant;

/// <summary>
/// The model's self-reported confidence in an answer. A closed set (rather than a free 0..1 score)
/// because a small enum is schema-enforceable, trivial to validate, and avoids uncalibrated
/// false-precision like "0.73".
/// </summary>
public enum Confidence
{
    Low,
    Medium,
    High,
}
