import { createContext, useContext } from "react";
import { User, AuthState } from "@/types";

export const AuthContext = createContext<{
  auth: AuthState;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
  register: (userData: RegisterData) => Promise<void>;
} | null>(null);

export interface RegisterData {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};

export const isAuthenticated = (): boolean => {
  const token = localStorage.getItem("access_token");
  if (!token) return false;

  try {
    const payload = JSON.parse(atob(token.split(".")[1]));
    return payload.exp > Date.now() / 1000;
  } catch {
    return false;
  }
};

export const isAdmin = (user: User | null): boolean => {
  return user?.role === "admin";
};

export const getAuthToken = (): string | null => {
  return localStorage.getItem("access_token");
};

export const setAuthToken = (token: string): void => {
  localStorage.setItem("access_token", token);
};

export const removeAuthToken = (): void => {
  localStorage.removeItem("access_token");
  localStorage.removeItem("refresh_token");
  localStorage.removeItem("user_data");
};

// Utility function to format user display name
export const getUserDisplayName = (user: User): string => {
  return `${user.firstName} ${user.lastName}`;
};

// Utility function to check if user can access admin features
export const canAccessAdmin = (user: User | null): boolean => {
  return user?.role === "admin";
};

// Mock JWT validation (in real app, this would validate with IdentityServer4)
export const validateToken = async (token: string): Promise<boolean> => {
  try {
    // In a real application, this would make a request to validate the JWT
    // against your IdentityServer4 instance
    if (!token || token === "undefined") return false;

    // Mock validation - just check if token exists and is recent
    const tokenData = token.split("_");
    if (tokenData.length < 3) return false;

    const timestamp = parseInt(tokenData[2]);
    const now = Date.now();
    const oneDay = 24 * 60 * 60 * 1000;

    return now - timestamp < oneDay;
  } catch {
    return false;
  }
};

// OAuth/OpenID Connect helpers for IdentityServer4 integration
export const getAuthCodeFlowUrl = (): string => {
  const baseUrl =
    process.env.VITE_IDENTITY_SERVER_URL || "http://localhost:5004";
  const clientId = process.env.VITE_CLIENT_ID || "toystore-spa";
  const redirectUri = encodeURIComponent(
    window.location.origin + "/auth/callback",
  );
  const responseType = "code";
  const scope =
    "openid profile email roles toystore.products toystore.orders toystore.users";
  const state = generateRandomState();

  localStorage.setItem("auth_state", state);

  return (
    `${baseUrl}/connect/authorize?` +
    `client_id=${clientId}&` +
    `redirect_uri=${redirectUri}&` +
    `response_type=${responseType}&` +
    `scope=${encodeURIComponent(scope)}&` +
    `state=${state}&` +
    `code_challenge_method=S256`
  );
};

export const generateRandomState = (): string => {
  return (
    Math.random().toString(36).substring(2, 15) +
    Math.random().toString(36).substring(2, 15)
  );
};

export const handleAuthCallback = async (
  code: string,
  state: string,
): Promise<User> => {
  const savedState = localStorage.getItem("auth_state");
  if (state !== savedState) {
    throw new Error("Invalid state parameter");
  }

  // In real app, exchange code for tokens
  const tokenResponse = await exchangeCodeForTokens(code);

  setAuthToken(tokenResponse.access_token);
  localStorage.setItem("refresh_token", tokenResponse.refresh_token);
  localStorage.removeItem("auth_state");

  // Decode user info from ID token or fetch from UserInfo endpoint
  const user = await getUserInfo(tokenResponse.access_token);
  localStorage.setItem("user_data", JSON.stringify(user));

  return user;
};

const exchangeCodeForTokens = async (code: string): Promise<any> => {
  const baseUrl =
    process.env.VITE_IDENTITY_SERVER_URL || "http://localhost:5004";
  const clientId = process.env.VITE_CLIENT_ID || "toy-store-spa";
  const redirectUri = window.location.origin + "/auth/callback";

  const response = await fetch(`${baseUrl}/connect/token`, {
    method: "POST",
    headers: {
      "Content-Type": "application/x-www-form-urlencoded",
    },
    body: new URLSearchParams({
      grant_type: "authorization_code",
      client_id: clientId,
      code,
      redirect_uri: redirectUri,
    }),
  });

  if (!response.ok) {
    throw new Error("Failed to exchange code for tokens");
  }

  return await response.json();
};

const getUserInfo = async (accessToken: string): Promise<User> => {
  const baseUrl =
    process.env.VITE_IDENTITY_SERVER_URL || "http://localhost:5004";

  const response = await fetch(`${baseUrl}/connect/userinfo`, {
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },
  });

  if (!response.ok) {
    throw new Error("Failed to fetch user info");
  }

  const userInfo = await response.json();

  return {
    id: userInfo.sub,
    email: userInfo.email,
    firstName: userInfo.given_name,
    lastName: userInfo.family_name,
    role: userInfo.role || "customer",
    createdAt: userInfo.created_at || new Date().toISOString(),
  };
};
