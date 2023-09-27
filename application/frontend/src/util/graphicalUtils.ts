export function selectColors(n: number): string[] {
  const pastelColors: string[] = [
    '#8884d8', // Muted blue
    '#82ca9d', // Muted green
    '#d88488', // Muted coral
    '#d8d084', // Muted yellow
    '#84d8d1', // Muted turquoise
    '#d8a484', // Muted peach
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
