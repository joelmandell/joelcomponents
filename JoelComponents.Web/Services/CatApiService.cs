namespace JoelComponents.Services;

using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;

// Use a record for the simple data structure from the API
public record CatImage(
    [property: JsonPropertyName("url")] string Url
);

public class CatApiService(IHttpClientFactory factory) {

    public async Task<string> GetImage()
    {
        try
        {
            using var client = factory.CreateClient();
    
            var jsonResponse = await client.GetStringAsync("https://api.thecatapi.com/v1/images/search");
    
            // 2. Deserialize the JSON to get the image URL
            var images = JsonSerializer.Deserialize<List<CatImage>>(jsonResponse);
            if (images == null || images.Count == 0 || string.IsNullOrEmpty(images[0].Url))
            {
                Console.WriteLine("Could not find a cat image URL in the response.");
                return "";
            }
    
            var imageUrl = images[0].Url;
            Console.WriteLine($"Image URL found: {imageUrl}");
    
            return imageUrl;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message} 😿");
        }

        return "";
    }

}