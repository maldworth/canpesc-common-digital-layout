﻿using System.Xml.Xsl;
using FastEndpoints;
using FluentStorage.Blobs;
using Microsoft.Extensions.Options;
using PescTranscriptConverter.Api.Config;

namespace PescTranscriptConverter.Api.HostedServices;

internal sealed class XsltLoadWorker(
    IServiceProvider serviceProvider,
    IOptions<CdlAssetsOptions> options,
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
        var cdlStorage = scope.TryResolve<IBlobStorage>();

        if (cdlStorage is null)
        {
            logger.LogWarning("Blob storage connection string was not set. XSLT Defaults will be used.");
            return;
        }

        var blobs = await cdlStorage.ListAsync(recurse: true, cancellationToken: cancellationToken);
        foreach (var blob in blobs)
        {
            var blobPath = $"{options.Value.RootDirectory}{blob.FullPath}";
            if (blob.IsFolder)
            {
                Directory.CreateDirectory(blobPath);
            }
            else if (blob.IsFile)
            {
                await cdlStorage.ReadToFileAsync(blob.FullPath, blobPath, cancellationToken);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
