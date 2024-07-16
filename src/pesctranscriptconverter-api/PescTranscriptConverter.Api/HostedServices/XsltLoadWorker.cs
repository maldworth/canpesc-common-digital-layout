using System.Xml.Xsl;
using FastEndpoints;
using FluentStorage.Blobs;

namespace PescTranscriptConverter.Api.HostedServices;

internal sealed class XsltLoadWorker(
    IServiceProvider serviceProvider,
    ILogger<XsltLoadWorker> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        await TryFetchXsltFromStorage(scope, cancellationToken);

        scope.Resolve<XslCompiledTransform>("CollegeTranscript");

        scope.Resolve<XslCompiledTransform>("HighSchoolTranscript");
    }

    private async Task TryFetchXsltFromStorage(IServiceScope scope, CancellationToken cancellationToken)
    {
        var blobStorage = scope.TryResolve<IBlobStorage>();

        if (blobStorage is null)
        {
            logger.LogWarning("Blob storage connection string was not set. XSLT Defaults will be used.");
            return;
        }

        var blobs = await blobStorage.ListAsync(recurse: true, cancellationToken: cancellationToken);

        foreach (var blob in blobs)
        {
            if (blob.IsFolder)
            {
                Directory.CreateDirectory($"./cdl-assets{blob.FullPath}");
            }
            else if (blob.IsFile)
            {
                await blobStorage.ReadToFileAsync(blob.FullPath, $"./cdl-assets{blob.FullPath}", cancellationToken);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
