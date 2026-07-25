import React, { useState, useEffect } from 'react';

interface CountdownTimerProps {
  endTime: string;
  className?: string;
}

const CountdownTimer: React.FC<CountdownTimerProps> = ({ endTime, className = '' }) => {
  const [timeLeft, setTimeLeft] = useState<{ days: number; hours: number; minutes: number; seconds: number } | null>(null);
  const [expired, setExpired] = useState(false);

  useEffect(() => {
    const calculate = () => {
      const now = new Date();
      const end = new Date(endTime);
      const diff = end.getTime() - now.getTime();

      if (diff <= 0) {
        setExpired(true);
        setTimeLeft(null);
        return;
      }

      const days = Math.floor(diff / (1000 * 60 * 60 * 24));
      const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
      const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
      const seconds = Math.floor((diff % (1000 * 60)) / 1000);

      setTimeLeft({ days, hours, minutes, seconds });
      setExpired(false);
    };

    calculate();
    const interval = setInterval(calculate, 1000);
    return () => clearInterval(interval);
  }, [endTime]);

  if (expired) {
    return <span className={`text-on-surface-variant font-label-sm ${className}`}>Đã kết thúc</span>;
  }

  if (!timeLeft) {
    return <span className={`text-on-surface-variant font-label-sm ${className}`}>Đang tải...</span>;
  }

  const parts: string[] = [];
  if (timeLeft.days > 0) parts.push(`${timeLeft.days} ngày`);
  if (timeLeft.hours > 0 || timeLeft.days > 0) parts.push(`${timeLeft.hours}g`);
  parts.push(`${timeLeft.minutes}m ${timeLeft.seconds}s`);

  return (
    <span className={`text-error font-label-md font-bold ${className}`}>
      Kết thúc trong {parts.join(' ')}
    </span>
  );
};

export default CountdownTimer;
