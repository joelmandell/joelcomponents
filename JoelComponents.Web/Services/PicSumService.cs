namespace JoelComponents.Services;

public class PicSumService(IHttpClientFactory factory)
{
    public async Task<string> GetImage()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("https://picsum.photos/800/600");
        var mimeType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";

        var imageBytes = await response.Content.ReadAsByteArrayAsync();
        var base64String = Convert.ToBase64String(imageBytes);

        return $"data:{mimeType};base64,{base64String}";
    }
}