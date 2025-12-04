using AutoMapper;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Book;
using LibraryManagementAPI.Models.BorrowRequest;
using LibraryManagementAPI.Models.Pagination;
using LibraryManagementAPI.Models.Train;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.ML;
using Microsoft.ML;

namespace LibraryManagementAPI.Services
{
    public class RecommendationService : IRecommendationService
    {
        const string CACHE_IDs_KEY = "BookIds";

        private readonly PredictionEnginePool<BookRating, BookRatingPrediction> _predictionEnginePool;
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        private List<Guid> bookIds;
        private readonly IBookRepository repository;

        public RecommendationService(
            PredictionEnginePool<BookRating, BookRatingPrediction> predictionEnginePool,
            IMemoryCache cache,
            IMapper mapper,
            IBookRepository repository)
        {
            _predictionEnginePool = predictionEnginePool;
            _cache = cache;
            _mapper = mapper;
            this.repository = repository;
        }

        public async Task<Response<List<BookDto>>> GetRecommendedBooksForUser(Guid memberId, int pageNumber = 1, int pageSize = 20)
        {
            // check cache first
            var cacheKey = $"RecommendedBooks_{memberId}_{pageNumber}_{pageSize}";
            if (_cache.TryGetValue(cacheKey, out Response<List<BookDto>>? cachedResponse))
            {
                if(cachedResponse != null)
                    return cachedResponse;
            }

            GetAllBookIds();
            var recommendations = new List<BookIdAndPrediction>();

            // danh gia rating cua moi cuon sach
            foreach (var bookId in bookIds)
            {

                var input = new BookRating
                {
                    MemberId = memberId.ToString(),
                    BookId = bookId.ToString()
                };

                // Preload the prediction engine pool
                var prediction = _predictionEnginePool.Predict(input);
                if (prediction != null)
                {
                    var res = new BookIdAndPrediction
                    {
                        bookId = bookId,
                        prediction = prediction
                    };
                    recommendations.Add(res);
                }
            }

            // Sap xep
            recommendations.Sort((p1, p2) => p2.prediction.Score.CompareTo(p1.prediction.Score));

            var fromIndex = (pageNumber - 1) * pageSize;
            var toIndex = pageNumber * pageSize;
            var numberOfBook = bookIds.Count;


            var bookIdsPage = new List<Guid>();
            for (int i = fromIndex; i < Math.Min(toIndex, numberOfBook); i++)
            {
                var id = recommendations[i].bookId;
                bookIdsPage.Add(id);
            }

            // lay thong tin sach
            var books = await repository.GetBooksAsync(bookIdsPage);
            var result = _mapper.Map<List<BookDto>>(books);
            // cache trong 30 phut
            var response = Response<List<BookDto>>.Success(result);
            _cache.Set(cacheKey, response, TimeSpan.FromMinutes(30));
            return response;
        }

        private void GetAllBookIds()
        {
            // lay id cua tat ca sach
            // kiem tra cache truoc
            if (_cache.TryGetValue(CACHE_IDs_KEY, out List<Guid>? bookIds))
            {
                if(bookIds != null)
                {
                    this.bookIds = bookIds;
                }    
            }
            // cache khong co thi goi cua repo
            else
            {
                this.bookIds = repository.GetAllBookIdsAsync().Result.ToList();
                _cache.Set(CACHE_IDs_KEY, this.bookIds);
            }
        }

        public BookRatingPrediction PredictSingle(string memberId, string bookId)
        {
            var input = new BookRating
            {
                MemberId = memberId,
                BookId = bookId
            };

            return _predictionEnginePool.Predict(input);
        }
    }
}
