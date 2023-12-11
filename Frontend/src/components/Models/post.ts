export type Post = {
    id: number;
    user: string;
    image: File | FormData;
    description: string;
}