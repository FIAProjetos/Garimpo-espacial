import React, {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from 'react';
import { login as apiLogin, register as apiRegister } from '../services/api';
import * as authStorage from '../services/authStorage';
import type { RegisterRequest, UserDto } from '../types/api';

type AuthContextValue = {
  user: UserDto | null;
  isLoading: boolean;
  isAuthenticated: boolean;
  loginModalVisible: boolean;
  registerModalVisible: boolean;
  openLoginModal: () => void;
  closeLoginModal: () => void;
  openRegisterModal: () => void;
  closeRegisterModal: () => void;
  login: (email: string, password: string) => Promise<void>;
  register: (data: RegisterRequest) => Promise<void>;
  logout: () => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<UserDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [loginModalVisible, setLoginModalVisible] = useState(false);
  const [registerModalVisible, setRegisterModalVisible] = useState(false);

  useEffect(() => {
    (async () => {
      const storedUser = await authStorage.getUser();
      const token = await authStorage.getToken();
      if (storedUser && token) setUser(storedUser);
      setIsLoading(false);
    })();
  }, []);

  const openLoginModal = useCallback(() => {
    setRegisterModalVisible(false);
    setLoginModalVisible(true);
  }, []);

  const closeLoginModal = useCallback(() => setLoginModalVisible(false), []);

  const openRegisterModal = useCallback(() => {
    setLoginModalVisible(false);
    setRegisterModalVisible(true);
  }, []);

  const closeRegisterModal = useCallback(() => setRegisterModalVisible(false), []);

  const login = useCallback(async (email: string, password: string) => {
    const response = await apiLogin({ email, password });
    await authStorage.saveSession(response.token, response.user);
    setUser(response.user);
    setLoginModalVisible(false);
    setRegisterModalVisible(false);
  }, []);

  const register = useCallback(async (data: RegisterRequest) => {
    await apiRegister(data);
    await login(data.email, data.password);
  }, [login]);

  const logout = useCallback(async () => {
    await authStorage.clearSession();
    setUser(null);
  }, []);

  const value = useMemo(
    () => ({
      user,
      isLoading,
      isAuthenticated: !!user,
      loginModalVisible,
      registerModalVisible,
      openLoginModal,
      closeLoginModal,
      openRegisterModal,
      closeRegisterModal,
      login,
      register,
      logout,
    }),
    [
      user,
      isLoading,
      loginModalVisible,
      registerModalVisible,
      openLoginModal,
      closeLoginModal,
      openRegisterModal,
      closeRegisterModal,
      login,
      register,
      logout,
    ],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
