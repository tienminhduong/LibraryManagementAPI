using System.Text;
using System.Text.Json;

namespace LibraryManagementAPI.Models.Utility
{
    public interface IExternalRecommendationClient
    {
        Task<List<BookRatingContent>> GetRecommendedBookIdsForMemberAsync(Guid memberId, 
            int pageNumber, 
            int pageSize,
            float alpha,
            CancellationToken cancellationToken = default);
    }

    public class ExternalRecommendationClient : IExternalRecommendationClient
    {
        private readonly HttpClient _http;

        public ExternalRecommendationClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<BookRatingContent>> GetRecommendedBookIdsForMemberAsync(Guid memberId,
            int pageNumber,
            int pageSize,
            float alpha = 0.6f,
            CancellationToken cancellationToken = default)
        {
            var baseUrl = Environment.GetEnvironmentVariable("RECOMMENDER_API_URL");
            var requestUrl = $"{baseUrl}/models/recommend/hybrid"
                + $"?pageNumber={pageNumber}&pageSize={pageSize}" +
                $"&userId={memberId.ToString()}&alpha={alpha}";
            using var resp = await _http.GetAsync(requestUrl);
            resp.EnsureSuccessStatusCode();
            await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken);
            var ids = await JsonSerializer.DeserializeAsync<List<BookRatingContent>>(stream, cancellationToken: cancellationToken);
            return ids ?? new List<BookRatingContent>();
        }
    }

    public class RecommendRequest
    {
        public required string userId;
        public float alpha = 0.6f;
        public int pageNumber = 1;
        public int pageSize = 20;
    }

    public class BookRatingContent
    {
        public required string bookId { get; set; }
        public float score { get; set; }
    }
}
