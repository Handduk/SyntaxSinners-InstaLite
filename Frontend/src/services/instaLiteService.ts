import axios from "axios";
import { LoginModel } from "../components/Models/loginModel";
import { PostProps } from "../components/PostItem";
import { Post } from "../components/Models/post";

const baseUrl = `https://localhost:5000/api/Post`;
const userBaseUrl = `https://localhost:5000/api/User`;

export const getPosts = async (): Promise<Post[]> => {
    try {
      const response = await axios.get(baseUrl);
      return response.data;
    } catch (error) {
      throw new Error('Failed to fetch posts');
    }
}

export const createPost = (post: PostProps) => {
  const logFormData = (formData: FormData) => {
    for (const [key, value] of formData.entries()) {
      console.log(`${key}:`, value);
    }
  };

  const formData = new FormData();

  if (post.image instanceof File) {
    formData.append('image', post.image, post.image.name);
  } else if (post.image instanceof FormData) {
    for (const [key, value] of post.image.entries()) {
      formData.append(key, value);
    }
  }

  formData.append('description', post.description);

  // Logga FormData för att kolla filtyp
  console.log('FormData:');
  logFormData(formData);

  return axios.post(baseUrl, formData)
    .then(response => response.data)
    .catch(error => {
      console.error('Error during createPost:', error);
      throw error;
    });
};

export const deletePost = (id: number) => {
    return axios.delete(`${baseUrl}/${id}`)
    .then(response => response.data);
}

export const createUser = (user: {username: string,
     email: string, 
     password: string;}) => {
        return axios.post(userBaseUrl, {
            username: user.username,
            email: user.email,
            passwordHash: user.password
        }).then(response => response.data);
    }

export const loginUser = (loginModel: LoginModel) => {
    return axios.post(`${userBaseUrl}/Login`,loginModel, {
    })
    .then(response => response.data);
}