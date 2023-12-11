import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';

export function Registration() {
  return (
    <Form className='w-25 mt-5 mx-auto'>

        <Form.Group className="mb-3">
            <Form.Label>Username</Form.Label>
            <Form.Control type="username" placeholder="Username" />
        </Form.Group>

      <Form.Group className="mb-3" controlId="formBasicEmail">
        <Form.Label>Email address</Form.Label>
        <Form.Control type="email" placeholder="Enter email" />
      </Form.Group>

      <Form.Group className="mb-3" controlId="formBasicPassword">
        <Form.Label>Password</Form.Label>
        <Form.Control type="password" placeholder="Password" />
      </Form.Group>

      <Form.Group className='d-flex justify-content-center'>
        <Button variant="success" type="submit">
          Submit
        </Button>
      </Form.Group>
    </Form>
  );
}