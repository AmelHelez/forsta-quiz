import { useEffect, useState } from "react";
import { getQuiz } from "../api/quizApi";
import { Link, useParams } from "react-router-dom";

export const QuizDetails = () => {
    const { id } = useParams();
    const [quiz, setQuiz] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function load() {
            try {
                const data = await getQuiz(id);
                setQuiz(data);
            } catch (error) {
                setError("Failed to load the quiz..");
            } finally {
                setLoading(false);
            }
        }
        
        load();
    }, [id]);

    if (loading) {
        return (
            <div className="container py-5">
                <p>Quiz is loading...</p>
            </div>
        )
    }

     if (error) {
        return (
            <div className="container py-5">
                <div className="alert alert-danger">
                    {error}
                </div>
            </div>
        )
    }

    if (!quiz) {
        return (
            <div className="container py-5">
                <p>Quiz not found.</p>
            </div>
        )
    }

    return (
        <div className="container py-5">
            <div className="mb-3">
                <Link to="/" className="btn btn-link p-0">
                    Back
                </Link>
            </div>

            <div className="card shadow-sm mb-4">
                <div className="card-body">
                    <h1 className="h3 mb-0">
                        {quiz.title}
                    </h1>
                    {quiz.id && (
                        <p className="text-muted mb-0">
                            Quiz ID: {quiz.id}
                        </p>
                    )}
                </div>
            </div>

            {quiz.questions && quiz.questions.length > 0 ? (
                <div className="accordion" id="questionsAccordion">
                    {quiz.questions.map((q, index) => (
                        <div className="accordion-item" key={q.id}>
                            <h2 className="accordion-header" id={`header-${q.id}`}>
                                <button
                                    className={`accordion-button ${
                                        index === 0 ? "" : "collapsed"
                                    }`}
                                    type="button"
                                    data-bs-toggle="collapse"
                                    data-bs-target={`#collapse-${q.id}`}
                                    aria-expanded={index === 0 ? "true" : "false"}
                                    aria-controls={`collapse-${q.id}`}
                                >
                                    {index + 1}. {q.text}
                                </button>
                            </h2>
                            <div
                                id={`collapse-${q.id}`}
                                className={`accordion-collapse collapse ${
                                index === 0 ? "show" : ""
                                }`}
                                aria-labelledby={`header-${q.id}`}
                                data-bs-parent="#questionsAccordion"
                            >
                                <div className="accordion-body">
                                    <ul className="list-group list-group-flush">
                                        {q.answers?.map((a) => (
                                            <li key={a.id} className="list-group-item">
                                                {a.text}
                                            </li>
                                        ))}
                                    </ul>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            ) : (
                <p>No questions for this quiz.</p>
            )}
        </div>
  );
}