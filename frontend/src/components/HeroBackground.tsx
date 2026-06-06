import React, { useEffect, useRef, useState } from 'react';
import {
  Animated,
  Image,
  ImageSourcePropType,
  StyleSheet,
  View,
} from 'react-native';
import { LinearGradient } from 'expo-linear-gradient';
import { colors } from '../theme/colors';

const FADE_MS = 800;
const INTERVAL_MS = 5000;

type Props = {
  images: ImageSourcePropType[];
  height: number;
};

export function HeroBackground({ images, height }: Props) {
  const [index, setIndex] = useState(0);
  const opacity = useRef(new Animated.Value(1)).current;

  useEffect(() => {
    if (images.length <= 1) return;

    const timer = setInterval(() => {
      Animated.timing(opacity, {
        toValue: 0,
        duration: FADE_MS,
        useNativeDriver: true,
      }).start(() => {
        setIndex(prev => (prev + 1) % images.length);
        Animated.timing(opacity, {
          toValue: 1,
          duration: FADE_MS,
          useNativeDriver: true,
        }).start();
      });
    }, INTERVAL_MS);

    return () => clearInterval(timer);
  }, [images.length, opacity]);

  return (
    <View style={[styles.container, { height }]}>
      <Animated.View style={[styles.imageWrap, { opacity }]}>
        <Image source={images[index]} style={styles.image} resizeMode="cover" />
      </Animated.View>
      <LinearGradient
        colors={['rgba(11,15,26,0.3)', 'rgba(11,15,26,0.85)', colors.background]}
        locations={[0, 0.55, 1]}
        style={StyleSheet.absoluteFill}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    width: '100%',
    overflow: 'hidden',
    backgroundColor: colors.background,
  },
  imageWrap: {
    ...StyleSheet.absoluteFillObject,
  },
  image: {
    width: '100%',
    height: '100%',
  },
});
