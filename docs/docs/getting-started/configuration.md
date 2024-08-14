---
sidebar_position: 2
---

# Configuration

This API provides 2 primary functionalities.

**PESC to HTML** - Provide a pesc xml transcript and transform into a standard html transcript layout. This can be rendered in
a Website UI and localized for any language.

**PESC to PDF** - Takes the html output, and performs an additional step of rendering into a PDF to be viewed, download, and/or printed.

## PESC CDL API (to html)

CDL (Common Digital Layout) is an agreed upon style for displaying PESC xml as a transcript. This services primary functionality is taking the
PESC XML and transforming it into an html document. This docker service provides the extensibility to fully customize the transform and html
styles.

Assets blob storage, support for local (with volume mount), or azure, or amazon, or gcp

// Move the pdf stuff from CdlAssets into PdfAssets?

### Environment Variables

| Name                                              | Default Value   | Description                                                                                |
| ------------------------------------------------- | --------------- | ------------------------------------------------------------------------------------------ |
| CdlAssets__RootDirectory                          | ./cdl           | test                                                                                       |
| ConnectionStrings__cdlstorage                          | ./cdl           | test                                                                                       |


## PDF conversion (to pdf)

1. CdlAssets - this controls the directory where all the xslt, localized translations, pdf header/footers, and css styling is for the HTML and PDF generation.
2. [GotenbergSharpClient](https://github.com/ChangemakerStudios/GotenbergSharpApiClient) - this is the client used to initiate conversion from Html to Pdf. Pdf
conversion is an optin in feature, but if you use it, there's some additional configuration available. Leaving these values as the default is suitable for most cases. Additionally, Gotenberg container itself has [configuration](https://gotenberg.dev/docs/configuration) as well.

| Name                                              | Default Value   | Description                                                                                |
| ------------------------------------------------- | --------------- | ------------------------------------------------------------------------------------------ |
| CdlAssets__PdfHeader                              | pdf/header.html | test                                                                                       |
| CdlAssets__PdfFooter                              | pdf/footer.html | test                                                                                       |
| GotenbergSharpClient__ServiceUrl                  | null            | Set the url where the gotenberg api is available                                           |
| GotenbergSharpClient__HealthCheckUrl              | null            | Set the health check url where the gotenberg api is available                              |
| GotenbergSharpClient__RetryPolicy__Enabled        | true            | Enable [Polly](https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory) retry. |
| GotenbergSharpClient__RetryPolicy__RetryCount     | 4               | Set the retry count.                                                                       |
| GotenbergSharpClient__RetryPolicy__BackoffPower   | 1.5             | Set the factor for exponential backoff on the retries                                      |
| GotenbergSharpClient__RetryPolicy__LoggingEnabled | true            | Enable logging for polly.                                                                  |

