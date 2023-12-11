import { Routes, Route } from "react-router-dom"
import { Container } from "react-bootstrap"
import { Home } from "./pages/Home"
import { Login } from "./pages/Login"
import { Registration } from "./pages/Registration"
import { Navbar } from "./components/Navbar"
import { Upload } from "./pages/Upload"

function App() {
  return (
    <>
    <Navbar />
   <Container className="mb-4">
    <Routes>
      <Route path="/Home" element={<Home />} />
      <Route path="" element={<Login />} />
      <Route path="/Registration" element ={<Registration />} />
      <Route path="/Upload" element ={<Upload />} />
    </Routes>
   </Container>
   </>
  )
}
export default App
