import React, { useState } from 'react';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import { loginUser } from '../services/instaLiteService';
import { LoginModel } from '../components/Models/loginModel';
import { useNavigate } from 'react-router-dom';

export function Login() {
  const [loginData, setLoginData] = useState<LoginModel>({
    username: '',
    password: '',
  });

  const navigate = useNavigate();

  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = event.target;
    setLoginData((prevData) => ({ ...prevData, [name]: value }));
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    try {
      const response = await loginUser(loginData);
      if (response){
        navigate('/Home');
      }
      window.alert("Logged in!");
      console.log(response);
    } catch (error) {
      console.error('Login failed:', error);
    }
  };

  return (
    <Form onSubmit={handleSubmit} className='w-25 mt-5 mx-auto'>
      <Form.Group className="mb-3 mx-auto" controlId="formBasicEmail">
        <Form.Label>Username</Form.Label>
        <Form.Control
          type="username"
          placeholder="Username"
          name="username"
          value={loginData.username}
          onChange={handleInputChange}
        />

      </Form.Group>
      <Form.Group className="mb-3" controlId="formBasicPassword">
        <Form.Label>Password</Form.Label>
        <Form.Control
          type="password"
          placeholder="Password"
          name="password"
          value={loginData.password}
          onChange={handleInputChange}
        />

      </Form.Group>
      <Form.Group className="mb-3" controlId="formBasicCheckbox">
        <Form.Check type="checkbox" label="Remember me" />
      </Form.Group>
      <Form.Group className='d-flex justify-content-center'>

        <Button variant="success" className="me-4" type="submit">
          Login
        </Button>
        
        <Button variant="success" type="button" href='/Registration'>
          Register
        </Button>
      </Form.Group>
    </Form>
  );
}
