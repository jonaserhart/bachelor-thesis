
export type User = {
    id: string;
    displayName: string;
    eMail: string;
}

export type AuthResponse = {
    user: User;
    tokenInfo: {
        jwt: string;
        expires: number;
    }
}