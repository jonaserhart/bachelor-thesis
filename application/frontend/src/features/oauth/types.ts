
export type User = {
    id: string;
    displayName: string;
    eMail: string;
}

export type AuthResponse = {
    user: User;
    token: {
        jwt: string;
        expires: number;
    }
}