import {Card} from "react-bootstrap"
import { Post } from "./Models/post"

export type PostProps = {
    title: string
    image: File
    description: string
}

export function PostItem({image, description}: Post) {
    return(
        <div className="d-flex justify-content-center">
            <Card className="shadow-lg" style={{width: "60%", border: "0px"}}>
                <Card.Title 
                style={{height: "5%"}}
                className="bg-dark text-light">
                    <span>{"placeholder: User"}</span>
                </Card.Title>
            <Card.Img
                 variant="top"
                 //värde från getPost
                 //src={`${filePath}${image}`} 
                 height="600px"
                 style={{objectFit: "cover"}}
             />
             <Card.Footer
             className="bg-dark text-light">
                <span>{description}</span>
             </Card.Footer>
         </Card>
        </div>  
    );    
}