using System.Xml;
using System.Xml.Xsl;
using FluentStorage;
using Gotenberg.Sharp.API.Client;
using Gotenberg.Sharp.API.Client.Domain.Settings;
using Gotenberg.Sharp.API.Client.Extensions;
using PescTranscriptConverter.Api.Services;

namespace PescTranscriptConverter.Api;

public static partial class ProgramExtensions
{
    public static void AddCustomFluentStorage(this WebApplicationBuilder builder)
    {
        var blobConnectionString = builder.Configuration.GetConnectionString("blobstorage");

        if (string.IsNullOrWhiteSpace(blobConnectionString))
        {
            return;
        }

        switch (blobConnectionString)
        {
            case var s when s.StartsWith("disk", StringComparison.OrdinalIgnoreCase):
                break;
            case var s when s.StartsWith("aws.s3", StringComparison.OrdinalIgnoreCase):
                StorageFactory.Modules.UseAwsStorage();
                break;
            case var s when s.StartsWith("google.storage", StringComparison.OrdinalIgnoreCase):
                StorageFactory.Modules.UseGoogleCloudStorage();
                break;
            case var s when s.StartsWith("azure.file", StringComparison.OrdinalIgnoreCase):
                StorageFactory.Modules.UseAzureFilesStorage();
                break;
            case var s when s.StartsWith("azure.blob", StringComparison.OrdinalIgnoreCase):
                StorageFactory.Modules.UseAzureBlobStorage();
                break;
            default:
                throw new Exception("Please use a supported blob storage connection string. disk://, aws.s3://, google.storage://, azure.file://, azure.blob://");
        }

        builder.Services.AddTransient(s => StorageFactory.Blobs.FromConnectionString(blobConnectionString));
    }

    public static void AddCustomApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddKeyedSingleton("CollegeTranscript", (p, _) =>
        {
            var transform = new XslCompiledTransform(true);
            XmlUrlResolver resolver = new XmlUrlResolver();
            XsltSettings settings = new XsltSettings(true, false);

            transform.Load("./cdl-assets/CollegeTranscript.xsl", settings, new XmlUrlResolver());
            return transform;
        });

        builder.Services.AddKeyedSingleton("HighSchoolTranscript", (p, _) =>
        {
            var transform = new XslCompiledTransform(true);
            XmlUrlResolver resolver = new XmlUrlResolver();
            XsltSettings settings = new XsltSettings(true, false);

            transform.Load("./cdl-assets/HighSchoolTranscript.xsl", settings, new XmlUrlResolver());
            return transform;
        });

        var gotenbergConfig = builder.Configuration.GetSection(nameof(GotenbergSharpClient));

        if (gotenbergConfig.Get<GotenbergSharpClientOptions>() is not null)
        {
            builder.Services.AddOptions<GotenbergSharpClientOptions>()
                .Bind(gotenbergConfig);
            builder.Services.AddGotenbergSharpClient();
        }

        builder.Services.AddSingleton<FetchPdfAssets.Footer>(s => () => File.ReadAllTextAsync("./cdl-assets/pdf/footer.html"));
        builder.Services.AddSingleton<FetchPdfAssets.Header>(s => () => File.ReadAllTextAsync("./cdl-assets/pdf/header.html"));
    }
}
