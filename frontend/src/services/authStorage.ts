import AsyncStorage from '@react-native-async-storage/async-storage';
import type { UserDto } from '../types/api';

const TOKEN_KEY = '@garimpo/token';
const USER_KEY = '@garimpo/user';

export async function saveSession(token: string, user: UserDto): Promise<void> {
  await AsyncStorage.multiSet([
    [TOKEN_KEY, token],
    [USER_KEY, JSON.stringify(user)],
  ]);
}

export async function getToken(): Promise<string | null> {
  return AsyncStorage.getItem(TOKEN_KEY);
}

export async function getUser(): Promise<UserDto | null> {
  const raw = await AsyncStorage.getItem(USER_KEY);
  if (!raw) return null;
  return JSON.parse(raw) as UserDto;
}

export async function clearSession(): Promise<void> {
  await AsyncStorage.multiRemove([TOKEN_KEY, USER_KEY]);
}
