import React, { useState } from 'react';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import { createPost } from '../services/instaLiteService';

export function Upload() {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [description, setDescription] = useState<string>('');

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const files = event.target.files;
    if (files && files.length > 0) {
      setSelectedFile(files[0]);
      console.log(files);
    }
  };

  const handleDescriptionChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setDescription(event.target.value);
  };

  //Handlesubmit funkar inte
  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (selectedFile && description) {
      try {
        const formData = new FormData();
        formData.append('image', selectedFile, selectedFile.name);
        
        await createPost({ image: formData, description});
      } catch (error) {
        console.error("Error uploading image:", error);
      }
    }
  };
  return (
    <Form onSubmit={handleSubmit} className='w-25 mt-5 mx-auto'>
      <Form.Group controlId="formFileLg" className="mb-3">
        <Form.Label>Upload image</Form.Label>
        <Form.Control type="file" size="lg" onChange={handleFileChange} />
      </Form.Group>
      <Form.Group>
        <Form.Label>Image caption</Form.Label>
        <Form.Control type="text" placeholder='Description' value={description} onChange={handleDescriptionChange} />
      </Form.Group>
      <Form.Group className='d-flex justify-content-center mt-3'>
        <Button variant="success" type="submit">
          Upload
        </Button>
      </Form.Group>
    </Form>
  );
}