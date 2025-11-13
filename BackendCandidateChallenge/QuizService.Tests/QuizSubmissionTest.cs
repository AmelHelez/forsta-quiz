using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using QuizService.Model;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace QuizService.Tests
{
    public class QuizSubmissionTest
    {
        const string QuizApiEndPoint = "/api/quizzes/";

        [Fact]
        public async Task SubmitTwoQuestions_OneCorrect()
        {
            var quiz = new QuizCreateModel("New quiz");
            using (var testHost = new TestServer(new WebHostBuilder()
                  .UseStartup<Startup>()))
            {
                var client = testHost.CreateClient();
                var json = JsonConvert.SerializeObject(quiz);
                var content = new StringContent(JsonConvert.SerializeObject(quiz));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"),
                    content);
                
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                var createdQuizLocation = response.Headers.Location;
                Assert.NotNull(createdQuizLocation);

                var getQuiz = await client.GetAsync(createdQuizLocation);
                getQuiz.EnsureSuccessStatusCode();

                var createdQuiz = JsonConvert.DeserializeObject<QuizResponseModel>(await getQuiz.Content.ReadAsStringAsync());
                var quizId = createdQuiz.Id;

                var questionsPath = $"{QuizApiEndPoint}{quizId}/questions";
                var q1 = new QuestionCreateModel("1+1=?");
                var q2 = new QuestionCreateModel("Current month?");
                var q1Content = new StringContent(JsonConvert.SerializeObject(q1), Encoding.UTF8, "application/json");
                var q2Content = new StringContent(JsonConvert.SerializeObject(q2), Encoding.UTF8, "application/json");
                var q1Response = await client.PostAsync(questionsPath, q1Content);
                var q2Response = await client.PostAsync(questionsPath, q2Content);
                
                Assert.Equal(HttpStatusCode.Created, q1Response.StatusCode);
                Assert.Equal(HttpStatusCode.Created, q2Response.StatusCode);

                var quizData = await client.GetAsync($"{QuizApiEndPoint}{quizId}");
                quizData.EnsureSuccessStatusCode();
                var quizModel = JsonConvert.DeserializeObject<QuizResponseModel>(await quizData.Content.ReadAsStringAsync());

                var questionOne = quizModel.Questions.ElementAt(0);
                var questionTwo = quizModel.Questions.ElementAt(1);

                var a11 = new AnswerCreateModel("2");
                var a12 = new AnswerCreateModel("3");
                var a21 = new AnswerCreateModel("November");
                var a22 = new AnswerCreateModel("December");

                async Task<HttpResponseMessage> AddAnswer(int qId, AnswerCreateModel model)
                {
                    var cont = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                    return await client.PostAsync($"{questionsPath}/{qId}/answers", cont);
                }

                Assert.Equal(HttpStatusCode.Created, (await AddAnswer(questionOne.Id, a11)).StatusCode);
                Assert.Equal(HttpStatusCode.Created, (await AddAnswer(questionOne.Id, a12)).StatusCode);
                Assert.Equal(HttpStatusCode.Created, (await AddAnswer(questionTwo.Id, a21)).StatusCode);
                Assert.Equal(HttpStatusCode.Created, (await AddAnswer(questionTwo.Id, a22)).StatusCode);

                var quizWithAnswersAsync = await client.GetAsync($"{QuizApiEndPoint}{quizId}");
                quizWithAnswersAsync.EnsureSuccessStatusCode();
                var quizWithAnswers = JsonConvert.DeserializeObject<QuizResponseModel>(await quizWithAnswersAsync.Content.ReadAsStringAsync());
                var q1Answers = quizWithAnswers.Questions.First(q => q.Id == questionOne.Id).Answers.ToList();
                var q2Answers = quizWithAnswers.Questions.First(q => q.Id == questionTwo.Id).Answers.ToList();
                var q1CorrectAnswerId = q1Answers.First(q => q.Text == "2").Id;
                var q2CorrectAnswerId = q2Answers.First(q => q.Text == "November").Id;

                var q1SetCorrect = new QuestionUpdateModel { Text = "1+1=?", CorrectAnswerId = q1CorrectAnswerId };
                var q2SetCorrect = new QuestionUpdateModel { Text = "Current month?", CorrectAnswerId = q2CorrectAnswerId };

                var q1Update = await client.PutAsync($"{questionsPath}/{questionOne.Id}",
                    new StringContent(JsonConvert.SerializeObject(q1SetCorrect), Encoding.UTF8, "application/json"));
                var q2Update = await client.PutAsync($"{questionsPath}/{questionTwo.Id}",
                    new StringContent(JsonConvert.SerializeObject(q2SetCorrect), Encoding.UTF8, "application/json"));

                Assert.Equal(HttpStatusCode.NoContent, q1Update.StatusCode);
                Assert.Equal(HttpStatusCode.NoContent, q2Update.StatusCode);

                var submitAnswers = new SubmitQuizRequestModel
                {
                    Answers =
                    {
                        new SubmitAnswer
                        {
                            QuestionId = questionOne.Id,
                            AnswerId = q1CorrectAnswerId
                        },
                        new SubmitAnswer
                        {
                            QuestionId = questionTwo.Id,
                            AnswerId = -1
                        }
                    }
                };

                var submitData = await client.PostAsync(
                    $"{QuizApiEndPoint}{quizId}/submit",
                    new StringContent(JsonConvert.SerializeObject(submitAnswers), Encoding.UTF8, "application/json"));
                submitData.EnsureSuccessStatusCode();

                var submitResponse = JsonConvert.DeserializeObject<SubmitQuizResponseModel>(
                    await submitData.Content.ReadAsStringAsync());

                Assert.Equal(2, submitResponse.TotalQuestions);
                Assert.Equal(1, submitResponse.Correct);
                Assert.Equal(1, submitResponse.Score);
            }
        }
    }
}
