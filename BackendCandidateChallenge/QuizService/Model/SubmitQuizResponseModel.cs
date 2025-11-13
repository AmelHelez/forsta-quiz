namespace QuizService.Model
{
    public class SubmitQuizResponseModel(int quizId, int total, int correctAnswers)
    {
        public int QuizId { get; set; } = quizId;
        public int TotalQuestions { get; set; } = total;
        public int Correct { get; set; } = correctAnswers;
        public int Score { get; set; } = correctAnswers;
        public double Percent => TotalQuestions == 0 ? 0 : (double)Correct / TotalQuestions * 100.0;
    }
}
