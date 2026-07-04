export const formatCurrency = (value: number): string => {
  return new Intl.NumberFormat('vi-VN').format(Math.round(value / 1000) * 1000) + ' đ';
};
