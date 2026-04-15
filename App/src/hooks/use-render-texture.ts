import { useEffect, useRef } from "onejs-preact/hooks";
import { RenderTexture, Resources } from "UnityEngine";
import { Background, BackgroundRepeat, Repeat, StyleBackground, StyleBackgroundRepeat } from "UnityEngine/UIElements";

export const useRenderTexture = (path: string, repeat = Repeat.NoRepeat) => {
  const TEXTURE = Resources.Load(path) as RenderTexture;
  
  const target = useRef<HTMLElement>(null);
      
  useEffect(() => {
    const image = getVisualElement(target.current);

    if (!image || !TEXTURE) {
      return;
    }

    image.style.backgroundImage  = new StyleBackground(Background.FromRenderTexture(TEXTURE));
    image.style.backgroundRepeat = new StyleBackgroundRepeat(new BackgroundRepeat(repeat, repeat));
  }, []);

    return target;
}

export function getVisualElement(element: any): (VisualElement & { scaledPixelsPerPoint: number }) | null {
  if (!element) {
    return null;
  }

  return element.ve ?? element.__ve;
}