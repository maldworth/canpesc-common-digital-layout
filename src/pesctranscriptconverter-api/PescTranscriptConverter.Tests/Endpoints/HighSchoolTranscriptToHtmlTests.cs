using FluentAssertions;

namespace PescTranscriptConverter.Tests.Endpoints;

public class HighSchoolTranscriptToHtmlTests : IAsyncLifetime
{
    private DistributedApplication? _app;
    private PescTranscriptConverterClient? _apiClient;

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.PescTranscriptConverter_AppHost>();

        _app = await appHost.BuildAsync();
        await _app.StartAsync();
        _apiClient = new PescTranscriptConverterClient(_app.CreateHttpClient("api"));
    }

    public Task DisposeAsync()
    {
        _app?.Dispose();

        return Task.CompletedTask;
    }

    [Theory]
    [InlineData("Canada.Ontario.HighSchool.HighSchoolTranscript.xml", "en-CA", "<html")]
    [InlineData("Canada.Nova_Scotia.HighSchool.HighSchoolTranscript.xml", "en-CA", "<html")]
    [InlineData("Canada.Nova_Scotia.HighSchool.HighSchoolTranscript2.xml", "en-CA", "<html")]
    [InlineData("Canada.Nova_Scotia.HighSchool.HighSchoolTranscript3.xml", "en-CA", "<html")]
    public async Task Should_convert_highschool_pesc_to_html(string pescXml, string locale, string assertContains)
    {
        // Arrange
        var request = new HighSchoolTranscriptToHtmlRequest
        {
            Pesc = SampleHelper.ReadResourceAsString(pescXml),
            Locale = locale
        };

        // Act
        var response = await _apiClient!.HighSchoolTranscriptToHtmlAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Html.Should().Contain(assertContains);
    }
}
