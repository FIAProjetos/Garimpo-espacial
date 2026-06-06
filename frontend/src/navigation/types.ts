import type { NavigatorScreenParams } from '@react-navigation/native';

export type PublicStackParamList = {
  Home: undefined;
  Pricing: undefined;
};

export type MainTabParamList = {
  Dashboard: undefined;
  Alerts: undefined;
  Profile: undefined;
};

export type RootStackParamList = {
  Public: NavigatorScreenParams<PublicStackParamList> | undefined;
  Analyst: NavigatorScreenParams<MainTabParamList> | undefined;
};
