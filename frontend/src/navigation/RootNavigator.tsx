import React, { useEffect, useRef } from 'react';
import { ActivityIndicator, View } from 'react-native';
import { NavigationContainer, useNavigationContainerRef } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { LoginModal } from '../components/LoginModal';
import { RegisterModal } from '../components/RegisterModal';
import { useAuth } from '../hooks/useAuth';
import { colors } from '../theme/colors';
import { MainTabs } from './MainTabs';
import { PublicStack } from './PublicStack';
import type { RootStackParamList } from './types';

const Stack = createNativeStackNavigator<RootStackParamList>();

export function RootNavigator() {
  const { isAuthenticated, isLoading } = useAuth();
  const navigationRef = useNavigationContainerRef<RootStackParamList>();
  const prevAuth = useRef<boolean | null>(null);

  useEffect(() => {
    if (!navigationRef.isReady() || isLoading) return;

    if (prevAuth.current === null) {
      prevAuth.current = isAuthenticated;
      if (isAuthenticated) {
        navigationRef.navigate('Analyst', { screen: 'Dashboard' });
      }
      return;
    }

    if (prevAuth.current !== isAuthenticated) {
      prevAuth.current = isAuthenticated;
      if (isAuthenticated) {
        navigationRef.navigate('Analyst', { screen: 'Dashboard' });
      } else {
        navigationRef.navigate('Public', { screen: 'Home' });
      }
    }
  }, [isAuthenticated, isLoading, navigationRef]);

  if (isLoading) {
    return (
      <View style={{ flex: 1, justifyContent: 'center', backgroundColor: colors.background }}>
        <ActivityIndicator color={colors.primary} size="large" />
      </View>
    );
  }

  return (
    <NavigationContainer ref={navigationRef}>
      <Stack.Navigator
        initialRouteName="Public"
        screenOptions={{ headerShown: false, animation: 'fade' }}>
        <Stack.Screen name="Public" component={PublicStack} />
        <Stack.Screen name="Analyst" component={MainTabs} />
      </Stack.Navigator>
      <LoginModal />
      <RegisterModal />
    </NavigationContainer>
  );
}
