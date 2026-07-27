export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoggedInUser {
  id: number;
  username: string;
  role: UserRole;
}

export interface LoginResponse {
  token: string;
  expiresIn: number;
  user: LoggedInUser;
}

export type UserRole = 'Admin' | 'Mentor' | 'Trainee';