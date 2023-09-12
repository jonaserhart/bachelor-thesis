export function selectColors(n: number): string[] {
  const pastelColors: string[] = [
    '#FFD1DC', // Pink
    '#D1FFD1', // Light Green
    '#D1D1FF', // Light Blue
    '#FFF3D1', // Light Yellow
    '#FFD1FF', // Lavender
    '#D1FFFF', // Light Cyan
    '#FFC4D1', // Light Rose
    '#C4FFD1', // Mint
    '#D1C4FF', // Periwinkle
    '#FFF7D1', // Cream
  ];

  let selectedColors: string[] = [];

  if (n <= pastelColors.length) {
    const shuffledColors = [...pastelColors].sort(() => 0.5 - Math.random());
    selectedColors = shuffledColors.slice(0, n);
  } else {
    // If n is greater than the length of the pastelColors array, allow duplicates
    for (let i = 0; i < n; i++) {
      const randomIndex = Math.floor(Math.random() * pastelColors.length);
      selectedColors.push(pastelColors[randomIndex]);
    }
  }

  return selectedColors;
}
