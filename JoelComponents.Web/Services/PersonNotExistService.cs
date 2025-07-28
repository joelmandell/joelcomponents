namespace JoelComponents.Services;

public class PersonNotExistService(IHttpClientFactory factory)
{
    public async Task<string> GetImage()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("https://thispersondoesnotexist.com");
        var mimeType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";

        var imageBytes = await response.Content.ReadAsByteArrayAsync();
        var base64String = Convert.ToBase64String(imageBytes);

        return $"data:{mimeType};base64,{base64String}";
    }
}