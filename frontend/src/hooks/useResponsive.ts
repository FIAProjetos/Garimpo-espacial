import { useWindowDimensions } from 'react-native';
import { layout } from '../theme/layout';

export function useResponsive() {
  const { width } = useWindowDimensions();
  const isDesktop = width >= layout.desktopBreakpoint;

  return {
    width,
    isDesktop,
    contentMaxWidth: layout.contentMaxWidth,
    narrowMaxWidth: layout.narrowMaxWidth,
    modalMaxWidth: layout.modalMaxWidth,
  };
}
