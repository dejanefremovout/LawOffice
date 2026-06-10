namespace LawOffice.AI.Retrieval;

/// <summary>
/// The end-to-end Retrieval-Augmented Generation seam: broad tenant-scoped retrieval → rerank to a tight
/// top-K → grounded, contract-validated answer. One call that ties all the pieces together so a
/// caller never wires retrieval and the assistant by hand.
/// </summary>
public interface IRagPipeline
{
    /// <summary>
    /// Answers <paramref name="question"/> from <paramref name="officeId"/>'s documents only. The tenant
    /// is the caller's authenticated identity and is never derived from the question or model output.
    /// </summary>
    Task<RagResult> AskAsync(string officeId, string question, CancellationToken cancellationToken = default);
}
