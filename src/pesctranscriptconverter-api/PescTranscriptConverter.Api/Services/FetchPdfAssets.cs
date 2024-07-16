namespace PescTranscriptConverter.Api.Services;

public static class FetchPdfAssets
{
    public delegate Task<string> Header();
    public delegate Task<string> Footer();
}
