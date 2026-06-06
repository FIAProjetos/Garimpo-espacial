import React from 'react';
import { StyleSheet, View } from 'react-native';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { Text } from 'react-native';
import { AnalystNavbar } from '../components/AnalystNavbar';
import { AlertsScreen } from '../screens/AlertsScreen';
import { DashboardScreen } from '../screens/DashboardScreen';
import { ProfileScreen } from '../screens/ProfileScreen';
import { analyst } from '../theme/analyst';
import { colors } from '../theme/colors';
import { fontFamily } from '../theme/typography';
import type { MainTabParamList } from './types';

const Tab = createBottomTabNavigator<MainTabParamList>();

function TabLabel({ label, focused }: { label: string; focused: boolean }) {
  return (
    <Text
      style={{
        fontFamily: fontFamily.medium,
        color: focused ? colors.accentCyan : colors.textMuted,
        fontSize: 11,
      }}>
      {label}
    </Text>
  );
}

export function MainTabs() {
  return (
    <View style={styles.shell}>
      <AnalystNavbar />
      <Tab.Navigator
        initialRouteName="Dashboard"
        screenOptions={{
          headerShown: false,
          sceneStyle: styles.scene,
          tabBarStyle: styles.tabBar,
          tabBarActiveTintColor: colors.accentCyan,
          tabBarInactiveTintColor: colors.textMuted,
        }}>
        <Tab.Screen
          name="Dashboard"
          component={DashboardScreen}
          options={{
            tabBarLabel: ({ focused }) => <TabLabel label="Painel" focused={focused} />,
          }}
        />
        <Tab.Screen
          name="Alerts"
          component={AlertsScreen}
          options={{
            tabBarLabel: ({ focused }) => <TabLabel label="Alertas" focused={focused} />,
          }}
        />
        <Tab.Screen
          name="Profile"
          component={ProfileScreen}
          options={{
            tabBarLabel: ({ focused }) => <TabLabel label="Perfil" focused={focused} />,
          }}
        />
      </Tab.Navigator>
    </View>
  );
}

const styles = StyleSheet.create({
  shell: {
    flex: 1,
    backgroundColor: colors.background,
  },
  scene: {
    backgroundColor: colors.background,
  },
  tabBar: {
    backgroundColor: analyst.panelBg,
    borderTopColor: analyst.panelBorder,
    borderTopWidth: 1,
    height: 56,
    paddingBottom: 4,
  },
});
