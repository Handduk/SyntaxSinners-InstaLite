import { Container, Nav, Navbar as NavbarBs } from
"react-bootstrap"
import { NavLink } from "react-router-dom"
import Dropdown from "react-bootstrap/Dropdown"



export function Navbar (){
    return(
        <NavbarBs sticky="top" className="bg-dark shadow-lg mb-3">
        <Container>
            <Nav className="d-flex" style={{width:"100%"}}>
                <Nav.Link className="link-light me-4" to="/Home" as={NavLink}>
                    <img src="../public/images/InstaLite.png"
                     alt="instaLite"
                    style={{width: 70}}/>
                </Nav.Link>
                <Nav.Link className="link-light flex-grow-1" to="/Home" as={NavLink}
                style={{fontSize: 40}}>
                    InstaLite
                </Nav.Link>
                <Nav.Link className="link-light me-4" to="/Upload" as={NavLink}
                style={{fontSize: 40}}>
                    Upload
                </Nav.Link>
            </Nav>

            <Dropdown>
                <Dropdown.Toggle variant="success" id="dropdown-basic">
                  Username
                </Dropdown.Toggle>
                <Dropdown.Menu>
                    {/**Ej implementerade sidor*/}
                  <Dropdown.Item href="#/Profile">Profile</Dropdown.Item>
                  <Dropdown.Item href="#/Settings">Settings</Dropdown.Item>
                  <Dropdown.Item href="#/onclicklogout">Log out</Dropdown.Item>
                </Dropdown.Menu>
            </Dropdown>
        </Container>
    </NavbarBs>
    )
}