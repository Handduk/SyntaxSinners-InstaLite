import { afterEach, beforeEach, describe, expect, it } from "vitest";
import MockAdapter from "axios-mock-adapter";
import { createPost, getPosts } from "../services/instaLiteService";
import axios from "axios";

describe("Test InstaLiteServices", () => {
    
    let mockAxios : MockAdapter;
    const baseUrl = "https://localhost:5000/api/Post"

    beforeEach(() => {
      mockAxios = new MockAdapter(axios);
    });
  
    afterEach(() => {
      mockAxios.restore();
    });
  
    it ("should fetch posts data successfully", async () => {
        
    const userData = { id: 1, userId: 1, image: "../public/images/maxresdefault.jpg", description: "post" };

    mockAxios.onGet(baseUrl).reply(200, userData);

    const result = await getPosts();
    console.log(result);

    expect(result).toEqual(userData);

    });

    it("Should send post data successfully", async () => {
        //arrange
        const file = new File(['(⌐□_□)'], 'baba.jpg', { type: 'image/jpeg' });

        const fileData = { title: "bingbong", image: file, description: "hejhej"};

        const formData = new FormData();

        formData.append('title', fileData.title);
        formData.append('ImageFile', fileData.image, fileData.image.name);
        formData.append('description', fileData.description);

        //act

        console.log("BINGBONG",fileData);
        mockAxios.onPost(baseUrl, formData).reply(200, {data: "success"});
        //assert
        const result = await createPost(fileData);
        console.log("resultat:");
        console.log(result);

        expect(result).toEqual({data: "success"});
    });
})