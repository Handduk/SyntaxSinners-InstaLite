import { expect, test, it, describe} from 'vitest'
import { promises as fsPromises} from "fs"
import { readFile } from "fs/promises"
import path from "path"
import { render, screen } from '@testing-library/react';
import { Login } from '../pages/Login';

//Skriv nya tester som kollar mot api istället för json-fil.

interface User {
    username: string;
    password: string;
}
const credentials: User = {
    username: "AronAsk",
    password: "Bullen69"
};

const filePath = path.join(__dirname, "../data/users.json");
const data = await readFile(filePath, "utf-8");
const users: User[] = JSON.parse(data);

test("Check if json file exists", async () => {
    
    try {
        await fsPromises.access(filePath);
        console.log("Filen existerar")
        expect(true).toBe(true);

    } catch(error){
        console.error("Filen existerar ej", error)
        expect(false).toBe(true);
    }
});

test("Check if username and password matches with jonFiles_checkUser", async () => {

    try{
        console.log(users)

        const userAndPasswordExists = users.some((user) => user.username === credentials.username 
        && user.password === credentials.password);

        if(userAndPasswordExists)
        {
            console.log("Användare och lösenord matchar");
            expect(userAndPasswordExists).toBe(true);
        }
        else {
            console.log("ingen match");
            expect(userAndPasswordExists).toBe(true);
        }
    }catch(error){
        console.error("nja nu blev det lite fel?", error)
    }
});

//Test för rendering
describe ("Login components", () => {

    it("Checks if EmailForm renders", () => {
        render(<Login />);
    
        const emailForm = screen.getByLabelText("Email address");

        console.log("Email formen renderas ut!");
        expect(emailForm).toBeDefined();
    });

    it("Check if PasswordForm renders", () => {
        render(<Login/>);

        const passwordForm = screen.getAllByLabelText("Password");

        console.log("password formen renderas!");
        expect(passwordForm).toBeDefined();
    });

    it("Check if buttons renders", () => {
        render(<Login/>);

        const loginButton = screen.getAllByText(/Login/i);
        const registerButton = screen.getAllByText(/Register/i);

        console.log("Knappar renderas")

        expect(loginButton).toBeDefined();
        expect(registerButton).toBeDefined();
    })
});