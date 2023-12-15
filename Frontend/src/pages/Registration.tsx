import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import { RegisterModel } from '../components/Models/registerModel';
import { useState } from 'react';
import { createUser } from '../services/instaLiteService';
import { useNavigate } from 'react-router-dom';

export function Registration() {

  const [registerData, setRegisterData] = useState<RegisterModel>({
    username: '',
    password: '',
    email: ''
  });

  const navigate = useNavigate();

  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = event.target;
    setRegisterData((prevData) => ({ ...prevData, [name]: value }));
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    try {
      const response = await createUser(registerData);
      window.alert("user registered!");
      if (response){
        navigate('/');
      }
    } catch (error) {
      console.error('Login failed:', error);
    }
  };

  return (
    <Form onSubmit={handleSubmit} className='w-25 mt-5 mx-auto'>

        <Form.Group className="mb-3">
            <Form.Label>Username</Form.Label>
            <Form.Control 
            type="username" 
            placeholder="Username"
            name='username'
            value={registerData.username}
            onChange={handleInputChange} />
        </Form.Group>

      <Form.Group className="mb-3" controlId="formBasicEmail">
        <Form.Label>Email address</Form.Label>
        <Form.Control 
        type="email" 
        placeholder="Enter email"
        name='email'
        value={registerData.email}
        onChange={handleInputChange} />
      </Form.Group>

      <Form.Group className="mb-3" controlId="formBasicPassword">
        <Form.Label>Password</Form.Label>
        <Form.Control 
        type="password" 
        placeholder="Password"
        name='password'
        value={registerData.password}
        onChange={handleInputChange} />
      </Form.Group>

      <Form.Group className='d-flex justify-content-center'>
        <Button variant="success" type="submit">
          Submit
        </Button>
      </Form.Group>
    </Form>
  );
}