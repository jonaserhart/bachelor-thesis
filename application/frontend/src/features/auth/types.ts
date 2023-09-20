export interface User {
  id: string;
  displayName: string;
  eMail: string;
}

export interface AuthResponse {
  user: User;
  token: {
    jwt: string;
    expires: number;
  };
}
