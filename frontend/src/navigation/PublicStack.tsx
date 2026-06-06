import React from 'react';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { HomeScreen } from '../screens/HomeScreen';
import { PricingScreen } from '../screens/PricingScreen';
import { colors } from '../theme/colors';
import type { PublicStackParamList } from './types';

const Stack = createNativeStackNavigator<PublicStackParamList>();

export function PublicStack() {
  return (
    <Stack.Navigator
      screenOptions={{
        headerShown: false,
        contentStyle: { backgroundColor: colors.background },
      }}>
      <Stack.Screen name="Home" component={HomeScreen} />
      <Stack.Screen name="Pricing" component={PricingScreen} />
    </Stack.Navigator>
  );
}
