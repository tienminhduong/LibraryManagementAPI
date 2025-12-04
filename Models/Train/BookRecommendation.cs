namespace LibraryManagementAPI.Models.Train
{
    public class BookRating
    {
        public string MemberId { get; set; }
        public string BookId { get; set; }
        public float Label { get; set; }  // Không bắt buộc khi predict
    }

    public class BookRatingPrediction
    {
        public float Label { get; set; }
        public float Score { get; set; }  // Đây là predicted rating score
    }

    public class BookIdAndPrediction
    {
        public Guid bookId { get; set; }
        public BookRatingPrediction prediction { get; set; }
    }
}
