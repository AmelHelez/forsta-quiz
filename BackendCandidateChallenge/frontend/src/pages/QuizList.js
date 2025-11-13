import { useEffect, useState } from "react"
import { getQuizzes } from "../api/quizApi";
import { useNavigate } from "react-router-dom";

export const QuizList = () => {
    const [quizzes, setQuizzes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const navigate = useNavigate();

    useEffect(() => {
        async function load() {
            try {
                const data = await getQuizzes();
                setQuizzes(data);
            } catch (error) {
                setError("Failed to load quizzes..");
            } finally {
                setLoading(false);
            }
        }

        load();
    }, []);

    if (loading) {
        return (
            <div className="container py-5">
                <p>Loading quizzes...</p>
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

     return (
        <div className="container py-5">
            <h1 className="h3 mb-4">Quiz List</h1>
            <div className="row">
                {quizzes.map((quiz) => (
                    <div key={quiz.id} className="col-md-6 col-lg-4 mb-4">
                        <div
                            className="card h-100 shadow-sm quiz-card"
                            onClick={() => navigate(`/quizzes/${quiz.id}`)}
                            style={{
                                cursor: "pointer",
                                transition: "transform 0.15s ease, box-shadow 0.15s ease",
                            }}
                            onMouseEnter={(e) => {
                                e.currentTarget.style.transform = "translateY(-4px)";
                                e.currentTarget.style.boxShadow =
                                "0 8px 16px rgba(0,0,0,0.15)";
                            }}
                            onMouseLeave={(e) => {
                                e.currentTarget.style.transform = "translateY(0)";
                                e.currentTarget.style.boxShadow =
                                "0 2px 6px rgba(0,0,0,0.1)";
                            }}
                        >
                            <div className="card-body">
                                <h5 className="card-title">
                                    {quiz.title}
                                </h5>
                                <p className="card-text text-muted mb-0">
                                    Click to view details.
                                </p>
                            </div>
                        </div>
                    </div>
                ))}
            </div>

            {quizzes.length === 0 && <p>No quizzes found.</p>}
        </div>
  );
}