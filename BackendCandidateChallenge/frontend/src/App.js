import './App.css';
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { QuizList } from './pages/QuizList';
import { QuizDetails } from './pages/QuizDetails';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<QuizList />} />
        <Route path="/quizzes/:id" element={<QuizDetails />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
