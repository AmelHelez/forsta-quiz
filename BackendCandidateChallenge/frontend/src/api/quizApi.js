const API_URL = "http://localhost:5000/api/quizzes";

export const getQuizzes = async () => {
    const response = await fetch(API_URL);
    if (!response.ok) {
        throw new Error("Cannot fetch quizzes..");
    }

    return response.json();
}

export const getQuiz = async (id) => {
    const response = await fetch(`${API_URL}/${id}`);
    if (!response.ok) {
        throw new Error("Failed to retrieve the quiz");
    }

    return response.json();
}