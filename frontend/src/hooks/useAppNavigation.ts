import { useNavigation } from '@react-navigation/native';
import type { BottomTabNavigationProp } from '@react-navigation/bottom-tabs';
import type { CompositeNavigationProp } from '@react-navigation/native';
import type { NativeStackNavigationProp } from '@react-navigation/native-stack';
import type { MainTabParamList, PublicStackParamList, RootStackParamList } from '../navigation/types';

export type AnalystNav = CompositeNavigationProp<
  BottomTabNavigationProp<MainTabParamList>,
  NativeStackNavigationProp<RootStackParamList>
>;

export type PublicNav = CompositeNavigationProp<
  NativeStackNavigationProp<PublicStackParamList>,
  NativeStackNavigationProp<RootStackParamList>
>;

export function useRootNavigation() {
  return useNavigation<NativeStackNavigationProp<RootStackParamList>>();
}

export function useAnalystNavigation() {
  return useNavigation<AnalystNav>();
}

export function usePublicNavigation() {
  return useNavigation<PublicNav>();
}
