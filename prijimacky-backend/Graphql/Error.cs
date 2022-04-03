using System.Diagnostics.CodeAnalysis;

namespace prijimacky_backend.Graphql;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class GraphQLErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        return error.WithMessage(error.Exception?.Message ?? "An error occurred while processing the query.");
    }
}