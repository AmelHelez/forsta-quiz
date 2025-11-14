namespace QuizService.Model;

public class QuizCreateModel
{
    public QuizCreateModel(string title)
    {
        Title = title;
    }

    // TODO: Add model validation
    public string Title { get; set; }
}