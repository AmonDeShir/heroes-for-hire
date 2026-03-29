import { useState } from "onejs-preact/hooks";

export function useClick(onClick?: () => void, timeout: number = 250) {
  const [isActive, setIsActive] = useState(false);

  const handleClick = () => {
    setIsActive(true)
    
    if (onClick) {
      onClick();
    }
    
    const ref = setTimeout(() => {
      setIsActive(false);
    }, timeout);
  
    return () => clearTimeout(ref);
  }

  return { state: isActive, register: handleClick };
}