import { useEffect, useState } from 'react';
import { Col, Row } from 'react-bootstrap';
import { PostItem } from '../components/PostItem';
import { getPosts } from '../services/instaLiteService';
import { Post } from '../components/Models/post';

export function Home() {
  const [posts, setPosts] = useState<Post[]>([]);

  useEffect(() => {
    fetchPosts();
  }, []);

  const fetchPosts = async () => {
    try {
      const postsData = await getPosts();
      setPosts(postsData);
    } catch (error) {
      console.error('Failed to fetch posts:', error);
    }
  };

  return (
    <>
      <Row md={1} xs={1} lg={1} className="g-5 ">
        {posts.map((item) => (
          <Col key={item.id}>
            <PostItem {...item} />
          </Col>
        ))}
      </Row>
    </>
  );
}
