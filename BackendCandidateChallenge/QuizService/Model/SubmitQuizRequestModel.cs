using System.Collections.Generic;

namespace QuizService.Model
{
    public class SubmitQuizRequestModel
    {
        public List<SubmitAnswer> Answers { get; set; } = [];
    }

    public class SubmitAnswer
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
    }
}
