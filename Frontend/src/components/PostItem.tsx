import {Card} from "react-bootstrap"

export type PostProps = {
    image: File | FormData
    description: string 
}

export function PostItem({image, description}: PostProps) {
    return(
        <div className="d-flex justify-content-center">
            <Card className="shadow-lg" style={{width: "60%", border: "0px"}}>
                <Card.Title 
                style={{height: "5%"}}
                className="bg-dark text-light">
                    <span>{"placeholder"}</span>
                </Card.Title>
            <Card.Img
                 variant="top"
                 //värde från getPost
                 //src={image.name} 
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