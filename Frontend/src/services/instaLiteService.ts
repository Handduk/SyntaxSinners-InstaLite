import axios from "axios";
import { LoginModel } from "../components/Models/loginModel";
import { PostProps } from "../components/PostItem";
import { Post } from "../components/Models/post";
import { RegisterModel } from "../components/Models/registerModel";

const postBaseUrl = `https://localhost:5000/api/Post`;
const userBaseUrl = `https://localhost:5000/api/User`;

export const getPosts = async (): Promise<Post[]> => {
    try {
      const response = await axios.get(postBaseUrl);
      console.log(response);
      return response.data;
    } catch (error) {
      throw new Error('Failed to fetch posts');
    }
}

export const createPost = (post: PostProps) => {

  const formData = new FormData();

  formData.append('title', post.title);
  formData.append('ImageFile', post.image, post.image.name);
  formData.append('description', post.description);

  return axios.post(postBaseUrl, formData, {headers: {"Content-Type": "multipart/form-data"}})
    .then(response => response.data)
    .catch(error => {
      console.error('Error during createPost:', error);
      throw error;
    });
};

export const deletePost = (id: number) => {
    return axios.delete(`${postBaseUrl}/${id}`)
    .then(response => response.data);
}

export const createUser = (user: RegisterModel) => {
        return axios.post(userBaseUrl, {
          username: user.username,
          passwordHash: user.password,
          email: user.email
        })
        .then(response => response.data)
        .catch(error => {
          console.error('Error during createUser:', error);
          throw error;
        });
    }

export const loginUser = (loginModel: LoginModel) => {
    return axios.post(`${userBaseUrl}/Login`,loginModel, {
    })
    .then(response => response.data);
}